﻿using System;
using leanvision;
namespace ctmeasure
{
    partial class frmsetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmsetting));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cbcolor6 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbcolor5 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbcolor4 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbcolor3 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbcolor2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbcolor1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnok = new System.Windows.Forms.Button();
            this.btncancel = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.rbdata = new System.Windows.Forms.RadioButton();
            this.rbstatistic = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.tmove = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnfile = new System.Windows.Forms.Button();
            this.tbfile = new System.Windows.Forms.TextBox();
            this.btnpic = new System.Windows.Forms.Button();
            this.tbpic = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tfontsize = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.typixel = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txpixel = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.cbcolor6);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cbcolor5);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cbcolor4);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cbcolor3);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cbcolor2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbcolor1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(25, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(278, 238);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "  颜色设置  ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(22, 209);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 12;
            this.label9.Text = "文字大小：";
            // 
            // cbcolor6
            // 
            this.cbcolor6.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbcolor6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbcolor6.FormattingEnabled = true;
            this.cbcolor6.Items.AddRange(new object[] {
            "white",
            "black",
            "gray",
            "salmon",
            "coral",
            "red",
            "firebrick",
            "maroon",
            "pink",
            "thistle",
            "plum",
            "violet",
            "magenta",
            "wheat",
            "tan",
            "khaki",
            "gold",
            "orange",
            "yellow",
            "aquamarine",
            "turquoise",
            "cyan",
            "green",
            "blue",
            "navy"});
            this.cbcolor6.Location = new System.Drawing.Point(87, 172);
            this.cbcolor6.Name = "cbcolor6";
            this.cbcolor6.Size = new System.Drawing.Size(151, 22);
            this.cbcolor6.TabIndex = 11;
            this.cbcolor6.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox1_DrawItem);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 177);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "文字颜色：";
            // 
            // cbcolor5
            // 
            this.cbcolor5.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbcolor5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbcolor5.FormattingEnabled = true;
            this.cbcolor5.Items.AddRange(new object[] {
            "white",
            "black",
            "gray",
            "salmon",
            "coral",
            "red",
            "firebrick",
            "maroon",
            "pink",
            "thistle",
            "plum",
            "violet",
            "magenta",
            "wheat",
            "tan",
            "khaki",
            "gold",
            "orange",
            "yellow",
            "aquamarine",
            "turquoise",
            "cyan",
            "green",
            "blue",
            "navy"});
            this.cbcolor5.Location = new System.Drawing.Point(87, 143);
            this.cbcolor5.Name = "cbcolor5";
            this.cbcolor5.Size = new System.Drawing.Size(151, 22);
            this.cbcolor5.TabIndex = 9;
            this.cbcolor5.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox1_DrawItem);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "选框颜色：";
            // 
            // cbcolor4
            // 
            this.cbcolor4.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbcolor4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbcolor4.FormattingEnabled = true;
            this.cbcolor4.Items.AddRange(new object[] {
            "white",
            "black",
            "gray",
            "salmon",
            "coral",
            "red",
            "firebrick",
            "maroon",
            "pink",
            "thistle",
            "plum",
            "violet",
            "magenta",
            "wheat",
            "tan",
            "khaki",
            "gold",
            "orange",
            "yellow",
            "aquamarine",
            "turquoise",
            "cyan",
            "green",
            "blue",
            "navy"});
            this.cbcolor4.Location = new System.Drawing.Point(87, 115);
            this.cbcolor4.Name = "cbcolor4";
            this.cbcolor4.Size = new System.Drawing.Size(151, 22);
            this.cbcolor4.TabIndex = 7;
            this.cbcolor4.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox1_DrawItem);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "识别区域：";
            // 
            // cbcolor3
            // 
            this.cbcolor3.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbcolor3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbcolor3.FormattingEnabled = true;
            this.cbcolor3.Items.AddRange(new object[] {
            "white",
            "black",
            "gray",
            "salmon",
            "coral",
            "red",
            "firebrick",
            "maroon",
            "pink",
            "thistle",
            "plum",
            "violet",
            "magenta",
            "wheat",
            "tan",
            "khaki",
            "gold",
            "orange",
            "yellow",
            "aquamarine",
            "turquoise",
            "cyan",
            "green",
            "blue",
            "navy"});
            this.cbcolor3.Location = new System.Drawing.Point(87, 87);
            this.cbcolor3.Name = "cbcolor3";
            this.cbcolor3.Size = new System.Drawing.Size(151, 22);
            this.cbcolor3.TabIndex = 5;
            this.cbcolor3.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox1_DrawItem);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "调节手柄：";
            // 
            // cbcolor2
            // 
            this.cbcolor2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbcolor2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbcolor2.FormattingEnabled = true;
            this.cbcolor2.Items.AddRange(new object[] {
            "white",
            "black",
            "gray",
            "salmon",
            "coral",
            "red",
            "firebrick",
            "maroon",
            "pink",
            "thistle",
            "plum",
            "violet",
            "magenta",
            "wheat",
            "tan",
            "khaki",
            "gold",
            "orange",
            "yellow",
            "aquamarine",
            "turquoise",
            "cyan",
            "green",
            "blue",
            "navy"});
            this.cbcolor2.Location = new System.Drawing.Point(87, 59);
            this.cbcolor2.Name = "cbcolor2";
            this.cbcolor2.Size = new System.Drawing.Size(151, 22);
            this.cbcolor2.TabIndex = 3;
            this.cbcolor2.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox1_DrawItem);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "选中边框：";
            // 
            // cbcolor1
            // 
            this.cbcolor1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbcolor1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbcolor1.FormattingEnabled = true;
            this.cbcolor1.Items.AddRange(new object[] {
            "white",
            "black",
            "gray",
            "salmon",
            "coral",
            "red",
            "firebrick",
            "maroon",
            "pink",
            "thistle",
            "plum",
            "violet",
            "magenta",
            "wheat",
            "tan",
            "khaki",
            "gold",
            "orange",
            "yellow",
            "aquamarine",
            "turquoise",
            "cyan",
            "green",
            "blue",
            "navy"});
            this.cbcolor1.Location = new System.Drawing.Point(87, 31);
            this.cbcolor1.Name = "cbcolor1";
            this.cbcolor1.Size = new System.Drawing.Size(151, 22);
            this.cbcolor1.TabIndex = 1;
            this.cbcolor1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox1_DrawItem);
            this.cbcolor1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbcolor1_MouseMove);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "区域边框：";
            // 
            // btnok
            // 
            this.btnok.Location = new System.Drawing.Point(102, 378);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(86, 35);
            this.btnok.TabIndex = 1;
            this.btnok.Text = "确  定";
            this.btnok.UseVisualStyleBackColor = true;
            this.btnok.Click += new System.EventHandler(this.btnok_Click);
            // 
            // btncancel
            // 
            this.btncancel.Location = new System.Drawing.Point(230, 378);
            this.btncancel.Name = "btncancel";
            this.btncancel.Size = new System.Drawing.Size(86, 35);
            this.btncancel.TabIndex = 2;
            this.btncancel.Text = "取  消";
            this.btncancel.UseVisualStyleBackColor = true;
            this.btncancel.Click += new System.EventHandler(this.btncancel_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(65, 308);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "结果显示：";
            // 
            // rbdata
            // 
            this.rbdata.AutoSize = true;
            this.rbdata.Location = new System.Drawing.Point(136, 308);
            this.rbdata.Name = "rbdata";
            this.rbdata.Size = new System.Drawing.Size(71, 16);
            this.rbdata.TabIndex = 12;
            this.rbdata.TabStop = true;
            this.rbdata.Text = "测量结果";
            this.rbdata.UseVisualStyleBackColor = true;
            // 
            // rbstatistic
            // 
            this.rbstatistic.AutoSize = true;
            this.rbstatistic.Location = new System.Drawing.Point(213, 308);
            this.rbstatistic.Name = "rbstatistic";
            this.rbstatistic.Size = new System.Drawing.Size(71, 16);
            this.rbstatistic.TabIndex = 13;
            this.rbstatistic.TabStop = true;
            this.rbstatistic.Text = "测量统计";
            this.rbstatistic.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(41, 347);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 12);
            this.label8.TabIndex = 14;
            this.label8.Text = "十字微调参数：";
            // 
            // tmove
            // 
            this.tmove.Location = new System.Drawing.Point(136, 344);
            this.tmove.Name = "tmove";
            this.tmove.Size = new System.Drawing.Size(145, 21);
            this.tmove.TabIndex = 15;
            this.tmove.Text = "5";
            this.tmove.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tmove_KeyPress);
            this.tmove.Leave += new System.EventHandler(this.tmove_Leave);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.btnfile);
            this.groupBox2.Controls.Add(this.tbfile);
            this.groupBox2.Controls.Add(this.btnpic);
            this.groupBox2.Controls.Add(this.tbpic);
            this.groupBox2.Location = new System.Drawing.Point(64, 32);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(265, 100);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "追踪路径";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(23, 62);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 12);
            this.label11.TabIndex = 5;
            this.label11.Text = "文件";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(23, 27);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 4;
            this.label10.Text = "图片";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // btnfile
            // 
            this.btnfile.Location = new System.Drawing.Point(214, 62);
            this.btnfile.Name = "btnfile";
            this.btnfile.Size = new System.Drawing.Size(45, 23);
            this.btnfile.TabIndex = 3;
            this.btnfile.Text = "选择";
            this.btnfile.UseVisualStyleBackColor = true;
            this.btnfile.Click += new System.EventHandler(this.btnfile_Click);
            // 
            // tbfile
            // 
            this.tbfile.Location = new System.Drawing.Point(58, 59);
            this.tbfile.Name = "tbfile";
            this.tbfile.Size = new System.Drawing.Size(145, 21);
            this.tbfile.TabIndex = 2;
            // 
            // btnpic
            // 
            this.btnpic.Location = new System.Drawing.Point(214, 22);
            this.btnpic.Name = "btnpic";
            this.btnpic.Size = new System.Drawing.Size(45, 23);
            this.btnpic.TabIndex = 1;
            this.btnpic.Text = "选择";
            this.btnpic.UseVisualStyleBackColor = true;
            this.btnpic.Click += new System.EventHandler(this.btnpic_Click);
            // 
            // tbpic
            // 
            this.tbpic.Location = new System.Drawing.Point(58, 24);
            this.tbpic.Name = "tbpic";
            this.tbpic.Size = new System.Drawing.Size(145, 21);
            this.tbpic.TabIndex = 0;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(65, 267);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 17;
            this.label12.Text = "字体大小：";
            // 
            // tfontsize
            // 
            this.tfontsize.Location = new System.Drawing.Point(136, 264);
            this.tfontsize.Name = "tfontsize";
            this.tfontsize.Size = new System.Drawing.Size(148, 21);
            this.tfontsize.TabIndex = 18;
            this.tfontsize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tfontsize_KeyPress);
            this.tfontsize.Leave += new System.EventHandler(this.tfontsize_Leave);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.typixel);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.txpixel);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Location = new System.Drawing.Point(64, 150);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(265, 98);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "相机标定  ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(171, 59);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 12);
            this.label13.TabIndex = 23;
            this.label13.Text = "um/pixel  ";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(171, 29);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 12);
            this.label14.TabIndex = 22;
            this.label14.Text = "um/pixel  ";
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
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(15, 58);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(71, 12);
            this.label15.TabIndex = 19;
            this.label15.Text = "垂直方向:  ";
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
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 29);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(71, 12);
            this.label16.TabIndex = 16;
            this.label16.Text = "水平方向:  ";
            // 
            // frmsetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 485);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.tfontsize);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.tmove);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.rbstatistic);
            this.Controls.Add(this.rbdata);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btncancel);
            this.Controls.Add(this.btnok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmsetting";
            this.ShowInTaskbar = false;
            this.Text = "系统设置";
            this.Load += new System.EventHandler(this.frmsetting_Load);
            this.Shown += new System.EventHandler(this.frmsetting_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void frmsetting_Shown(object sender, EventArgs e)
        {
            this.tbpic.Text = vcommon.picpath;
            this.tbfile.Text = vcommon.filepath;
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbcolor1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnok;
        private System.Windows.Forms.ComboBox cbcolor5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbcolor4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbcolor3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbcolor2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btncancel;
        private System.Windows.Forms.ComboBox cbcolor6;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton rbdata;
        private System.Windows.Forms.RadioButton rbstatistic;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tmove;
        //private System.Windows.Forms.TextBox tfontsize;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnfile;
        private System.Windows.Forms.TextBox tbfile;
        private System.Windows.Forms.Button btnpic;
        private System.Windows.Forms.TextBox tbpic;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tfontsize;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox typixel;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txpixel;
        private System.Windows.Forms.Label label16;
    }
}