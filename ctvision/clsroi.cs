using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HalconDotNet;
using ctmeasure;
using System.Drawing;

namespace leanvision
{
    [Serializable]
    public class roishape
    {
        //定义
        public string shape { get; set; }//rect,circle,region,text
        public int num { get; set; } //编号

        //位置
        public double row { get; set; }
        public double col { get; set; }
        public double row1 { get; set; }
        public double col1 { get; set; }

        //特征定位
        public int roipoint { get; set; }//0-空,1-面积中心,2-几何中心,3-上边,4-下边,5-左边,6-右边
        //选择
        public int thway { get; set; } //0-空,1-自动,2-黑色区域,3-白色区域,4-阀值
        public double thmin { get; set; }
        public double thmax { get; set; }

        //过滤 select_shape
        public double rowmin { get; set; }
        public double rowmax { get; set; }
        public double colmin { get; set; }
        public double colmax { get; set; }
        public double widthmin { get; set; }
        public double widthmax { get; set; }
        public double heightmin { get; set; }
        public double heightmax { get; set; }
        public double areamin { get; set; }
        public double areamax { get; set; }
        public bool areamaxcheck { get; set; }
        public bool combinecheck { get; set; }
        
        //精度 closing_circle
        public int closingcircle { get; set; }//0-一般, 1-高, 2-最高

        //表面检测
        public bool surfacecheck { get; set; }//区域是否进行表面检测
        public bool surfacemaxcheck { get; set; }//最大面积限制
        public int thminsurface { get; set; }//最小阀值
        public int thmaxsurface { get; set; }//最大阀值
        public int areasurface { get; set; }//面积测量值
        public double stdsurface { get; set; }//残缺比标准
        public double msurface { get; set; }//残缺比测量值
        

        //计算结果
        [NonSerialized]
        public HRegion roirng;
        [NonSerialized]
        public double area, ar, ac, cr, cc, cradius, gr,gc;
        [NonSerialized]
        public int r1, c1, r2, c2;

        public roishape() {
            shape = "rect";
            num = -1;
            row = col = row1 = col1 = 0;
            roipoint = 0;
            thway = 0;
            thmin = 0; thmax = 128;
            closingcircle = 0;
            
            rowmin = colmin = widthmin = heightmin = areamin = 0;
            rowmax = colmax = widthmax = heightmax = areamax = 99999;
            areamaxcheck = true;
            area = ar = ac = cr = cc = cradius = gr = gc= 0;
        }

        public roishape(string strshape, double vr1, double vc1, double vr2, double vc2):this() 
        {
            shape = strshape;
            row = vr1; col = vc1; row1 = vr2; col1 = vc2;
        }

        public void checkrowcol()
        {
            double r1 = Math.Min(row, row1);
            double r2 = Math.Max(row, row1);
            double c1 = Math.Min(col, col1);
            double c2 = Math.Max(col, col1);
            row = r1; row1 = r2; col = c1; col1 = c2;
        }

        public void show(HWindow hw) {
            hw.SetColor(vcommon.hcolor);
            hw.SetDraw("margin");
            //checkrowcol();
            if (shape == "rect") {
                hw.DispRectangle1(row, col, row1, col1);
                hw.SetColor(vcommon.hcolortext);
                hw.SetTposition((int)(row + 5.0), (int)(col + 5.0));
                hw.WriteString(num.ToString());
            }
        }

        //原点跟踪,dr,dc偏移量
        public void show(HWindow hw,double dr,double dc)
        {
            hw.SetColor(vcommon.hcolor);
            hw.SetDraw("margin");
            if (shape == "rect")
            {
                hw.DispRectangle1(row+dr, col+dc, row1+dr, col1+dc);
                hw.SetColor(vcommon.hcolortext);
                hw.SetTposition((int)(row+dr + 5.0), (int)(col+dc + 5.0));
                hw.WriteString(num.ToString());
            }

            //显示定位
            hw.SetColor(vcommon.hcoloractive);
            if (roipoint == 0) { hw.DispCross(ar, ac, 50, 0); }
            if (roipoint == 1) { hw.DispCross(gr, gc, 50, 0); }
            if (roipoint == 2) { hw.DispLine(r1, col+dc, r1, col1+dc); }
            if (roipoint == 3) { hw.DispLine(r2, col+dc, r2, col1+dc); }
            if (roipoint == 4) { hw.DispLine(row+dr, c1, row1+dr, c1); }
            if (roipoint == 5) { hw.DispLine(row+dr, c2, row1+dr, c2); }
        }

