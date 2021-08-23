using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ctmeasure
{
    public partial class verifyInstall : Form
    {
        public verifyInstall()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string na = name.Text;
            string pa = password.Text;
            if (Apphelper.verifyInstall(na, pa))
            {
                this.Dispose();
                this.Close();
            }
            else {
                this.Dispose();
                this.Close();
                Environment.Exit(1);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
            Environment.Exit(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Apphelper.deleteValue();
        }
    }
}
