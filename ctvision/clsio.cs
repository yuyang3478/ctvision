using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AxMSCommLib;
using MSCommLib;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ctmeasure;

namespace leanvision
{
    [Serializable]
    public class clsio
    {
        public int comport { get; set; }
        public string trigger { get; set; }
        public int triggerdelay { get; set; }
        public string sokon { get; set; }
        public string sokoff { get; set; }
        public bool ckok { get; set; }
        public int soktime { get; set; }
        public string sngon { get; set; }
        public string sngoff { get; set; }
        public int sngtime { get; set; }
        public bool ckng { get; set; }

        [NonSerialized]
        private AxMSComm mscom;

        public clsio() {
            mscom= Program.fmain.mscom;
            mscom.OnComm += new EventHandler(this.mscom_OnComm);
            loaddata();
        }

        public void comopen() {
            try
            {
                if (mscom.PortOpen == true) mscom.PortOpen = false;
                mscom._CommPort = (short)comport;
                mscom.Settings = "9600,n,8,1";
                mscom.RThreshold = 1;
                mscom.InputMode = InputModeConstants.comInputModeBinary;
                mscom.InBufferSize = 512;
                mscom.OutBufferSize = 512;
                mscom.PortOpen = true;
            }
            catch {
                MessageBox.Show("端口打开错误！");
            }
        }

        public void comclose() {
            if (mscom.PortOpen == true) mscom.PortOpen = false;
        }

        public void sendok() {
            if (!ckok) return;
            if (mscom.PortOpen == false) { MessageBox.Show("端口未打开！"); return; }
            comwrite(sokon.ToUpper());
            if (soktime > 0) Thread.Sleep(soktime);
            comwrite(sokoff.ToUpper());
        }

        public void sendng() {
            if (!ckng) return;
            if (mscom.PortOpen == false) { MessageBox.Show("端口未打开！"); return; }
            comwrite(sngon.ToUpper());
            if (sngtime > 0) Thread.Sleep(sngtime);
            comwrite(sngoff.ToUpper());
        }

        private void comwrite(string sendstr)
        {
            if (mscom.PortOpen == false) return;
            string sstr = "";
            sstr = sendstr.Trim();
            if (sstr == "") return;
            sstr = sstr.Replace(" ", "");
            if (sstr.Length % 2 == 1) sstr = "0" + sstr;
            byte[] obuff = new byte[sstr.Length / 2];
            for (int i = 0; i < obuff.Length; i++) obuff[i] = Convert.ToByte(sstr.Substring(i * 2, 2), 16);
            mscom.Output = obuff;
            Thread.Sleep(20);
        }

        //接收信号
        private void mscom_OnComm(object sender, EventArgs e)
        {
            if (mscom.CommEvent == (short)OnCommConstants.comEvReceive) {
                string rstr = "";
                Thread.Sleep(10);
                byte[] ibuff = (byte[])mscom.Input;
                for (int i = 0; i < ibuff.Length; i++) rstr += ibuff[i].ToString("X2") + " ";
                rstr = rstr.Trim();
                mscomhandler(rstr);
            }
        }

        private void mscomhandler(string rstr) {
            int len = trigger.Trim().Length;
            rstr = rstr.Substring(0, len).Trim();
            if (rstr == "0A") return;
            if (rstr == trigger.ToUpper()) {
                if (Program.fmain.fio != null) Program.fmain.fio.triggeron();
                if (triggerdelay > 0) Thread.Sleep(triggerdelay);
                //触发执行
                Program.fmain.run();
                if (Program.fmain.fio != null) Program.fmain.fio.triggeroff();
            }
        }

        ///IO保存
        public void savedata()
        {
            string fn = Environment.CurrentDirectory + "\\cameraio.bin";
            FileStream fs = new FileStream(fn, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, this);
            fs.Flush();
            fs.Close();
        }
        public void loaddata()
        {
            string fn = Environment.CurrentDirectory + "\\cameraio.bin";
            if (!File.Exists(fn)) return;
            FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            clsio nio = (clsio)bf.Deserialize(fs);
            this.comport = nio.comport;
            this.trigger = nio.trigger;
            this.triggerdelay = nio.triggerdelay;
            this.sokon = nio.sokon;
            this.sokoff = nio.sokoff;
            this.soktime = nio.soktime;
            this.sngon = nio.sngon;
            this.sngoff = nio.sngoff;
            this.sngtime = nio.sngtime;
            this.ckok = nio.ckok;
            this.ckng = nio.ckng;
            nio = null;
            fs.Close();
            if (this.comport>-1) comopen();
        }

        
    }
}