        public void showselect(HWindow hw) {
            hw.SetColor(vcommon.hcoloractive);
            hw.SetDraw("margin");
            if (shape == "rect")
            {
                hw.DispRectangle1(row, col, row1, col1);
                hw.SetColor(vcommon.hcolortext);
                hw.SetTposition((int)(row + 5.0), (int)(col + 5.0));
                hw.WriteString(num.ToString());

                hw.SetColor(vcommon.hcolorhandle);
                double hlen = 5 / vcommon.viewscale;
                hw.DispRectangle2(row, col, 0, hlen, hlen);
                hw.DispRectangle2(row, col1,0,hlen, hlen);
                hw.DispRectangle2(row1, col,0,hlen, hlen);
                hw.DispRectangle2(row1, col1,0,hlen, hlen);
            }
        }

        //调节时显示区域
        public void getregion(HWindow hw, HImage himg) {
            HRegion nrng = new HRegion(row, col, row1, col1);
            if (nrng.Area == 0) return;
            HImage timg = himg.ReduceDomain(nrng);
            if(closingcircle==0) timg=timg.MeanImage(9,9);
            if (closingcircle == 1) timg = timg.MeanImage(5, 5);
            if (closingcircle == 2) timg = timg.MeanImage(2, 2);

            
            //hw.DispImage(timg);
            nrng = timg.Threshold(thmin, thmax);
            nrng = nrng.Connection();

            //过滤
            if (rowmin > 0 || rowmax < 99999) nrng = nrng.SelectShape("row", "and", rowmin, rowmax);
            if (colmin > 0 || colmax < 99999) nrng = nrng.SelectShape("column", "and", colmin, colmax);
            if (widthmin > 0 || widthmax < 99999) nrng = nrng.SelectShape("width", "and", widthmin, widthmax);
            if (heightmin > 0 || heightmax < 99999) nrng = nrng.SelectShape("height", "and", heightmin, heightmax);
            if (areamin > 0 || areamax < 99999) nrng = nrng.SelectShape("area", "and", areamin, areamax);
            //精度
            if (closingcircle == 0) { nrng = nrng.OpeningCircle(9.5); nrng = nrng.ClosingCircle(9.5); }
            if (closingcircle == 1) { nrng = nrng.OpeningCircle(5.5); nrng = nrng.ClosingCircle(5.5); }
            if (closingcircle == 2) { nrng = nrng.OpeningCircle(2.5); nrng = nrng.ClosingCircle(2.5); }
            nrng = nrng.Connection();
            //最大区域
            if (areamaxcheck) nrng = nrng.SelectShapeStd("max_area", 70);
            nrng = nrng.FillUp();
            roirng = nrng.Union1();
            if (!areamaxcheck && combinecheck) roirng = roirng.ClosingRectangle1(500,500);
            
            //计算特征
            if (nrng.Area.Length == 0)
            {
                area = 0; ar = row; ac = col; 
                r1=r2=(int)row;
                c1=c2=(int)col;
                gr = row; gc = col;
                cradius = 0;cr = row; cc = col;
            }
            else
            {
                area = roirng.AreaCenter(out ar, out ac);
                roirng.SmallestRectangle1(out r1, out c1, out r2, out c2);
                gr = (r2 + r1) / 2;
                gc = (c2 + c1) / 2;
                roirng.SmallestCircle(out cr, out cc, out cradius);
            }
            nrng.Dispose();
            timg.Dispose();
        }

