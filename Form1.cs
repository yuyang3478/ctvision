using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLoginSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://localhost/userpass.php");
            textBox3.Text = "0";

            if (Properties.Settings.Default.Remember == true)
            {
                textBox1.Text = Properties.Settings.Default.Username;
                textBox2.Text = Properties.Settings.Default.Password;
                checkBox1.Checked = Properties.Settings.Default.Remember;
            }
            else
            {
                Properties.Settings.Default.Username = String.Empty;
                Properties.Settings.Default.Password = String.Empty;
                Properties.Settings.Default.Remember = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Text = "1";

            if (checkBox1.Checked == true)
            {
                Properties.Settings.Default.Username = textBox1.Text;
                Properties.Settings.Default.Password = textBox2.Text;
                Properties.Settings.Default.Remember = checkBox1.Checked;
                Properties.Settings.Default.Save();
            }
            else if (checkBox1.Checked == false)
            {
                Properties.Settings.Default.Username = String.Empty;
                Properties.Settings.Default.Password = String.Empty;
                Properties.Settings.Default.Remember = false;
                Properties.Settings.Default.Save();
            }

            try
            {
                webBrowser1.Document.GetElementById("username").SetAttribute("value", textBox1.Text);
                webBrowser1.Document.GetElementById("password").SetAttribute("value", textBox2.Text);
                webBrowser1.Document.GetElementById("submit").InvokeMember("click");
            }
            catch
            {
                MessageBox.Show("Elements not found");
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (textBox3.Text == "1")
            {
                if (webBrowser1.DocumentText.Contains("0"))
                {
                    button1.Enabled = true;
                    MessageBox.Show("Password incorrect");
                    textBox3.Text = "0";
                }
                else if (webBrowser1.DocumentText.Contains("1"))
                {
                    button1.Enabled = true;
                    Form2 frm2 = new Form2();
                    frm2.Show();
                    this.Hide();
                    textBox3.Text = "0";
                }
            }
        }
    }
}
