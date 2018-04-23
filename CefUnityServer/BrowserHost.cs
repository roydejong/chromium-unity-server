using CefSharp;
using CefSharp.OffScreen;
using CefUnityLib.Helpers;
using CefUnityLib.Messages;
using CefUnityServer.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CefUnityServer
{
    public class BrowserHost
    {
        protected const int BROWSER_INIT_TIMEOUT_MS = 15000;

        protected TaskRunner runner;
        protected BrowserSettings settings;

        protected RequestContext requestContext;
        protected ChromiumWebBrowser webBrowser;

        protected byte[] paintBitmap = null;
        protected int paintBufferSize = 0;

        public BrowserHost(TaskRunner runner) : this(runner, new BrowserSettings())
        {
        }

        public BrowserHost(TaskRunner runner, BrowserSettings settings)
        {
            this.runner = runner;
            this.settings = settings;
        }

        public async void Start()
        {
            this.settings.WindowlessFrameRate = 30;

            var reqCtxSettings = new RequestContextSettings
            {
                CachePath = "",
                IgnoreCertificateErrors = false,
                PersistSessionCookies = false,
                PersistUserPreferences = false
            };

            this.requestContext = new RequestContext(reqCtxSettings);

            this.webBrowser = new ChromiumWebBrowser("about:blank", this.settings, this.requestContext, false);
            this.webBrowser.CreateBrowser(IntPtr.Zero);

            Resize(1024, 768);
            WaitForBrowserInit();

            await LoadPageAsync("https://www.youtube.com/watch?v=gsAqDppP3Ec");

            this.webBrowser.OnPaint += WebBrowser_OnPaint;
            this.webBrowser.LoadError += WebBrowser_LoadError;
            this.webBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
        }

        public void Stop()
        {
            if (this.webBrowser != null)
            {
                this.webBrowser.Dispose();
                this.webBrowser = null;

                this.requestContext.Dispose();
                this.requestContext = null;
            }
        }

        protected bool LmbIsDown = false;

        public object CefMouseButtonType { get; private set; }

        public void HandleKeyEvent(KeyEventPipeMessage keyMessage)
        {
            var host = this.webBrowser.GetBrowserHost();

            // Get character info
            int character = 0;

            var cefKeyModifiers = new CefEventFlags();

            if (keyMessage.KeyEventType != KeyEventPipeMessage.TYPE_KEY_CHAR)
            {
                var keyFlags = keyMessage.Keys;
                var keyFlagsChar = keyFlags & Keys.KeyCode; // bit shift to remove modifiers from character code

                character = (int)keyFlagsChar; 

                if (keyMessage.Modifiers.HasFlag(Keys.Control))
                {
                    cefKeyModifiers |= CefEventFlags.ControlDown;
                }
                
                if (keyMessage.Modifiers.HasFlag(Keys.Shift))
                {
                    cefKeyModifiers |= CefEventFlags.ShiftDown;
                }

                if (keyMessage.Modifiers.HasFlag(Keys.Alt))
                {
                    cefKeyModifiers |= CefEventFlags.AltDown;
                }

                //if (keyMessage.KeyEventType == 0)
                //{
                //    Logr.Log(
                //        keyMessage.KeyEventType <= 0 ? "Key down:" : "Key up:",
                //        keyMessage.Keys,
                //        keyMessage.Modifiers != Keys.None ? "+ Mod " + keyMessage.Modifiers.ToString() : ""
                //    );
                //}
            }
            else
            {
                character = (int)keyMessage.Keys;
                //Logr.Log("Char down:", (char)character, character);
            }

            var keyEvent = new KeyEvent()
            {
                Type = KeyEventType.Char,
                WindowsKeyCode = character,
                Modifiers = cefKeyModifiers
            };

            if (keyMessage.KeyEventType == KeyEventPipeMessage.TYPE_KEY_DOWN)
                keyEvent.Type = KeyEventType.KeyDown;

            if (keyMessage.KeyEventType == KeyEventPipeMessage.TYPE_KEY_UP)
                keyEvent.Type = KeyEventType.KeyUp;

            host.SendKeyEvent(keyEvent);
        }

        public void HandleMouseEvent(MouseEventPipeMessage mouseMessage)
        {
            var host = this.webBrowser.GetBrowserHost();
            
            // Read X & Y coords
            int x = mouseMessage.CoordX;
            int y = mouseMessage.CoordY;

            // Read primary event button & generate modifier struct
            var modifiers = new CefEventFlags();
            var mouseButton = MouseButtonType.Left;

            if (mouseMessage.MouseButtons == MouseButtons.Left)
            {
                modifiers |= CefEventFlags.LeftMouseButton;
                mouseButton = MouseButtonType.Left;
            }

            if (mouseMessage.MouseButtons == MouseButtons.Right)
            {
                modifiers |= CefEventFlags.RightMouseButton;
                mouseButton = MouseButtonType.Right;
            }

            if (mouseMessage.MouseButtons == MouseButtons.Middle)
            {
                modifiers |= CefEventFlags.MiddleMouseButton;
                mouseButton = MouseButtonType.Middle;
            }

            // Generate generic event
            var mouseEvent = new MouseEvent(x, y, modifiers);

            // Dispatch event to browser host
            if (mouseMessage.MouseEventType == MouseEventPipeMessage.TYPE_MOVE)
            {
                host.SendMouseMoveEvent(mouseEvent, false);
            }
            else
            {
                bool isUpEvent = (mouseMessage.MouseEventType == MouseEventPipeMessage.TYPE_MOUSE_UP);
                host.SendMouseClickEvent(mouseEvent, mouseButton, isUpEvent, 1);
            }
        }
        
        public void HandleMouseWheelEvent(MouseWheelEventPipeMessage mouseWheelMsg)
        {
            var host = this.webBrowser.GetBrowserHost();
            
            int x = mouseWheelMsg.CoordX;
            int y = mouseWheelMsg.CoordY;
            int delta = mouseWheelMsg.Delta;

            var mouseEvent = new MouseEvent(x, y, CefEventFlags.None);
            host.SendMouseWheelEvent(mouseEvent, 0, delta);
        }

        private void WebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            // Ensure browser is in "focus" mode so that mouse move events are handled
            this.webBrowser.GetBrowserHost().SendFocusEvent(true);
        }

        private void WebBrowser_LoadError(object sender, LoadErrorEventArgs e)
        {
            // TODO
        }

        public void Resize(int width, int height)
        {
            // Resize
            this.webBrowser.Size = new System.Drawing.Size(width, height);

            // Reset frame buffer
            paintBitmap = null;

            // Force repaint if browser is ready
            Repaint();

            Logr.Log(String.Format("Browser host: Resized viewport to {0}x{1}.", width, height));
        }

        public void Repaint()
        {
            if (webBrowser.IsBrowserInitialized)
            {
                var host = this.webBrowser.GetBrowserHost();

                if (host != null)
                {
                    host.Invalidate(PaintElementType.View);
                }
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private void WebBrowser_OnPaint(object sender, OnPaintEventArgs e)
        {
            if (paintBitmap == null)
            {
                paintBufferSize = this.webBrowser.Size.Width * this.webBrowser.Size.Height * 4;
                paintBitmap = new byte[paintBufferSize];
            }

            try
            {
                Marshal.Copy(e.BufferHandle, paintBitmap, 0, paintBufferSize);
            }
            catch (AccessViolationException ex)
            {
                Logr.Log("WARN: Marshal copy failed: Access violation while reading frame buffer.");
            }

            this.runner.AddTask(new SendFrameTask(paintBitmap));
        }

        public Task LoadPageAsync(string address = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            EventHandler<LoadingStateChangedEventArgs> handler = null;

            handler = (sender, args) =>
            {
                if (!args.IsLoading)
                {
                    this.webBrowser.LoadingStateChanged -= handler;
                    tcs.TrySetResult(true);
                }
            };

            this.webBrowser.LoadingStateChanged += handler;

            if (!string.IsNullOrEmpty(address))
            {
                this.webBrowser.Load(address);
            }

            return tcs.Task;
        }

        public void WaitForBrowserInit()
        {
            int remainingTries = BROWSER_INIT_TIMEOUT_MS;

            while (!this.webBrowser.IsBrowserInitialized)
            {
                if (remainingTries <= 0)
                {
                    throw new TimeoutException(String.Format("Browser failed to initialize after a timeout of {0} ms.", BROWSER_INIT_TIMEOUT_MS));
                }

                Thread.Sleep(100);
                remainingTries -= 100;
            }
        }
    }
}