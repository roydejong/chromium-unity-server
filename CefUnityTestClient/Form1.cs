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
        private Bitmap _texture;
        private CefController controller;

        private ulong frameCounterStat;

        private int frameCounterCounter;
        private int frameCounterCurrent;

        public Form1()
        {
            InitializeComponent();

            int defWidth = 1024;
            int defHeight = 768;

            _texture = new Bitmap(defWidth, defHeight);

            controller = new CefController();
            controller.MessageReceived += MessageReceived;
            controller.ConnectionStateChanged += ConnectionChanged;

            this.KeyPreview = true;
            this.KeyUp += Form1_KeyUp;
            this.KeyDown += Form1_KeyDown;
            this.KeyPress += Form1_KeyPress;
            this.PreviewKeyDown += Form1_PreviewKeyDown;

            pictureBox1.MouseMove += PictureBox1_MouseMove;

            this.Shown += Form1_Shown;

            SetKeyEventsForControls(Controls);
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
            ConnectionChanged(controller, false);

            Task.Run(new Action(() =>
            {
                while (this.Visible && !this.Disposing && !this.IsDisposed)
                {
                    bool haveFps = false;
                    int fps = 0;

                    if (frameCounterCounter >= 10)
                    {
                        fps = frameCounterCurrent;

                        frameCounterCounter = 0;
                        frameCounterCurrent = 0;

                        haveFps = true;
                    }

                    frameCounterCounter++;

                    Invoke(new Action(() =>
                    {
                        if (controller != null && controller.Connected)
                        {
                            if (frameCounterStat > 0)
                                lblFrames.Text = frameCounterStat.ToString();

                            if (controller.MessagesReceivedCount > 0)
                                lblPkIn.Text = controller.MessagesReceivedCount.ToString();

                            if (controller.MessagesSentCount > 0)
                                lblPkOut.Text = controller.MessagesSentCount.ToString();

                            if (haveFps)
                                lblFps.Text = fps.ToString() + " FPS";
                        }
                    }));

                    Thread.Sleep(100);
                }
            }));
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
            controller.SendMessage(new KeyEventPipeMessage(KeyEventPipeMessage.TYPE_KEY_CHAR, (int)e.KeyChar));
            e.Handled = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            pictureBox1.Focus();
            controller.SendMessage(new KeyEventPipeMessage(KeyEventPipeMessage.TYPE_KEY_DOWN, (int)e.KeyCode));
            e.Handled = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            pictureBox1.Focus();
            controller.SendMessage(new KeyEventPipeMessage(KeyEventPipeMessage.TYPE_KEY_UP, (int)e.KeyCode));
            e.Handled = true;
        }

        public void MessageReceived(object sender, PipeProtoMessage e)
        {
            switch (e.Opcode)
            {
                case PipeProto.OPCODE_FRAME:

                    frameCounterStat++;
                    frameCounterCurrent++;

                    Rectangle rect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
                    BitmapData bmpData = _texture.LockBits(rect, ImageLockMode.WriteOnly, _texture.PixelFormat);
                    IntPtr ptr = bmpData.Scan0;
                    System.Runtime.InteropServices.Marshal.Copy(e.Payload, 0, ptr, e.Payload.Length);
                    _texture.UnlockBits(bmpData);
                    pictureBox1.Image = _texture;
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
                controller.Connect();
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

            controller.SendMouseEvent(MouseEventPipeMessage.TYPE_MOVE, pos.X, pos.Y, (CefUnityLib.Helpers.MouseButtons)e.Button);

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            var pos = pictureBox1.PointToClient(Cursor.Position);

            controller.SendMouseEvent(MouseEventPipeMessage.TYPE_MOUSE_DOWN, pos.X, pos.Y, (CefUnityLib.Helpers.MouseButtons)e.Button);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            var pos = pictureBox1.PointToClient(Cursor.Position);

            controller.SendMouseEvent(MouseEventPipeMessage.TYPE_MOUSE_UP, pos.X, pos.Y, (CefUnityLib.Helpers.MouseButtons)e.Button);
        }

        private void btnShut_Click(object sender, EventArgs e)
        {
            controller.SendShutdownMessage();
        }

        private void btnDis_Click(object sender, EventArgs e)
        {
            controller.Disconnect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
