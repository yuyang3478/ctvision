using System;
using System.Collections.Generic;
using System.Text;
using HalconDotNet;
using System.Collections;
using ctmeasure;

namespace leanvision
{
    [Serializable]
    class clsmeasure
    {
        public string mname { get; set; }//检测名称
        public roishape roi1 { get; set; }//测量对象
        public roishape roi2 { get; set; }//基准对象
        public int mtype { get; set; }//0-x距离, 1-y距离, 2-两点距离, 3-面积, 4-直径
        public double mstd { get; set; }//标准值
        public double mllimit { get; set; }//下限
        public double mulimit { get; set; }//上限
        public double mvalue { get; set; }//检测结果值
        public string mresult { get; set; }//检测结果ok/ng
        public double moffset { get; set; }//补偿值

        //统计值
        public int mnums { get; set; }
        public int mngs { get; set; }
        public int mngslarge { get; set; }
        public int mngssmall { get; set; }


        [NonSerialized]
        private double xpixel,ypixel;
        
        public clsmeasure() {
            mname = "";
            roi1 = roi2 = null;
            mtype = 0;
            mstd = mllimit = mulimit = moffset=0;
        }

        public string mtypename(){
            if (mtype == 0) return "x距离";
            if (mtype == 1) return "y距离";
            if (mtype == 2) return "两点距离";
            if (mtype == 3) return "面积";
            if (mtype == 4) return "直径";
            if (mtype == 5) return "长度";
            if (mtype == 6) return "宽度";
            return "";
        }

        public void run(){
            if (!Program.getversion()) return;
            xpixel = Program.fmain.dcamera.xpixel;
            ypixel = Program.fmain.dcamera.ypixel;
            mnums++;
            if (roi1 == null && roi2 == null) {
                mvalue = 0;
                mresult = "OK";
                return;
            }
            if (mtype == 0) runmeasure0();
            if (mtype == 1) runmeasure1();
            if (mtype == 2) runmeasure2();
            if (mtype == 3) runmeasure3();
            if (mtype == 4) runmeasure4();
            if (mtype == 5) runmeasure5();
            if (mtype == 6) runmeasure6();
        }
        //roi.roipoint 定位点 0-面积中心, 1-几何中心,2-上边,3-下边,4-左边,5-右边
        //mtype 测量类型 0-x距离,1-y距离,2-两点距离,3-面积,4-直径,5-长度,6-宽度
        //0-x距离
        private void runmeasure0() {
            mvalue = 0;
            if (roi1 == null || roi2 == null) {
                mvalue = 0; mresult = "NG";
                return;
            }
            if (!roi1.Equals(roi2)) { 
                double p1=0,p2=0;
                if(roi1.roipoint==0) p1=roi1.ac;
                if (roi1.roipoint == 1) p1 = roi1.gc;
                if(roi1.roipoint==4) p1=roi1.c1;
                if(roi1.roipoint==5) p1=roi1.c2;
                if (roi2.roipoint == 0) p2 = roi2.ac;
                if (roi2.roipoint == 1) p2 = roi2.gc;
                if (roi2.roipoint == 4) p2 = roi2.c1;
                if (roi2.roipoint == 5) p2 = roi2.c2;
                mvalue = Math.Abs((p1 - p2) * xpixel / 1000) + moffset;
            }

            mresult = "NG";
            if (mvalue >= (mstd - mllimit) && mvalue <= (mstd + mulimit)) mresult = "OK";
            if (mvalue < (mstd - mllimit)) mngssmall++;
            if (mvalue > (mstd + mulimit)) mngslarge++;
        }
        //1-y距离
        private void runmeasure1()
        {
            mvalue = 0;
            double p1 = 0, p2 = 0;
            if (roi1 == null || roi2 == null)
            {
                mvalue = 0; mresult = "NG";
                return;
            }
            if (!roi1.Equals(roi2))
            {
                if (roi1.roipoint == 0) p1 = roi1.ar;
                if (roi1.roipoint == 1) p1 = roi1.gr;
                if (roi1.roipoint == 2) p1 = roi1.r1;
                if (roi1.roipoint == 3) p1 = roi1.r2;
                if (roi2.roipoint == 0) p2 = roi2.ar;
                if (roi2.roipoint == 1) p2 = roi2.gr;
                if (roi2.roipoint == 2) p2 = roi2.r1;
                if (roi2.roipoint == 3) p2 = roi2.r2;
                mvalue = Math.Abs((p1 - p2) * ypixel / 1000) + moffset;
            }

            mresult = "NG";
            if (mvalue >= (mstd - mllimit) && mvalue <= (mstd + mulimit)) mresult = "OK";
            if (mvalue < (mstd - mllimit)) mngssmall++;
            if (mvalue > (mstd + mulimit)) mngslarge++;
        }
        //2-两点距离
        private void runmeasure2()
        {
            mvalue = 0;
            double dist=0;
            if (roi1 == null || roi2 == null)
            {
                mvalue = 0; mresult = "NG";
                return;
            }
            double p1 = 0, p11=0, p2 = 0,p22=0;
            if (!roi1.Equals(roi2) && (roi1.roipoint==1 || roi1.roipoint==2) && (roi2.roipoint==1||roi2.roipoint==2))
            {
                if (roi1.roipoint == 0) { p1 = roi1.ar; p11 = roi1.ac; }
                if (roi1.roipoint == 1) { p1 = roi1.gr; p11 = roi1.gc; }
                if (roi2.roipoint == 0) { p2 = roi2.ar; p22 = roi2.ac; }
                if (roi2.roipoint == 1) { p2 = roi2.gr; p22 = roi2.gc; }
                dist=HMisc.DistancePp(p1*ypixel/1000,p11*xpixel/1000,p2*ypixel/1000,p22*ypixel/1000);
                mvalue = Math.Abs(dist) + moffset;
            }
            
            mresult = "NG";
            if (mvalue >= (mstd - mllimit) && mvalue <= (mstd + mulimit)) mresult = "OK";
            if (mvalue < (mstd - mllimit)) mngssmall++;
            if (mvalue > (mstd + mulimit)) mngslarge++;
        }
        //3-面积
        private void runmeasure3()
        {
            mvalue = 0;
            if(roi1!=null){
                if (roi1.Equals(roi2) || roi2 == null) { mvalue = roi1.area; }
                else {
                    mvalue = roi1.area + roi2.area;
                }
            }
            else if (roi2 != null) {
                mvalue = roi2.area;
            }
            mvalue = mvalue + moffset;
            mresult = "NG";
            if (mvalue >= (mstd - mllimit) && mvalue <= (mstd + mulimit)) mresult = "OK";
            if (mvalue < (mstd - mllimit)) mngssmall++;
            if (mvalue > (mstd + mulimit)) mngslarge++;
        }
        //4-直径
        private void runmeasure4()
        {
            mvalue = 0;
            if (roi1 != null) mvalue = Math.Abs(roi1.cradius * 2 * xpixel / 1000) + moffset;
            mresult = "NG";
            if (mvalue >= (mstd - mllimit) && mvalue <= (mstd + mulimit)) mresult = "OK";
            if (mvalue < (mstd - mllimit)) mngssmall++;
            if (mvalue > (mstd + mulimit)) mngslarge++;
        }
        //5-长度
        private void runmeasure5()
        {
            mvalue = 0;
            if (roi1 != null) mvalue = Math.Abs((roi1.c2 - roi1.c1) * xpixel / 1000) + moffset;
            mresult = "NG";
            if (mvalue >= (mstd - mllimit) && mvalue <= (mstd + mulimit)) mresult = "OK";
            if (mvalue < (mstd - mllimit)) mngssmall++;
            if (mvalue > (mstd + mulimit)) mngslarge++;
        }
        //6-宽度
        private void runmeasure6()
        {
            mvalue = 0;
            if (roi1 != null) mvalue = Math.Abs((roi1.r2 - roi1.r1) * ypixel / 1000) + moffset;
            mresult = "NG";
            if (mvalue >= (mstd - mllimit) && mvalue <= (mstd + mulimit)) mresult = "OK";
            if (mvalue < (mstd - mllimit)) mngssmall++;
            if (mvalue > (mstd + mulimit)) mngslarge++;
        }
    }

