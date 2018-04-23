namespace CefUnityTestClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnCon = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnDis = new System.Windows.Forms.Button();
            this.btnShut = new System.Windows.Forms.Button();
            this.lblFrames = new System.Windows.Forms.Label();
            this.lblPkIn = new System.Windows.Forms.Label();
            this.lblPkOut = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblFps = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCon
            // 
            this.btnCon.Location = new System.Drawing.Point(12, 12);
            this.btnCon.Name = "btnCon";
            this.btnCon.Size = new System.Drawing.Size(126, 37);
            this.btnCon.TabIndex = 0;
            this.btnCon.Text = "Connect";
            this.btnCon.UseVisualStyleBackColor = true;
            this.btnCon.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(12, 65);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1024, 768);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // btnDis
            // 
            this.btnDis.Location = new System.Drawing.Point(144, 12);
            this.btnDis.Name = "btnDis";
            this.btnDis.Size = new System.Drawing.Size(126, 37);
            this.btnDis.TabIndex = 3;
            this.btnDis.Text = "Disconnect";
            this.btnDis.UseVisualStyleBackColor = true;
            this.btnDis.Click += new System.EventHandler(this.btnDis_Click);
            // 
            // btnShut
            // 
            this.btnShut.Location = new System.Drawing.Point(276, 12);
            this.btnShut.Name = "btnShut";
            this.btnShut.Size = new System.Drawing.Size(126, 37);
            this.btnShut.TabIndex = 4;
            this.btnShut.Text = "Shut down server";
            this.btnShut.UseVisualStyleBackColor = true;
            this.btnShut.Click += new System.EventHandler(this.btnShut_Click);
            // 
            // lblFrames
            // 
            this.lblFrames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFrames.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblFrames.Location = new System.Drawing.Point(934, 9);
            this.lblFrames.Name = "lblFrames";
            this.lblFrames.Size = new System.Drawing.Size(100, 16);
            this.lblFrames.TabIndex = 5;
            this.lblFrames.Text = "Frame counter";
            this.lblFrames.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.lblFrames, "Frame counter");
            // 
            // lblPkIn
            // 
            this.lblPkIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPkIn.ForeColor = System.Drawing.Color.Blue;
            this.lblPkIn.Location = new System.Drawing.Point(934, 25);
            this.lblPkIn.Name = "lblPkIn";
            this.lblPkIn.Size = new System.Drawing.Size(100, 16);
            this.lblPkIn.TabIndex = 6;
            this.lblPkIn.Text = "Packets in";
            this.lblPkIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.lblPkIn, "Packets in");
            // 
            // lblPkOut
            // 
            this.lblPkOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPkOut.ForeColor = System.Drawing.Color.Fuchsia;
            this.lblPkOut.Location = new System.Drawing.Point(934, 41);
            this.lblPkOut.Name = "lblPkOut";
            this.lblPkOut.Size = new System.Drawing.Size(100, 16);
            this.lblPkOut.TabIndex = 7;
            this.lblPkOut.Text = "Packets out";
            this.lblPkOut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.lblPkOut, "Packets out");
            // 
            // lblFps
            // 
            this.lblFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblFps.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFps.ForeColor = System.Drawing.Color.Yellow;
            this.lblFps.Location = new System.Drawing.Point(792, 12);
            this.lblFps.Name = "lblFps";
            this.lblFps.Size = new System.Drawing.Size(110, 37);
            this.lblFps.TabIndex = 8;
            this.lblFps.Text = "FPS Counter";
            this.lblFps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1046, 842);
            this.Controls.Add(this.lblFps);
            this.Controls.Add(this.lblPkOut);
            this.Controls.Add(this.lblPkIn);
            this.Controls.Add(this.lblFrames);
            this.Controls.Add(this.btnShut);
            this.Controls.Add(this.btnDis);
            this.Controls.Add(this.btnCon);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CEF Unity Server - Test Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCon;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnDis;
        private System.Windows.Forms.Button btnShut;
        private System.Windows.Forms.Label lblFrames;
        private System.Windows.Forms.Label lblPkIn;
        private System.Windows.Forms.Label lblPkOut;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblFps;
    }
}

