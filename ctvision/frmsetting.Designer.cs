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
            this.tfontsize = new System.Windows.Forms.TextBox();
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
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tfontsize);
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
            // tfontsize
            // 
            this.tfontsize.Location = new System.Drawing.Point(86, 204);
            this.tfontsize.Name = "tfontsize";
            this.tfontsize.Size = new System.Drawing.Size(151, 21);
            this.tfontsize.TabIndex = 13;
            this.tfontsize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tfontsize_KeyPress);
            this.tfontsize.Leave += new System.EventHandler(this.tfontsize_Leave);
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
            this.btnok.Location = new System.Drawing.Point(49, 348);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(86, 35);
            this.btnok.TabIndex = 1;
            this.btnok.Text = "确  定";
            this.btnok.UseVisualStyleBackColor = true;
            this.btnok.Click += new System.EventHandler(this.btnok_Click);
            // 
            // btncancel
            // 
            this.btncancel.Location = new System.Drawing.Point(177, 348);
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
            this.label7.Location = new System.Drawing.Point(47, 284);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "结果显示：";
            // 
            // rbdata
            // 
            this.rbdata.AutoSize = true;
            this.rbdata.Location = new System.Drawing.Point(117, 283);
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
            this.rbstatistic.Location = new System.Drawing.Point(194, 282);
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
            this.label8.Location = new System.Drawing.Point(23, 315);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 12);
            this.label8.TabIndex = 14;
            this.label8.Text = "十字微调参数：";
            // 
            // tmove
            // 
            this.tmove.Location = new System.Drawing.Point(118, 312);
            this.tmove.Name = "tmove";
            this.tmove.Size = new System.Drawing.Size(145, 21);
            this.tmove.TabIndex = 15;
            this.tmove.Text = "5";
            this.tmove.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tmove_KeyPress);
            this.tmove.Leave += new System.EventHandler(this.tmove_Leave);
            // 
            // frmsetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 402);
            this.Controls.Add(this.tmove);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.rbstatistic);
            this.Controls.Add(this.rbdata);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btncancel);
            this.Controls.Add(this.btnok);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmsetting";
            this.ShowInTaskbar = false;
            this.Text = "系统设置";
            this.Load += new System.EventHandler(this.frmsetting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.TextBox tfontsize;
        private System.Windows.Forms.Label label9;
    }
}