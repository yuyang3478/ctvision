namespace ctmeasure
{
    partial class frmio
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmio));
            this.cbcomport = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ttriggerdelay = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ttrigger = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tngoff = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tngtime = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tngon = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tokoff = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.toktime = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tokon = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnclose = new System.Windows.Forms.Button();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.triggershape = new Microsoft.VisualBasic.PowerPacks.OvalShape();
            this.btnsend = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ckok = new System.Windows.Forms.CheckBox();
            this.ckng = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbcomport
            // 
            this.cbcomport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbcomport.FormattingEnabled = true;
            this.cbcomport.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9"});
            this.cbcomport.Location = new System.Drawing.Point(87, 12);
            this.cbcomport.Name = "cbcomport";
            this.cbcomport.Size = new System.Drawing.Size(245, 20);
            this.cbcomport.TabIndex = 24;
            this.cbcomport.SelectedIndexChanged += new System.EventHandler(this.cbcomport_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(29, 17);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 12);
            this.label12.TabIndex = 23;
            this.label12.Text = "端口选择:  ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ttriggerdelay);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ttrigger);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(27, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 81);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "  相机触发  ";
            // 
            // ttriggerdelay
            // 
            this.ttriggerdelay.Location = new System.Drawing.Point(119, 51);
            this.ttriggerdelay.Name = "ttriggerdelay";
            this.ttriggerdelay.Size = new System.Drawing.Size(165, 21);
            this.ttriggerdelay.TabIndex = 22;
            this.ttriggerdelay.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 21;
            this.label1.Text = "触发延时（ms）:  ";
            // 
            // ttrigger
            // 
            this.ttrigger.Location = new System.Drawing.Point(87, 24);
            this.ttrigger.Name = "ttrigger";
            this.ttrigger.Size = new System.Drawing.Size(197, 21);
            this.ttrigger.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 19;
            this.label6.Text = "触发信号:  ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tngoff);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tngtime);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.tngon);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.tokoff);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.toktime);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tokon);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(28, 146);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(304, 201);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "  OK/NG信号  ";
            // 
            // tngoff
            // 
            this.tngoff.Location = new System.Drawing.Point(87, 140);
            this.tngoff.Name = "tngoff";
            this.tngoff.Size = new System.Drawing.Size(197, 21);
            this.tngoff.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 145);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 29;
            this.label5.Text = "NG 关:  ";
            // 
            // tngtime
            // 
            this.tngtime.Location = new System.Drawing.Point(118, 167);
            this.tngtime.Name = "tngtime";
            this.tngtime.Size = new System.Drawing.Size(165, 21);
            this.tngtime.TabIndex = 28;
            this.tngtime.Text = "50";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 12);
            this.label7.TabIndex = 27;
            this.label7.Text = "NG延时（ms）:  ";
            // 
            // tngon
            // 
            this.tngon.Location = new System.Drawing.Point(87, 113);
            this.tngon.Name = "tngon";
            this.tngon.Size = new System.Drawing.Size(197, 21);
            this.tngon.TabIndex = 26;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 118);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 25;
            this.label8.Text = "NG 开:  ";
            // 
            // tokoff
            // 
            this.tokoff.Location = new System.Drawing.Point(87, 51);
            this.tokoff.Name = "tokoff";
            this.tokoff.Size = new System.Drawing.Size(197, 21);
            this.tokoff.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 23;
            this.label4.Text = "OK 关:  ";
            // 
            // toktime
            // 
            this.toktime.Location = new System.Drawing.Point(118, 78);
            this.toktime.Name = "toktime";
            this.toktime.Size = new System.Drawing.Size(165, 21);
            this.toktime.TabIndex = 22;
            this.toktime.Text = "50";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 12);
            this.label2.TabIndex = 21;
            this.label2.Text = "OK延时（ms）:  ";
            // 
            // tokon
            // 
            this.tokon.Location = new System.Drawing.Point(87, 24);
            this.tokon.Name = "tokon";
            this.tokon.Size = new System.Drawing.Size(197, 21);
            this.tokon.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 19;
            this.label3.Text = "OK 开:  ";
            // 
            // btnclose
            // 
            this.btnclose.Location = new System.Drawing.Point(115, 362);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(122, 36);
            this.btnclose.TabIndex = 28;
            this.btnclose.Text = "确  定";
            this.btnclose.UseVisualStyleBackColor = true;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.triggershape});
            this.shapeContainer1.Size = new System.Drawing.Size(387, 410);
            this.shapeContainer1.TabIndex = 29;
            this.shapeContainer1.TabStop = false;
            // 
            // triggershape
            // 
            this.triggershape.BackColor = System.Drawing.Color.White;
            this.triggershape.FillColor = System.Drawing.Color.White;
            this.triggershape.FillStyle = Microsoft.VisualBasic.PowerPacks.FillStyle.Solid;
            this.triggershape.Location = new System.Drawing.Point(336, 74);
            this.triggershape.Name = "triggershape";
            this.triggershape.Size = new System.Drawing.Size(19, 19);
            // 
            // btnsend
            // 
            this.btnsend.Location = new System.Drawing.Point(338, 200);
            this.btnsend.Name = "btnsend";
            this.btnsend.Size = new System.Drawing.Size(20, 20);
            this.btnsend.TabIndex = 30;
            this.btnsend.Text = ">";
            this.btnsend.UseVisualStyleBackColor = true;
            this.btnsend.Click += new System.EventHandler(this.btnsend_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(338, 287);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(20, 20);
            this.button1.TabIndex = 31;
            this.button1.Text = ">";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(337, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(20, 20);
            this.button2.TabIndex = 32;
            this.button2.Text = "V";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ckok
            // 
            this.ckok.AutoSize = true;
            this.ckok.Checked = true;
            this.ckok.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckok.Location = new System.Drawing.Point(341, 173);
            this.ckok.Name = "ckok";
            this.ckok.Size = new System.Drawing.Size(15, 14);
            this.ckok.TabIndex = 33;
            this.ckok.UseVisualStyleBackColor = true;
            // 
            // ckng
            // 
            this.ckng.AutoSize = true;
            this.ckng.Checked = true;
            this.ckng.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckng.Location = new System.Drawing.Point(341, 259);
            this.ckng.Name = "ckng";
            this.ckng.Size = new System.Drawing.Size(15, 14);
            this.ckng.TabIndex = 34;
            this.ckng.UseVisualStyleBackColor = true;
            // 
            // frmio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 410);
            this.Controls.Add(this.ckng);
            this.Controls.Add(this.ckok);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnsend);
            this.Controls.Add(this.btnclose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbcomport);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmio";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IO 设置";
            this.Load += new System.EventHandler(this.frmio_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbcomport;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox ttriggerdelay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ttrigger;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tngoff;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tngtime;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tngon;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tokoff;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox toktime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tokon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnclose;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.OvalShape triggershape;
        private System.Windows.Forms.Button btnsend;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox ckok;
        private System.Windows.Forms.CheckBox ckng;
    }
}