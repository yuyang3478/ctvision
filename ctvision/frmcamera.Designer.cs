namespace ctmeasure
{
    partial class frmcamera
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmcamera));
            this.cbcameras = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnclose = new System.Windows.Forms.Button();
            this.tplay = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tcontrast = new System.Windows.Forms.TextBox();
            this.tbcontrast = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.tgamma = new System.Windows.Forms.TextBox();
            this.tbgamma = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.tgain = new System.Windows.Forms.TextBox();
            this.tbgain = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.texposuretime = new System.Windows.Forms.TextBox();
            this.tbexposuretime = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.typixel = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txpixel = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbcontrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbgamma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbgain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbexposuretime)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbcameras
            // 
            this.cbcameras.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbcameras.FormattingEnabled = true;
            this.cbcameras.Location = new System.Drawing.Point(85, 21);
            this.cbcameras.Name = "cbcameras";
            this.cbcameras.Size = new System.Drawing.Size(245, 20);
            this.cbcameras.TabIndex = 22;
            this.cbcameras.SelectedIndexChanged += new System.EventHandler(this.cbcameras_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(21, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 12);
            this.label12.TabIndex = 21;
            this.label12.Text = "相机选择:  ";
            // 
            // btnclose
            // 
            this.btnclose.Location = new System.Drawing.Point(105, 360);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(119, 38);
            this.btnclose.TabIndex = 23;
            this.btnclose.Text = "确  定";
            this.btnclose.UseVisualStyleBackColor = true;
            this.btnclose.Click += new System.EventHandler(this.button1_Click);
            // 
            // tplay
            // 
            this.tplay.Interval = 80;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tcontrast);
            this.groupBox1.Controls.Add(this.tbcontrast);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tgamma);
            this.groupBox1.Controls.Add(this.tbgamma);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tgain);
            this.groupBox1.Controls.Add(this.tbgain);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.texposuretime);
            this.groupBox1.Controls.Add(this.tbexposuretime);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(23, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 160);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "  相机设置  ";
            // 
            // tcontrast
            // 
            this.tcontrast.Location = new System.Drawing.Point(250, 121);
            this.tcontrast.Name = "tcontrast";
            this.tcontrast.ReadOnly = true;
            this.tcontrast.Size = new System.Drawing.Size(55, 21);
            this.tcontrast.TabIndex = 27;
            // 
            // tbcontrast
            // 
            this.tbcontrast.AutoSize = false;
            this.tbcontrast.BackColor = System.Drawing.SystemColors.Control;
            this.tbcontrast.LargeChange = 20;
            this.tbcontrast.Location = new System.Drawing.Point(60, 121);
            this.tbcontrast.Margin = new System.Windows.Forms.Padding(0);
            this.tbcontrast.Maximum = 200;
            this.tbcontrast.Name = "tbcontrast";
            this.tbcontrast.Size = new System.Drawing.Size(187, 19);
            this.tbcontrast.SmallChange = 10;
            this.tbcontrast.TabIndex = 26;
            this.tbcontrast.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbcontrast.Value = 100;
            this.tbcontrast.ValueChanged += new System.EventHandler(this.tbexposuretime_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 122);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 25;
            this.label8.Text = "对比度:  ";
            // 
            // tgamma
            // 
            this.tgamma.Location = new System.Drawing.Point(250, 87);
            this.tgamma.Name = "tgamma";
            this.tgamma.ReadOnly = true;
            this.tgamma.Size = new System.Drawing.Size(55, 21);
            this.tgamma.TabIndex = 24;
            // 
            // tbgamma
            // 
            this.tbgamma.AutoSize = false;
            this.tbgamma.BackColor = System.Drawing.SystemColors.Control;
            this.tbgamma.LargeChange = 20;
            this.tbgamma.Location = new System.Drawing.Point(60, 87);
            this.tbgamma.Margin = new System.Windows.Forms.Padding(0);
            this.tbgamma.Maximum = 400;
            this.tbgamma.Minimum = 100;
            this.tbgamma.Name = "tbgamma";
            this.tbgamma.Size = new System.Drawing.Size(187, 19);
            this.tbgamma.SmallChange = 10;
            this.tbgamma.TabIndex = 23;
            this.tbgamma.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbgamma.Value = 100;
            this.tbgamma.ValueChanged += new System.EventHandler(this.tbexposuretime_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 22;
            this.label2.Text = "Gamma:  ";
            // 
            // tgain
            // 
            this.tgain.Location = new System.Drawing.Point(250, 57);
            this.tgain.Name = "tgain";
            this.tgain.ReadOnly = true;
            this.tgain.Size = new System.Drawing.Size(55, 21);
            this.tgain.TabIndex = 21;
            // 
            // tbgain
            // 
            this.tbgain.AutoSize = false;
            this.tbgain.BackColor = System.Drawing.SystemColors.Control;
            this.tbgain.LargeChange = 20;
            this.tbgain.Location = new System.Drawing.Point(60, 57);
            this.tbgain.Margin = new System.Windows.Forms.Padding(0);
            this.tbgain.Maximum = 800;
            this.tbgain.Minimum = 100;
            this.tbgain.Name = "tbgain";
            this.tbgain.Size = new System.Drawing.Size(187, 19);
            this.tbgain.SmallChange = 10;
            this.tbgain.TabIndex = 20;
            this.tbgain.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbgain.Value = 100;
            this.tbgain.ValueChanged += new System.EventHandler(this.tbexposuretime_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 19;
            this.label1.Text = "增  益:  ";
            // 
            // texposuretime
            // 
            this.texposuretime.Location = new System.Drawing.Point(250, 28);
            this.texposuretime.Name = "texposuretime";
            this.texposuretime.ReadOnly = true;
            this.texposuretime.Size = new System.Drawing.Size(55, 21);
            this.texposuretime.TabIndex = 18;
            // 
            // tbexposuretime
            // 
            this.tbexposuretime.AutoSize = false;
            this.tbexposuretime.BackColor = System.Drawing.SystemColors.Control;
            this.tbexposuretime.LargeChange = 50;
            this.tbexposuretime.Location = new System.Drawing.Point(60, 28);
            this.tbexposuretime.Margin = new System.Windows.Forms.Padding(0);
            this.tbexposuretime.Maximum = 50000;
            this.tbexposuretime.Minimum = 1000;
            this.tbexposuretime.Name = "tbexposuretime";
            this.tbexposuretime.Size = new System.Drawing.Size(187, 19);
            this.tbexposuretime.SmallChange = 10;
            this.tbexposuretime.TabIndex = 17;
            this.tbexposuretime.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbexposuretime.Value = 1000;
            this.tbexposuretime.ValueChanged += new System.EventHandler(this.tbexposuretime_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "曝  光:  ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.typixel);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txpixel);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(23, 240);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(311, 98);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "  相机标定  ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(171, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 23;
            this.label7.Text = "um/pixel  ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(171, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 22;
            this.label3.Text = "um/pixel  ";
            // 
            // typixel
            // 
            this.typixel.Location = new System.Drawing.Point(82, 55);
            this.typixel.Name = "typixel";
            this.typixel.Size = new System.Drawing.Size(83, 21);
            this.typixel.TabIndex = 21;
            this.typixel.Text = "10.00";
            this.typixel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txpixel_KeyPress);
            this.typixel.Leave += new System.EventHandler(this.txpixel_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 19;
            this.label4.Text = "垂直方向:  ";
            // 
            // txpixel
            // 
            this.txpixel.Location = new System.Drawing.Point(82, 24);
            this.txpixel.Name = "txpixel";
            this.txpixel.Size = new System.Drawing.Size(83, 21);
            this.txpixel.TabIndex = 18;
            this.txpixel.Text = "10.00";
            this.txpixel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txpixel_KeyPress);
            this.txpixel.Leave += new System.EventHandler(this.txpixel_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 16;
            this.label6.Text = "水平方向:  ";
            // 
            // frmcamera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 420);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnclose);
            this.Controls.Add(this.cbcameras);
            this.Controls.Add(this.label12);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmcamera";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "相机选择及设置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmcamera_FormClosing);
            this.Load += new System.EventHandler(this.frmcamera_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbcontrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbgamma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbgain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbexposuretime)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbcameras;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.Timer tplay;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tgamma;
        private System.Windows.Forms.TrackBar tbgamma;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tgain;
        private System.Windows.Forms.TrackBar tbgain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox texposuretime;
        private System.Windows.Forms.TrackBar tbexposuretime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox typixel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txpixel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tcontrast;
        private System.Windows.Forms.TrackBar tbcontrast;
        private System.Windows.Forms.Label label8;
    }
}