using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ctmeasure
{
    public class clslog
    {
        private string fn;
        public clslog() {
            string fpath = Environment.CurrentDirectory + "\\log";
            if (!Directory.Exists(fpath)) Directory.CreateDirectory(fpath);
            fn = Environment.CurrentDirectory+"\\log\\"+ DateTime.Now.ToString("yyyyMMdd") + ".log";
            if (!File.Exists(fn)) File.Create(fn).Close();
        }
        public void write(string str) {
            StreamWriter sw = File.AppendText(fn);
            sw.WriteLine(DateTime.Now.ToString("F")+": "+str);
            sw.Flush();
            sw.Close();
        }
    }
}
