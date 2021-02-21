using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;
using ctmeasure;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace leanvision
{
    [Serializable]
    public class clscamera
    {
        public string cname { get; set; }
        public double exposuretime { get; set; }
        public double gain { get; set; }
        public double gamma { get; set; }
        public int contrast { get; set; }
        public double xpixel { get; set; }
        public double ypixel { get; set; }
        
        [NonSerialized]
        private HFramegrabber hfg;
        [NonSerialized]
        private bool hfgon=false;
        [NonSerialized]
        public HImage himg=new HImage("byte",3000,3000);
        [NonSerialized]
        private string ctype = "";
        [NonSerialized]
        public int iw = 0, ih = 0;

        public clscamera() {
            cname = "";
            exposuretime=0;
            gain = 0;
            gamma = 0;
            contrast = 0;
            xpixel = 10.0;
            ypixel = 10.0;
            ctype = "";
            iw = 0;
            ih = 0;
            loaddata();
        }

        public void cameraopen() {
            if (cname == "") return;
            if(hfgon) hfg.Dispose();
            HOperatorSet.CloseAllFramegrabbers();
            hfgon = false;
            try
            {
                hfg = new HFramegrabber();
                //open_framegrabber ('GigEVision', 0, 0, 0, 0, 0, 0, 'default', -1, 'default', -1, 'false', 'default', 'xiangji1', 0, -1, AcqHandle)
                hfg.OpenFramegrabber("GigEVision", 0, 0, 0, 0, 0, 0, "default", -1, "default", -1, "false", "default", cname, 0, -1);
                
                Thread.Sleep(300);
                try
                {
                    if (cname.IndexOf("MindVision") > -1)
                    {
                        HOperatorSet.SetFramegrabberParam(hfg, "AcquisitionMode", "SingleFrame");
                        HOperatorSet.SetFramegrabberParam(hfg, "AcquisitionFrameRate", "Mid");
                    }
                    try { if (exposuretime > 0) hfg.SetFramegrabberParam("ExposureTime", exposuretime); }catch { }
                    try { if (gain > 0) hfg.SetFramegrabberParam("Gain", gain / 100); }catch { }
                    try { if (contrast > 0) hfg.SetFramegrabberParam("Contrast", contrast); }catch { }
                    try { if (gamma > 0) hfg.SetFramegrabberParam("Gamma", gamma); }catch { }
                }
                catch { }

                hfg.GrabImageStart(-1);
                himg = hfg.GrabImageAsync(-1);
                string ctype = "";
                himg.GetImagePointer1(out ctype, out iw, out ih);
                
                hfgon = true;
            }
            catch {
                //MessageBox.Show("相机未连接！");
            }            
        }

        public void loadphoto(string filephoto) {
            himg.Dispose();
            himg.ReadImage(filephoto);
            himg.GetImagePointer1(out ctype, out iw, out ih);
        }

        public void savephoto(string filename) {
            himg.WriteImage("jpeg", 0, filename);
        }

        public void cameraparam() {
            if (hfgon == false) return;
            try
            {
                exposuretime = hfg.GetFramegrabberParam("ExposureTime");
                gain = hfg.GetFramegrabberParam("Gain")*100;
                gamma = hfg.GetFramegrabberParam("Gamma");
                contrast = hfg.GetFramegrabberParam("Contrast");
            }
            catch { }
        }

        public void cameraparamset() {
            if (hfgon == false) return;
            try{if (exposuretime > 0) hfg.SetFramegrabberParam("ExposureTime", exposuretime);}catch{}
            try{if (gain > 0) hfg.SetFramegrabberParam("Gain", gain / 100);}catch{}
            try{if (contrast > 0) hfg.SetFramegrabberParam("Contrast", contrast);}catch{}
            try{if (gamma > 0) hfg.SetFramegrabberParam("Gamma", gamma);}catch { }
        }

        public void cameraclose(){
            HOperatorSet.CloseAllFramegrabbers();
            hfgon=false;
        }

        public HTuple listcameras(){
            HTuple dinfo,dnames;
            HOperatorSet.InfoFramegrabber("GigEVision", "device", out dinfo, out dnames);
            return dnames;
        }

        public HImage grabasync() {
            //if (!hfgon) cameraopen();
            if (!hfgon) return himg;
            try
            {
                himg.Dispose();
                himg = hfg.GrabImageAsync(-1);
                //himg = hfg.GrabImage();
                //Program.fmain.hwin.HalconWindow.DispImage(himg);
            }catch {}
            return himg;
        }

        public HImage grabimg()
        {
            //if (!hfgon) cameraopen();
            if (!hfgon) return himg;
            try
            {
                himg.Dispose();
                himg = hfg.GrabImage();
                Program.fmain.hwin.HalconWindow.DispImage(himg);
            }
            catch { }
            return himg;
        }

        public bool iscameraopen() {
            return hfgon;
        }

        ///camera保存
        public void savedata()
        {
            string fn = Environment.CurrentDirectory + "\\camera.bin";
            FileStream fs = new FileStream(fn, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, this);
            fs.Flush();
            fs.Close();
        }
        public void loaddata()
        {
            string fn = Environment.CurrentDirectory + "\\camera.bin";
            if (!File.Exists(fn)) return;
            FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            clscamera ncmaera = (clscamera)bf.Deserialize(fs);
            this.cname = ncmaera.cname;
            this.exposuretime = ncmaera.exposuretime;
            this.gain = ncmaera.gain;
            this.gamma = ncmaera.gamma;
            this.contrast = ncmaera.contrast;
            this.xpixel = ncmaera.xpixel;
            this.ypixel = ncmaera.ypixel;
            ncmaera = null;
            fs.Close();
            if (cname.Trim() != "")
            {
                cameraopen();
                if (!hfgon) MessageBox.Show("相机未连接！");
            }
        }
    }
}
