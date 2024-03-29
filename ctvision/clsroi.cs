﻿using System;
using System.Collections;
using System.Collections.Generic;
//using System.Text;using HalconDotNet;
using ctmeasure;
using System.Drawing;
using System.Windows.Forms;

using OpenCvSharp;    //添加相应的引用即可
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

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
        public int roipoint { get; set; }//0-面积中心,1-几何中心,2-上边,3-下边,4-左边,5-右边
        //选择
        public int thway { get; set; } //0-灰度,1-白色区域,2-黑色区域
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
        //public double stdsurface { get; set; }//残缺比标准
        public double msurface { get; set; }//残缺比测量值
        public int grayThresh { get;set; }//缺陷与模板亮度差
        public int grayThresh1 { get; set; }

        public int minDefectArea { get; set; }//最小缺陷面积

        public int minDefectWidth { get; set; }//最小缺陷宽度
        public int minDefectHeight { get; set; }//最小缺陷长度
        public int shrinkPixel { get; set; }
        public int spreadPixel { get; set; }

        public int defectArea { get; set; }//实际缺陷面积

        //public bool isTrackedPoint { get; set; }//是不是被跟踪点

        public int trackNumber { get; set; }//跟踪点编号
        public bool isTracked { get; set; }
        public double templateAr { get; set; }
        public double templateAc { get; set; }

        public double shiftr { get; set; }
        public double shiftc { get; set; }

        int maxWhiteConIdx = -1;
        Point[][] whiteContours;

        [NonSerialized]
        HierarchyIndex[] whiteHierarchly;
        [NonSerialized]
        Point[][] regionContours;//测量
        [NonSerialized]
        HierarchyIndex[] regionHierarchly;

        [NonSerialized]
        public Mat whiteMask;//要做序列化或者保存
        [NonSerialized]
        public Mat blackMask;//要做序列化或者保存
        [NonSerialized]
        public Mat mask;
        //public Mat origImagePart = new Mat();

        //计算结果
        [NonSerialized]
        public Rect roirng;
        [NonSerialized]
        public double area, ar, ac, cr, cc, cradius, gr,gc;
        [NonSerialized]
        public int r1, c1, r2, c2;
        [NonSerialized]
        Mat srcCopy;
        [NonSerialized]
        Mat roiCopy;
        [NonSerialized]
        Rect boundingRect;
        [NonSerialized]
        Moments mmt;
        [NonSerialized]
        int brx = int.MaxValue;
        [NonSerialized]
        int bry = int.MaxValue;
        [NonSerialized]
        int brx1 = 0;
        [NonSerialized]
        int bry1 = 0; 


        public roishape() {
            shape = "rect";
            num = -1;
            row = col = row1 = col1 = 0;
            roipoint = 0;
            thway = 0;
            thmin = 2; thmax = 128;
            closingcircle = 0;
            
            rowmin = colmin = widthmin = heightmin = areamin = 0;
            rowmax = colmax = widthmax = heightmax = areamax = 99999;
            areamaxcheck = true;
            area = ar = ac = cr = cc = cradius = gr = gc= 0;

            //isTrackedPoint = false;
            trackNumber = 0;
            templateAr = templateAc = 0;
            shiftr = shiftc = 0;
            isTracked = false;
            
        }

        public roishape(string strshape, Mat himgbak, double vr1, double vc1, double vr2, double vc2):this() 
        {
            shape = strshape;
            row = vr1; col = vc1; row1 = vr2; col1 = vc2;
            srcCopy = new Mat();
            roiCopy = new Mat();

            //isTrackedPoint = false;
            trackNumber = 0;
            templateAr = templateAc = 0;
            shiftr = shiftc = 0;
            isTracked = false;
        }

        public void repair(Mat himg, Mat himgback, int iw, int ih)
        {
            ////if (himgback == null) return;
            //double wRatio = himgback.Width * 1.0 / iw;// hw.Width;
            //double hRatio = himgback.Height * 1.0 / ih;// hw.Height;
            //double hshift = (ih * wRatio * 1.0 - himgback.Height) / 2.0;
            Rect roi = new Rect(new OpenCvSharp.Point(col - vcommon.viewx, row - vcommon.viewy), new OpenCvSharp.Size((col1 - col) , (row1 - row) ));// Convert.ToInt32(col), Convert.ToInt32(row), Convert.ToInt32(), Convert.ToInt32(row1 - row));// ;
            //Console.WriteLine("width,wRatio :{0},{1}", (col1 - col) * wRatio, wRatio);
            //if (roi.Width == 0 || roi.Height == 0) return;
            if (roi.X <= 0 || roi.Y <= 0 ||roi.Width<=0||roi.Height<=0|| ((roi.X + roi.Width) >= himg.Width) || ((roi.Y + roi.Height) > himg.Height))
            {
                //MessageBox.Show("ROI 超出边界");
                return;
            }
            himgback[roi].CopyTo(himg[roi]);
            //Cv2.ImWrite("C:\\Users\\24981\\Desktop\\ct\\aab.bmp", himgback);
        }

        //public void updateOrigImagePart(Mat himg, int iw, int ih)
        //{
        //    double wRatio = himg.Width * 1.0 / iw;// hw.Width;
        //    double hRatio = himg.Height * 1.0 / ih;// hw.Height;
        //    double hshift = (ih * wRatio * 1.0 - himg.Height) / 2.0;
        //    Rect roi = new Rect(new OpenCvSharp.Point(col * wRatio, row * wRatio - hshift), new OpenCvSharp.Size((col1 - col) * wRatio, (row1 - row) * wRatio));// Convert.ToInt32(col), Convert.ToInt32(row), Convert.ToInt32(), Convert.ToInt32(row1 - row));// ;
        //    Console.WriteLine("width,wRatio :{0},{1}", (col1 - col) * wRatio, wRatio);
        //    if (roi.Width == 0 || roi.Height == 0) return; 
        //    new Mat(himg, roi).CopyTo(origImagePart);
             
        //}

        public void checkrowcol()
        {
            double r1 = Math.Min(row, row1);
            double r2 = Math.Max(row, row1);
            double c1 = Math.Min(col, col1);
            double c2 = Math.Max(col, col1);
            row = r1; row1 = r2; col = c1; col1 = c2;
        }

        public void show(PaintEventArgs e) {
            
            if (shape == "rect")
            {
                RectangleList rect = new RectangleList(Convert.ToInt32(col), Convert.ToInt32(row ), Convert.ToInt32((col1 - col)), Convert.ToInt32((row1 - row)));
                //Console.WriteLine("show :{0},{1},{2},{3}", col, row, col1, row1);
                e.Graphics.DrawRectangle(Pens.Green, rect.rectangle);
                //e.Graphics.DrawRectangles(vcommon.hcolor, rect.subRectList.ToArray());
                e.Graphics.DrawString(num.ToString(), new Font("Arial", Convert.ToInt32(180 * vcommon.viewscale)), new SolidBrush(Color.Green), Convert.ToSingle(col ), Convert.ToSingle(row ));
                if (trackNumber > 0) { 
                    e.Graphics.DrawString(trackNumber.ToString(), new Font("Arial", Convert.ToInt32(180 * vcommon.viewscale)), new SolidBrush(Color.Blue), Convert.ToSingle(col1 ), Convert.ToSingle(row1 ));
                }
            }
            //if (action == "ondraw")
            //{
            //    RectangleList rect = getRectangle();
            //    e.Graphics.DrawRectangle(Pens.Lime, rect.rectangle);
            //}
            //if (selectedRectCounter == counter && mouseModeEnum == MouseMode.OBJ_SELECT)
            //{

            //}
            //counter++;
        }

        //原点跟踪,dr,dc偏移量
        public void show(Mat himg, roishape rs,bool isng=false)
        {
            Rect roi = new Rect(new OpenCvSharp.Point(rs.col - vcommon.viewx, rs.row - vcommon.viewy), new OpenCvSharp.Size((rs.col1 - rs.col), (rs.row1 - rs.row)));// Convert.ToInt32(col), Convert.ToInt32(row), Convert.ToInt32(), Convert.ToInt32(row1 - row));// ;
            if (roi.X <= 0 || roi.Y <= 0 || roi.Width <= 0 || roi.Height <= 0 || ((roi.X + roi.Width) >= himg.Width) || ((roi.Y + roi.Height) > himg.Height))
            {
                //MessageBox.Show("ROI 超出边界");
                Program.fmain.mstatus.Text = "ROI 超出边界";
                return;
            }
            if (srcCopy != null) {
                Console.WriteLine(isng);
                /////////////////////////////////////////
                if (isng && Program.fmain.tbrun.Checked)
                { 
                    Cv2.DrawContours(
                                srcCopy,
                                regionContours,
                                -1,
                                color: new Scalar(0, 0, 255),
                                thickness: 3,//CV_FILLED
                                lineType: LineTypes.Link8,
                                hierarchy: regionHierarchly,
                                maxLevel: int.MaxValue);
                }
                srcCopy.CopyTo(himg[roi]);
            }
            else
            {
                Console.WriteLine("showresult srcCopy is null");
            }

            //Cv2.ImShow("srcCopy", srcCopy);
            //Cv2.WaitKey(10000000);
            //double wRatio = himg.Width * 1.0 / iw;// hw.Width;
            //double hRatio = himg.Height * 1.0 / ih;// hw.Height;
            //double hshift = (ih * wRatio * 1.0 - himg.Height) / 2.0;
            //Rect roi = new Rect(new OpenCvSharp.Point(col * wRatio, row * wRatio - hshift), new OpenCvSharp.Size((col1 - col) * wRatio, (row1 - row) * wRatio));// Convert.ToInt32(col), Convert.ToInt32(row), Convert.ToInt32(), Convert.ToInt32(row1 - row));// ;
            //Console.WriteLine("width,wRatio :{0},{1}", (col1 - col) * wRatio, wRatio);
            //if (roi.Width == 0 || roi.Height == 0) return;
            //Mat ImageROI = new Mat();//himg[roi];// 
            //Mat srcCopy = new Mat();
            //if (origImagePart.Width == 0 || origImagePart.Height == 0)
            //{
            //    new Mat(himg, roi).CopyTo(origImagePart);
            //}
            ////Cv2.ImShow("origImagePart", origImagePart);
            ////Cv2.WaitKey(10000000);
            //origImagePart.CopyTo(srcCopy);
            //origImagePart.CopyTo(ImageROI);
            ////#hw.SetColor(vcommon.hcolor);
            //hw.SetDraw("margin");
            //if (shape == "rect")
            //{
            //    hw.DispRectangle1(row+dr, col+dc, row1+dr, col1+dc);
            //    hw.SetColor(vcommon.hcolortext);
            //    hw.SetTposition((int)(row+dr + 5.0), (int)(col+dc + 5.0));
            //    hw.WriteString(num.ToString());
            //}

            ////显示定位
            ////#hw.SetColor(vcommon.hcoloractive);
            //if (roipoint == 0) { hw.DispCross(ar, ac, 50, 0); }
            //if (roipoint == 1) { hw.DispCross(gr, gc, 50, 0); }
            //if (roipoint == 2) { hw.DispLine(r1, col+dc, r1, col1+dc); }
            //if (roipoint == 3) { hw.DispLine(r2, col+dc, r2, col1+dc); }
            //if (roipoint == 4) { hw.DispLine(row+dr, c1, row1+dr, c1); }
            //if (roipoint == 5) { hw.DispLine(row+dr, c2, row1+dr, c2); }
        }

        public void showselect(PaintEventArgs e) {
            //#hw.SetColor(vcommon.hcoloractive);
            //hw.SetDraw("margin");
            if (shape == "rect")
            {
                //hw.DispRectangle1(row, col, row1, col1);
                //RectangleList rect = new RectangleList(Convert.ToInt32(col), Convert.ToInt32(row), Convert.ToInt32(col1 - col), Convert.ToInt32(row1 - row));
                RectangleList rect = new RectangleList(Convert.ToInt32(col), Convert.ToInt32(row), Convert.ToInt32((col1 - col)), Convert.ToInt32((row1 - row)));
                e.Graphics.DrawRectangle(vcommon.hcoloractive, rect.rectangle);
                e.Graphics.DrawRectangles(vcommon.hcolorhandle, rect.subRectList.ToArray());
                //#hw.SetColor(vcommon.hcolortext);
                //hw.SetTposition((int)(row + 5.0), (int)(col + 5.0));
                //hw.WriteString(num.ToString());

                ////#hw.SetColor(vcommon.hcolorhandle);
                //double hlen = 5 / vcommon.viewscale;
                //hw.DispRectangle2(row, col, 0, hlen, hlen);
                //hw.DispRectangle2(row, col1,0,hlen, hlen);
                //hw.DispRectangle2(row1, col,0,hlen, hlen);
                //hw.DispRectangle2(row1, col1,0,hlen, hlen);
            }

        }
         
        public void getWhiteMask(Mat himg, Mat himgback, bool isdraw = false, bool isshow =false)
        { 
            Rect roi = new Rect(new OpenCvSharp.Point(col - vcommon.viewx, row - vcommon.viewy), new OpenCvSharp.Size((col1 - col), (row1 - row)));
            if (roi.Width == 0 || roi.Height == 0) return;
            if (himgback == null) return;
            if (roi.X <= 0 || roi.Y <= 0 || roi.Width <= 0 || roi.Height <= 0 || ((roi.X + roi.Width) >= himg.Width) || ((roi.Y + roi.Height) > himg.Height))
            {
                //MessageBox.Show("ROI 超出边界");
                return;
            }
            Mat ImageROI = himgback[roi];// 
            if (!isdraw) { 
                roiCopy = Mat.Zeros(ImageROI.Size(), MatType.CV_8UC3);
                ImageROI.CopyTo(roiCopy);
            }
            whiteMask = Mat.Zeros(ImageROI.Size(), MatType.CV_8UC1);
                //new Mat(new OpenCvSharp.Size(ImageROI.Width, ImageROI.Height), MatType.CV_8UC1);
               
             
           
            Cv2.CvtColor(ImageROI, ImageROI, ColorConversionCodes.BGRA2GRAY);
             
            ////处理ImageROI
            if (closingcircle == 0) { Cv2.Blur(ImageROI, ImageROI, new OpenCvSharp.Size(9, 9)); }
            if (closingcircle == 1) { Cv2.Blur(ImageROI, ImageROI, new OpenCvSharp.Size(5, 5)); }
            if (closingcircle == 2) { Cv2.Blur(ImageROI, ImageROI, new OpenCvSharp.Size(2, 2)); }

            Cv2.Threshold(ImageROI, ImageROI, 127+thminsurface, 255, ThresholdTypes.Binary);//thmin, thmax
            //Cv2.ImShow("BitwiseNot", ImageROI);
            //Cv2.WaitKey(10000000);
            if (closingcircle == 0)
            {
                InputArray kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(9, 9));
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Close, kernel);
            }
            else if (closingcircle == 1)
            {
                InputArray kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(5, 5));
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Close, kernel);
            }
            else
            {
                InputArray kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(3, 3));
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Close, kernel);
            }

            if (shrinkPixel != 0) {
                //腐蚀
                Mat se = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(shrinkPixel, shrinkPixel), new Point(-1, -1));
                Cv2.Erode(ImageROI, ImageROI, se, new Point(-1, -1), 1);
            }

            //获得轮廓
            //OpenCvSharp.Point[][] contours;
            
            Cv2.FindContours(ImageROI, out whiteContours, out whiteHierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new OpenCvSharp.Point(0, 0));
            if (whiteContours.Length == 0) return;
            double maxContourArea = 0;
            maxWhiteConIdx = 0;
            //brx = int.MaxValue;
            //bry = int.MaxValue;
            //brx1 = 0;
            //bry1 = 0;
            //int thickns = -1;
            //if (Program.fmain.isRunOrRunOnceChecked)
            //{
            //    thickns = 3;
            //}
            ImageROI.CopyTo(whiteMask);
            
            
            //if (Program.fmain.cksurfaceareamax.Checked)
            //{
            //    //ImageROI.CopyTo(whiteMask);
            //    Cv2.DrawContours(
            //                roiCopy,
            //                whiteContours,
            //                maxWhiteConIdx,
            //                color: new Scalar(0, 0, 255),
            //                thickness: 3,//CV_FILLED
            //                lineType: LineTypes.Link8,
            //                hierarchy: whiteHierarchly,
            //                maxLevel: 0);
            //}
            //else
            {
                Cv2.DrawContours(
                           roiCopy,
                           whiteContours,
                           -1,
                           color: new Scalar(0, 0, 255),
                           thickness: 3,//CV_FILLED
                           lineType: LineTypes.Link8,
                           hierarchy: whiteHierarchly,
                           maxLevel: int.MaxValue);
            }

            //Cv2.ImWrite(".\\aaa_ImageROI.bmp", ImageROI);
            //Cv2.ImWrite(".\\aaa_whitemask.bmp", whiteMask);
            if (isshow)
            {
                roiCopy.CopyTo(himg[roi]);
            }
        }
        public void getBlackMask(Mat himg, Mat himgback, bool isdraw = false, bool isshow = false)
        {
            Rect roi = new Rect(new OpenCvSharp.Point(col - vcommon.viewx, row - vcommon.viewy), new OpenCvSharp.Size((col1 - col), (row1 - row)));
            if (roi.Width == 0 || roi.Height == 0) return;
            if (himgback == null) return;
            if (roi.X <= 0 || roi.Y <= 0 || roi.Width <= 0 || roi.Height <= 0 || ((roi.X + roi.Width) >= himg.Width) || ((roi.Y + roi.Height) > himg.Height))
            {
                //MessageBox.Show("ROI 超出边界");
                return;
            }
            Mat ImageROI = himgback[roi];// 
            if (!isdraw) { 
                roiCopy = Mat.Zeros(ImageROI.Size(), MatType.CV_8UC3);
                ImageROI.CopyTo(roiCopy);
            }
            blackMask = Mat.Zeros(ImageROI.Size(), MatType.CV_8UC1);

            
            Cv2.CvtColor(ImageROI, ImageROI, ColorConversionCodes.BGRA2GRAY);

            ////处理ImageROI
            if (closingcircle == 0) { Cv2.Blur(ImageROI, ImageROI, new OpenCvSharp.Size(9, 9)); }
            if (closingcircle == 1) { Cv2.Blur(ImageROI, ImageROI, new OpenCvSharp.Size(5, 5)); }
            if (closingcircle == 2) { Cv2.Blur(ImageROI, ImageROI, new OpenCvSharp.Size(2, 2)); }
            Cv2.BitwiseNot(ImageROI, ImageROI);
            Cv2.Threshold(ImageROI, ImageROI, 255-thmaxsurface, 255, ThresholdTypes.Binary);//thmin, thmax
            //Cv2.ImShow("BitwiseNot", ImageROI);
            //Cv2.WaitKey(10000000);
            if (closingcircle == 0)
            {
                InputArray kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(9, 9));
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Close, kernel);
            }
            else if (closingcircle == 1)
            {
                InputArray kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(5, 5));
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Close, kernel);
            }
            else
            {
                InputArray kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(3, 3));
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Close, kernel);
            }

            if (spreadPixel != 0)
            {
                //腐蚀
                Mat se = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(spreadPixel, spreadPixel), new Point(-1, -1));
                Cv2.Erode(ImageROI, ImageROI, se, new Point(-1, -1), 1);
            }

            //获得轮廓
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchly;
            Cv2.FindContours(ImageROI, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new OpenCvSharp.Point(0, 0));
            if (contours.Length == 0) return;
            double maxContourArea = 0;
            int maxConIdx = 0;
            brx = int.MaxValue;
            bry = int.MaxValue;
            brx1 = 0;
            bry1 = 0;
            //int thickns = -1;
            //if (Program.fmain.isRunOrRunOnceChecked)
            //{
            //    thickns = 3;
            //}
            ImageROI.CopyTo(blackMask);
            
            //if (Program.fmain.tbrun.Checked) { 
            //    for (int i = 0; i < contours.Length; i++)
            //    {
            //    //Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            //    //Cv2.DrawContours(dst_Image, contours, i, color, 2, LineTypes.Link8, hierarchly);

            //        var contour = contours[i];
                 
            //        double area1 = Cv2.ContourArea(contour);
            //        if (area1 > maxContourArea)
            //        {
            //            maxContourArea = area1;
            //            maxConIdx = i;
            //        }
            //    }
            //    Rect br = Cv2.BoundingRect(contours[maxConIdx]);
            //    int cex = (br.X + br.Width) / 2;
            //    int cey = (br.Y + br.Height) / 2;
            //    Cv2.Circle(roiCopy, cex, cey, 100, new Scalar(0, 0, 255), 3);
            //}
            //if (Program.fmain.cksurfaceareamax.Checked)
            //{ 
            //    Cv2.DrawContours(
            //                roiCopy,
            //                contours,
            //                maxConIdx,
            //                color: new Scalar(0, 255,0),
            //                thickness: 3,//CV_FILLED
            //                lineType: LineTypes.Link8,
            //                hierarchy: hierarchly,
            //                maxLevel: 0);

            //}
            //else

            {
                Cv2.DrawContours(
                            roiCopy,
                            contours,
                            -1,
                            color: new Scalar(0, 255, 0),
                            thickness: 3,//CV_FILLED
                            lineType: LineTypes.Link8,
                            hierarchy: hierarchly,
                            maxLevel: int.MaxValue);
            }

           
            //Cv2.ImWrite("C:\\Users\\24981\\Desktop\\ctvision源码\\result.bmp", himg);
            if (isshow) { 
                roiCopy.CopyTo(himg[roi]);
            }

        }

        //进行空位测距
        //阈值分割定位空位中心点
        //根据中心点进行尺寸相关测量
        //为保证测量精度，原始图像不进行缩放。定位中心点更准确，发挥出高分辨率图像优势。
        public void getregion(Mat himg, Mat himgback)
        {
            //double wRatio = himg.Width * 1.0 / iw;// hw.Width;
            //double hRatio = himg.Height * 1.0 / ih;// hw.Height;
            //double hshift = (ih * wRatio * 1.0 - himg.Height) / 2.0;
            Rect roi = new Rect(new OpenCvSharp.Point(col - vcommon.viewx, row - vcommon.viewy), new OpenCvSharp.Size((col1 - col), (row1 - row)));// Convert.ToInt32(col), Convert.ToInt32(row), Convert.ToInt32(), Convert.ToInt32(row1 - row));// ;
            //Console.WriteLine("width,wRatio :{0},{1}", (col1 - col), wRatio);
            if (roi.Width == 0 || roi.Height == 0) return;
            if (himgback == null) return;
            if (roi.X <= 0 || roi.Y <= 0 || roi.Width <= 0 || roi.Height <= 0 || ((roi.X + roi.Width) >= himg.Width) || ((roi.Y + roi.Height) > himg.Height))
            {
                //MessageBox.Show("ROI 超出边界");
                return;
            }
            Mat ImageROI = new Mat();// himgback[roi];// 
            srcCopy = new Mat();
            himgback[roi].CopyTo(ImageROI);
            ImageROI.CopyTo(srcCopy);
            //if (origImagePart.Width == 0 || origImagePart.Height == 0) {
            //    new Mat(himg, roi).CopyTo(origImagePart );
            //}

            //himg.CopyTo(origImagePart);

            //Cv2.ImShow("origImagePart", origImagePart);
            //Cv2.WaitKey(10000000);
            //origImagePart.CopyTo(srcCopy);
            //origImagePart.CopyTo(ImageROI);
            //if (srcCopy == null)
            //{
            //    srcCopy = new Mat();
            //}
            
            //ImageROI.CopyTo(srcCopy);
            Cv2.CvtColor(ImageROI, ImageROI, ColorConversionCodes.BGRA2GRAY);
            if ((thmin + thmax) / 2.0 < 127)
            {
                Cv2.BitwiseNot(ImageROI, ImageROI);
            }
            //处理ImageROI
            if (closingcircle == 0) { 
                Cv2.Blur(ImageROI, ImageROI, new OpenCvSharp.Size(9, 9));
            }
            else if (closingcircle == 1) { Cv2.Blur(ImageROI, ImageROI, new OpenCvSharp.Size(5, 5)); }
            else { Cv2.Blur(ImageROI, ImageROI, new OpenCvSharp.Size(2, 2)); }

            Cv2.Threshold(ImageROI, ImageROI, thmin, thmax, ThresholdTypes.Binary);//thmin, thmax
            //Cv2.ImShow("BitwiseNot", ImageROI);
            //Cv2.WaitKey(10000000);
            if (closingcircle == 0)
            {
                InputArray kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(9, 9));
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Close, kernel);
            }
            else if (closingcircle == 1)
            {
                InputArray kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(5, 5));
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Close, kernel);
            }
            else
            {
                InputArray kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(3, 3));
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(ImageROI, ImageROI, MorphTypes.Close, kernel);
            }
            

            //获得轮廓
            //OpenCvSharp.Point[][] regionContours;
            //HierarchyIndex[] regionHierarchly;
            Cv2.FindContours(ImageROI, out regionContours, out regionHierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new OpenCvSharp.Point(0, 0));
            if (regionContours.Length == 0)
            {
                area = 0; ar = row; ac = col;
                r1 = r2 = (int)row;
                c1 = c2 = (int)col;
                gr = row; gc = col;
                cradius = 0; cr = row; cc = col;
                return;
            }
            double maxContourArea = 0;
            int maxConIdx = 0;
            brx = int.MaxValue;
            bry = int.MaxValue;
            brx1 = 0;
            bry1 = 0;
            int thickns = -1;
            if (Program.fmain.isRunOrRunOnceChecked)
            {
                thickns = 3;
            }


            if (!areamaxcheck && combinecheck)
            {
                for (int i = 0; i < regionContours.Length; i++)
                {
                    //Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                    //Cv2.DrawContours(dst_Image, contours, i, color, 2, LineTypes.Link8, hierarchly);

                    var contour = regionContours[i];    
                    Rect br = Cv2.BoundingRect(regionContours[i]);
                    if ((br.X + br.Width) > brx1)
                    {
                        brx1 = (br.X + br.Width);
                    }
                    if ((br.Y + br.Height) > bry1)
                    {
                        bry1 = br.Y + br.Height;
                    }

                    if (br.X < brx)
                    {
                        brx = br.X;
                    }
                    if (br.Y < bry)
                    {
                        bry = br.Y;
                    }

                     
                    double area1 = Cv2.ContourArea(contour);
                    if (area1 > maxContourArea)
                    {
                        maxContourArea = area1;
                        maxConIdx = i;
                    }
                }
            }
            if (areamaxcheck && !combinecheck)
            {
                maxConIdx = 0;
                for (int i = 0; i < regionContours.Length; i++)
                { 

                    var contour = regionContours[i];
                    
                    double area2 = Cv2.ContourArea(contour);
                    if (area2 > maxContourArea)
                    {
                        maxContourArea = area2;
                        maxConIdx = i;
                    }
                }
                Rect br = Cv2.BoundingRect(regionContours[maxConIdx]);
                if ((br.X + br.Width) > brx1)
                {
                    brx1 = (br.X + br.Width);
                }
                if ((br.Y + br.Height) > bry1)
                {
                    bry1 = br.Y + br.Height;
                }

                if (br.X < brx)
                {
                    brx = br.X;
                }
                if (br.Y < bry)
                {
                    bry = br.Y;
                }
                 
            }
            
            
            boundingRect = new Rect(brx, bry, (brx1 - brx), (bry1 - bry));
            /////////////////////////////////////////
            //if (!Program.fmain.tbrun.Checked)
            {
                if (areamaxcheck && !combinecheck)
                {
                    //Cv2.ImWrite("C:\\Users\\Administrator\\Pictures\\result.bmp", srcCopy);
                    //return;
                    Cv2.DrawContours(
                                srcCopy,
                                regionContours,
                                maxConIdx,
                                color: new Scalar(0, 0, 255),
                                thickness: thickns,//CV_FILLED
                                lineType: LineTypes.Link8,
                                hierarchy: regionHierarchly,
                                maxLevel: 0);

                    boundingRect = Cv2.BoundingRect(regionContours[maxConIdx]); //Find bounding rect for each contour
                }
                else
                {
                    Cv2.DrawContours(
                                srcCopy,
                                regionContours,
                                -1,
                                color: new Scalar(0, 0, 255),
                                thickness: thickns,//CV_FILLED
                                lineType: LineTypes.Link8,
                                hierarchy: regionHierarchly,
                                maxLevel: int.MaxValue);
                }
            }
            //Cv2.Rectangle(srcCopy,
            //        new OpenCvSharp.Point(boundingRect.X, boundingRect.Y),
            //        new OpenCvSharp.Point(boundingRect.X + boundingRect.Width, boundingRect.Y + boundingRect.Height),
            //        new Scalar(255, 0, 0),
            //        20);



            mmt = Cv2.Moments(regionContours[maxConIdx]);
            double cx = mmt.M10 / mmt.M00, cy = mmt.M01 / mmt.M00;
            if (!areamaxcheck && combinecheck)
            {
                cx = (brx + brx1) / 2.0;
                cy = (bry + bry1) / 2.0;
            }
            //显示定位
            //#hw.SetColor(vcommon.hcoloractive);
            if (roipoint == 0)
            {//new OpenCvSharp.Point(centerx-10,centery)
                Cv2.Line(srcCopy, Convert.ToInt32(cx) - boundingRect.Width / 12, Convert.ToInt32(cy), Convert.ToInt32(cx) + boundingRect.Width / 12, Convert.ToInt32(cy), new Scalar(0, 255, 0), 4, LineTypes.Link8);
                Cv2.Line(srcCopy, Convert.ToInt32(cx), Convert.ToInt32(cy) - boundingRect.Height / 12, Convert.ToInt32(cx), Convert.ToInt32(cy) + boundingRect.Height / 12, new Scalar(0, 255, 0), 4, LineTypes.Link8);
                //hw.DispCross(ar, ac, 50, 0);
            }
            if (roipoint == 1)
            {
                int centerx = (boundingRect.X + boundingRect.Width / 2);
                int centery = (boundingRect.Y + boundingRect.Height / 2);
                Cv2.Line(srcCopy, centerx - boundingRect.Width / 12, centery, centerx + boundingRect.Width / 12, centery, new Scalar(255, 0, 0), 4, LineTypes.Link8);
                Cv2.Line(srcCopy, centerx, centery - boundingRect.Height / 12, centerx, centery + boundingRect.Height / 12, new Scalar(255, 0, 0), 4, LineTypes.Link8);
            }
            if (roipoint == 2)
            {//上边
                //hw.DispLine(r1, col + dc, r1, col1 + dc); 
                Cv2.Line(srcCopy, boundingRect.X, boundingRect.Y, boundingRect.X + boundingRect.Width, boundingRect.Y, new Scalar(255, 0, 0), 8, LineTypes.Link8);
            }
            if (roipoint == 3)
            { //下边
              //hw.DispLine(r2, col + dc, r2, col1 + dc); 
                Cv2.Line(srcCopy, boundingRect.X, boundingRect.Y + boundingRect.Height, boundingRect.X + boundingRect.Width, boundingRect.Y + boundingRect.Height, new Scalar(255, 0, 0), 8, LineTypes.Link8);
            }
            if (roipoint == 4)
            { //左边
                //hw.DispLine(row + dr, c1, row1 + dr, c1); 
                Cv2.Line(srcCopy, boundingRect.X, boundingRect.Y, boundingRect.X, boundingRect.Y + boundingRect.Height, new Scalar(255, 0, 0), 8, LineTypes.Link8);
            }
            if (roipoint == 5)
            {//
             // hw.DispLine(row + dr, c2, row1 + dr, c2); 
                Cv2.Line(srcCopy, boundingRect.X + boundingRect.Width, boundingRect.Y, boundingRect.X + boundingRect.Width, boundingRect.Y + boundingRect.Height, new Scalar(255, 0, 0), 8, LineTypes.Link8);
            }

            //Cv2.ImShow("DrawContours srcCopy.", srcCopy);
            //Cv2.WaitKey(500);

            //计算特征
            if (regionContours.Length == 0)
            {
                area = 0; ar = row; ac = col;
                r1 = r2 = (int)row;
                c1 = c2 = (int)col;
                gr = row; gc = col;
                cradius = 0; cr = row; cc = col;
            }
            else
            {//要加换算公式
                ar = roi.Y + cy;
                ac = roi.X + cx;
                area = maxContourArea;
                r1 = roi.Y + Convert.ToInt32(boundingRect.Y);
                c1 = roi.X + Convert.ToInt32(boundingRect.X);
                r2 = roi.Y + Convert.ToInt32(boundingRect.Y + boundingRect.Height);
                c2 = roi.X + Convert.ToInt32((boundingRect.X + boundingRect.Width));
                //roirng.SmallestRectangle1(out r1, out c1, out r2, out c2);
                gr = (r2 + r1) / 2;
                gc = (c2 + c1) / 2;

                //循环进行绘制
                Point2f center;  //定义圆中心坐标
                float radius;  //定义圆半径
                //if (Program.fmain.cbmtype.SelectedIndex == 4) {
                //    Cv2.MinEnclosingCircle(contours[maxConIdx], out center, out radius);
                //    cr = roi.Y + center.Y;
                //    cc = roi.X + center.X;
                //    cradius = Convert.ToDouble(radius);
                //}
                cr = gr;
                cc = gc;
                cradius = (cr + gr) / 2;
                
            }
            if (isTracked)
            {
                if (!Program.fmain.tbrun.Checked)
                {
                    templateAr = ar;
                    templateAc = ac;
                }
                else
                {
                    
                    shiftr = ar - templateAr;
                    shiftc = ac - templateAc;
                    
                }
            }

           



            //Cv2.ImWrite("C:\\Users\\24981\\Desktop\\ctvision源码\\result.bmp", himg);
            //srcCopy.CopyTo(himg[roi]);


        }

        ////原点跟踪， dr，dc偏移量
        //public void getregion(HWindow hw, HImage himg,double dr,double dc)
        //{

        //    HRegion nrng = new HRegion(row+dr, col+dc, row1+dr, col1+dc);
        //    HImage timg = himg.ReduceDomain(nrng);
        //    if (closingcircle == 0) timg = timg.MeanImage(9, 9);
        //    if (closingcircle == 1) timg = timg.MeanImage(5, 5);
        //    if (closingcircle == 2) timg = timg.MeanImage(2, 2);
        //    //hw.DispImage(timg);
        //    nrng = timg.Threshold(thmin, thmax);
        //    nrng = nrng.Connection();

        //    //过滤
        //    if (rowmin > 0 || rowmax < 99999) nrng = nrng.SelectShape("row", "and", rowmin, rowmax);
        //    if (colmin > 0 || colmax < 99999) nrng = nrng.SelectShape("column", "and", colmin, colmax);
        //    if (widthmin > 0 || widthmax < 99999) nrng = nrng.SelectShape("width", "and", widthmin, widthmax);
        //    if (heightmin > 0 || heightmax < 99999) nrng = nrng.SelectShape("height", "and", heightmin, heightmax);
        //    if (areamin > 0 || areamax < 99999) nrng = nrng.SelectShape("area", "and", areamin, areamax);
        //    //精度
        //    if (closingcircle == 0) { nrng = nrng.OpeningCircle(9.5); nrng = nrng.ClosingCircle(9.5); }
        //    if (closingcircle == 1) { nrng = nrng.OpeningCircle(5.5); nrng = nrng.ClosingCircle(5.5); }
        //    if (closingcircle == 2) { nrng = nrng.OpeningCircle(2.5); nrng = nrng.ClosingCircle(2.5); }
        //    //最大区域
        //    nrng = nrng.Connection();
        //    if (areamaxcheck) nrng = nrng.SelectShapeStd("max_area", 70);
        //    nrng = nrng.FillUp();

        //    //roirng = nrng.Union1();
        //    //if (!areamaxcheck && combinecheck) roirng = roirng.ClosingRectangle1(500, 500);
        //    ////计算特征
        //    //if (roirng.Area.Length == 0) {
        //    //    area = 0; ar = row; ac = col;
        //    //    r1 = r2 = (int)row; c1 = c2 = (int)col; gr = row; gc = col;
        //    //    cradius = 0; cr = row; cc = col;
        //    //} else {
        //    //    area = roirng.AreaCenter(out ar, out ac);
        //    //    roirng.SmallestRectangle1(out r1, out c1, out r2, out c2);
        //    //    gr = (r2 + r1) / 2;
        //    //    gc = (c2 + c1) / 2;
        //    //    roirng.SmallestCircle(out cr, out cc, out cradius);
        //    //}
        //    nrng.Dispose();
        //    timg.Dispose();
        //}

        public void showregion(Mat himg ,bool showregion)
        { 
            if (showregion)
            {
                Rect roi = new Rect(new OpenCvSharp.Point(col - vcommon.viewx, row - vcommon.viewy), new OpenCvSharp.Size((col1 - col), (row1 - row)));// Convert.ToInt32(col), Convert.ToInt32(row), Convert.ToInt32(), Convert.ToInt32(row1 - row));// ;
                if (roi.X <= 0 || (roi.X + roi.Width) > himg.Width || roi.Y <= 0 || (roi.Y + roi.Height) > himg.Height) {
                    return;
                }
                srcCopy.CopyTo(himg[roi]);
            } 
            //显示定位
            //#hw.SetColor(vcommon.hcoloractive);
            //if (roipoint == 0) { hw.DispCross(ar, ac, 50, 0); }
            //if (roipoint == 1) { hw.DispCross(gr, gc, 50, 0); }
            //if (roipoint == 2) { hw.DispLine(r1, col, r1, col1); }
            //if (roipoint == 3) { hw.DispLine(r2, col, r2, col1); }
            //if (roipoint == 4) { hw.DispLine(row, c1, row1, c1); }
            //if (roipoint == 5) { hw.DispLine(row, c2, row1, c2); }
        }

        public void showmargin(PictureBox pb) {
            //hw.SetDraw("margin");
            //hw.SetColor("green");
            if (roirng == null) return;
            //#hw.DispRegion(roirng);
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
        public void showsurface(PictureBox pb, Mat himg, bool showarea) 
        {
            if (surfacecheck == false) return;
            //HRegion nrng = new HRegion(row, col, row1, col1);
            //HImage timg = himg.ReduceDomain(nrng);
            //nrng = timg.Threshold(thminsurface*1.0, thmaxsurface*1.0);
            //nrng = nrng.Connection();
            //nrng = nrng.SelectShape("area", "and", 50.0, 999999);
            //nrng = nrng.Union1();
            //nrng = nrng.ClosingCircle(3.0);
            //double ar,ac;
            //if (nrng.Area.Length>0) areasurface = nrng.AreaCenter(out ar, out ac);
            //else areasurface = 0;
            //hw.SetDraw("fill");
            //hw.SetColor("red");
            //hw.DispRegion(nrng);
            //timg.Dispose();
        }
        enum CONNECTIONS_STATS
        {
            X = 0,
            Y,
            WIDTH,
            HEIGHT,
            AREA,
            STATS_INDEX_MAX
        };


        public Mat drawCountours(Mat himgbak,  Mat srcCopy , Rect roi, Rect roi1, Mat mask,int grayThresh) {

            //Mat submat = new Mat(new OpenCvSharp.Size(srcCopy.Width, srcCopy.Height), MatType.CV_8UC1);
            Mat subgray = new Mat(new OpenCvSharp.Size(srcCopy.Width, srcCopy.Height), MatType.CV_8SC1);
            Mat subgray1 = new Mat(new OpenCvSharp.Size(srcCopy.Width, srcCopy.Height), MatType.CV_8SC1);
            Mat temlateRoi = new Mat(new OpenCvSharp.Size(srcCopy.Width, srcCopy.Height), MatType.CV_8SC1);
            Mat himgbakRoi = new Mat(new OpenCvSharp.Size(srcCopy.Width, srcCopy.Height), MatType.CV_8SC1);
            //Mat grayMask = new Mat(new OpenCvSharp.Size(srcCopy.Width, srcCopy.Height), MatType.CV_8SC1);



            //Cv2.ImWrite(".\\aa_whitemask.bmp",whiteMask);
            //Cv2.ImWrite(".\\aa_blackMask.bmp", blackMask);
            //Cv2.ImWrite(".\\aa_mask.bmp", mask);

            //Cv2.CvtColor(mask, grayMask, ColorConversionCodes.BGR2GRAY);
            //Cv2.ImShow("whitemask", whiteMask);
            //Cv2.ImShow("subgray", mask);


            Cv2.CvtColor(Program.fmain.template[roi1], temlateRoi, ColorConversionCodes.BGR2GRAY);
            Cv2.CvtColor(himgbak[roi], himgbakRoi, ColorConversionCodes.BGR2GRAY);
            //himgbakRoi = Cv2.ImRead(".\\aa_himgbak.bmp");
            //temlateRoi = Cv2.ImRead(".\\aa_template.bmp");
            Cv2.Subtract(himgbakRoi, temlateRoi, subgray, mask);
            //Cv2.ImWrite(".\\aa_template.bmp", temlateRoi);
            //Cv2.ImWrite(".\\aa_himgbak.bmp", himgbakRoi);
            //Cv2.ImWrite(".\\aa_mask.bmp", mask);
            //Cv2.ImWrite(".\\aa_subgray.bmp", subgray);
            Cv2.Subtract(temlateRoi, himgbakRoi, subgray1, mask);
            //Cv2.ImWrite(".\\aa_subgray1.bmp", subgray1);

            Cv2.Add(subgray, subgray1, subgray, mask);
            //Cv2.ImWrite(".\\aa_subgray2.bmp", subgray);
            double etime1 = Environment.TickCount;
            //Console.WriteLine("Subtract耗时： {0}", etime1 - stime1);

            //drawHist(subgray);
            //double min, max;
            //Cv2.MinMaxLoc(subgray, out min, out max);
            //Cv2.CvtColor(submat, subgray, ColorConversionCodes.BGR2GRAY);
            //Cv2.EqualizeHist(subgray, subgray);

            Cv2.Threshold(subgray, subgray, grayThresh, 255, ThresholdTypes.Binary);
            //Cv2.ImWrite(".\\aaa_subgray4.bmp", subgray);


            //Cv2.WaitKey(10);
            //获得轮廓

            return subgray;
            
            //double start = Environment.TickCount;
        }

        public bool measuresuface(Mat himg, Mat himgbak, bool isset,bool isshowregion,double shift_r=0,double shift_c=0) {

            //没有mask 直接计算整个bbox的匹配度
            if (surfacecheck == false) return true;
            if (thminsurface ==0 && thmaxsurface == 0) return true;
            Rect roi = new Rect();
            Rect roi1 = new Rect();
            //未加偏移量
            roi1 = new Rect(new OpenCvSharp.Point(col - vcommon.viewx, row - vcommon.viewy), new OpenCvSharp.Size((col1 - col), (row1 - row)));// Convert.ToInt32(col), Convert.ToInt32(row), Convert.ToInt32(), Convert.ToInt32(row1 - row));// ;
            if (roi1.X <= 0 || (roi1.X + roi1.Width) > himg.Width || roi1.Width <= 0 || roi1.Height <= 0 || roi1.Y <= 0 || (roi1.Y + roi1.Height) > himg.Height)
            {
                //MessageBox.Show("ROI 超出边界");
                return false;
            }
            if (shift_c != 0 || shift_r != 0)
            {
                Console.WriteLine("*******************");

                Console.WriteLine(shift_r);
                Console.WriteLine(shift_c);
                Console.WriteLine("*******************");
            }
            //return false;
            //加偏移量后
            double shiftr_ = row - vcommon.viewy + shift_r;
            double shiftc_ = col - vcommon.viewx + shift_c;

            roi = new Rect(new OpenCvSharp.Point(shiftc_, shiftr_), new OpenCvSharp.Size((col1 - col), (row1 - row)));// Convert.ToInt32(col), Convert.ToInt32(row), Convert.ToInt32(), Convert.ToInt32(row1 - row));// ;
            if (roi.X <= 0 || (roi.X + roi.Width) > himg.Width || roi.Width <= 0 || roi.Height <= 0 || roi.Y <= 0 || (roi.Y + roi.Height) > himg.Height)
            {
                //MessageBox.Show("ROI 超出边界");
                return false;
            }
            //Cv2.ImWrite(".\\test\\aa_a.bmp",himgbak[roi]);

            if (srcCopy==null)
                srcCopy = new Mat();
            double stime1 = Environment.TickCount;
            himgbak[roi].CopyTo(srcCopy);

            mask = Mat.Zeros(srcCopy.Size(), MatType.CV_8UC1);

            //if (whiteMask == null && blackMask != null)
            //{
            //    if ((blackMask.Width == mask.Width) && (blackMask.Height == mask.Height))
            //    {
            //        blackMask.CopyTo(mask);
            //    }
            //}
            //else if (blackMask == null && whiteMask != null)
            //{
            //    if ((whiteMask.Width == mask.Width) && (whiteMask.Height == mask.Height))
            //    {
            //        whiteMask.CopyTo(mask);
            //    }
            //}
            //else if (blackMask != null && whiteMask != null)
            //{
            //    if ((whiteMask.Width == mask.Width) && (whiteMask.Height == mask.Height) && (blackMask.Width == mask.Width) && (blackMask.Height == mask.Height))
            //    {

            //        Cv2.Add(whiteMask, blackMask, mask);
            //    }
            //}
            //else if (blackMask == null && whiteMask == null)
            //{
            //    Console.WriteLine("BOTH BLACK MASK AND WHITEMASK ARE NULL");
            //    return true;
            //}
            int maxarea = 0;
            // not drawCountours 分割
            Mat subgray = drawCountours(himgbak, srcCopy, roi, roi1, whiteMask, grayThresh );
            Mat subgray1 = drawCountours(himgbak, srcCopy, roi, roi1,blackMask, grayThresh1 );

            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchly;
            Cv2.FindContours(subgray, out contours, out hierarchly, RetrievalModes.List, ContourApproximationModes.ApproxNone, new OpenCvSharp.Point(0, 0));

            OpenCvSharp.Point[][] contours1;
            HierarchyIndex[] hierarchly1;
            Cv2.FindContours(subgray1, out contours1, out hierarchly1, RetrievalModes.List, ContourApproximationModes.ApproxNone, new OpenCvSharp.Point(0, 0));


            int minareaIdx = 0;
            //int maxarea = int.MinValue;
            defectArea = 0;
            int maxareaIdx = int.MinValue;
            int maxareaIdx1 = int.MinValue;
            for (int i = 0; i < contours.Length; i++)
            {
                var contour = contours[i];
                int area1 = Convert.ToInt32(Cv2.ContourArea(contour));

                if (area1 < minDefectArea) continue;
                //Rect br = Cv2.BoundingRect(contours[i]);
                //RotatedRect minBbox = Cv2.MinAreaRect(contours[i]);
                //if ((minBbox.Size.Height < minDefectHeight) && (minBbox.Size.Width < minDefectWidth)) continue;

                if (area1 >= maxarea)
                {
                    maxarea = area1;
                    maxareaIdx = i;
                } 
            }
            bool wb = true;

            for (int i = 0; i < contours1.Length; i++)
            {
                var contour = contours1[i];
                int area1 = Convert.ToInt32(Cv2.ContourArea(contour));

                if (area1 < minDefectArea) continue;
                //Rect br = Cv2.BoundingRect(contours[i]);
                RotatedRect minBbox = Cv2.MinAreaRect(contours1[i]);
                //if ((minBbox.Size.Height < minDefectHeight) && (minBbox.Size.Width < minDefectWidth)) continue;

                if (area1 >= maxarea)
                {
                    maxarea = area1;
                    maxareaIdx1 = i;
                    wb = false;
                }
            }

            if (maxareaIdx >= 0)
            {
                Cv2.DrawContours(
                    srcCopy,
                    contours,
                    maxareaIdx,
                    color: new Scalar(0, 0, 255),
                    thickness: 3,
                    lineType: LineTypes.Link8,
                    hierarchy: hierarchly,
                    maxLevel: int.MaxValue);
                //defectArea = maxarea;
                if (wb) { 
                    Rect br = Cv2.BoundingRect(contours[maxareaIdx]);
                    int cex = br.X + (br.Width) / 2;
                    int cey = br.Y + (br.Height) / 2;
                    Cv2.Circle(srcCopy, cex, cey, 200, new Scalar(0, 0, 255), 3);
                }
            }
            if(maxareaIdx1 >= 0)
            {
                Cv2.DrawContours(
                    srcCopy,
                    contours1,
                    maxareaIdx1,
                    color: new Scalar(0, 0, 255),
                    thickness: 3,
                    lineType: LineTypes.Link8,
                    hierarchy: hierarchly1,
                    maxLevel: int.MaxValue);

                if (!wb) { 
                    Rect br = Cv2.BoundingRect(contours1[maxareaIdx1]);
                    int cex = br.X + (br.Width) / 2;
                    int cey = br.Y + (br.Height) / 2;
                    Cv2.Circle(srcCopy, cex, cey, 200, new Scalar(0, 0, 255), 3);
                }
            }
            defectArea = maxarea;
            srcCopy.CopyTo(himg[roi]);
            if (maxareaIdx < 0 && maxareaIdx1 < 0)
            {
                return false;
            }
            return true;
        }

        public void drawHist(Mat src) {
            Mat hist = new Mat();
            Cv2.CalcHist(new Mat[] { src }, new int[] { 0 }, new Mat(), hist, 1, new int[] { 256 }, new Rangef[] { new Rangef(0, 256) });
            Cv2.Normalize(hist, hist);

            Mat result = new Mat(new OpenCvSharp.Size(256, 500), MatType.CV_8UC1, new Scalar(255));
            for (int i = 0; i < 256; i++)
            {
                float v = hist.Get<float>(i);
                int len = (int)(v * 500);
                if (len != 0)
                {
                    Cv2.Line(result, i, 499, i, 500 - len, new Scalar(0));
                }
            }
            Cv2.ImWrite(".\\hist.bmp", result);
            //Cv2.ImShow("hist.bmp", result);
            //Cv2.WaitKey(10);
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
        //public HWindow hw;
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
        public double tr1 = 20, tc1=20, 
            tr2=80, tc2=80;//两个显示文本的位置

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
            tr1 = 70; tc1 = 100; tr2 = 80; tc2 = 80;
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
        public void deleteAll(PictureBox pb, Mat himg, Mat himgback, int iw, int ih)
        {
            foreach (roishape rs in roilist)
            {
                if (rs.num == broi)
                {
                    broi = -1;
                }

                //Cv2.ImWrite("C:\\Users\\24981\\Desktop\\ctvision源码\\result3.bmp", dcamera.himg);
                rs.repair(himg, himgback, iw, ih);
                //rs.showregion(himg, true);
                //
                //roilist.Remove(rs);
                //rs.updateOrigImagePart(dcamera.himg, iw, ih);
            }
            roilist.Clear();
            croi = null;
            if (roilist.Count == 0) nums = 0;
            pb.Invalidate();
        }
        public void delete(PictureBox pb, Mat himg,Mat himgback,int iw,int ih) {
            foreach (roishape rs in srois.rois) {
                if (rs.num == broi) {
                    broi = -1;
                }
                
                //Cv2.ImWrite("C:\\Users\\24981\\Desktop\\ctvision源码\\result3.bmp", dcamera.himg);
                rs.repair(himg, himgback, iw, ih);
                //rs.showregion(himg, true);
                
                roilist.Remove(rs);
                //rs.updateOrigImagePart(dcamera.himg, iw, ih);
            }
            srois.clear();
            croi = null;
            if (roilist.Count == 0) nums = 0;
            pb.Invalidate();
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
            double tmx = mx/vcommon.viewscale  ;
            double tmy = my/vcommon.viewscale  ;
            //Console.WriteLine("mouse down:{0} {1}", tmx, tmy);
            if (action == "")
            {
                //Program.fmain.zscale;
                //Console.WriteLine("{0} {1} {2} {3}",tc1,tr1,tc2,tr2);
                if (mx > (tc1 - 40) && mx < (tc1 + 40) && my > (tr1 - 40) && my < (tr1 + 40)) action = "ontext1";
                else if (mx > (tc2 - 40) && mx < (tc2 + 40) && my > (tr2 - 40) && my < (tr2 + 40)) action = "ontext2";
                if (action == "ontext1" || action == "ontext2") return;
            }
            //if (action == "onselect") return;
            //点击在选择roi的内部开始移动，点击在某一roi手柄开始resize
            foreach (roishape cr in srois.rois)
            {
                //Console.WriteLine("draw selected roi,mouse down:{0} {1} {2} {3}", cr.col, cr.col1, cr.row,  cr.row1);
                if (tmx > (cr.col - 50) && tmx < (cr.col1 + 50) && tmy > (cr.row - 50) && tmy < (cr.row1 + 50))
                {
                    //double aaa = cr.col1 - cr.col;
                    //Console.WriteLine(cr.col);
                    //Console.WriteLine(cr.col1);
                    
                    croi = cr;
                    action = "onmove";
                    acthandle = -1;
                    double hlen = 10 / vcommon.viewscale;
                    if (tmx > (croi.col - hlen) && tmx < (croi.col + hlen) && tmy > (croi.row - hlen) && tmy < (croi.row + hlen)) { action = "onresize"; acthandle = 0; }
                    else if (tmx > (croi.col1 - hlen) && tmx < (croi.col1 + hlen) && tmy > (croi.row - hlen) && tmy < (croi.row + hlen)) { action = "onresize"; acthandle = 1; }
                    else if (tmx > (croi.col1 - hlen) && tmx < (croi.col1 + hlen) && tmy > (croi.row1 - hlen) && tmy < (croi.row1 + hlen)) { action = "onresize"; acthandle = 2; }
                    else if (tmx > (croi.col - hlen) && tmx < (croi.col + hlen) && tmy > (croi.row1 - hlen) && tmy < (croi.row1 + hlen)) { action = "onresize"; acthandle = 3; }
                    break;
                }
            }

        }
        private RectangleList getRectangle()
        {
            return new RectangleList(
                Math.Min(Convert.ToInt32(mousex/vcommon.viewscale), Convert.ToInt32(mousex1/vcommon.viewscale)),
                Math.Min(Convert.ToInt32(mousey/vcommon.viewscale), Convert.ToInt32(mousey1/vcommon.viewscale)),
                Math.Abs(Convert.ToInt32(mousex/vcommon.viewscale) - Convert.ToInt32(mousex1/vcommon.viewscale)),
                Math.Abs(Convert.ToInt32(mousey/vcommon.viewscale) - Convert.ToInt32(mousey1/vcommon.viewscale)));
        }

        internal void zoomshape(int x, int y, double oldzoom, double zscale)
        {
            int oldimagex = (int)(x / oldzoom);  // Where in the IMAGE is it now
            int oldimagey = (int)(y / oldzoom);

            int newimagex = (int)(x / zscale);     // Where in the IMAGE will it be when the new zoom i made
            int newimagey = (int)(y / zscale);
            foreach (roishape rs in roilist)
            { 
                rs.col = newimagex - oldimagex + rs.col;  // Where to move image to keep focus on one point
                rs.col1 = newimagex - oldimagex + rs.col1;
                rs.row = newimagey - oldimagey + rs.row;
                rs.row1 = newimagey - oldimagey + rs.row1; 
            }
            //tc1 = newimagex - oldimagex + tc1;
            //tr1 = newimagex - oldimagex + tr1;
            //tc2 = newimagex - oldimagex + tc2;
            //tr2 = newimagex - oldimagex + tr2;
        }

        internal void zoomshapeall(int x, int y, double oldzoom, double zscale,int imgx,int imgy)
        {
            foreach (roishape rs in roilist)
            {
                int oldimagex = (int)(x / oldzoom);  // Where in the IMAGE is it now
                int oldimagey = (int)(y / oldzoom);

                int newimagex = (int)(x / zscale);     // Where in the IMAGE will it be when the new zoom i made
                int newimagey = (int)(y / zscale);

                rs.col = newimagex - oldimagex + rs.col - imgx;  // Where to move image to keep focus on one point
                rs.col1 = newimagex - oldimagex + rs.col1 - imgx;
                rs.row = newimagey - oldimagey + rs.row - imgy;
                rs.row1 = newimagey - oldimagey + rs.row1 - imgy;
            }

        }

        double odx = 0;
        double ody = 0;
        public void pbmove(double deltaX, double deltaY)
        {
            double mx = deltaX - odx;
            odx = deltaX;
            double my = deltaY - ody;
            ody = deltaY;
            //平移
            //"onmove";
            foreach (roishape rs in roilist)
            {
                //rs.repair(dcamera.himg, himgback, iw, ih);
                rs.row += my;
                rs.row1 += my;
                rs.col += mx;
                rs.col1 += mx;
                //rs.updateOrigImagePart(dcamera.himg, iw, ih);
            }
            
        }

        public void mousemove(Mat himg, Mat himgback, int iw,int ih, double mx, double my)
        {
            mousex1 = mx; mousey1 = my;

            if (action == "onselect"||action=="ondraw"){
                double x1,x2,y1,y2;
                x1 = Math.Min(mousex, mousex1);
                x2 = Math.Max(mousex, mousex1);
                y1 = Math.Min(mousey, mousey1);
                y2 = Math.Max(mousey, mousey1);

                //RectangleList rect = new RectangleList(Convert.ToInt32(x1), Convert.ToInt32(y1), Convert.ToInt32(x2 - x1), Convert.ToInt32(y2 - y1));
                //e.Graphics.DrawRectangle(vcommon.hcolor, rect.rectangle);
                ////e.Graphics.DrawRectangles(vcommon.hcolor, rect.subRectList.ToArray());
                //e.Graphics.DrawString(num.ToString(), new Font("Arial", 16), new SolidBrush(Color.Red), Convert.ToSingle(col), Convert.ToSingle(row));

                if (action == "onselect")
                {
                    //#hw.SetColor(vcommon.hcolorselect);
                    //#hw.SetLineStyle((new HTuple(3)).TupleConcat(1).TupleConcat(3).TupleConcat(1));
                }
                else {
                    //#hw.SetColor("red");
                    //#hw.SetLineStyle(new HTuple());
                }
                 
                //hw.DispRectangle1(y1, x1, y2, x2);
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
            
            double motionx = (mx - mousex)/vcommon.viewscale;
            double motiony = (my - mousey)/vcommon.viewscale;
            mousex = mx; mousey = my;
            if (action == "onresize")
            {
                croi.repair(dcamera.himg, himgback, iw, ih);
                if (acthandle == 0) {croi.row = my/vcommon.viewscale;croi.col = mx / vcommon.viewscale; }
                else if (acthandle == 1) {croi.row = my / vcommon.viewscale; croi.col1 = mx / vcommon.viewscale; }
                else if (acthandle == 2) {croi.row1 = my / vcommon.viewscale; croi.col1 = mx / vcommon.viewscale; }
                else if (acthandle == 3) {croi.row1 = my / vcommon.viewscale; croi.col = mx / vcommon.viewscale; }
                croi.getWhiteMask(himg, himgback, false, false);
                croi.getBlackMask(himg, himgback, true, false);
            }
            else
            {
                //平移
                //"onmove";
                foreach (roishape rs in srois.rois)
                {
                    rs.repair(dcamera.himg,himgback, iw,ih);
                    rs.row += motiony;
                    rs.row1 += motiony;
                    rs.col += motionx;
                    rs.col1 += motionx;
                    //rs.updateOrigImagePart(dcamera.himg, iw, ih);
                }
            }
        }

        public void mouseup(Mat himgback, double mx,double my,int iw,int ih) {
            mousex1 = mx; mousey1 = my;
            odx = ody = 0;
            if (action == "ondraw") {
                double x1, x2, y1, y2;
                x1 = Math.Min(mousex, mousex1);
                x2 = Math.Max(mousex, mousex1);
                y1 = Math.Min(mousey, mousey1);
                y2 = Math.Max(mousey, mousey1);
                if(Math.Abs(x2-x1)>20 || Math.Abs(y2-y1)>20 ){
                    croi = new roishape("rect",himgback, y1/vcommon.viewscale, x1/vcommon.viewscale, y2/vcommon.viewscale, x2/vcommon.viewscale);
                    //Console.WriteLine("mouse up:{0},{1},{2},{3}", x1,y1, x2, y2);
                    roilist.Add(croi);
                    nums++;
                    croi.num = nums;
                    srois.clear();
                    srois.add(croi);
                    Program.fmain.combtrack.Items.Add(nums.ToString().PadLeft(3,'0'));
                }
            }
            double tmx = mx/vcommon.viewscale;
            double tmy = my/vcommon.viewscale;
            if (action == "onselect") {
                srois.clear();
                croi=null;
                //未移动，点在那个roi中选中那个roi
                if (Math.Abs(mx - mousex) < 5 || Math.Abs(my - mousey) < 5) {
                    foreach (roishape rs in roilist) { 
                        if(tmx > (rs.col-6) && tmx < (rs.col1+6) && tmy > (rs.row-6) && tmy < (rs.row1+6)){
                            croi=rs; 
                            srois.add(croi);
                            break;
                        }
                    }
                    return;
                }
                //移动后，框选
                double x1 = Math.Min(mx, mousex)/vcommon.viewscale;
                double x2 = Math.Max(mx, mousex)/vcommon.viewscale;
                double y1 = Math.Min(my, mousey)/vcommon.viewscale;
                double y2 = Math.Max(my, mousey)/vcommon.viewscale;
                Rectangle srect=new Rectangle((int)x1,(int)y1,(int)(x2-x1),(int)(y2-y1));
                Rectangle rrect;
                foreach (roishape rs in roilist)
                {
                    rrect = new Rectangle((int)rs.col, (int)rs.row, (int)(rs.col1 - rs.col), (int)(rs.row1 - rs.row));
                    if(srect.IntersectsWith(rrect) && !rrect.Contains(srect)) srois.add(rs);//相交不包含
                }
                if(srois.count>0) croi=srois[0];
                //pb.Image = dcamera.himg.ToBitmap();
                //pb.Refresh();
            }
            if (action == "onresize") {
                croi.checkrowcol();
                //croi.getregion(dcamera.himg, iw, ih);
                if (croi.num==broi) { croi.getregion(dcamera.himg, himgback); croi.getbasepoint(out brow, out bcol); }
            }
            if (action == "onmove") {
                croi.checkrowcol();
                //croi.getregion(dcamera.himg, himgback);
                if (croi.num==broi) { croi.getregion(dcamera.himg, himgback); croi.getbasepoint(out brow, out bcol); }
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

        public void painttext(double zscale, PaintEventArgs e,bool isrun) {
            //foreach (roishape cr in roilist)
            //{
            //    cr.show(e);
            //}
            //srois.paintroi(e);
            showtext(zscale,e, isrun);
        }

        /// 绘制roi到窗口控件
        public void paintroi(double zscale,PictureBox pb, PaintEventArgs e)
        {
            
            if (action == "ondraw")
            {
                RectangleList rect = getRectangle();
                e.Graphics.DrawRectangle(vcommon.hcoloractive, rect.rectangle);
            }
            else if (action == "onselect")
            {
                Pen p = new Pen(Color.Green);
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                RectangleList rect = getRectangle();
                e.Graphics.DrawRectangle(p, rect.rectangle);
                //Console.WriteLine("painting roi not ondraw");
            }
            //hw.SetLineStyle(new HTuple());
            foreach (roishape cr in roilist)
            {
                cr.show(e);
            }
            srois.paintroi(e);
            showtext(zscale,e, false);
        }

        //public void paintroi() {
        //    hw.SetLineStyle(new HTuple());
        //    foreach (roishape cr in roilist)
        //    {
        //        cr.show(hwd);
        //    }
        //    srois.paintroi(hwd);
        //    showtext(hwd);
        //}

        public void showtext(double zscale,PaintEventArgs e,bool isrun) {
            //string text1 = "OK";
            SolidBrush drawBrush;
            if (text1 == "OK") drawBrush = new SolidBrush(Color.Blue);
            else drawBrush = new SolidBrush(Color.Red);
            //string fs = Math.Round(1.0 * vcommon.fontsize * vcommon.viewscale, 0).ToString(); 
            StringFormat drawFormat = new StringFormat();

            e.Graphics.DrawLine(new Pen(drawBrush), Convert.ToSingle(vcommon.viewx - 40), Convert.ToSingle(vcommon.viewy), Convert.ToSingle(vcommon.viewx+ 40), Convert.ToSingle(vcommon.viewy));
            e.Graphics.DrawLine(new Pen(drawBrush), Convert.ToSingle(vcommon.viewx), Convert.ToSingle(vcommon.viewy-40), Convert.ToSingle(vcommon.viewx), Convert.ToSingle(vcommon.viewy + 40));
            int tfontsize = vcommon.fontsize;
            int linel = 15;
            Font drawFont;
            drawFont = new Font("Arial", (int)(tfontsize));
            StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
            SizeF margin = e.Graphics.MeasureString("OK", drawFont, 1000, sf);
            int w = Program.fmain.pictureBox1.Width;
            int h = Program.fmain.pictureBox1.Height;
            //if (tr1 < 60 * linel) tr1 = 60 * linel;
            //if (tr1 > h - 60 * linel) tr1 = h - 60 * linel;
            //if (tc1 < 60 * linel) tc1 = w - 60 * linel;
            //if (tc1 > h - 60 * linel) tc1 = h - 60 * linel;
            //Console.WriteLine("{0},{1},{2},{3}",tr1.ToString(), tc1.ToString(), tr2.ToString(), tc2.ToString());
            //Console.WriteLine("{0},{1}",h.ToString(), w.ToString());
            //tc1 = 200;
            //tr1 = 200;
            //tc2 = 400;
            //tr2 = 400;
            int tr = (int)(tr2);
            int tc = (int)(tc2);
            if (isrun)
            {
                //if (!text1.Equals(""))
                //{
                //    //十字线
                //    e.Graphics.DrawLine(new Pen(drawBrush), Convert.ToSingle(tc1 - linel), Convert.ToSingle(tr1), Convert.ToSingle(tc1 + linel), Convert.ToSingle(tr1));
                //    e.Graphics.DrawLine(new Pen(drawBrush), Convert.ToSingle(tc1), Convert.ToSingle(tr1 - linel), Convert.ToSingle(tc1), Convert.ToSingle(tr1 + linel));
                //    // OK NG
                //    e.Graphics.DrawString(text1, drawFont, drawBrush, Convert.ToSingle(tc1), Convert.ToSingle(tr1), drawFormat);
                //}
                string[] ostr = text2.Split('\r');
                //int margin1 = 80 + Math.Abs(tfontsize - 80) / 4;


                e.Graphics.DrawLine(new Pen(drawBrush), (tc - linel), tr, tc + linel, tr);
                e.Graphics.DrawLine(new Pen(drawBrush), tc, tr - linel, tc, tr + linel);
                e.Graphics.DrawString(text2.Trim(), drawFont, drawBrush, Convert.ToSingle(tc), Convert.ToSingle(tr), drawFormat);
                //foreach (string str in ostr)
                //{
                     
                //    e.Graphics.DrawString(str.Trim(), drawFont, drawBrush, Convert.ToSingle(tc), Convert.ToSingle(tr), drawFormat);
                //    tr += Convert.ToInt32(margin.Height);
                //}
            }
            else {
                tfontsize = Convert.ToInt32( tfontsize / zscale);
                margin.Height = Convert.ToInt32(margin.Height / zscale);
                tr = Convert.ToInt32(tr / zscale);
                tc = Convert.ToInt32(tc / zscale);
                drawFont = new Font("Arial", (int)(tfontsize ));
                e.Graphics.DrawLine(new Pen(drawBrush), (tc - linel), tr, tc + linel, tr);
                e.Graphics.DrawLine(new Pen(drawBrush), tc, tr - linel, tc, tr + linel);
                tr += Convert.ToInt32(margin.Height);
                e.Graphics.DrawString("data...", drawFont, drawBrush, Convert.ToSingle(tc), Convert.ToSingle(tr), drawFormat);
            }
            return;
            //double tmptc1 = tc1;
            //double tmptr1 = tr1;
            //double tmptc2 = tc2;
            //double tmptr2 = tr2;
            //int linel = 40;
            //if (isshowtext) {
                
            //    int tfontsize = vcommon.fontsize;
            //    int margin = 5+ Math.Abs(tfontsize-80)/4;
            //    int margin1 = 20 + Math.Abs(tfontsize - 80)/4 ;
            //    if (!Program.fmain.tbrun.Checked) {
            //        tfontsize = tfontsize * 4;
            //        margin = 100 + Math.Abs(tfontsize - 80)/4;
            //        margin1 = 80 + Math.Abs(tfontsize - 80)/4;
            //    }
            //    else
            //    {
            //        tmptc1 = tmptc1 / 4;
            //        tmptr1 = tmptr1 / 4;
            //        tmptc2 = tmptc2 / 4;
            //        tmptr2 = tmptr2 / 4;
            //        linel = linel / 4;
            //    }
            //    //text1 = OK/NG
            //    Font drawFont;
            //    if (isrun) { 
            //        drawFont = new Font("Arial", (int)(1.3 * tfontsize * vcommon.viewscale));  
            //        //if (text1 != "") { 
            //        //    e.Graphics.DrawLine(new Pen(drawBrush), Convert.ToSingle( (tmptc1 - linel) * vcommon.viewscale), Convert.ToSingle(tmptr1 * vcommon.viewscale), Convert.ToSingle((tmptc1 + linel) * vcommon.viewscale), Convert.ToSingle(tmptr1 * vcommon.viewscale));
            //        //    e.Graphics.DrawLine(new Pen(drawBrush), Convert.ToSingle(tmptc1 * vcommon.viewscale), Convert.ToSingle((tmptr1 - linel) * vcommon.viewscale), Convert.ToSingle(tmptc1 * vcommon.viewscale), Convert.ToSingle((tmptr1 + linel) * vcommon.viewscale));
            //        //    e.Graphics.DrawString(text1, drawFont, drawBrush, Convert.ToSingle(tmptc1 * vcommon.viewscale), Convert.ToSingle(tmptr1 * vcommon.viewscale), drawFormat);
            //        //}
            //    }
            //    else
            //    {
            //        drawFont = new Font("Arial", (int)(1.3 * tfontsize)); 
            //        //if (text1 != "")
            //        //{
            //        //    e.Graphics.DrawLine(new Pen(drawBrush), Convert.ToSingle(tmptc1 - linel), Convert.ToSingle(tmptr1), Convert.ToSingle(tmptc1 + linel), Convert.ToSingle(tmptr1));
            //        //    e.Graphics.DrawLine(new Pen(drawBrush), Convert.ToSingle(tmptc1), Convert.ToSingle(tmptr1 - linel), Convert.ToSingle(tmptc1), Convert.ToSingle(tmptr1 + linel));
            //        //    e.Graphics.DrawString(text1, drawFont, drawBrush, Convert.ToSingle(tmptc1), Convert.ToSingle(tmptr1), drawFormat);
                         
            //        //}
            //    }
            //    if (!text1.Equals("")) {
            //        e.Graphics.DrawLine(new Pen(drawBrush), Convert.ToSingle(tmptc1 - linel), Convert.ToSingle(tmptr1), Convert.ToSingle(tmptc1 + linel), Convert.ToSingle(tmptr1));
            //        e.Graphics.DrawLine(new Pen(drawBrush), Convert.ToSingle(tmptc1), Convert.ToSingle(tmptr1 - linel), Convert.ToSingle(tmptc1), Convert.ToSingle(tmptr1 + linel));
            //        e.Graphics.DrawString(text1, drawFont, drawBrush, Convert.ToSingle(tmptc1), Convert.ToSingle(tmptr1), drawFormat);
            //    }
            //    string[] ostr = text2.Split('\r');
            //    if (isrun) { 
            //        int tr = (int)(tmptr2 * vcommon.viewscale);
            //        int tc = (int)(tmptc2 * vcommon.viewscale);

            //        e.Graphics.DrawLine(new Pen(drawBrush),Convert.ToSingle((tc- linel) * vcommon.viewscale),Convert.ToSingle( tr * vcommon.viewscale),Convert.ToSingle( (tc + linel) * vcommon.viewscale),Convert.ToSingle( tr * vcommon.viewscale));
            //        e.Graphics.DrawLine(new Pen(drawBrush),Convert.ToSingle( tc * vcommon.viewscale),Convert.ToSingle( (tr- linel) * vcommon.viewscale),Convert.ToSingle( tc * vcommon.viewscale),Convert.ToSingle( (tr+ linel) * vcommon.viewscale));

            //        foreach (string str in ostr)
            //        {
            //            drawFont = new Font("Arial", (int)(0.5 * tfontsize * vcommon.viewscale));
            //            e.Graphics.DrawString(str.Trim(), drawFont, drawBrush, Convert.ToSingle(tc * vcommon.viewscale), Convert.ToSingle(tr * vcommon.viewscale), drawFormat);
            //            tr += margin1;
            //        }
            //    }
            //    else
            //    {
            //        int tr = (int)(tmptr2 );
            //        int tc = (int)(tmptc2 );

            //        e.Graphics.DrawLine(new Pen(drawBrush),(tc - linel) , tr, tc + linel, tr);
            //        e.Graphics.DrawLine(new Pen(drawBrush), tc, tr - linel, tc, tr + linel);
            //        foreach (string str in ostr)
            //        {
            //            drawFont = new Font("Arial", (int)(0.5 * tfontsize));
            //            e.Graphics.DrawString(str.Trim(), drawFont, drawBrush, Convert.ToSingle(tc), Convert.ToSingle(tr), drawFormat);
            //            tr += margin1;
            //        }
            //    }
            //    //e.Graphics.DrawRectangle(Pens.Green, rect.rectangle);
            //    ////e.Graphics.DrawRectangles(vcommon.hcolor, rect.subRectList.ToArray());
            //    //e.Graphics.DrawString(num.ToString(), new Font("Arial", Convert.ToInt32(60)), new SolidBrush(Color.Green), Convert.ToSingle(col), Convert.ToSingle(row));
                 

            //}
        }

        //public void showtext(HWindow hwd)
        //{
        //    if (isshowtext)
        //    {
        //        if (text1 == "OK") hw.SetColor("green");
        //        else hw.SetColor("red");
        //        string fs = Math.Round(1.0 * vcommon.fontsize * vcommon.viewscale, 0).ToString();
        //        hwd.SetFont("-Arial-" + fs + "-");
        //        //hwd.DispCross(tr1, tc1, 20, 0);
        //        //hwd.SetTposition((int)(tr1 + 5), (int)(tc1 + 5));
        //        //hwd.WriteString(text1);

        //        fs = Math.Round(0.5 * vcommon.fontsize * vcommon.viewscale, 0).ToString();
        //        hwd.SetFont("-Arial-" + fs + "-");
        //        hwd.DispCross(tr2, tc2, 20, 0);
        //        int tr = (int)(tr2 + 5);
        //        int tc = (int)(tc2 + 5);
        //        hwd.SetTposition(tr, tc);
        //        string[] ostr = text2.Split('\r');
        //        foreach (string str in ostr)
        //        {
        //            hwd.WriteString(str);
        //            tr += (int)(0.5 * vcommon.fontsize+15);
        //            hwd.SetTposition(tr, tc);
        //        }
        //        hwd.SetFont("");
        //    }
        //}

        public void copy(Mat himgbak) {
            foreach (roishape rs in srois.rois) {
                roishape nrs = new roishape("rect", himgbak,rs.row, rs.col1+10, rs.row1, rs.col1+10+rs.col1-rs.col);
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
                nrs.grayThresh = rs.grayThresh;//
                nrs.grayThresh1 = rs.grayThresh1;
                nrs.minDefectArea = rs.minDefectArea;//
                nrs.minDefectHeight = rs.minDefectHeight;
                nrs.minDefectWidth = rs.minDefectWidth;
                nrs.shrinkPixel = rs.shrinkPixel;//
                nrs.spreadPixel = rs.spreadPixel;
                nrs.thminsurface = rs.thminsurface;//
                nrs.thmaxsurface = rs.thmaxsurface;
                nrs.surfacecheck = rs.surfacecheck;
                nrs.surfacemaxcheck = rs.surfacemaxcheck;
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
            //#bshape.getregion(hw, dcamera.himg);
            bshape.getbasepoint(out brow, out bcol);
        }

        // TODO 跟踪功能
        public void run(PictureBox pictureBox1) {
            //#hw.DispImage(dcamera.himg);
            dr=dc=0;
            //识别跟踪点
            if (broi > 0)
            {
                roishape bshape = this[broi.ToString()];
                if (bshape != null)
                {
                    //#bshape.getregion(hw, dcamera.himg);
                    bshape.showregion(dcamera.himg ,false);
                    double nbrow = 0, nbcol = 0;
                    bshape.getbasepoint(out nbrow, out nbcol);
                    dr = nbrow - brow; dc = nbcol - bcol;
                }
            }
            //计算识别点
            foreach (roishape rs in roilist)
            {
                if (rs.num==broi) continue;
               
                rs.getregion(dcamera.himg, Program.fmain.himgbak);
            }
        }
         
        public void showresult(int iw,int ih,roishape rs,bool isng=false) {
            if (rs == null) return;
            rs.show(dcamera.himg, rs,isng);
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
        public void paintroi(PaintEventArgs e)
        {
            foreach (roishape cr in slist)  cr.showselect(e);
        }

        public int count {
            get {return slist.Count;} 
        }

        public ArrayList rois {
            get { return slist; }
        }

        
        public void alignleft(PictureBox pb,Mat himg,Mat himgback, int iw,int ih)
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                 
                
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
            pb.Invalidate();
        }
        public void alignright(PictureBox pb, Mat himg, Mat himgback, int iw, int ih)
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                 
                
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
            pb.Invalidate();
        }
        public void alignmidv(PictureBox pb, Mat himg, Mat himgback, int iw, int ih)
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                
                
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
            pb.Invalidate();
        }
        public void aligntop(PictureBox pb, Mat himg, Mat himgback, int iw, int ih)
        {

            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                
                
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
            pb.Invalidate();
        }
        public void alignbot(PictureBox pb, Mat himg, Mat himgback, int iw, int ih)
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                 
                
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
            pb.Invalidate();
        }
        public void alignmidh(PictureBox pb, Mat himg, Mat himgback, int iw, int ih)
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                 
                
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
            pb.Invalidate();
        }
        public void alignwidth(PictureBox pb, Mat himg, Mat himgback, int iw, int ih)
        {
            double rr = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                 
                
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
            pb.Invalidate();
        }
        public void alignheight(PictureBox pb, Mat himg, Mat himgback, int iw, int ih)
        {
            double rr = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                 
                
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
            pb.Invalidate();
        }
        public void alignsamesize(PictureBox pb, Mat himg, Mat himgback, int iw, int ih)
        {
            double rr = 0, cc = 0;
            int i = 0;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                //rs.showregion(himg, true);
                
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
            pb.Invalidate();
        }
        public void alignsamegap(PictureBox pb, Mat himg, Mat himgback, int iw, int ih)
        {
            if (slist.Count < 2) return;
            int i=slist.Count,j=0;
            double rr = 0, cc = 0,kk=0;
            rr=((slist[i-1] as roishape).col-(slist[0] as roishape).col)/(i-1);
            cc=(slist[0] as roishape).col;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);

                
                if (j>0)
                {
                    kk=rs.col1-rs.col;
                    rs.col = cc+rr; rs.col1 = rs.col + kk;
                }
                j++;
                cc=rs.col;
                //rs.showregion(himg, true);
            }
            pb.Invalidate();
            
           
        }

        public void moveup(Mat himg,Mat himgback,int iw,int ih) {
            int dy=vcommon.posmove;
            foreach (roishape rs in slist) { 
                rs.repair(himg, himgback, iw, ih); 
                rs.row -= dy; rs.row1 -= dy;
            }
        }

        public void movedown(Mat himg, Mat himgback, int iw, int ih)
        {
            int dy = vcommon.posmove;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                rs.row += dy; rs.row1 += dy;
            }
        }

        public void moveleft(Mat himg, Mat himgback, int iw, int ih) {
            int dy = vcommon.posmove;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                rs.col -= dy; rs.col1 -= dy;
            }            
        }

        public void moveright(Mat himg, Mat himgback, int iw, int ih)
        {
            int dy = vcommon.posmove;
            foreach (roishape rs in slist)
            {
                rs.repair(himg, himgback, iw, ih);
                rs.col += dy; rs.col1 += dy;
            }
        }
    }//class

}//namespace