        //原点跟踪， dr，dc偏移量
        public void getregion(HWindow hw, HImage himg,double dr,double dc)
        {
            HRegion nrng = new HRegion(row+dr, col+dc, row1+dr, col1+dc);
            HImage timg = himg.ReduceDomain(nrng);
            if (closingcircle == 0) timg = timg.MeanImage(9, 9);
            if (closingcircle == 1) timg = timg.MeanImage(5, 5);
            if (closingcircle == 2) timg = timg.MeanImage(2, 2);
            //hw.DispImage(timg);
            nrng = timg.Threshold(thmin, thmax);
            nrng = nrng.Connection();

            //过滤
            if (rowmin > 0 || rowmax < 99999) nrng = nrng.SelectShape("row", "and", rowmin, rowmax);
            if (colmin > 0 || colmax < 99999) nrng = nrng.SelectShape("column", "and", colmin, colmax);
            if (widthmin > 0 || widthmax < 99999) nrng = nrng.SelectShape("width", "and", widthmin, widthmax);
            if (heightmin > 0 || heightmax < 99999) nrng = nrng.SelectShape("height", "and", heightmin, heightmax);
            if (areamin > 0 || areamax < 99999) nrng = nrng.SelectShape("area", "and", areamin, areamax);
            //精度
            if (closingcircle == 0) { nrng = nrng.OpeningCircle(9.5); nrng = nrng.ClosingCircle(9.5); }
            if (closingcircle == 1) { nrng = nrng.OpeningCircle(5.5); nrng = nrng.ClosingCircle(5.5); }
            if (closingcircle == 2) { nrng = nrng.OpeningCircle(2.5); nrng = nrng.ClosingCircle(2.5); }
            //最大区域
            nrng = nrng.Connection();
            if (areamaxcheck) nrng = nrng.SelectShapeStd("max_area", 70);
            nrng = nrng.FillUp();

            roirng = nrng.Union1();
            if (!areamaxcheck && combinecheck) roirng = roirng.ClosingRectangle1(500, 500);
            //计算特征
            if (roirng.Area.Length == 0) {
                area = 0; ar = row; ac = col;
                r1 = r2 = (int)row; c1 = c2 = (int)col; gr = row; gc = col;
                cradius = 0; cr = row; cc = col;
            } else {
                area = roirng.AreaCenter(out ar, out ac);
                roirng.SmallestRectangle1(out r1, out c1, out r2, out c2);
                gr = (r2 + r1) / 2;
                gc = (c2 + c1) / 2;
                roirng.SmallestCircle(out cr, out cc, out cradius);
            }
            nrng.Dispose();
            timg.Dispose();
        }

        public void showregion(HWindow hw,bool showregion) {
            if (showregion)
            {
                hw.SetColor(vcommon.hcolorregion);
                hw.SetDraw("fill");
                if (roirng == null) return;
                hw.DispRegion(roirng);
            }
            //显示定位
            hw.SetColor(vcommon.hcoloractive);
            if (roipoint == 0) { hw.DispCross(ar, ac, 50, 0); }
            if (roipoint == 1) { hw.DispCross(gr, gc, 50, 0); }
            if (roipoint == 2) { hw.DispLine(r1, col, r1, col1); }
            if (roipoint == 3) { hw.DispLine(r2, col, r2, col1); }
            if (roipoint == 4) { hw.DispLine(row, c1, row1, c1); }
            if (roipoint == 5) { hw.DispLine(row, c2, row1, c2); }
        }

        public void showmargin(HWindow hw) {
            hw.SetDraw("margin");
            hw.SetColor("green");
            if (roirng == null) return;
            hw.DispRegion(roirng);
        }


        public void getbasepoint(out double br,out double bc)
        {
            br = 0; bc = 0;
            if (roipoint == 0) { br=ar; bc=ac; }
            if (roipoint == 1) { br = gr; bc = gc; }
            if (roipoint == 2) { br = r1; bc = (col + col1) / 2; }
            if (roipoint == 3) { br = r2; bc = (col + col1) / 2; }
            if (roipoint == 4) { br=(row+row1)/2; bc= c1; }
            if (roipoint == 5) { br=(row+row1)/2; bc=c2; }
        }

