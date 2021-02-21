namespace ctmeasure
{
    partial class frmzebra
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
            this.ttpl = new System.Windows.Forms.TextBox();
            this.tprinter = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtprint = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.dlgopen = new System.Windows.Forms.OpenFileDialog();
            this.dlgprint = new System.Windows.Forms.PrintDialog();
            this.SuspendLayout();
            // 
            // ttpl
            // 
            this.ttpl.Location = new System.Drawing.Point(21, 256);
            this.ttpl.Name = "ttpl";
            this.ttpl.Size = new System.Drawing.Size(239, 21);
            this.ttpl.TabIndex = 11;
            // 
            // tprinter
            // 
            this.tprinter.Location = new System.Drawing.Point(21, 293);
            this.tprinter.Name = "tprinter";
            this.tprinter.Size = new System.Drawing.Size(239, 21);
            this.tprinter.TabIndex = 10;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(279, 288);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(105, 29);
            this.button3.TabIndex = 9;
            this.button3.Text = "printer";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(279, 252);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(105, 29);
            this.button2.TabIndex = 8;
            this.button2.Text = "load tpl";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtprint
            // 
            this.txtprint.BackColor = System.Drawing.SystemColors.Window;
            this.txtprint.Location = new System.Drawing.Point(20, 12);
            this.txtprint.Multiline = true;
            this.txtprint.Name = "txtprint";
            this.txtprint.Size = new System.Drawing.Size(363, 222);
            this.txtprint.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(139, 337);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 35);
            this.button1.TabIndex = 7;
            this.button1.Text = "确  定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dlgopen
            // 
            this.dlgopen.DefaultExt = "zpl";
            this.dlgopen.Filter = "zpl file(*.zpl)|*.zpl";
            // 
            // dlgprint
            // 
            this.dlgprint.UseEXDialog = true;
            // 
            // frmzebra
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 389);
            this.Controls.Add(this.ttpl);
            this.Controls.Add(this.tprinter);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtprint);
            this.Name = "frmzebra";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "打印机设置";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmzebra_FormClosing);
            this.Load += new System.EventHandler(this.frmzebra_Load);
            this.VisibleChanged += new System.EventHandler(this.frmzebra_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ttpl;
        private System.Windows.Forms.TextBox tprinter;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtprint;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog dlgopen;
        private System.Windows.Forms.PrintDialog dlgprint;
    }
}