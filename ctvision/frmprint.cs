using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using leanvision;

namespace ctmeasure
{
    public partial class frmprint : Form
    {
        private Dictionary<string, string> dini = new Dictionary<string, string>();
        
        public string folder { get; set; }
        public string viewfolder { get; set; }
        public string factory { get; set; }
        public string device { get; set; }
        public string worker { get; set; }
        public string btime { get; set; }
        public string etime { get; set; }


        public frmprint()
        {
            InitializeComponent();
            folder = "";
            viewfolder = "";
            factory = "";
            device = "";
            worker = "";
            btime = "";
            etime = "";
            loaddata();
        }

        private void frmprint_Load(object sender, EventArgs e)
        {

        }

        private void loaddata()
        {
            String fn = Application.StartupPath + "\\labelprint.dat";
            if (!File.Exists(fn)) return;
            StreamReader sr = new StreamReader(fn);
            string[] strs = new string[2] { "", "" };
            string rstr = sr.ReadLine();
            while (rstr != null)
            {
                strs = rstr.Split('=');
                if (strs.Length > 1) dini.Add(strs[0], strs[1]);
                rstr = sr.ReadLine();
            }
            sr.Close();

            factory = dini["factory"];
            device = dini["device"];
            worker = dini["worker"];
        }

        private void savedata()
        {
            string fn = Application.StartupPath + "\\labelprint.dat";
            factory = txtfactory.Text.Trim();
            device = txtdevice.Text.Trim();
            worker = txtworker.Text.Trim();
            dini["factory"] = factory;
            dini["device"] = device;
            dini["worker"] = worker;
            StreamWriter fw = new StreamWriter(fn);
            foreach (string key in dini.Keys)
            {
                fw.WriteLine(string.Format("{0}={1}", key, dini[key]));
            }
            fw.Flush();
            fw.Close();
        }

        //开始追踪
        public void pbegin() {
            btime = DateTime.Now.ToString("yyyyMMddHHmm");
            if (factory == ""||device==""||worker=="") return;
            if (vcommon.picpath == "")
            {
                folder = Application.StartupPath + "\\photos\\" + DateTime.Now.ToString("yyyyMMdd");
            }
            else
            {
                folder = vcommon.picpath;
            }
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            folder += "\\" + DateTime.Now.ToString("yyyyMMddHHmm");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            viewfolder = folder;
        }

        //结束追踪
        public void pend() {
            if (btime.Trim() == "") return;
            etime = DateTime.Now.ToString("yyyyMMddHHmm");
            folder = "";
            //pendprint();
        }

        //结束打印
        public void pendprint() {
            if (factory == "" || device == "" || worker == "") return;
            if ((this.Owner as frmmain).fzebra.tpldata == null) (this.Owner as frmmain).fzebra.tpldata = "";
            if ((this.Owner as frmmain).fzebra.tpldata == "") return;
            string slabel = (this.Owner as frmmain).fzebra.tpldata;
            slabel = slabel.Replace("{factory}", factory);
            slabel = slabel.Replace("{device}", device);
            slabel = slabel.Replace("{worker}", worker);
            slabel = slabel.Replace("{btime}", btime);
            slabel = slabel.Replace("{etime}", etime);
            (this.Owner as frmmain).fzebra.printdata=slabel;
            (this.Owner as frmmain).fzebra.print();
        }

        //手工打印
        private void mprint() {
            if (txtbtime.Text.Trim() == "" || txtetime.Text.Trim() == "") {
                MessageBox.Show("请填写开始时间及结束时间");
                return;
            }
            btime = txtbtime.Text.Trim();
            etime = txtetime.Text.Trim();
            pendprint();
        }

        //查看NG图片
        public void photosview() {
            string fpath = "";
            if (vcommon.picpath == "")
            {
                fpath = viewfolder.Trim();
                if (fpath == "") fpath = Application.StartupPath + "\\photos";
                if (!Directory.Exists(fpath)) Directory.CreateDirectory(fpath);  
            }
            else if (viewfolder==""){
                fpath = vcommon.picpath; 
            }
            else
            {
                fpath = viewfolder;
            }

            System.Diagnostics.Process.Start("explorer.exe", fpath);
        }

        private void btnprint_Click(object sender, EventArgs e)
        {
            mprint();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            savedata();
            Close();
        }

        private void frmprint_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void frmprint_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                txtbtime.Text = "";
                txtetime.Text = "";
                txtfactory.Text = factory;
                txtdevice.Text = device;
                txtworker.Text = worker;
            }
            else
            {
                savedata();
            }
        }

        private void txtetime_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            //当输入为0-9,a-z,A-Z的数字、退格键时不阻止
            if (e.KeyChar >= '0' && e.KeyChar <= '9') e.Handled=false;
            if (e.KeyChar >= 'a' && e.KeyChar <= 'z') e.Handled=false;
            if (e.KeyChar >= 'A' && e.KeyChar <= 'Z') e.Handled=false;
            if (e.KeyChar == (char)8) e.Handled = false;
        }

        private void txtbtime_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            //当输入为0-9,a-z,A-Z的数字、退格键时不阻止
            if (e.KeyChar >= '0' && e.KeyChar <= '9') e.Handled = false;
            if (e.KeyChar == (char)8) e.Handled = false;
        }

        private void txtfactory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V) e.Handled = true;
        }

        private void txtfactory_TextChanged(object sender, EventArgs e)
        {
            if (!(sender as TextBox).Focused) return;
            savedata();
        }

        private void btnset_Click(object sender, EventArgs e)
        {
            (this.Owner as frmmain).fzebra.Owner = this;
            (this.Owner as frmmain).fzebra.Show();
        }
    }
}
