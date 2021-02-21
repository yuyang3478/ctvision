using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ctmeasure
{
    public partial class frmoffset : Form
    {
        public frmoffset()
        {
            InitializeComponent();
        }

        private void frmoffset_Load(object sender, EventArgs e)
        {
            //textBox1.Text = Program.fmain.getoffset();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Program.fmain.setoffset(textBox1.Text);
            this.Close();
        }
    }
}
