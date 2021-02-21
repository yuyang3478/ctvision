using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using leanvision;

namespace ctmeasure
{
    public partial class frmio : Form
    {
        private clsio comio;
        public frmio()
        {
            InitializeComponent();
        }

        private void frmio_Load(object sender, EventArgs e)
        {
            comio = Program.fmain.dio;
            cbcomport.SelectedIndex = comio.comport-1;
            ttrigger.Text = comio.trigger;
            ttriggerdelay.Text = comio.triggerdelay.ToString();
            tokon.Text = comio.sokon;
            tokoff.Text = comio.sokoff;
            toktime.Text = comio.soktime.ToString();
            tngon.Text = comio.sngon;
            tngoff.Text = comio.sngoff;
            tngtime.Text = comio.sngtime.ToString();
            ckok.Checked = comio.ckok;
            ckng.Checked = comio.ckng;
            triggershape.FillColor = Color.White;
        }

        public void triggeron() {
            triggershape.FillColor = Color.Red;
        }
        public void triggeroff() {
            triggershape.FillColor = Color.White;
        }

        private void cbcomport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cbcomport.Focused) return;
            comio.comclose();
            comio.comport = cbcomport.SelectedIndex + 1;
            updateio();
            comio.comopen();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            updateio();
            comio.savedata();
            this.Close();
        }

        private void btnsend_Click(object sender, EventArgs e)
        {
            updateio();
            comio.sendok();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateio();
            comio.sendng();
        }

        private void updateio(){
            if (ttriggerdelay.Text.Trim() == "") ttriggerdelay.Text = "0";
            if (toktime.Text.Trim() == "") toktime.Text = "0";
            if (tngtime.Text.Trim() == "") tngtime.Text = "0";
            comio.trigger=ttrigger.Text;
            comio.triggerdelay=int.Parse(ttriggerdelay.Text);
            comio.sokon=tokon.Text.Trim();
            comio.sokoff=tokoff.Text.Trim();
            comio.soktime=int.Parse(toktime.Text.Trim());
            comio.sngon=tngon.Text.Trim();
            comio.sngoff=tngoff.Text.Trim();
            comio.sngtime=int.Parse(tngtime.Text.Trim());
            comio.ckok = ckok.Checked;
            comio.ckng = ckng.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comio.comopen();
        }
        
    }
}