        //surface 表面检测
        public void showsurface(HWindow hw, HImage himg, bool showarea) 
        {
            if (surfacecheck == false) return;
            HRegion nrng = new HRegion(row, col, row1, col1);
            HImage timg = himg.ReduceDomain(nrng);
            nrng = timg.Threshold(thminsurface*1.0, thmaxsurface*1.0);
            nrng = nrng.Connection();
            nrng = nrng.SelectShape("area", "and", 50.0, 999999);
            nrng = nrng.Union1();
            nrng = nrng.ClosingCircle(3.0);
            double ar,ac;
            if (nrng.Area.Length>0) areasurface = nrng.AreaCenter(out ar, out ac);
            else areasurface = 0;
            hw.SetDraw("fill");
            hw.SetColor("red");
            hw.DispRegion(nrng);
            timg.Dispose();
        }
        public bool measuresuface(HWindow hw, HImage himg, bool isset,bool isshowregion) {
            if (surfacecheck == false) return true;
            HRegion nrng = new HRegion(row, col, row1, col1);
            HImage timg = himg.ReduceDomain(nrng);
            HImage timg1 = new HImage();
            timg = timg.MeanImage(3, 3);
            nrng = timg.Threshold(thminsurface * 1.0, thmaxsurface * 1.0);
            nrng = nrng.Connection();
            nrng = nrng.OpeningCircle(3.5);
            nrng = nrng.ClosingCircle(2.5);
            nrng = nrng.SelectShape("area", "and", 50.0, 999999);
            if (surfacemaxcheck)
            {
                nrng = nrng.SelectShapeStd("max_area", 70);
                nrng = nrng.FillUp();
                timg1 = timg.ReduceDomain(nrng);
                nrng = timg1.Threshold(thminsurface * 1.0, thmaxsurface * 1.0);
                nrng = nrng.Connection();
                nrng = nrng.SelectShape("area", "and", 30.0, 999999);
                nrng = nrng.ClosingCircle(2.0);
            }
            nrng = nrng.Union1();
            
            double ar, ac;
            int area=0;
            if (nrng.Area.Length > 0) area = nrng.AreaCenter(out ar, out ac);
            if (isset) areasurface = area;
            if (areasurface == 0) areasurface = 1;
            
            bool ckresult = true;
            msurface = Math.Abs(1.0*area / areasurface * 100-100);
            if (msurface >= stdsurface || isset==true) {
                ckresult = false;
                if (isshowregion)
                {
                    hw.SetDraw("margin");
                    hw.SetColor("red");
                    hw.DispRegion(nrng);
                }
            }
            timg.Dispose();
            timg1.Dispose();
            return ckresult;
        }
    }//class

    /// ROI对象管理类
    /// 处理窗口 mouseDownAction及mouseMoveAction动作操作roi 
    /// ROI对象管理
    /// 
    public class roimanager
    {
        /// ROI对象集合
        private ArrayList roilist;//图形元素
        public roishape croi;//当前元素
        public int broi;//跟踪点元素编号
        public roiselections srois;//选择元素
        public HWindow hw;
        public clscamera dcamera;
        public string action;//ondraw,onselect,onmove,onresize,ontext
        private int acthandle;//resize handler
        public int nums;//图形编号
        public bool isshowtext;//是否显示测量结果
        public string text1, text2;//视图显示数据
        public double brow, bcol;//基准点坐标存储
        public double dr, dc;//运行时临时存放基准差异

        /// mouse 事件
        private double mousex, mousey, mousex1,mousey1;
        public double tr1, tc1, tr2, tc2;//两个显示文本的位置

        /// 初始化
        public roimanager()
        {
            srois = new roiselections();
            roilist = new ArrayList();
            croi = null;
            broi = -1;
            brow = 0; bcol = 0;
            nums =0;
            isshowtext = true;
            tr1 = 70; tc1 = 100; tr2 = 150; tc2 = 100;
            text1 = "";
            text2 = "data...";
        }

        //索引器
        public roishape this[int index]
        {
            get { return (roishape)roilist[index]; }
            set { roilist[index] = value; }
        }

        //编号器
        public roishape this[string roino]
        {
            get
            {
                if (roino.Trim() == "") return null;
                int roinum = int.Parse(roino);
                foreach (roishape ri in roilist)
                {
                    if (ri.num == roinum)
                    {
                        return ri;
                    }
                }
                return null;
            }
            set
            {
                int roinum = int.Parse(roino);
                foreach (roishape ri in roilist)
                {
                    if (ri.num == roinum)
                    {
                        roilist[roilist.IndexOf(ri)] = value;
                    }
                }
            }
        }

