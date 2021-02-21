
using System;

namespace ctmeasure
{
    partial class frmlogin
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
            this.btnlogin = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbname = new System.Windows.Forms.TextBox();
            this.tbpwd = new System.Windows.Forms.TextBox();
            this.btnupdate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnlogin
            // 
            this.btnlogin.Location = new System.Drawing.Point(142, 146);
            this.btnlogin.Name = "btnlogin";
            this.btnlogin.Size = new System.Drawing.Size(75, 23);
            this.btnlogin.TabIndex = 0;
            this.btnlogin.Text = "登陆";
            this.btnlogin.UseVisualStyleBackColor = true;
            this.btnlogin.Click += new System.EventHandler(this.btnlogin_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "用户名";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "密码";
            // 
            // tbname
            // 
            this.tbname.Location = new System.Drawing.Point(100, 51);
            this.tbname.Name = "tbname";
            this.tbname.Size = new System.Drawing.Size(165, 21);
            this.tbname.TabIndex = 4;
            // 
            // tbpwd
            // 
            this.tbpwd.Location = new System.Drawing.Point(100, 88);
            this.tbpwd.Name = "tbpwd";
            this.tbpwd.PasswordChar = '*';
            this.tbpwd.Size = new System.Drawing.Size(165, 21);
            this.tbpwd.TabIndex = 5;
            // 
            // btnupdate
            // 
            this.btnupdate.Location = new System.Drawing.Point(239, 146);
            this.btnupdate.Name = "btnupdate";
            this.btnupdate.Size = new System.Drawing.Size(75, 23);
            this.btnupdate.TabIndex = 6;
            this.btnupdate.Text = "修改";
            this.btnupdate.UseVisualStyleBackColor = true;
            this.btnupdate.Click += new System.EventHandler(this.btnupdate_Click);
            // 
            // frmlogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 243);
            this.Controls.Add(this.btnupdate);
            this.Controls.Add(this.tbpwd);
            this.Controls.Add(this.tbname);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnlogin);
            this.Name = "frmlogin";
            this.Text = "frmlogin";
            this.Shown += new System.EventHandler(this.frmlogin_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void frmlogin_Shown(object sender, EventArgs e)
        {
            if (!Program.fmain.isLoginSuccess)
            {
                this.btnupdate.Visible = false;
            }
            else {
                this.btnupdate.Visible = true;
                this.btnlogin.Text = "退出";
            }
        }

        #endregion

        private System.Windows.Forms.Button btnlogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbname;
        private System.Windows.Forms.TextBox tbpwd;
        private System.Windows.Forms.Button btnupdate;
    }
}