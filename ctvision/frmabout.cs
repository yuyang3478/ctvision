using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ctmeasure
{
    public partial class frmabout : Form
    {
        public frmabout()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmabout_Load(object sender, EventArgs e)
        {
            lbversion.Text = lbversion.Text + Application.ProductVersion;
            //lbhight.Visible = false;
            if (!Program.getversion()) lbhight.Visible = true;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lbversion_Click(object sender, EventArgs e)
        {

        }
    }
}