        /// 返回所有roi对象列表
        public ArrayList rois
        {
            get { return roilist; }
            set { roilist = value; }
        }

        public roishape addroi(roishape roi)
        {
            roilist.Add(roi);
            nums++;
            roi.num = nums;
            croi = roi;
            return roi;
        }

        public roishape addroi(string strshape)
        {
            roishape nroi = new roishape();
            nroi.shape = strshape;
            roilist.Add(nroi);
            nums++;
            nroi.num = nums;
            croi = nroi;
            return nroi;
        }

        public void delete() {
            foreach (roishape rs in srois.rois) {
                if (rs.num == broi) broi = -1;
                roilist.Remove(rs);
            }
            srois.clear();
            croi = null;
            if (roilist.Count == 0) nums = 0;
        }

        public void unselect() {
            srois.clear();
            croi = null;
        }
        /// mousedown events
        /// 
        public void mousedown(double mx, double my)
        {
            mousex = mx; mousey = my;

            if (action == "ondraw") return;
            //是否选中文本
            if (action == "")
            {
                if (mx > (tc1 - 40) && mx < (tc1 + 40) && my > (tr1 - 40) && my < (tr1 + 40)) action = "ontext1";
                else if (mx > (tc2 - 40) && mx < (tc2 + 40) && my > (tr2 - 40) && my < (tr2 + 40)) action = "ontext2";
                if(action=="ontext1"||action=="ontext2")return;
            }
            //if (action == "onselect") return;
            //点击在选择roi的内部开始移动，点击在某一roi手柄开始resize
            foreach (roishape cr in srois.rois)
            {
                if (mx > (cr.col - 6) && mx < (cr.col1 + 6) && my > (cr.row - 6) && my < (cr.row1 + 6))
                {
                    croi = cr;
                    action = "onmove";
                    acthandle = -1;
                    double hlen = 6 / vcommon.viewscale;
                    if (mx > (croi.col - hlen) && mx < (croi.col + hlen) && my > (croi.row - hlen) && my < (croi.row + hlen)) { action = "onresize"; acthandle = 0; }
                    else if (mx > (croi.col1 - hlen) && mx < (croi.col1 + hlen) && my > (croi.row - hlen) && my < (croi.row + hlen)) { action = "onresize"; acthandle = 1; }
                    else if (mx > (croi.col1 - hlen) && mx < (croi.col1 + hlen) && my > (croi.row1 - hlen) && my < (croi.row1 + hlen)) { action = "onresize"; acthandle = 2; }
                    else if (mx > (croi.col - hlen) && mx < (croi.col + hlen) && my > (croi.row1 - hlen) && my < (croi.row1 + hlen)) { action = "onresize"; acthandle = 3; }
                    break;
                }
            }
            
        }

        public void mousemove(double mx, double my)
        {
            mousex1 = mx; mousey1 = my;

            if (action == "onselect"||action=="ondraw"){
                double x1,x2,y1,y2;
                x1 = Math.Min(mousex, mousex1);
                x2 = Math.Max(mousex, mousex1);
                y1 = Math.Min(mousey, mousey1);
                y2 = Math.Max(mousey, mousey1);
                
                if (action == "onselect")
                {
                    hw.SetColor(vcommon.hcolorselect);
                    hw.SetLineStyle((new HTuple(3)).TupleConcat(1).TupleConcat(3).TupleConcat(1));
                }
                else {
                    hw.SetColor(vcommon.hcolor);
                    hw.SetLineStyle(new HTuple());
                }
                
                hw.DispRectangle1(y1, x1, y2, x2);
                return;
            }

            if (action == "ontext1") {
                tr1 = my; tc1 = mx;
                return;
            }
            if (action == "ontext2") {
                tr2 = my; tc2 = mx;
                return;
            }
            
            if (croi == null) return;
            double motionx = mx - mousex;
            double motiony = my - mousey;
            mousex = mx; mousey = my;

            if (action == "onresize")
            {
                if (acthandle == 0) {croi.row = my;croi.col = mx;}
                else if (acthandle == 1) {croi.row = my;croi.col1 = mx;}
                else if (acthandle == 2) {croi.row1 = my;croi.col1 = mx;}
                else if (acthandle == 3) {croi.row1 = my;croi.col = mx;}
            }
            else
            {
                //平移
                //"onmove";
                foreach (roishape rs in srois.rois)
                {
                    rs.row += motiony;
                    rs.row1 += motiony;
                    rs.col += motionx;
                    rs.col1 += motionx;
                }
            }
        }

