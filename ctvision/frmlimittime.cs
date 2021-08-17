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
    public partial class frmlimittime : Form
    {
        public  DateTime dt = new DateTime(2008, 8, 21);
        //private DateTime tmpdt;
        public frmlimittime()
        {
            //if (dt == null)
            //{
            //    dt = new DateTime(2008, 8, 21);
            //}
            //tmpdt = new DateTime(2008, 8, 21);
            InitializeComponent();
        }

        private void btnaply_Click(object sender, EventArgs e)
        {
            //设置程序到期时间
            //dt = tmpdt;
            this.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dt = dateTimePicker1.Value;
        }

        private void frmlimittime_Shown(object sender, EventArgs e)
        {
            dateTimePicker1.Value = dt;
        }
    }
}
