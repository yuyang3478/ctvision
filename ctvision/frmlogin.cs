using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ctmeasure
{
    public partial class frmlogin : Form
    {
        
        public frmlogin()
        {
            InitializeComponent();
        }
        public string Read(string path)
        {
            string hash = "";
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(path, Encoding.Default);
                String line; 
                while ((line = sr.ReadLine()) != null)
                {
                    hash = line.ToString();
                    //Console.WriteLine(line.ToString());
                }
            }
            catch (IOException e)
            {
                MessageBox.Show("登陆异常！");
                Console.WriteLine(e.Message);
            }
            finally {
                sr.Close();
            }
            return hash;
        }

        public void Write(string path,string hash)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(path, FileMode.Create);
                sw = new StreamWriter(fs);
                //开始写入
                sw.Write(hash);
                //清空缓冲区
                sw.Flush();
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message); 
            }
            finally {
                //关闭流
                sw.Close();
                fs.Close();
            }
        }

        private void btnlogin_Click(object sender, EventArgs e)
        { 
            if (this.btnlogin.Text == "退出") {
                foreach (ToolStripItem tb in Program.fmain.mtools.Items)
                {
                    if (tb.Name != "tbrun" && tb.Name != "tbrunstrop" && tb.Name != "tbcheckimage" && tb.Name != "tblogmenu") tb.Enabled = false;
                }
                Program.fmain.isLoginSuccess = false;
                Program.fmain.btnbugmode.Visible = false;
                this.Close();
                return;
            }
            string uname = tbname.Text.ToString().Trim();
            string pwd = tbpwd.Text.ToString().Trim();
            if (uname.Length == 0 || pwd.Length == 0)
            {
                MessageBox.Show("用户名或密码不许为空！");
                return;
            }
            string input = uname + pwd;
            //string hash = GetSha1Hash(input);
            //Write(".\\info.txt", hash);

            string hash = Read(".\\info.txt");

            bool sucess = VerifySha1Hash(input, hash);
            if (sucess)
            {
                foreach (ToolStripItem tb in Program.fmain.mtools.Items)
                {
                   tb.Enabled = true;
                }
                Program.fmain.isLoginSuccess = true;
                Program.fmain.btnbugmode.Visible = true;
                this.Close();
                //MessageBox.Show("登陆成功！");
            }
            else {
                MessageBox.Show("用户名或密码错误！");
            }
        }

        public static string GetSha1Hash(string input)
        {
            byte[] inputBytes = Encoding.Default.GetBytes(input);

            SHA1 sha = new SHA1CryptoServiceProvider();

            byte[] result = sha.ComputeHash(inputBytes);

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                sBuilder.Append(result[i].ToString("x2"));
            }

            return sBuilder.ToString().ToUpper();
        }

        public static bool VerifySha1Hash(string input, string hash)
        {
            string hashOfInput = GetSha1Hash(input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnupdate_Click(object sender, EventArgs e)
        {
            string uname = tbname.Text.ToString().Trim();
            string pwd = tbpwd.Text.ToString().Trim();
            if (uname.Length == 0 || pwd.Length == 0)
            {
                MessageBox.Show("用户名或密码不许为空！");
                return;
            }
            string input = uname + pwd;
            string hash = GetSha1Hash(input);
            Write(".\\info.txt", hash);
            MessageBox.Show("修改密码成功！");
            this.Close();
        }
    }
}