    class clsmeasurelist {
        public string productname { get; set; }
        
        private ArrayList mlist;

        public clsmeasurelist() {
            mlist = new ArrayList();
        }

        public clsmeasure this[int index]{
            get { return (clsmeasure)mlist[index]; }
            set { mlist[index] = value; }
        }

        public ArrayList ilist {
            get { return mlist; }
            set { mlist = value; }
        }

        public int count {
            get { return mlist.Count; }
        }

        public int indexof(clsmeasure mroi) { 
            return mlist.IndexOf(mroi);
        }

        public clsmeasure add() {
            clsmeasure nm=new clsmeasure();
            mlist.Add(nm);
            return nm;
        }
        public clsmeasure add(clsmeasure msure) {
            mlist.Add(msure);
            return msure;
        }

        public clsmeasure addcopy(int ifrom) {
            clsmeasure nmsure = new clsmeasure();
            clsmeasure mfrom = this[ifrom];
            nmsure.mname = mfrom.mname;
            nmsure.roi1 = mfrom.roi1;
            nmsure.roi2 = mfrom.roi2;
            nmsure.mtype = mfrom.mtype;
            nmsure.mstd = mfrom.mstd;
            nmsure.mllimit = mfrom.mllimit;
            nmsure.mulimit = mfrom.mulimit;
            nmsure.mvalue = mfrom.mvalue;
            nmsure.mresult = mfrom.mresult;
            nmsure.moffset = mfrom.moffset;
            mlist.Add(nmsure);
            return nmsure;
        }

        public clsmeasure insertcopy(int ifrom) {
            clsmeasure nmsure = new clsmeasure();
            clsmeasure mfrom = this[ifrom];
            nmsure.mname = mfrom.mname;
            nmsure.roi1 = mfrom.roi1;
            nmsure.roi2 = mfrom.roi2;
            nmsure.mtype = mfrom.mtype;
            nmsure.mstd = mfrom.mstd;
            nmsure.mllimit = mfrom.mllimit;
            nmsure.mulimit = mfrom.mulimit;
            nmsure.mvalue = mfrom.mvalue;
            nmsure.mresult = mfrom.mresult;
            nmsure.moffset = mfrom.moffset;
            mlist.Insert(ifrom, nmsure);
            return nmsure;
        }

        public void clear() {
            mlist.Clear();
        }

        public void removeat(int index) {
            mlist.RemoveAt(index);
        }

        public void delete(roishape rs) {
            if (rs == null) return;
            foreach (clsmeasure cms in mlist) {
                if (cms.roi1 == rs) cms.roi1 = null;
                if (cms.roi2 == rs) cms.roi2 = null;
            }
        }

        public void run() {
            foreach (clsmeasure cms in mlist) cms.run();
        }
    }
}