        public void mouseup(double mx,double my) {
            mousex1 = mx; mousey1 = my;
            if (action == "ondraw") {
                double x1, x2, y1, y2;
                x1 = Math.Min(mousex, mousex1);
                x2 = Math.Max(mousex, mousex1);
                y1 = Math.Min(mousey, mousey1);
                y2 = Math.Max(mousey, mousey1);
                if(Math.Abs(x2-x1)>20 || Math.Abs(y2-y1)>20 ){
                    croi = new roishape("rect", y1, x1, y2, x2);
                    roilist.Add(croi);
                    nums++;
                    croi.num = nums;
                    srois.clear();
                    srois.add(croi);
                }
            }
            if (action == "onselect") {
                srois.clear();
                croi=null;
                //未移动，点在那个roi中选中那个roi
                if (Math.Abs(mx - mousex) < 5 || Math.Abs(my - mousey) < 5) {
                    foreach (roishape rs in roilist) { 
                        if(mx>(rs.col-6) && mx<(rs.col1+6) && my>(rs.row-6) && my<(rs.row1+6)){
                            croi=rs; 
                            srois.add(croi);
                            break;
                        }
                    }
                    return;
                }
                //移动后，框选
                double x1 = Math.Min(mx, mousex);
                double x2 = Math.Max(mx, mousex);
                double y1 = Math.Min(my, mousey);
                double y2 = Math.Max(my, mousey);
                Rectangle srect=new Rectangle((int)x1,(int)y1,(int)(x2-x1),(int)(y2-y1));
                Rectangle rrect;
                foreach (roishape rs in roilist)
                {
                    rrect = new Rectangle((int)rs.col, (int)rs.row, (int)(rs.col1 - rs.col), (int)(rs.row1 - rs.row));
                    if(srect.IntersectsWith(rrect) && !rrect.Contains(srect)) srois.add(rs);//相交不包含
                }
                if(srois.count>0) croi=srois[0];
            }
            if (action == "onresize") {
                croi.checkrowcol();
                if (croi.num==broi) { croi.getregion(hw,dcamera.himg); croi.getbasepoint(out brow, out bcol); }
            }
            if (action == "onmove") {
                if (croi.num==broi) { croi.getregion(hw, dcamera.himg); croi.getbasepoint(out brow, out bcol); }
            }
            if (action == "ontext1" || action == "ontext2") {
                vcommon.postext1r = tr1;
                vcommon.postext1c = tc1;
                vcommon.postext2r = tr2;
                vcommon.postext2c = tc2;
            }
            action = "";
        }

        public roishape getroi(int num)
        {
            croi = null;
            foreach (roishape cr in roilist)
            {
                if (cr.num == num)
                {
                    croi = cr;
                    break;
                }
            }
            return croi;
        }

        /// <summary>
        /// 清除所有roi对象
        /// </summary>
        public void clear()
        {
            roilist.Clear();
            srois.clear();
            croi = null;
            broi = -1;
            nums = 0;
            brow = 0; bcol = 0;
        }


        /// 绘制roi到窗口控件
        public void paintroi()
        {
            hw.SetLineStyle(new HTuple());
            foreach (roishape cr in roilist)
            {
                cr.show(hw);
            }
            srois.paintroi(hw);
            showtext();
        }

        public void paintroi(HWindow hwd) {
            hw.SetLineStyle(new HTuple());
            foreach (roishape cr in roilist)
            {
                cr.show(hwd);
            }
            srois.paintroi(hwd);
            showtext(hwd);
        }

