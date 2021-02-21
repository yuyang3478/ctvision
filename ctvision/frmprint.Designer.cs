namespace ctmeasure
{
    partial class frmprint
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtworker = new System.Windows.Forms.TextBox();
            this.txtdevice = new System.Windows.Forms.TextBox();
            this.txtfactory = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtbtime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtetime = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnset = new System.Windows.Forms.Button();
            this.btnprint = new System.Windows.Forms.Button();
            this.btnclose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtworker);
            this.groupBox1.Controls.Add(this.txtdevice);
            this.groupBox1.Controls.Add(this.txtfactory);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(292, 141);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "  内容定义  ";
            // 
            // txtworker
            // 
            this.txtworker.Location = new System.Drawing.Point(103, 99);
            this.txtworker.MaxLength = 12;
            this.txtworker.Name = "txtworker";
            this.txtworker.Size = new System.Drawing.Size(162, 21);
            this.txtworker.TabIndex = 5;
            this.txtworker.TextChanged += new System.EventHandler(this.txtfactory_TextChanged);
            this.txtworker.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtfactory_KeyDown);
            this.txtworker.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtetime_KeyPress);
            // 
            // txtdevice
            // 
            this.txtdevice.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtdevice.Location = new System.Drawing.Point(103, 61);
            this.txtdevice.MaxLength = 12;
            this.txtdevice.Name = "txtdevice";
            this.txtdevice.Size = new System.Drawing.Size(162, 21);
            this.txtdevice.TabIndex = 4;
            this.txtdevice.TextChanged += new System.EventHandler(this.txtfactory_TextChanged);
            this.txtdevice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtfactory_KeyDown);
            this.txtdevice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtetime_KeyPress);
            // 
            // txtfactory
            // 
            this.txtfactory.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtfactory.Location = new System.Drawing.Point(103, 28);
            this.txtfactory.MaxLength = 12;
            this.txtfactory.Name = "txtfactory";
            this.txtfactory.Size = new System.Drawing.Size(162, 21);
            this.txtfactory.TabIndex = 3;
            this.txtfactory.TextChanged += new System.EventHandler(this.txtfactory_TextChanged);
            this.txtfactory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtfactory_KeyDown);
            this.txtfactory.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtetime_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "员工编码:  ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "设备编码:  ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "工厂编码:  ";
            // 
            // txtbtime
            // 
            this.txtbtime.Location = new System.Drawing.Point(115, 170);
            this.txtbtime.MaxLength = 12;
            this.txtbtime.Name = "txtbtime";
            this.txtbtime.Size = new System.Drawing.Size(162, 21);
            this.txtbtime.TabIndex = 7;
            this.txtbtime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtfactory_KeyDown);
            this.txtbtime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtbtime_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 172);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "开始时间:  ";
            // 
            // txtetime
            // 
            this.txtetime.Location = new System.Drawing.Point(115, 197);
            this.txtetime.MaxLength = 12;
            this.txtetime.Name = "txtetime";
            this.txtetime.Size = new System.Drawing.Size(162, 21);
            this.txtetime.TabIndex = 9;
            this.txtetime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtfactory_KeyDown);
            this.txtetime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtbtime_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "结束时间:  ";
            // 
            // btnset
            // 
            this.btnset.Location = new System.Drawing.Point(12, 235);
            this.btnset.Name = "btnset";
            this.btnset.Size = new System.Drawing.Size(70, 28);
            this.btnset.TabIndex = 10;
            this.btnset.Text = "打印机";
            this.btnset.UseVisualStyleBackColor = true;
            this.btnset.Click += new System.EventHandler(this.btnset_Click);
            // 
            // btnprint
            // 
            this.btnprint.Location = new System.Drawing.Point(149, 235);
            this.btnprint.Name = "btnprint";
            this.btnprint.Size = new System.Drawing.Size(70, 28);
            this.btnprint.TabIndex = 11;
            this.btnprint.Text = "打  印";
            this.btnprint.UseVisualStyleBackColor = true;
            this.btnprint.Click += new System.EventHandler(this.btnprint_Click);
            // 
            // btnclose
            // 
            this.btnclose.Location = new System.Drawing.Point(234, 235);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(70, 28);
            this.btnclose.TabIndex = 12;
            this.btnclose.Text = "退  出";
            this.btnclose.UseVisualStyleBackColor = true;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // frmprint
            // 
            this.AcceptButton = this.btnclose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 281);
            this.Controls.Add(this.btnclose);
            this.Controls.Add(this.btnprint);
            this.Controls.Add(this.btnset);
            this.Controls.Add(this.txtetime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtbtime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmprint";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "标签打印";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmprint_FormClosing);
            this.Load += new System.EventHandler(this.frmprint_Load);
            this.VisibleChanged += new System.EventHandler(this.frmprint_VisibleChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtworker;
        private System.Windows.Forms.TextBox txtdevice;
        private System.Windows.Forms.TextBox txtfactory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtbtime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtetime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnset;
        private System.Windows.Forms.Button btnprint;
        private System.Windows.Forms.Button btnclose;
    }
}