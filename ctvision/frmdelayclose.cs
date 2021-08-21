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
    public partial class frmdelayclose : Form
    {
        public frmdelayclose(int interval = 500)
        {
            InitializeComponent();
            //计时器
            this.components = new Container();
            Timer timer1 = new Timer(this.components);
            timer1.Enabled = true;
            timer1.Interval = interval;
            timer1.Tick += timer1_Tick;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