        public void showtext() {
            if (isshowtext) {
                if (text1=="OK") hw.SetColor("green");
                else hw.SetColor("red");
                string fs = Math.Round(1.0*vcommon.fontsize * vcommon.viewscale,0).ToString();
                hw.SetFont("-Arial-"+fs+"-");
                //hw.DispCross(tr1, tc1, 20, 0);
                //hw.SetTposition((int)(tr1 + 5), (int)(tc1 + 5));
                //hw.WriteString(text1);

                fs = Math.Round(0.5*vcommon.fontsize * vcommon.viewscale, 0).ToString();
                hw.SetFont("-Arial-"+fs+"-");
                hw.DispCross(tr2, tc2, 20, 0);
                int tr = (int)(tr2 + 5);
                int tc = (int)(tc2 + 5);
                hw.SetTposition(tr, tc);
                string[] ostr = text2.Split('\r');
                foreach (string str in ostr)
                {
                    hw.WriteString(str);
                    tr += (int)(0.5 * vcommon.fontsize+15);
                    hw.SetTposition(tr, tc);
                }
                hw.SetFont("");
            }
        }

        public void showtext(HWindow hwd)
        {
            if (isshowtext)
            {
                if (text1 == "OK") hw.SetColor("green");
                else hw.SetColor("red");
                string fs = Math.Round(1.0 * vcommon.fontsize * vcommon.viewscale, 0).ToString();
                hwd.SetFont("-Arial-" + fs + "-");
                //hwd.DispCross(tr1, tc1, 20, 0);
                //hwd.SetTposition((int)(tr1 + 5), (int)(tc1 + 5));
                //hwd.WriteString(text1);

                fs = Math.Round(0.5 * vcommon.fontsize * vcommon.viewscale, 0).ToString();
                hwd.SetFont("-Arial-" + fs + "-");
                hwd.DispCross(tr2, tc2, 20, 0);
                int tr = (int)(tr2 + 5);
                int tc = (int)(tc2 + 5);
                hwd.SetTposition(tr, tc);
                string[] ostr = text2.Split('\r');
                foreach (string str in ostr)
                {
                    hwd.WriteString(str);
                    tr += (int)(0.5 * vcommon.fontsize+15);
                    hwd.SetTposition(tr, tc);
                }
                hwd.SetFont("");
            }
        }

        public void copy() {
            foreach (roishape rs in srois.rois) {
                roishape nrs = new roishape("rect", rs.row, rs.col1+10, rs.row1, rs.col1+10+rs.col1-rs.col);
                nrs.roipoint = rs.roipoint;
                nrs.thway = rs.thway;
                nrs.thmin = rs.thmin;
                nrs.thmax = rs.thmax;
                nrs.rowmin = rs.rowmin;
                nrs.rowmax = rs.rowmax;
                nrs.colmin = rs.colmin;
                nrs.colmax = rs.colmax;
                nrs.widthmin = rs.widthmin;
                nrs.widthmax = rs.widthmax;
                nrs.heightmin = rs.heightmin;
                nrs.heightmax = rs.heightmax;
                nrs.areamin = rs.areamin;
                nrs.areamax = rs.areamax;
                nrs.areamaxcheck = rs.areamaxcheck;
                nrs.closingcircle = rs.closingcircle;
                nums++;
                nrs.num = nums;
                roilist.Add(nrs);
            }
        }

        public void getbasepoint() {
            if (broi <= 0 )
            {
                brow = 0; bcol = 0;
                return;
            }
            roishape bshape = this[broi.ToString()];
            bshape.getregion(hw, dcamera.himg);
            bshape.getbasepoint(out brow, out bcol);
        }

        public void run() {
            hw.DispImage(dcamera.himg);
            dr=dc=0;
            //识别跟踪点
            if (broi > 0)
            {
                roishape bshape = this[broi.ToString()];
                if (bshape != null)
                {
                    bshape.getregion(hw, dcamera.himg);
                    bshape.showregion(hw, false);
                    double nbrow = 0, nbcol = 0;
                    bshape.getbasepoint(out nbrow, out nbcol);
                    dr = nbrow - brow; dc = nbcol - bcol;
                }
            }
            //计算识别点
            foreach (roishape rs in roilist)
            {
                if (rs.num==broi) continue;
                rs.getregion(hw, dcamera.himg,dr,dc);
            }
        }

        public void showresult(roishape rs) {
            if (rs == null) return;
            rs.show(hw, dr, dc);
        }
    }//class

    ///roi selection
    public class roiselections {
        private ArrayList slist;

