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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefUnityTestClient
{
    public partial class Form1 : Form
    {
        private Bitmap _texture;
        private CefController controller;

        public Form1()
        {
            InitializeComponent();

            int defWidth = 1024;
            int defHeight = 768;

            _texture = new Bitmap(defWidth, defHeight);

            controller = new CefController();
            controller.MessageReceived += MessageReceived;
        }

        public void MessageReceived(object sender, PipeProtoMessage e)
        {
            switch (e.Opcode)
            {
                case PipeProto.OPCODE_FRAME:
                    
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
            controller.Connect();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            var pos = pictureBox1.PointToClient(Cursor.Position);

            controller.SendMouseEvent(MouseEventPipeMessage.TYPE_MOVE, pos.X, pos.Y);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            var pos = pictureBox1.PointToClient(Cursor.Position);

            controller.SendMouseEvent(MouseEventPipeMessage.TYPE_MOUSE_DOWN, pos.X, pos.Y);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            var pos = pictureBox1.PointToClient(Cursor.Position);

            controller.SendMouseEvent(MouseEventPipeMessage.TYPE_MOUSE_UP, pos.X, pos.Y);
        }
    }
}
