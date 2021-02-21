using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ctmeasure;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace leanvision
{
    public class vcommon
    {
        public static softkeyyt skey;
        //显示颜色
        public static string hcolor = "red";
        public static string hcoloractive = "green";
        public static string hcolorhandle = "red";
        public static string hcolorregion = "red";
        public static string hcolorselect = "red";
        public static string hcolortext = "red";
        public static bool hshowresult = false;//是否显示测量结果
        public static bool hshowstatistic = true;//是否显示统计结果
        public static string productname = "";//当前产品名称
        public static double postext1r = 70;//视图文本显示坐标
        public static double postext1c = 100;
        public static double postext2r = 150;
        public static double postext2c = 100;
        public static double viewscale = 1.0;//视图缩放比例
        public static double viewx = 0;//视图缩放比例
        public static double viewy = 0;//视图缩放比例
        public static int posmove = 5;//上下左右键调节位置的步距
        public static int fontsize = 100;//视图字体大小设置
        
        //统计数据
        public static int qty = 0;
        public static int qtypass = 0;
        public static int qtyng = 0;
        public static double qtyrate = 100.0;
        

        public static void showstatistic()
        {
            Program.fmain.tqty.Text = qty.ToString();
            Program.fmain.tqtypass.Text = qtypass.ToString();
            Program.fmain.tqtyng.Text = qtyng.ToString();
            //if (qty == 0) qtyrate = 100.0;
            //else qtyrate = qtypass*1.0 / qty * 100;
            //Program.fmain.tqtyrate.Text = qtyrate.ToString("f1") + " %";
        }

        public static void resetstatistic()
        {
            qty = 0; qtypass = 0; qtyng = 0;
            showstatistic();
        }

        public static void gccollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        ///camera保存
        public static void savedata()
        {
            string fn = Environment.CurrentDirectory + "\\sys.bin";
            FileStream fs = new FileStream(fn, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            ArrayList al = new ArrayList();
            al.Add(hcolor);
            al.Add(hcoloractive);
            al.Add(hcolorhandle);
            al.Add(hcolorregion);
            al.Add(hcolorselect);
            al.Add(hcolortext);
            al.Add(qty);
            al.Add(qtyng);
            al.Add(qtypass);
            al.Add(hshowresult);
            al.Add(hshowstatistic);
            al.Add(posmove);
            al.Add(productname);
            al.Add(postext1r);
            al.Add(postext1c);
            al.Add(postext2r);
            al.Add(postext2c);
            al.Add(fontsize);
            al.Add(viewscale);
            al.Add(viewx);
            al.Add(viewy);
            bf.Serialize(fs, al);
            fs.Flush();
            fs.Close();
            al.Clear();
            al = null;
        }
        public static void loaddata()
        {
            string fn = Environment.CurrentDirectory + "\\sys.bin";
            if (!File.Exists(fn)) return;
            FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            ArrayList al = (ArrayList)bf.Deserialize(fs);
            hcolor = (string)al[0];
            hcoloractive = (string)al[1];
            hcolorhandle = (string)al[2];
            hcolorregion = (string)al[3];
            hcolorselect = (string)al[4];
            hcolortext = (string)al[5];
            qty = (int)al[6];
            qtyng = (int)al[7];
            qtypass = (int)al[8];
            hshowresult = (bool)al[9];
            hshowstatistic = (bool)al[10];
            posmove = (int)al[11];
            productname = (string)al[12];
            postext1r = (double)al[13];
            postext1c = (double)al[14];
            postext2r = (double)al[15];
            postext2c = (double)al[16];
            fontsize = (int)al[17];
            viewscale = (double)al[18];
            viewx = (double)al[19];
            viewy = (double)al[20];
            fs.Close();
            al.Clear();
            al = null;
        }
    }//class
}//namespace