        public roiselections() {
            slist = new ArrayList();
        }

        //索引器
        public roishape this[int index]
        {
            get { return (roishape)slist[index]; }
        }

        public void add(roishape roi) {
            slist.Add(roi);
        }

        public void clear() {
            slist.Clear();
        }

        /// 绘制roi到窗口控件
        public void paintroi(HWindow hw)
        {
            foreach (roishape cr in slist)  cr.showselect(hw);
        }

        public int count {
            get {return slist.Count;} 
        }

        public ArrayList rois {
            get { return slist; }
        }

        
        public void alignleft()
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                if (i == 0)
                {
                    rr = rs.col;
                    i++;
                }
                else
                {
                    cc = rs.col1 - rs.col;
                    rs.col = rr; rs.col1 = rs.col + cc;
                }
            }
        }
        public void alignright()
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                if (i == 0)
                {
                    rr = rs.col1;
                    i++;
                }
                else
                {
                    cc = rs.col1 - rs.col;
                    rs.col1 = rr; rs.col = rs.col1 - cc;
                }
            }
        }
        public void alignmidv()
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                if (i == 0)
                {
                    rr = (rs.row+rs.row1)/2;
                    i++;
                }
                else
                {
                    cc = (rs.row1 - rs.row)/2;
                    rs.row = rr-cc; rs.row1 = rr + cc;
                }
            }
        }
        public void aligntop()
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                if (i == 0)
                {
                    rr = rs.row;
                    i++;
                }
                else
                {
                    cc = rs.row1 - rs.row;
                    rs.row = rr; rs.row1 = rs.row + cc;
                }
            }
        }
        public void alignbot()
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                if (i == 0)
                {
                    rr = rs.row1;
                    i++;
                }
                else
                {
                    cc = rs.row1 - rs.row;
                    rs.row1 = rr; rs.row = rs.row1 - cc;
                }
            }
        }
        public void alignmidh()
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                if (i == 0)
                {
                    rr = (rs.col + rs.col1) / 2;
                    i++;
                }
                else
                {
                    cc = (rs.col1 - rs.col) / 2;
                    rs.row = rr - cc; rs.row1 = rr + cc;
                }
            }
        }
        public void alignwidth()
        {
            double rr = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                if (i == 0)
                {
                    rr = rs.col1 - rs.col;
                    i++;
                }
                else
                {
                    rs.col1 = rs.col +rr;
                }
            }
        }
        public void alignheight()
        {
            double rr = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                if (i == 0)
                {
                    rr = rs.row1 - rs.row;
                    i++;
                }
                else
                {
                    rs.row1 = rs.row + rr;
                }
            }
        }
        public void alignsamesize()
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                if (i == 0)
                {
                    rr = rs.row1 - rs.row;
                    cc = rs.col1 - rs.col;
                    i++;
                }
                else
                {
                    rs.row1 = rs.row + rr; rs.col1 = rs.col + cc;
                }
            }
        }
        public void alignsamegap()
        {
            if (slist.Count < 2) return;
            int i=slist.Count,j=0;
            double rr = 0, cc = 0,kk=0;
            rr=((slist[i-1] as roishape).col-(slist[0] as roishape).col)/(i-1);
            cc=(slist[0] as roishape).col;
            foreach (roishape rs in slist)
            {
                if(j>0)
                {
                    kk=rs.col1-rs.col;
                    rs.col = cc+rr; rs.col1 = rs.col + kk;
                }
                j++;
                cc=rs.col;
            }
        }

        public void moveup() {
            int dy=vcommon.posmove;
            foreach (roishape rs in slist) {
                rs.row -= dy; rs.row1 -= dy;
            }
        }

        public void movedown()
        {
            int dy = vcommon.posmove;
            foreach (roishape rs in slist)
            {
                rs.row += dy; rs.row1 += dy;
            }
        }

        public void moveleft() {
            int dy = vcommon.posmove;
            foreach (roishape rs in slist)
            {
                rs.col -= dy; rs.col1 -= dy;
            }            
        }

        public void moveright()
        {
            int dy = vcommon.posmove;
            foreach (roishape rs in slist)
            {
                rs.col += dy; rs.col1 += dy;
            }
        }
    }//class

}//namespace
