using CefUnityLib;
using CefUnityLib.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefUnityTestClient
{
    public partial class Form1 : Form
    {
        private CefController _controller;

        private ulong _frameCounterStat;
        private int _frameCounterCounter;
        private int _frameCounterCurrent;

        private int defWidth = 1024;
        private int defHeight = 768;

        public Form1()
        {
            InitializeComponent();
            
            this.Shown += Form1_Shown;
        }

        private void SetKeyEventsForControls(Control.ControlCollection cc)
        {
            if (cc != null)
            {
                foreach (Control control in cc)
                {
                    control.PreviewKeyDown += new PreviewKeyDownEventHandler(Form1_PreviewKeyDown);
                    SetKeyEventsForControls(control.Controls);
                }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            _controller = new CefController();
            _controller.MessageReceived += MessageReceived;
            _controller.ConnectionStateChanged += ConnectionChanged;

            this.KeyPreview = true;
            this.KeyUp += Form1_KeyUp;
            this.KeyDown += Form1_KeyDown;
            this.KeyPress += Form1_KeyPress;
            this.PreviewKeyDown += Form1_PreviewKeyDown;

            pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;

            ConnectionChanged(_controller, false);

            Task.Run(new Action(() =>
            {
                while (this.Visible && !this.Disposing && !this.IsDisposed)
                {
                    bool haveFps = false;
                    int fps = 0;

                    if (_frameCounterCounter >= 10)
                    {
                        fps = _frameCounterCurrent;

                        _frameCounterCounter = 0;
                        _frameCounterCurrent = 0;

                        haveFps = true;
                    }

                    _frameCounterCounter++;

                    try
                    {
                        Invoke(new Action(() =>
                        {
                            if (_controller != null && _controller.Connected)
                            {
                                if (_frameCounterStat > 0)
                                    lblFrames.Text = _frameCounterStat.ToString();

                                if (_controller.MessagesReceivedCount > 0)
                                    lblPkIn.Text = _controller.MessagesReceivedCount.ToString();

                                if (_controller.MessagesSentCount > 0)
                                    lblPkOut.Text = _controller.MessagesSentCount.ToString();

                                if (haveFps)
                                    lblFps.Text = fps.ToString() + " FPS";
                            }
                        }));
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }

                    Thread.Sleep(100);
                }
            }));
            
            SetKeyEventsForControls(Controls);
        }

        private void ConnectionChanged(object sender, bool connected)
        {
            this.Invoke(new Action(() =>
            {
                btnCon.Enabled = !connected;
                btnDis.Enabled = !!connected;
                btnShut.Enabled = !!connected;

                if (!connected)
                {
                    pictureBox1.Image = null;
                }
            }));
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            pictureBox1.Focus();
            _controller.SendKeyCharEvent(e.KeyChar);
            e.Handled = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            pictureBox1.Focus();
            _controller.SendKeyEvent(KeyEventPipeMessage.TYPE_KEY_DOWN, (CefUnityLib.Helpers.Keys)e.KeyCode, (CefUnityLib.Helpers.Keys)e.Modifiers);
            e.Handled = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            pictureBox1.Focus();
            _controller.SendKeyEvent(KeyEventPipeMessage.TYPE_KEY_UP, (CefUnityLib.Helpers.Keys)e.KeyCode, (CefUnityLib.Helpers.Keys)e.Modifiers);
            e.Handled = true;
        }

        public void MessageReceived(object sender, PipeProtoMessage e)
        {
            switch (e.Opcode)
            {
                case PipeProto.OPCODE_FRAME:
                    _frameCounterStat++;
                    _frameCounterCurrent++;
                    
                    var rect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
                    var texture = new Bitmap(defWidth, defHeight);
                    var bmpData = texture.LockBits(rect, ImageLockMode.WriteOnly, texture.PixelFormat);
                    
                    IntPtr ptr = bmpData.Scan0;
                    System.Runtime.InteropServices.Marshal.Copy(e.Payload, 0, ptr, e.Payload.Length);

                    texture.UnlockBits(bmpData);
                    pictureBox1.Image = texture;

                    // NB: Disposing of the texture causes awful issues where the PictureBox will try to access it and crash.
                    // Also, re-using the same texture causes locking issues when testing high FPS.
                    // Conclusion: we are intentionally leaking memory here, and hoping GC will clean up unused textures fast enough.
                    break;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            // Grey out form
            this.Enabled = false;
            btnCon.Text = "Connecting...";
            Application.DoEvents();

            // Try and handle connect attempt
            try
            {
                _controller.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failure:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Restore
            this.Enabled = true;
            btnCon.Text = "Connect";
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = pictureBox1.PointToClient(Cursor.Position);

            _controller.SendMouseEvent(MouseEventPipeMessage.TYPE_MOVE, pos.X, pos.Y, (CefUnityLib.Helpers.MouseButtons)e.Button);

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            var pos = pictureBox1.PointToClient(Cursor.Position);

            _controller.SendMouseEvent(MouseEventPipeMessage.TYPE_MOUSE_DOWN, pos.X, pos.Y, (CefUnityLib.Helpers.MouseButtons)e.Button);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            var pos = pictureBox1.PointToClient(Cursor.Position);

            _controller.SendMouseEvent(MouseEventPipeMessage.TYPE_MOUSE_UP, pos.X, pos.Y, (CefUnityLib.Helpers.MouseButtons)e.Button);
        }
        
        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            _controller.SendMouseWheelEvent(e.X, e.Y, e.Delta);
        }

        private void btnShut_Click(object sender, EventArgs e)
        {
            _controller.SendShutdownMessage();
        }

        private void btnDis_Click(object sender, EventArgs e)
        {
            _controller.Disconnect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
