using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ctmeasure
{
    public partial class ucalign : UserControl
    {
        public ucalign()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Parent.Hide();
        }

        private void ucalign_MouseHover(object sender, EventArgs e)
        {
            button1.Focus();
        }
    }
}
