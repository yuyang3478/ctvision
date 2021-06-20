using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using HalconDotNet;
using System.Threading;
using leanvision;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

using OpenCvSharp;    //添加相应的引用即可
using OpenCvSharp.Extensions;

using MVSDK;//使用SDK接口
//using Snapshot;
using CameraHandle = System.Int32;
using MvApi = MVSDK.MvApi;
using System.Timers;
namespace ctmeasure
{
    public partial class frmmain : Form
    {
        //log
        public clslog log;
        //主图像
        public Mat himgbak;
        //视图控制: 放大, 缩小, 平移
        private Rectangle vrect;
        public double zscale = 1.0;
        private int iw = 1, ih = 1;
        //private double movex,movey,movex1,movey1,motionx,motiony;
        System.Drawing.Point mouseDown;
        int startx = 0;                         // offset of image when mouse was pressed
        int starty = 0;
        int imgx = 0;                         // current offset of image
        int imgy = 0;

        //bool mousepressed = false;  // true as long as left mousebutton is pressed
        private bool mouseleftpress = false;
        private bool mousemidpress = false;
        private bool onselect = false;
        private bool ondrawing = false;
        public string ondrawingstr;
        //放大镜视图, magicsize-范围内的图放大3倍显示
        //private HWindow magicwindow;
        private int magicsize; //视图半径尺寸
        private Rectangle magicrect=new Rectangle();//视图rect, x-row,y-col,width-row1,height-col1
        private double hlrow, hlcol, hlrow1, hlcol1, vlrow, vlcol, vlrow1, vlcol1;

        //camera
        //dcamera 暂时只做建模板时load and show图片功能
        public clscamera dcamera;
        public frmcamera fcamera;
        //IO
        public clsio dio;
        public frmio fio;
        //roi
        private roimanager rois;
        
        //measure program
        private clsmeasurelist mrois;
        public string mver;
        private string testresult = "OK";

        //printer
        public frmprint fprint;
        public frmzebra fzebra;

        #region variable
        protected CameraHandle m_hCamera = 0;             // 句柄
        protected IntPtr m_ImageBuffer;             // 预览通道RGB图像缓存
        protected IntPtr m_ImageBufferSnapshot;     // 抓拍通道RGB图像缓存
        protected tSdkCameraCapbility tCameraCapability;  // 相机特性描述
        protected int m_iDisplayedFrames = 0;    //已经显示的总帧数
        protected CAMERA_SNAP_PROC m_CaptureCallback;
        protected IntPtr m_iCaptureCallbackCtx;     //图像回调函数的上下文参数
        protected Thread m_tCaptureThread;          //图像抓取线程
        protected Thread saveNgPicThread;
        protected bool m_bExitCaptureThread = false;//采用线程采集时，让线程退出的标志
        protected IntPtr m_iSettingPageMsgCallbackCtx; //相机配置界面消息回调函数的上下文参数   
        protected tSdkFrameHead m_tFrameHead;
        //protected SnapshotDlg m_DlgSnapshot = new SnapshotDlg();               //显示抓拍图像的窗口
        protected bool m_bEraseBk = false;
        protected bool m_bSaveImage = false;
        #endregion
        private static System.Timers.Timer aTimer;
        private  string fullPath = "";
        private FileInfo fi = null;

        private  FileStream fs_csv = null;
        private  StreamWriter sw_csv = null;
        private string data = "";
        private bool isCsvTitleUpdated = true;
        private bool isTriggered = false;
        private bool isSaveToTemplate = false;
        public bool isRunOrRunOnceChecked = false;
        public bool isLoginSuccess = false;
        public static string templateFile = "";
        public Mat template = new Mat();
        
        

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        public frmmain()
        {
            InitializeComponent();
        }

        private void frmmain_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + " V" + Application.ProductVersion;
            //HSystem.SetSystem("help_dir", "");
            //HSystem.SetSystem("do_low_error", "false");
            //hwin.HalconWindow.SetColor("red");
            //hwin.HalconWindow.SetLineWidth(1);
            //hwin.HalconWindow.SetDraw("margin");

            phwin.Width = pview.Width - tabControl1.Width - 12;
            //pictureBox1.Height = phwin.Height - 6;
            pictureBox1.Width = phwin.Width - 6;

            mver = "lip";

            //log
            log = new clslog();
            log.write("打开软件");

            //camera
            dcamera = new clscamera();
            //IO
            dio = new clsio();

            //待重写
            //roi
            rois = new roimanager();
            //rois.hw = hwin.HalconWindow;
            rois.dcamera = dcamera;

            //measures
            mrois = new clsmeasurelist();
            initdgview();

            //print
            fprint = new frmprint();
            fzebra = new frmzebra();

            vcommon.loaddata();

            vcommon.showstatistic();
            rois.tr1 = vcommon.postext1r;
            rois.tc1 = vcommon.postext1c;
            rois.tr2 = vcommon.postext2r;
            rois.tc2 = vcommon.postext2c;

            //初始化图形视图
            clearroidata();
            initmeasure();
            lbng.Visible = false;


            if (InitCamera() == true) {
                MvApi.CameraPlay(m_hCamera);
                MvApi.CameraSetTriggerMode(m_hCamera, (int)emSdkSnapMode.SOFT_TRIGGER);
            }

            initview();
            //initcsv();
            
            //verifylog();

            //调试代码
            //实例化Timer类，设置间隔时间为10000毫秒； 
            aTimer = new System.Timers.Timer(1000);

            //注册计时器的事件
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            //设置时间间隔为2秒（2000毫秒），覆盖构造函数设置的间隔
            aTimer.Interval = 500;

            //设置是执行一次（false）还是一直执行(true)，默认为true
            aTimer.AutoReset = false;

            //开始计时
            aTimer.Enabled = false;

            Console.WriteLine("按任意键退出程序。");
            Console.ReadLine();
        }

        public void verifylog() {
            if (!isLoginSuccess) {
                foreach (ToolStripItem tb in mtools.Items)
                {
                    if (tb.Name != "tbrun"&& tb.Name != "tbrunstrop"&& tb.Name != "tbcheckimage" && tb.Name != "tblogmenu") tb.Enabled = false;
                }
            }
        }

        private  void initcsv()
        {
            if (vcommon.filepath == "")
            {
                fullPath = Application.StartupPath + "\\csv\\" + DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + ".csv";
                
            }
            else
            {
                fullPath = vcommon.filepath + "\\" + DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + ".csv";
            }
            fi = new System.IO.FileInfo(fullPath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            fs_csv = new System.IO.FileStream(fullPath, System.IO.FileMode.Create,
            System.IO.FileAccess.Write);
            sw_csv = new System.IO.StreamWriter(fs_csv, System.Text.Encoding.UTF8);
        }

        //指定Timer触发的事件
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            //int len = trigger.Trim().Length;
            //string rstr = trigger.ToUpper();
            //if (rstr == "0A") return;
            
            if (true)
            {
                //if (fio != null) fio.triggeron();
                if (30 > 0) Thread.Sleep(30);
                //触发执行
                run();
                //if (fio != null) fio.triggeroff();
            }
            
            Console.WriteLine("触发的事件发生在： {0}", e.SignalTime);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {

            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
            {
                return false;
            }
            else
            {
                return base.ProcessDialogKey(keyData);
            }
        }
        public class MPbPanel : System.Windows.Forms.Panel
        {
            protected override System.Drawing.Point ScrollToControl(System.Windows.Forms.Control activeControl)
            {
                return this.DisplayRectangle.Location;
            }
        }
        //======================================================halwin 视图控制
        //功能： 放大，缩小，100%，全屏，初始化
        private void pictureBoxPaint(object sender, PaintEventArgs e) {
            //HSystem.SetSystem("flush_graphic", "false");
            //hwin.HalconWindow.ClearWindow();
            //hwin.HalconWindow.SetLineStyle(new HTuple());

            //显示图像
            try
            {
                if (dcamera != null)
                //pictureBox1.Image = dcamera.himg.ToBitmap();
                {

                    if (!tbcameraplay.Checked)
                    {
                        if (tbrun.Checked|| isRunOrRunOnceChecked)
                        {
                            //double stime1 = Environment.TickCount;
                            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                            //e.Graphics.ScaleTransform(Convert.ToSingle(zscale), Convert.ToSingle(zscale));
                            Mat temp = new Mat();
                            Cv2.Resize(dcamera.himg, temp, new OpenCvSharp.Size(dcamera.himg.Width * zscale, dcamera.himg.Height * zscale));
                            e.Graphics.DrawImage(temp.ToBitmap(), imgx, imgy);//140ms

                            rois.painttext(zscale, e,true);
                            //double etime1 = Environment.TickCount;
                            //Console.WriteLine("e.Graphics.DrawImage cost time ：{0}", etime1 - stime1);
                        }
                        else {
                            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                            //Mat temp = new Mat();
                            //Cv2.Resize(dcamera.himg, temp, new OpenCvSharp.Size(dcamera.himg.Width * zscale, dcamera.himg.Height * zscale));
                            //e.Graphics.DrawImage(temp.ToBitmap(), imgx, imgy);//140ms
                            if (dcamera.himg == null || dcamera.himg.Width == 0 || dcamera.himg.Height == 0) {
                                return;
                            }
                            //Mat temp = new Mat();
                            //Cv2.Resize(dcamera.himg, temp, new OpenCvSharp.Size(dcamera.himg.Width * zscale, dcamera.himg.Height * zscale));
                            //e.Graphics.DrawImage(temp.ToBitmap(), imgx, imgy);//140ms

                            e.Graphics.ScaleTransform(Convert.ToSingle(zscale), Convert.ToSingle(zscale));
                            e.Graphics.DrawImage(dcamera.himg.ToBitmap(), imgx, imgy);//140ms

                            if (tplay.Enabled == false)
                            {
                                try
                                {
                                    if (rois != null)
                                        rois.paintroi(zscale, pictureBox1, e);
                                }
                                catch { }
                            }

                        }
                        //    double stime = Environment.TickCount;
                        //    pictureBox1.Image = dcamera.himg.ToBitmap();
                        //    double etime = Environment.TickCount;
                        //    Console.WriteLine("pictureBox1.Image = dcamera.himg.ToBitmap() cost time ：{0}", etime - stime);
                        //}
                    }
                    
                }
            }
            catch { }
            //Console.Write("文本");
            //更新roi





            //if (tbrun.Checked)
            //{
            //    rois.showtext(e);
            //}
            //if (rois != null)
            //{
            //    rois.showtext(e);
            //}


            //更新视图
            //#HSystem.SetSystem("flush_graphic", "true");
            //#hwin.HalconWindow.DispCross(0.0, 0.0, 100.0, 0.0);
        }

        private void hwin_Paint(object sender, PaintEventArgs e)
        {
            //pictureBox1.Invalidate();
        }
        private void hwin_Resize(object sender, EventArgs e)
        {
            //保持比例不变
            //vrect.Width = (int)(hwin.Width / zscale);
            //vrect.Height = (int)(hwin.Height / zscale);
            //if (vrect.Width == 0 || vrect.Height == 0) return;
            //hwin.ImagePart = vrect;
            //pictureBox1.Invalidate();
        }

        private void initview(){//100%显示

            tabControl1.SelectedIndex = 2;
            if (dcamera.iw == 0) dcamera.iw = pictureBox1.Width;
            if (dcamera.ih == 0) dcamera.ih = pictureBox1.Height;
            iw = dcamera.iw; ih = dcamera.ih;

            //未选择任何区域情况下，默认“表面检测区域”不可用
            foreach (Control control in groupBox6.Controls)
            {
                control.Enabled = false;
            }
            

            //zscale = vcommon.viewscale;
            //imgx = Convert.ToInt32(vcommon.viewx);
            //imgy = Convert.ToInt32(vcommon.viewy);


            //zoomimage(1.1);
            //zoomimage(0.9);

            //pictureBox1.Image = dcamera.himg.ToBitmap();

            //Graphics g = this.CreateGraphics();

            //// Fit whole image
            //zscale = Math.Min(
            //    ((float)pictureBox1.Height / (float)iw)  ,
            //    ((float)pictureBox1.Width / (float)ih) 
            //);
            //vcommon.viewscale = zscale;
            //pictureBox1.Location = new System.Drawing.Point(
            //    0, 
            //    (pictureBox1.Parent.ClientSize.Height / 2) - (Convert.ToInt32((float)dcamera.himg.Height*zscale / 2))
            //    );
            //zscale = vcommon.viewscale;
            //放大镜相关代码
            //vrect = hwin.Bounds;
            hlrow = (vrect.Top + vrect.Bottom) / 2; hlcol = vrect.Left;
            hlrow1 = hlrow; hlcol1 = vrect.Right;
            vlrow = vrect.Top; vlcol = (vrect.Left + vrect.Right) / 2;
            vlrow1 = vrect.Bottom; vlcol1 = vlcol;

            //zoomimageto(zscale);
            //zoommove(-vcommon.viewx, -vcommon.viewy);
            //showroidata();

        }

        private void zoomall(){
            if (dcamera.himg == null || dcamera.himg.Width == 0) return;
            vrect.X = 0;
            vrect.Y = 0;
            double oldzoom = zscale;
            // Fit whole image
            zscale =
                ((float)phwin.Width / (float)dcamera.himg.Width);

            //zscale = (float)phwin.Width / (float)iw;
            
            vcommon.viewscale = zscale;
            

             
            //MouseEventArgs mouse = e as MouseEventArgs;
            //System.Drawing.Point mousePosNow = mouse.Location;

            int x = Convert.ToInt32(pictureBox1.Width* oldzoom / 2) - pictureBox1.Location.X;    // Where location of the mouse in the pictureframe
            int y = Convert.ToInt32(pictureBox1.Height * oldzoom / 2) - pictureBox1.Location.Y;

            int oldimagex = (int)(x / oldzoom);  // Where in the IMAGE is it now
            int oldimagey = (int)(y / oldzoom);

            int newimagex = (int)(x / zscale);     // Where in the IMAGE will it be when the new zoom i made
            int newimagey = (int)(y / zscale);

            imgx = newimagex - oldimagex + imgx;  // Where to move image to keep focus on one point
            imgy = newimagey - oldimagey + imgy;
            

            rois.zoomshapeall(x, y, oldzoom, zscale,imgx,imgy) ;
            vcommon.viewx = imgx = 0;
            vcommon.viewy = imgy = 0;
            if (this.pictureBox1.InvokeRequired)
            {
                pictureBox1.Invoke(new MethodInvoker(
                   delegate ()
                   {
                       pictureBox1.Refresh();
                   }));
            }
            else
            {
                pictureBox1.Refresh();
            }
            //pictureBox1.Refresh();  // calls imageBox_Paint

            slabelzoom.Text = "zoom: " + zscale.ToString("f2");
        }

        private void zoomin()
        { 
            zoomimage(0.9); 
        }
        private void zoomout() { 
            zoomimage(1.1);
        }
        //从当前比例再缩放scale比例
        private void zoomimage(double scale) {
            double oldzoom = zscale;
            zscale /= scale;
            vcommon.viewscale = zscale;
            if (zscale <= 0.1) { zscale = 0.1; return; }
            if (zscale >= 10) { zscale = 10; return; }
            //if (e.Delta > 0)
            //{
            //    zoom += 0.1F;
            //}

            //else if (e.Delta < 0)
            //{
            //    zoom = Math.Max(zoom - 0.1F, 0.01F);
            //}

            //MouseEventArgs mouse = e as MouseEventArgs;
            System.Drawing.Point mousePosNow = new System.Drawing.Point(pictureBox1.Width/2,pictureBox1.Height/2);

            int x = mousePosNow.X - pictureBox1.Location.X;    // Where location of the mouse in the pictureframe
            int y = mousePosNow.Y - pictureBox1.Location.Y;

            int oldimagex = (int)(x / oldzoom);  // Where in the IMAGE is it now
            int oldimagey = (int)(y / oldzoom);

            int newimagex = (int)(x / zscale);     // Where in the IMAGE will it be when the new zoom i made
            int newimagey = (int)(y / zscale);

            imgx = newimagex - oldimagex + imgx;  // Where to move image to keep focus on one point
            imgy = newimagey - oldimagey + imgy;
            vcommon.viewx = imgx;
            vcommon.viewy = imgy;
            rois.zoomshape(x, y, oldzoom, zscale);
            pictureBox1.Refresh();  // calls imageBox_Paint
        }

        private void zoomImageWithMouse(object sender, MouseEventArgs e,double scale)
        {
            //Console.WriteLine("pb width height:{0},{1}", pictureBox1.Width, pictureBox1.Height);

            double oldzoom = zscale;
            zscale /= scale;
            vcommon.viewscale = zscale;
            if (zscale <= 0.1) { zscale = 0.1; return; }
            if (zscale >= 10) { zscale = 10; return; }
            //if (e.Delta > 0)
            //{
            //    zoom += 0.1F;
            //}

            //else if (e.Delta < 0)
            //{
            //    zoom = Math.Max(zoom - 0.1F, 0.01F);
            //}

            MouseEventArgs mouse = e as MouseEventArgs;
            System.Drawing.Point mousePosNow = mouse.Location;

            int x = mousePosNow.X - pictureBox1.Location.X;    // Where location of the mouse in the pictureframe
            int y = mousePosNow.Y - pictureBox1.Location.Y;

            int oldimagex = (int)(x / oldzoom);  // Where in the IMAGE is it now
            int oldimagey = (int)(y / oldzoom);

            int newimagex = (int)(x / zscale);     // Where in the IMAGE will it be when the new zoom i made
            int newimagey = (int)(y / zscale);
            
            imgx = newimagex - oldimagex + imgx;  // Where to move image to keep focus on one point
            imgy = newimagey - oldimagey + imgy;
            vcommon.viewx = imgx;
            vcommon.viewy = imgy;
            rois.zoomshape(x,y,oldzoom,zscale);


            pictureBox1.Refresh();  // calls imageBox_Paint

        }

        //缩放至scale比例
        private void zoomimageto(double scale)
        {
            vrect.Width = (int)(pictureBox1.Width / scale);
            vrect.Height = (int)(pictureBox1.Height / scale);
            //hwin.ImagePart = vrect;
            zscale = scale;
            vcommon.viewscale = zscale;
            hlcol = vrect.Left; hlcol1 = vrect.Right;
            vlrow = vrect.Top; vlrow1 = vrect.Bottom;
            pictureBox1.Invalidate();
            slabelzoom.Text = "zoom: " + zscale.ToString("f2");
        }

        private void zoommove(double dx,double dy){
            //vrect = hwin.ImagePart;
            vrect.X -= (int)Math.Round(dx);
            vrect.Y -= (int)Math.Round(dy);
            //hwin.ImagePart = vrect;
            vcommon.viewx = vrect.X;
            vcommon.viewy = vrect.Y;
            hlcol = vrect.Left; hlcol1 = vrect.Right;
            vlrow = vrect.Top; vlrow1 = vrect.Bottom;
            pictureBox1.Invalidate();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            zoomall();
            showroidata();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            zoomin(); 
            if(tplay.Enabled==false) showroidata();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            zoomout();
            if(tplay.Enabled==false) showroidata();
        }

        //=======================================halwin 视图控制

        //=======================================halwin 交互操作begin
        //新建元素： 点击新建， 点击窗口并拖动， 放开鼠标完成
        //选择元素： 点击元素框内， 点击空白并拖动
        //删除元素： 选择roi， 按delete键
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)  = true;
            MouseEventArgs mouse = e as MouseEventArgs;
            if (mouse.Button == MouseButtons.Left)
            {
                if (!mouseleftpress)
                {
                    mouseleftpress = true;
                    mouseDown = mouse.Location;
                    startx = imgx;
                    starty = imgy;
                }
            }
            if (e.Button == MouseButtons.Middle) { 
                tb_move.Checked = true;
                Cursor = Cursors.Hand;
                mousemidpress = true;
                mouseDown = mouse.Location;
                startx = imgx;
                starty = imgy;
                //return; 
            }
            //检查是否选中roi
            //#pictureBox1.Invalidate();
            rois.mousedown(e.X, e.Y);
            if (rois.srois.count > 0)
            {
                foreach (Control control in groupBox6.Controls)
                {
                    control.Enabled = true;
                }
            }
            else
            {
                foreach (Control control in groupBox6.Controls)
                {
                    control.Enabled = false;
                }
            }
            if (tbrun.Checked) return;
            if (!mouseleftpress) return;
            
            if (ondrawing) { rois.action = "ondraw"; return; }
            if (rois.action!="") return;

            //新建或选中roi对象中
            if (tb_move.Checked) return; 
            if (tb_magic.Checked) { showmagicwindow(e.X, e.Y); return; }
            onselect = true;
            rois.action = "onselect";
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!pictureBox1.Focused) pictureBox1.Focus();
            //movex1=e.X;movey1=e.Y;
            slabelxy.Text = string.Format("XY: {0},{1}  scale:{2:0.00}", e.X.ToString("f0"), e.Y.ToString("f0"),zscale);

            if (!mouseleftpress&&!mousemidpress) return;
            //视图平移
            if (tb_move.Checked){
                //Cursor = Cursors.Hand;
                MouseEventArgs mouse = e as MouseEventArgs;

                if (mouse.Button == MouseButtons.Left|| mouse.Button == MouseButtons.Middle)
                {
                    System.Drawing.Point mousePosNow = mouse.Location;

                    int deltaX = mousePosNow.X - mouseDown.X; // the distance the mouse has been moved since mouse was pressed
                    int deltaY = mousePosNow.Y - mouseDown.Y;

                    imgx = (int)(startx + (deltaX / vcommon.viewscale));  // calculate new offset of image based on the current zoom factor
                    imgy = (int)(starty + (deltaY / vcommon.viewscale));
                    vcommon.viewx = imgx;
                    vcommon.viewy = imgy;
                    rois.pbmove(deltaX/vcommon.viewscale,deltaY/vcommon.viewscale);
                    pictureBox1.Refresh();
                }
            }

            if (tbrun.Checked) return;

            //roi平移
            if ((mouseleftpress&& rois.action != ""))
            {
                rois.mousemove(himgbak, iw,ih,e.X, e.Y);
                pictureBox1.Invalidate();
                return;
            }
            //if (mousemidpress ) {
            //    rois.mousemove1(delt);
            //    pictureBox1.Invalidate();
            //    return;
            //}
                //放大镜
                if (tb_magic.Checked)
            {
                movemagicwindow(e.X, e.Y);
                //子窗口mousemove事件在父窗口响应
                //Console.WriteLine(string.Format("{0},{1}", e.X, e.Y));
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                tb_move.Checked = false;
                Cursor = Cursors.Default;
            }
            if (tbrun.Checked) return;
            mouseleftpress = false;
            mousemidpress = false;
            if (e.Button == MouseButtons.Right) {
                rois.action = "";
                ondrawing = false;
                tbdrawrect.Checked = false;
                rois.srois.clear();
                rois.croi = null;
                template.CopyTo(dcamera.himg);
                template.CopyTo(himgbak);
                pictureBox1.Invalidate();
                showroidata();
                tb_magic.Checked = false;
                tb_move.Checked = false;
                Cursor = Cursors.Default;
                if (rois.srois.count == 0)
                {
                    foreach (Control control in groupBox6.Controls)
                    {
                        control.Enabled = false;
                    }
                }
                else
                {
                    foreach (Control control in groupBox6.Controls)
                    {
                        control.Enabled = true;
                    }
                }
                return;
            }

            
            //放大镜状态下关闭窗口
            if (tb_magic.Checked) { deletemagicwindow(); return; }

            if (ondrawing)
            {
                if (tabControl1.SelectedIndex >0 ) tabControl1.SelectedIndex = 0;
                if (tbdrawrect.Checked)
                {
                    btnroi.Focus();
                    btnroi.Capture = true;
                    btnroi_Click(null, null);
                }
                else
                {
                    rois.action = "";
                    ondrawing = false;
                }
            }
            if (onselect) onselect = false;
            rois.mouseup( himgbak, e.X, e.Y,iw,ih);

            if (rois.srois.count == 0)
            {
                foreach (Control control in groupBox6.Controls)
                {
                    control.Enabled = false;
                }
            }
            else {
                foreach (Control control in groupBox6.Controls)
                {
                    control.Enabled = true;
                }
            }
            pictureBox1.Refresh();
            showroidata();
        }

        private void hwin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                foreach (roishape rs in rois.srois.rois) 
                    mrois.delete(rs);
                rois.delete(pictureBox1, dcamera.himg, himgbak, iw,ih);
                if (rois.broi == -1)
                    cbbase.SelectedIndex = -1;
                dgupdatemeasureall();
                pictureBox1.Invalidate();
                showroidata();
            }
        }
        private void hwin_KeyDown(object sender, KeyEventArgs e)
        {
            if (tplay.Enabled) {
                //移动水平垂直线
                if (e.KeyData == Keys.Up)
                {
                    if(hlrow-5>vrect.Top) hlrow-=5; 
                    hlrow1 = hlrow;
                }
                if (e.KeyData == Keys.Down)
                {
                    if (hlrow + 5 < vrect.Bottom) hlrow += 5; 
                    hlrow1 = hlrow;
                }
                if (e.KeyData == Keys.Left)
                {
                    if(vlcol-5>vrect.Left) vlcol-=5; 
                    vlcol1 = vlcol;
                }
                if (e.KeyData == Keys.Right)
                {
                    if(vlcol+5<vrect.Right) vlcol +=5; 
                    vlcol1 = vlcol;
                }
                return;
            }
            if (e.KeyData == Keys.Up)
            {
                
                rois.srois.moveup(dcamera.himg,himgbak,iw,ih);
                pictureBox1.Invalidate();
                showroidata();
            }
            if (e.KeyData == Keys.Down)
            {
                rois.srois.movedown(dcamera.himg, himgbak, iw, ih);
                pictureBox1.Invalidate();
                showroidata();
            }
            if (e.KeyData == Keys.Left)
            {
                rois.srois.moveleft(dcamera.himg, himgbak, iw, ih);
                pictureBox1.Invalidate();
                showroidata();
            }
            if (e.KeyData == Keys.Right)
            {
                rois.srois.moveright(dcamera.himg, himgbak, iw, ih);
                pictureBox1.Invalidate();
                showroidata();
            }
        }
        //========================================================halwin 视图交互end

        //========================================================放大镜视图begin
        //点击放大镜， 点击窗口移动
        private void showmagicwindow(double cx, double cy){
            //cx,cy 鼠标点坐标
            //if (magicwindow != null) magicwindow.Dispose();
            ////HOperatorSet.SetSystem("border_width", 10);
            ////magicwindow = new HWindow();
            //magicsize = 70;
            //magicwindow.OpenWindow(0,0,magicsize*2,magicsize*2,hwin.HalconID, "visible","");
            //magicwindowpaint(cx, cy); 
        }
        private void movemagicwindow(double cx, double cy) {
            //if (magicwindow == null) return;
            magicwindowpaint(cx, cy);
        }
        private void deletemagicwindow() {
            //if (magicwindow == null) return;
            //magicwindow.Dispose();
        }
        private void magicwindowpaint(double cx, double cy) {
            //重新确定窗口位置, mousemove时位置变化
            //magicwindow.SetWindowExtents((int)((cy - vrect.Y) * zscale - magicsize), (int)((cx - vrect.X) * zscale - magicsize), magicsize * 2, magicsize * 2);
            //按缩放比例设置窗口区域
            magicrect.X = (int)(cx - magicsize / 2.0);
            magicrect.Y = (int)(cy - magicsize / 2.0);
            magicrect.Width = (int)(cx + magicsize / 2.0);
            magicrect.Height = (int)(cy + magicsize / 2.0);
            //magicwindow.SetPart(magicrect.Y, magicrect.X, magicrect.Height, magicrect.Width);
            //绘制图像
            //HSystem.SetSystem("flush_graphic", "false");
            //magicwindow.ClearWindow();
            //显示图像
            //#magicwindow.DispImage(dcamera.himg);
            //显示roi
            //if (rois != null) 
                //#rois.paintroi(magicwindow);
            //更新视图
            //HSystem.SetSystem("flush_graphic", "true");
            //magicwindow.DispCross(0.0, 0.0, 100.0, 0.0);
        }
        private void tb_magic_Click(object sender, EventArgs e)
        {
            tb_move.Checked = false;
            Cursor = Cursors.Default;
            tb_magic.Checked = !tb_magic.Checked;
        }
        //==============================================================放大镜视图end
        //图片操作
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            dlgopen.FileName = "";
            dlgopen.Filter = "photo files|*.jpg;*.bmp";
            dlgopen.InitialDirectory = "";
            if (dlgopen.ShowDialog() == DialogResult.OK) {
                //pictureBox1.Image = Image.FromFile(dlgopen.FileName);
                dcamera.loadphoto(dlgopen.FileName);
                himgbak = dcamera.getBackImg();
                himgbak.CopyTo(template);
                initview();
                btnnewproduct_Click(null, null);
                
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            dlgsave.FileName = "";
            dlgsave.Filter = "photo files|*.bmp";
            dlgsave.InitialDirectory = "";
            dlgsave.DefaultExt = ".bmp";
            if (dlgsave.ShowDialog() == DialogResult.OK) {
                //pictureBox1.Image.Save(dlgsave.FileName);
                dcamera.savephoto(dlgsave.FileName);
            }
        }

        //------------------


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void mtools_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (!sender.Equals(tbdrawrect))
            {
                tbdrawrect.Checked = false;
                rois.action = "";
                ondrawing = false;
            }
            if (!sender.Equals(tb_magic)) tb_magic.Checked = false;
            if (!sender.Equals(tb_move))
            {
                tb_move.Checked = false;
                Cursor = Cursors.Default;
            }
        }

        /// roi 图元操作
        /// 
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            //toolstripbutton 单击时 hwin 不能获取 mouse焦点, 通过 可见的button:btnroi转换单击, 必须加capture=true
            btnroi.Focus();
            btnroi.Capture = true;
            btnroi_Click(null, null);
            tbdrawrect.Checked = !tbdrawrect.Checked;
        }


        //文件操作

        //----------------------------

        private void toolStripButton8_Click_1(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
           
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            
        }
        
        //this 
        private void button2_Click(object sender, EventArgs e)
        {

        }

        

        private void button2_Click_1(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        /// 数据
        private void thmin_ValueChanged(object sender, EventArgs e)
        {
            if (!thmin.Focused) return;
            if (thmax.Value < thmin.Value) thmax.Value = thmin.Value;
            tthmin.Text = thmin.Value.ToString();
            drawregion();
        }

        private void thmax_ValueChanged(object sender, EventArgs e)
        {
            if (!thmax.Focused) return;
            if (thmin.Value > thmax.Value) thmin.Value = thmax.Value;
            tthmax.Text = thmax.Value.ToString();
            drawregion();
         }
        //显示区域
        private void showregion() {
            //if (rois.croi == null) return;
            //if (rois.srois.count == 0) return;
            if (!isRunOrRunOnceChecked) { 
                foreach (roishape rs in rois.rois) {
                    rs.repair(dcamera.himg, himgbak, iw, ih);
                }
            }
            foreach (roishape croi in rois.srois.rois)
            {
                croi.getregion(dcamera.himg, himgbak);
                croi.showregion(dcamera.himg,true);
            }
            pictureBox1.Refresh();
        }


        //重新计算并显示
        private void drawregion(){
            if (rois.croi == null) return;
            if (rois.srois.count == 0) return;
            
            
            foreach (roishape croi in rois.srois.rois)
            {
                //赋值
                croi.roipoint = cbpoint.SelectedIndex;
                croi.thway = cbthway.SelectedIndex;
                croi.thmin = thmin.Value;
                croi.thmax = thmax.Value;
                croi.closingcircle = cbcloseing.SelectedIndex;
                double fv;
                fv = trowmin.Text == "" ? 0 : double.Parse(trowmin.Text);
                croi.rowmin = fv;
                fv = trowmax.Text == "" ? 0 : double.Parse(trowmax.Text);
                croi.rowmax = fv;
                fv = tcolmin.Text == "" ? 0 : double.Parse(tcolmin.Text);
                croi.colmin = fv;
                fv = tcolmax.Text == "" ? 0 : double.Parse(tcolmax.Text);
                croi.colmax = fv;
                fv = twidthmin.Text == "" ? 0 : double.Parse(twidthmin.Text);
                croi.widthmin = fv;
                fv = twidthmax.Text == "" ? 0 : double.Parse(twidthmax.Text);
                croi.widthmax = fv;
                fv = theightmin.Text == "" ? 0 : double.Parse(theightmin.Text);
                croi.heightmin = fv;
                fv = theightmax.Text == "" ? 0 : double.Parse(theightmax.Text);
                croi.heightmax = fv;
                fv = tareamin.Text == "" ? 0 : double.Parse(tareamin.Text);
                croi.areamin = fv;
                fv = tareamax.Text == "" ? 0 : double.Parse(tareamax.Text);
                croi.areamax = fv;
                croi.areamaxcheck = ckareamax.Checked;
                croi.combinecheck = ckcombine.Checked;
                
                croi.getregion(dcamera.himg,himgbak);
                croi.showregion(dcamera.himg, true);
            }
            pictureBox1.Invalidate();
        }


        private void showroidata() {
            if (rois.croi == null)
            {
                showregion();
                clearroidata();
                return;
            }
            troi.Text = rois.croi.num.ToString("D3");
            cbpoint.SelectedIndex = rois.croi.roipoint;
            cbthway.SelectedIndex = rois.croi.thway;
            thmin.Value = (int)rois.croi.thmin;
            tthmin.Text = thmin.Value.ToString();
            thmax.Value = (int)rois.croi.thmax;
            tthmax.Text = thmax.Value.ToString();
            cbcloseing.SelectedIndex = rois.croi.closingcircle;

            trowmin.Text = rois.croi.rowmin.ToString();
            trowmax.Text = rois.croi.rowmax.ToString();
            tcolmin.Text = rois.croi.colmin.ToString();
            tcolmax.Text = rois.croi.colmax.ToString();
            twidthmin.Text = rois.croi.widthmin.ToString();
            twidthmax.Text = rois.croi.widthmax.ToString();
            theightmin.Text = rois.croi.heightmin.ToString();
            theightmax.Text = rois.croi.heightmax.ToString();
            tareamin.Text = rois.croi.areamin.ToString();
            tareamax.Text = rois.croi.areamax.ToString();
            ckareamax.Checked = rois.croi.areamaxcheck;
            ckcombine.Checked = rois.croi.combinecheck;
            //表面检测
            cksurface.Checked = rois.croi.surfacecheck;
            cksurfaceareamax.Checked = rois.croi.surfacemaxcheck;
            thminsurface.Value = rois.croi.thminsurface;
            thmaxsurface.Value = rois.croi.thmaxsurface;
            tthminsurface.Text = thminsurface.Value.ToString();
            tthmaxsurface.Text = thmaxsurface.Value.ToString();
            tbgraythresh.Text = bargraythresh.Value.ToString();
            //bararea.Value = Convert.ToInt32(rois.croi.stdsurface*10);
            //tbarea.Text = (bararea.Value*1.0/10.0).ToString();

            if (cbthway.SelectedIndex == 0) {
                thmin.Enabled = true;
                thmax.Enabled = true;
            }
            if (ckshowsurface.Checked) showsurface();
            else showregion();
        }
        private void clearroidata() {
            if (rois.croi != null) return;
            troi.Text = "";
            cbpoint.SelectedIndex = 0;
            cbthway.SelectedIndex = 0;
            thmin.Value = 0;
            thmax.Value = 128;
            tthmin.Text = "0";
            tthmax.Text = "128";
            thmin.Enabled = false;
            thmax.Enabled = false;
            cbcloseing.SelectedIndex = 0;

            trowmin.Text = "0";
            trowmax.Text = "99999";
            tcolmin.Text = "0";
            tcolmax.Text = "99999";
            twidthmin.Text = "0";
            twidthmax.Text = "99999";
            theightmin.Text = "0";
            theightmax.Text = "99999";
            tareamin.Text = "0";
            tareamax.Text = "99999";
            ckareamax.Checked = true;
            ckcombine.Checked = false;

            cksurface.Checked = false;
            thminsurface.Value = 0;
            tthminsurface.Text = "0";
            thmaxsurface.Value = 0;
            tthmaxsurface.Text = "0";
            bararea.Value = 0;
            tbarea.Text = "0";
        }

        private void trowmin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar==8 || e.KeyChar==46) e.Handled = false;
            else e.Handled = true;
            if (((sender as TextBox).Text.Length-(sender as TextBox).SelectionLength) >= 5 && e.KeyChar!=8) e.Handled = true;
        }

        private void trowmin_Leave(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text.Trim() == "") (sender as TextBox).Text = "0";
        }

        private void trowmax_Leave(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text.Trim() == "") (sender as TextBox).Text = "99999";
        }

        private void hwin_Enter(object sender, EventArgs e)
        {

        }

        private void btnroi_Click(object sender, EventArgs e)
        {
            pictureBox1.Focus();
            ondrawing = true;
        }

        //===========检测内容
        private void initdgview() {
            dgview.ColumnCount = 5;
            dgview.Columns[0].HeaderCell.Value = "项目";
            dgview.Columns[1].HeaderText = "内容";
            dgview.Columns[2].HeaderText = "标准";
            dgview.Columns[3].HeaderText = "下限";
            dgview.Columns[4].HeaderText = "上限";
            dgview.Columns[0].Width = 40;
            dgview.Columns[1].Width = 110;
            dgview.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgview.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgview.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgview.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgview.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
        }
        private void initmeasure() {
            tmname.Text = "";
            cbmroi1.SelectedIndex = -1;
            cbmroi2.SelectedIndex = -1;
            cbmtype.SelectedIndex = -1;
            tmstd.Text = "4.0";
            tmllimit.Text = "0.1";
            tmulimit.Text = "0.1";
            tmoffset.Text = "0.0";
        }
        private void initfilter() {
            trowmin.Text = "0";
            trowmax.Text = "99999";
            tcolmin.Text = "0";
            tcolmax.Text = "99999";
            twidthmin.Text = "0";
            twidthmax.Text = "99999";
            theightmin.Text = "0";
            theightmax.Text = "99999";
            tareamin.Text = "0";
            tareamax.Text = "99999";
            ckareamax.Checked = true;
            ckcombine.Checked = false;
        }

        private void btnlvcopy_Click(object sender, EventArgs e)
        {
            if (dgview.SelectedRows.Count == 1) {
                int crow=dgview.SelectedRows[0].Index;
                if (crow == (dgview.Rows.Count - 1))
                {
                    clsmeasure nm=mrois.addcopy(crow);
                    int i=dgview.Rows.Add();
                    dgview.Rows[i].Selected = true;
                    dgupdatemeasure(nm);
                }
                else
                {
                    mrois.insertcopy(crow);
                    dgview.Rows.Insert(crow, 1);
                    dgview.Rows[crow].Selected = true;
                    dgupdatemeasure(mrois[crow]);
                    dgview.Rows[crow+1].Selected = true;
                }
            }
        }

        private void btnlvdelete_Click(object sender, EventArgs e)
        {
            if (dgview.SelectedRows.Count > 0) {
                mrois.removeat(dgview.SelectedRows[0].Index);
                dgview.Rows.RemoveAt(dgview.SelectedRows[0].Index);
                dgview.ClearSelection();
                initmeasure();
            }
        }

        private void btnlvclear_Click(object sender, EventArgs e)
        {
            dgview.Rows.Clear();
            mrois.clear();
            initmeasure();
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show(Application.ProductVersion);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            dgview.Rows[dgview.RowCount - 1].Selected = true;
        }

        private void tmstd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && (e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '-' && e.KeyChar != '.'&&e.KeyChar!='+') e.Handled=true;
            if (e.KeyChar == '-') {
                if ((sender as TextBox).Text.IndexOf("-") > -1) e.Handled = true;
                if ((sender as TextBox).SelectionStart > 0) e.Handled = true;
                //if ((sender as TextBox).Text.Length > 0) e.Handled = true;
            }
            if (e.KeyChar == '+')
            {
                if ((sender as TextBox).Text.IndexOf("+") > -1) e.Handled = true;
                if ((sender as TextBox).SelectionStart > 0) e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf(".") > -1) e.Handled = true;
        }

        private void tmstd_Leave(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text.Trim() == "") (sender as TextBox).Text = "0.0";
        }

        //初始化roi
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tbrun.Checked) {
                if (tabControl1.SelectedIndex < 2) tabControl1.SelectedIndex = 2;
                return;
            }
            
            if (tabControl1.SelectedIndex == 1) {
                cbbase.Items.Clear();
                cbbase.Items.Add("");
                cbmroi1.Items.Clear();
                cbmroi2.Items.Clear();
                foreach (roishape cr in rois.rois) {
                    cbbase.Items.Add(cr.num.ToString("d3"));
                    cbmroi1.Items.Add(cr.num.ToString("d3"));
                    cbmroi2.Items.Add(cr.num.ToString("d3"));
                }
                if (rois.broi>-1) cbbase.SelectedIndex = cbbase.Items.IndexOf(rois.broi.ToString("d3"));
                else cbbase.SelectedIndex = -1;
                cbmroi1.SelectedIndex = -1;
                cbmroi2.SelectedIndex = -1;
                dgview.Focus();
                dgshowmeasure();
            }
        }

        /// 选择测量项
        private void dgview_Click(object sender, EventArgs e)
        {
            
        }
        //添加测量项
        private void btnaddmeasure_Click(object sender, EventArgs e)
        {
            if (cbmroi1.SelectedIndex == -1 && cbmroi2.SelectedIndex == -1) {
                MessageBox.Show("至少需要选择1个测量对象！");
                return;
            }
            if (tmstd.Text.Trim() == "") tmstd.Text = "0.0";
            if (tmllimit.Text.Trim() == "") tmllimit.Text = "0.0";
            if (tmulimit.Text.Trim() == "") tmulimit.Text = "0.0";
            if (tmoffset.Text.Trim() == "") tmoffset.Text = "0.0";
            clsmeasure nm = new clsmeasure();
            nm.mname = tmname.Text.Trim();
            nm.mtype = cbmtype.SelectedIndex;
            nm.roi1 = rois[cbmroi1.Text];
            nm.roi2 = rois[cbmroi2.Text];
            nm.mstd = double.Parse(tmstd.Text);
            nm.mllimit = double.Parse(tmllimit.Text);
            nm.mulimit = double.Parse(tmulimit.Text);
            nm.moffset = double.Parse(tmoffset.Text);
            mrois.add(nm);
            int i=dgview.Rows.Add();
            dgview.Rows[i].Selected = true;
            dgupdatemeasure(nm);
            isCsvTitleUpdated = true;
        }
        
        //修改测量项
        private void btneditmeasure_Click(object sender, EventArgs e)
        {
            if (dgview.SelectedRows.Count == 0) return;
            if (cbmroi1.SelectedIndex == -1 && cbmroi2.SelectedIndex == -1)
            {
                MessageBox.Show("至少需要选择1个测量对象！");
                return;
            }
            clsmeasure nm = mrois[dgview.SelectedRows[0].Index];
            if (tmstd.Text.Trim() == "") tmstd.Text = "0.0";
            if (tmllimit.Text.Trim() == "") tmllimit.Text = "0.0";
            if (tmulimit.Text.Trim() == "") tmulimit.Text = "0.0";
            if (tmoffset.Text.Trim() == "") tmoffset.Text = "0.0";
            nm.mname = tmname.Text.Trim();
            nm.mtype = cbmtype.SelectedIndex;
            nm.roi1 = rois[cbmroi1.Text];
            nm.roi2 = rois[cbmroi2.Text];
            nm.mstd = double.Parse(tmstd.Text);
            nm.mllimit = double.Parse(tmllimit.Text);
            nm.mulimit = double.Parse(tmulimit.Text);
            nm.moffset = double.Parse(tmoffset.Text);
            dgupdatemeasure(nm);
            isCsvTitleUpdated = true;
        }

        private void dgupdatemeasure(clsmeasure nmroi)
        {
            if (dgview.SelectedRows.Count == 0) return;
            int i = dgview.SelectedRows[0].Index;
            dgview[0, i].Value = nmroi.mname;
            string mcon = "";
            if (nmroi.roi1 == null) mcon = "";
            else mcon = nmroi.roi1.num.ToString("d3");
            if (nmroi.roi2 == null) mcon += "> ";
            else mcon += ">" + nmroi.roi2.num.ToString("d3");
            dgview[1, i].Value = mcon + " " + nmroi.mtypename();
            dgview[2, i].Value = nmroi.mstd;
            dgview[3, i].Value = nmroi.mllimit;
            dgview[4, i].Value = nmroi.mulimit;
        }

        private void dgupdatemeasureall() {
            //更新measure列表
            dgview.Rows.Clear();
            string mcon = "";
            int i=0;
            
            //foreach(clsmeasure cs in mrois.ilist)
            //{
            //    if (cs.roi1 == null || cs.roi2 == null)
            //    {
            //        mrois.removeat(idx);
            //        idx += 1; 
            //    }
            //}
            //int count = mrois.ilist.Count;
            //for (int idx = 0; idx < count; idx++) {
            //    clsmeasure cs = mrois[idx];
            //}

            foreach (clsmeasure cs in mrois.ilist)
            { 
                dgview.Rows.Add();
                i = dgview.Rows.Count - 1;
                if (cs.roi1 == null) mcon = "";
                else mcon = cs.roi1.num.ToString("d3");
                if (cs.roi2 == null) mcon += "> ";
                else mcon += ">" + cs.roi2.num.ToString("d3");
                dgview[0, i].Value = cs.mname;
                dgview[1, i].Value = mcon + " " + cs.mtypename();
                dgview[2, i].Value = cs.mstd;
                dgview[3, i].Value = cs.mllimit;
                dgview[4, i].Value = cs.mulimit;
            }
        }

        private void dgshowmeasure()
        {
            if (dgview.SelectedRows.Count == 0) return;
            clsmeasure mroi = mrois[dgview.SelectedRows[0].Index];
            tmname.Text = mroi.mname;
            if(mroi.roi1!=null) cbmroi1.SelectedIndex = cbmroi1.Items.IndexOf(mroi.roi1.num.ToString("d3"));
            if(mroi.roi2!=null) cbmroi2.SelectedIndex = cbmroi2.Items.IndexOf(mroi.roi2.num.ToString("d3"));
            cbmtype.SelectedIndex = cbmtype.Items.IndexOf(mroi.mtypename());
            tmstd.Text = mroi.mstd.ToString();
            tmllimit.Text = mroi.mllimit.ToString();
            tmulimit.Text = mroi.mulimit.ToString();
            tmoffset.Text = mroi.moffset.ToString("#0.000"); ;
        }

        private void dgview_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgview.SelectedRows.Count == 0) return;
            dgshowmeasure();
        }

        
        //自动运行检测
        public void run() {
            
            if (tbrun.Checked == false) return;
            isTriggered = true;
            if ((zscale !=
                (((float)phwin.Width / (float)dcamera.himg.Width))) || imgx != 0 || imgy != 0)
            {
                zoomall();
            }
            MvApi.CameraSoftTrigger(m_hCamera);
            ////抓取图像，改变dcamera.himg
            //tSdkFrameHead tFrameHead;
            //IntPtr uRawBuffer;//由SDK中给RAW数据分配内存，并释放
            //if (m_hCamera <= 0)
            //{
            //    //相机未完成初始化，返回
            //    return;
            //    //if (InitCamera() == true)
            //    //{
            //    //    MvApi.CameraPlay(m_hCamera);
            //    //}
            //    //else
            //    //    return;//相机还未初始化，句柄无效
            //}
            //else {
            //    MvApi.CameraPlay(m_hCamera);
            //}
            
            //if (MvApi.CameraSnapToBuffer(m_hCamera, out tFrameHead, out uRawBuffer, 500) == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            //{
               
                

            //    //此时，uRawBuffer指向了相机原始数据的缓冲区地址，默认情况下为8bit位宽的Bayer格式，如果
            //    //您需要解析bayer数据，此时就可以直接处理了，后续的操作演示了如何将原始数据转换为RGB格式
            //    //并显示在窗口上。
            //    double stime = Environment.TickCount;
            //    //将相机输出的原始数据转换为RGB格式到内存m_ImageBufferSnapshot中
            //    MvApi.CameraImageProcess(m_hCamera, uRawBuffer, m_ImageBufferSnapshot, ref tFrameHead);
            //    //CameraSnapToBuffer成功调用后必须用CameraReleaseImageBuffer释放SDK中分配的RAW数据缓冲区
            //    //否则，将造成死锁现象，预览通道和抓拍通道会被一直阻塞，直到调用CameraReleaseImageBuffer释放后解锁。
            //    MvApi.CameraReleaseImageBuffer(m_hCamera, uRawBuffer);
            //    //更新抓拍显示窗口。
            //    double etime = Environment.TickCount;
            //    //Console.WriteLine("一次触发全流程耗时：{0}", etime - stime);
            //    MvApi.CameraPause(m_hCamera);

            //    dcamera.himg = new Mat(tFrameHead.iHeight, tFrameHead.iWidth, MatType.CV_8UC1, m_ImageBufferSnapshot);//.CvtColor(ColorConversionCodes.RGB2BGR);
            //                                                                                                          //Cv2.ImWrite(".\\himgrgb.bmp",dcamera.himg);
            //                                                                                                          //Cv2.ImWrite(".\\himgbgr.bmp", dcamera.himg.CvtColor(ColorConversionCodes.RGB2BGR)); 
            //    Cv2.Flip(dcamera.himg, dcamera.himg, FlipMode.X);
            //    if (dcamera.himg.Type().Channels == 1)
            //    {
            //        Cv2.CvtColor(dcamera.himg, dcamera.himg, ColorConversionCodes.GRAY2BGR);
            //    }
            //    himgbak = dcamera.getBackImg();
                

            //}
            
            //runonce(pictureBox1);
            

            ////显示测试区域
            //if (ckregion.Checked)
            //{
            //    foreach (roishape rs in rois.rois)
            //    {
            //        if (rs.num == rois.broi) continue;
            //        rs.showregion(dcamera.himg, true);
            //    }
            //}
        }
        //手动运行一次
        public void runonce(PictureBox pictureBox1) {
            
            //vcommon.viewscale = zscale = (float)phwin.Width / (float)himgbak.Width;
            //vcommon.viewx = imgx = 0;
            //vcommon.viewy = imgy = 0;
            //pictureBox1.Refresh();
            
            //if (lbpass.Visible == false) Console.WriteLine("");
            //初始化
            if (this.lbpass.InvokeRequired){lbpass.Invoke(new MethodInvoker(delegate (){ lbpass.Visible = false;}));}
            else{ lbpass.Visible = false;}
            if (this.lbng.InvokeRequired) { lbng.Invoke(new MethodInvoker(delegate () { lbng.Visible = false; })); }
            else { lbng.Visible = false; }

            rois.text1 = "";
            rois.text2 = "";
            rois.srois.clear();
            rois.croi = null;
            //计算
            double estime = Environment.TickCount;
            rois.run(pictureBox1);
            double cenpoitend = Environment.TickCount;
            Console.WriteLine("计算中心点耗时----->: {0}", cenpoitend - estime);
            //测量
            mrois.run();
           
            testresult = "OK";
            if (this.rtresult.InvokeRequired) { rtresult.Invoke(new MethodInvoker(delegate () 
            {
                rtresult.Clear();
                rtresult.AppendText("序号\t检测项\t检测内容\t标准值\t下公差\t上公差\t实测\t结果\r\n");
                rtresult.AppendText("====\t======\t============\t======\t======\t======\t======\t====\r\n");
            })); }
            else {
                rtresult.Clear();
                rtresult.AppendText("序号\t检测项\t检测内容\t标准值\t下公差\t上公差\t实测\t结果\r\n");
                rtresult.AppendText("====\t======\t============\t======\t======\t======\t======\t====\r\n");
            }
            
            //rtresult.Clear();
            //rtresult.AppendText("序号\t检测项\t检测内容\t标准值\t下公差\t上公差\t实测\t结果\r\n");
            //rtresult.AppendText("====\t======\t============\t======\t======\t======\t======\t====\r\n");
            int i=1;
            string rstr = "";
            string mcon = "";
            foreach (clsmeasure cm in mrois.ilist) {

                //double stime1 = Environment.TickCount;

                //double etime1 = Environment.TickCount;
                //Console.WriteLine("ekdisplayall.Checked ：{0}", etime1 - stime1);
                //显示区域
                if (ckdisplayall.Checked) {
                    rois.showresult(iw, ih, cm.roi1);
                    if (cm.mtype < 3 || (cm.mtype == 3 && cm.roi2 != null))
                    {
                        rois.showresult(iw, ih, cm.roi2);
                    }
                }else if (ckdisplayng.Checked && cm.mresult == "NG") {
                    rois.showresult(iw, ih, cm.roi1); 
                    if (cm.mtype < 3 || (cm.mtype == 3 && cm.roi2 != null))
                    {
                        rois.showresult(iw, ih, cm.roi2); 
                    }
                }
                //输出结果
                if (this.rtresult.InvokeRequired)
                {
                    rtresult.Invoke(new MethodInvoker(delegate ()
                    {
                        if (cm.mresult == "NG")
                            rtresult.SelectionColor = Color.Red;
                        else
                            rtresult.SelectionColor = Color.Black;
                    }));
                }
                else
                {
                    if (cm.mresult == "NG")
                        rtresult.SelectionColor = Color.Red;
                    else
                        rtresult.SelectionColor = Color.Black;
                }
                //if (cm.mresult == "NG") 
                //    rtresult.SelectionColor= Color.Red;
                //else 
                //    rtresult.SelectionColor = Color.Black;
                if (cm.roi1 == null) mcon = "";
                else mcon = cm.roi1.num.ToString("d3");
                if (cm.roi2 == null) mcon += "> ";
                else mcon += ">" + cm.roi2.num.ToString("d3");
                mcon += cm.mtypename();
                
                if(ckdisplayall.Checked || (ckdisplayng.Checked && cm.mresult=="NG"))
                    if (this.rtresult.InvokeRequired)
                    {
                        rtresult.Invoke(new MethodInvoker(delegate ()
                        {
                            rtresult.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\r\n", i, cm.mname, mcon, cm.mstd, cm.mllimit, cm.mulimit, cm.mvalue.ToString("f3"), cm.mresult));
                        }));
                    }
                    else
                    {
                        rtresult.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\r\n", i, cm.mname, mcon, cm.mstd, cm.mllimit, cm.mulimit, cm.mvalue.ToString("f3"), cm.mresult));
                    }
                //rtresult.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\r\n",i,cm.mname,mcon,cm.mstd,cm.mllimit,cm.mulimit,cm.mvalue.ToString("f3"),cm.mresult));
                
                rstr += string.Format("{0} : {1}", cm.mname, cm.mvalue.ToString("f3")) + "\r\n";
                i++;
                if(cm.mresult=="NG") testresult="NG";
            }
            

            //表面检测
            foreach (roishape rs in rois.rois) {
                if (rs.surfacecheck) {
                    //if (rs.surfacecheck)
                    double stime1 = Environment.TickCount;
                    bool areack = rs.measuresuface(dcamera.himg,himgbak, false,true);
                    double etime1 = Environment.TickCount;

                    Console.WriteLine("表面检测耗时： {0}",etime1 - stime1);
                    if (areack == false) {
                        testresult = "NG";
                        mcon = rs.num.ToString("d3");
                        if (this.rtresult.InvokeRequired)
                        {
                            rtresult.Invoke(new MethodInvoker(delegate ()
                            {
                                rtresult.SelectionColor = Color.Red;
                                rtresult.AppendText(string.Format("{0}\t{1}\t{2}\t{3} {4}\t{5}   {6}\t{7}\r\n", i, "表面检测", mcon,  rs.minDefectArea+"pix", " 0 ", " 0 ", Convert.ToInt32( rs.defectArea).ToString()+ "pix", "NG"));

                            }));
                        }
                        else
                        {
                            rtresult.SelectionColor = Color.Red;
                            rtresult.AppendText(string.Format("{0}\t{1}\t{2}\t{3} {4}\t{5}   {6}\t{7}\r\n", i, "表面检测", mcon, rs.minDefectArea+ "pix", " 0 ", " 0 ", Convert.ToInt32( rs.defectArea).ToString()+ "pix", "NG"));

                        }
                        i++;
                    }
                }
            }

            //结果输出
            if (testresult == "NG")
            {
                if (this.lbng.InvokeRequired) { lbng.Invoke(new MethodInvoker(delegate () { lbng.Visible = true; })); }
                else { lbng.Visible = true; }
                //lbng.Visible = true;
                //if(tbrun.Checked) dio.sendng();
                vcommon.qtyng++;
            }
            else {
                if (this.lbpass.InvokeRequired) { lbpass.Invoke(new MethodInvoker(delegate () { lbpass.Visible = true; })); }
                else { lbpass.Visible = false; }
                //lbpass.Visible = true;
                //if(tbrun.Checked) dio.sendok();
                vcommon.qtypass++;
            }
            vcommon.qty++;
            vcommon.showstatistic();
            

            //显示测量数据统计结果
            if (this.rtdatastatistic.InvokeRequired)
            {
                rtdatastatistic.Invoke(new MethodInvoker(delegate ()
                {
                    rtdatastatistic.Clear();
                    rtdatastatistic.AppendText("序号\t检测项\t检测内容\t检测数\tNG数\tNG偏大\tNG偏小\r\n");
                    rtdatastatistic.AppendText("====\t======\t============\t======\t======\t======\t======\r\n");
                }));
            }
            else
            {
                rtdatastatistic.Clear();
                rtdatastatistic.AppendText("序号\t检测项\t检测内容\t检测数\tNG数\tNG偏大\tNG偏小\r\n");
                rtdatastatistic.AppendText("====\t======\t============\t======\t======\t======\t======\r\n");
            }
            //rtdatastatistic.Clear();
            //rtdatastatistic.AppendText("序号\t检测项\t检测内容\t检测数\tNG数\tNG偏大\tNG偏小\r\n");
            //rtdatastatistic.AppendText("====\t======\t============\t======\t======\t======\t======\r\n");
            i = 1;
            foreach (clsmeasure cm in mrois.ilist){
                if (ckdatang.Checked && (cm.mngssmall + cm.mngslarge) == 0) continue;
                if (cm.roi1 == null) mcon = "";
                else mcon = cm.roi1.num.ToString("d3");
                if (cm.roi2 == null) mcon += "> ";
                else mcon += ">" + cm.roi2.num.ToString("d3");
                mcon += cm.mtypename();
                if (this.rtdatastatistic.InvokeRequired)
                {
                    rtdatastatistic.Invoke(new MethodInvoker(delegate ()
                    {
                        rtdatastatistic.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t", i, cm.mname, mcon, cm.mnums, cm.mngssmall + cm.mngslarge));
                        rtdatastatistic.SelectionColor = Color.Red;
                        rtdatastatistic.AppendText(string.Format("{0}\t{1}\r\n", cm.mngslarge, cm.mngssmall));
                        rtdatastatistic.SelectionColor = Color.Black;
                    }));
                }
                else
                {
                    rtdatastatistic.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t", i, cm.mname, mcon, cm.mnums, cm.mngssmall + cm.mngslarge));
                    rtdatastatistic.SelectionColor = Color.Red;
                    rtdatastatistic.AppendText(string.Format("{0}\t{1}\r\n", cm.mngslarge, cm.mngssmall));
                    rtdatastatistic.SelectionColor = Color.Black;
                }
                //rtdatastatistic.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t", i, cm.mname, mcon, cm.mnums, cm.mngssmall+cm.mngslarge));
                //rtdatastatistic.SelectionColor = Color.Red;
                //rtdatastatistic.AppendText(string.Format("{0}\t{1}\r\n", cm.mngslarge, cm.mngssmall));
                //rtdatastatistic.SelectionColor = Color.Black;
                i++;
            }
            
            
            //测量区显示测量结果
            rois.text1 = testresult;
            if (vcommon.hshowresult) rois.text2 = rstr;
            else rois.text2 = string.Format("检测数： {0}\r\nPASS数： {1}\r\nNG数： {2}\r\n用时(ms)： {3}", tqty.Text, tqtypass.Text, tqtyng.Text, truntime.Text);

            
            ////显示原点
            //hwin.HalconWindow.SetColor("red");
            //hwin.HalconWindow.DispCross(0.0, 0.0, 100.0, 0.0);
            //NG图片保存
            //tscreen.Enabled = true;
            //if (tbrun.Checked)
            //{
            //    if (testresult == "NG") dio.sendng();
            //    else dio.sendok();
            //}
            
            if (btnstart.Enabled == false) {
                saveNgPicThread = new Thread(new ThreadStart(tscreen_Tick));
                saveNgPicThread.Start();
            }
            
            ////PLC信号输出
            //if (tbrun.Checked){
            //   if (testresult == "NG") dio.sendng();
            //   else dio.sendok();
            //}
            double showrtstart = Environment.TickCount;
            if (this.pictureBox1.InvokeRequired)
            {
                pictureBox1.Invoke(new MethodInvoker(
                   delegate ()
                   {
                       pictureBox1.Refresh();
                   }));
            }
            else
            { 
                pictureBox1.Refresh(); 
            }
            double howrtend = Environment.TickCount;
            Console.WriteLine("界面刷新耗时----->: {0}", howrtend - showrtstart);
            //时间
            double eetime = Environment.TickCount;
            if (this.truntime.InvokeRequired) { truntime.Invoke(new MethodInvoker(delegate () { truntime.Text = (eetime - estime).ToString(); })); }
            else { truntime.Text = (eetime - estime).ToString(); }
            
            //rois.text1 = testresult;
        }
        //===========检测内容

        //===========产品加载，保存
        private void btnsaveproduct_Click(object sender, EventArgs e)
        {
            if (himgbak == null) {
                MessageBox.Show("保存失败，模板为空！");
                return;
            }
            //保存数据
            if (!Program.getversion()) return;
            string fn = mrois.productname??"";
            dlgsave.FileName = Path.GetFileName(fn);
            dlgsave.InitialDirectory = fn==""?"":Path.GetFullPath(fn);
            dlgsave.Filter = "Measure File|*.lvd";
            dlgsave.DefaultExt = ".lvd";
            if (dlgsave.ShowDialog() == DialogResult.OK)
            {
                fn = dlgsave.FileName;
                mrois.productname = fn;
                ArrayList data = new ArrayList();
                data.Add(rois.broi);
                data.Add(rois.brow);
                data.Add(rois.bcol);
                data.Add(rois.rois);
                data.Add(mrois.ilist);
                templateFile = fn.Replace(".lvd", ".bmp");
                data.Add(templateFile);
                data.Add(zscale);
                data.Add(imgx);
                data.Add(imgy);
                FileStream fs = new FileStream(fn, FileMode.Create, FileAccess.Write);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, data);
                fs.Flush();
                fs.Close();
                data.Clear();
                data = null;
                tpart.Text = Path.GetFileName(fn);
                vcommon.productname = fn;
                
                Cv2.ImWrite(templateFile,template);
            }
        }

        private void btnloadproduct_Click(object sender, EventArgs e)
        {
            //加载数据
            if (!Program.getversion()) return;
            tpart.Text = "";
            string fn = mrois.productname??"";
            dlgopen.FileName = Path.GetFileName(fn);
            dlgopen.InitialDirectory = fn==""?"":Path.GetFullPath(fn);
            dlgopen.Filter = "Measure File|*.lvd";
            if (dlgopen.ShowDialog() == DialogResult.OK)
            {
                fn = dlgopen.FileName;
                ArrayList data;
                FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
                BinaryFormatter bf = new BinaryFormatter();
                data = (ArrayList)bf.Deserialize(fs);
                fs.Close();
                rois.clear();
                
                rois.broi = (int)data[0];
                rois.brow = (double)data[1];
                rois.bcol = (double)data[2];
                rois.rois = (ArrayList)data[3];
                mrois.clear();
                mrois.ilist = (ArrayList)data[4];
                templateFile = (string)data[5];
                zscale = (double)data[6];
                imgx = (int)data[7];
                imgy = (int)data[8];
                data.Clear();
                data = null;
            }
            else
            {
                return;
            }
            //文件名
            tpart.Text = Path.GetFileName(fn);
            vcommon.productname = fn;
            templateFile = fn.Replace(".lvd", ".bmp");
            dcamera.himg = Cv2.ImRead(templateFile);
            dcamera.himg.CopyTo(himgbak);
            dcamera.himg.CopyTo(template);
            pictureBox1.Refresh();
            //更新roi列表
            cbbase.Items.Clear();
            cbbase.Items.Add("");
            cbmroi1.Items.Clear();
            cbmroi2.Items.Clear();
            foreach (roishape rs in rois.rois) {
                cbbase.Items.Add(rs.num.ToString("d3"));
                cbmroi1.Items.Add(rs.num.ToString("d3"));
                cbmroi2.Items.Add(rs.num.ToString("d3")); 
            }
            if (rois.broi > -1) cbbase.SelectedIndex = cbbase.Items.IndexOf(rois.broi.ToString("d3"));
            rois.srois.clear();
            if (rois.rois.Count == 0) rois.nums = 0;
            else rois.nums = (rois.rois[rois.rois.Count - 1] as roishape).num;
            //同步rois与mrois
            int i = 0;
            foreach (clsmeasure cm in mrois.ilist) {
                if(cm.roi1!=null) (mrois.ilist[i] as clsmeasure).roi1 = rois.getroi(cm.roi1.num);
                if(cm.roi2!=null) (mrois.ilist[i] as clsmeasure).roi2 = rois.getroi(cm.roi2.num);
                i++;
            }
            //更新measure列表
            dgview.Rows.Clear();
            string mcon = "";
            foreach (clsmeasure cs in mrois.ilist) {
                dgview.Rows.Add();
                i = dgview.Rows.Count - 1;
                if (cs.roi1 == null) mcon = "";
                else mcon = cs.roi1.num.ToString("d3");
                if (cs.roi2 == null) mcon += "> ";
                else mcon += ">" + cs.roi2.num.ToString("d3");
                dgview[0, i].Value = cs.mname;
                dgview[1, i].Value = mcon + " " + cs.mtypename();
                dgview[2, i].Value = cs.mstd;
                dgview[3, i].Value = cs.mllimit;
                dgview[4, i].Value = cs.mulimit;
            }
            if (dgview.Rows.Count > 0) dgview.Rows[0].Selected = true;
            //statistic
            vcommon.qty = 0;
            vcommon.qtyng = 0;
            vcommon.qtypass = 0;
            vcommon.showstatistic();
            truntime.Text = "";
            rtresult.Text = "";
            if (dcamera.himg!=null && himgbak!=null)
                showroidata();
            pictureBox1.Invalidate();
        }

        private void openfile(string fn) {
            //加载数据
            if (!Program.getversion()) return;
            tpart.Text = "";
            //if (fn == "") return;
            if (!File.Exists(fn)) fn=Application.StartupPath+"\\default.lvd";
             
            ArrayList data;
            FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            data = (ArrayList)bf.Deserialize(fs);
            fs.Close();
            tpart.Text = Path.GetFileName(fn);
            vcommon.productname = fn;
            templateFile = fn.Replace(".lvd", ".bmp");
            dcamera.himg = Cv2.ImRead(templateFile);
            himgbak = dcamera.getBackImg();
            himgbak.CopyTo(template);
            imgx = Convert.ToInt32(vcommon.viewx);
            imgy = Convert.ToInt32(vcommon.viewy);
            zscale = vcommon.viewscale;
            //图形元素
            rois.clear();
            rois.srois.clear();
            rois.broi = (int)data[0];
            rois.brow = (double)data[1];
            rois.bcol = (double)data[2];
            if (rois.rois.Count == 0) rois.nums = 0;
            else rois.nums = (rois.rois[rois.rois.Count - 1] as roishape).num;

            rois.rois = (ArrayList)data[3];
            mrois.clear();
            mrois.ilist = (ArrayList)data[4];

            templateFile = (string)data[5];
            zscale = (double)data[6];
            imgx = (int)data[7];
            imgy = (int)data[8];
            //templateFile = "C:\\Users\\24981\\Desktop\\ct\\ctvision\\ctvision\\bin\\Debug\\default.bmp";
            //zscale = 0.2;
            //imgx = 0;
            //imgy = 0;

            data.Clear();
            data = null;
            
            //更新roi列表
            cbbase.Items.Clear();
            cbbase.Items.Add("");
            cbmroi1.Items.Clear();
            cbmroi2.Items.Clear();
            foreach (roishape rs in rois.rois)
            {
                cbbase.Items.Add(rs.num.ToString("d3"));
                cbmroi1.Items.Add(rs.num.ToString("d3"));
                cbmroi2.Items.Add(rs.num.ToString("d3"));
            }
            if (rois.broi > -1) cbbase.SelectedIndex = cbbase.Items.IndexOf(rois.broi.ToString("d3"));
            rois.srois.clear();
            if (rois.rois.Count == 0) rois.nums = 0;
            else rois.nums = (rois.rois[rois.rois.Count - 1] as roishape).num;

            //同步rois与mrois
            int i = 0;
            foreach (clsmeasure cm in mrois.ilist)
            {
                if (cm.roi1 != null) (mrois.ilist[i] as clsmeasure).roi1 = rois.getroi(cm.roi1.num);
                if (cm.roi2 != null) (mrois.ilist[i] as clsmeasure).roi2 = rois.getroi(cm.roi2.num);
                i++;
            }
            //更新measure列表
            dgview.Rows.Clear();
            string mcon = "";
            int j = 1;
            foreach (clsmeasure cs in mrois.ilist)
            {
                dgview.Rows.Add();
                i = dgview.Rows.Count - 1;
                if (cs.roi1 == null) mcon = "";
                else mcon = cs.roi1.num.ToString("d3");
                if (cs.roi2 == null) mcon += "> ";
                else mcon += ">" + cs.roi2.num.ToString("d3");
                dgview[0, i].Value = cs.mname;
                dgview[1, i].Value = mcon + " " + cs.mtypename();
                dgview[2, i].Value = cs.mstd;
                dgview[3, i].Value = cs.mllimit;
                dgview[4, i].Value = cs.mulimit;
                j++;
            }
            if (dgview.Rows.Count > 0) dgview.Rows[0].Selected = true;

            //statistic
            vcommon.showstatistic();
            truntime.Text = "";
            rtresult.Text = "";
            
            //pictureBox1.Invalidate();
        }

        private void btnnewproduct_Click(object sender, EventArgs e)
        {
            //foreach (roishape rs in rois.srois.rois) mrois.delete(rs);
            rois.deleteAll(pictureBox1, dcamera.himg, himgbak, iw, ih);
            if (rois.broi == -1) cbbase.SelectedIndex = -1;
            dgupdatemeasureall();
            //pictureBox1.Invalidate();
            //showroidata();

            //rois
            rois.clear();
            cbmroi1.Items.Clear();
            cbmroi2.Items.Clear();
            clearroidata();
            rois.text1 = "";
            rois.text2 = "data...";
            rois.isshowtext = true;
            ckresult.Checked = true;
            //mrois
            mrois.clear();
            mrois.productname = "";
            dgview.Rows.Clear();
            initmeasure();
            //statistic
            tpart.Text = "";
            vcommon.qty = 0;
            vcommon.qtyng = 0;
            vcommon.qtypass = 0;
            vcommon.showstatistic();
            truntime.Text = "";
            rtresult.Text = "";
            pictureBox1.Refresh();
        }
        //================产品管理

        //================相机设置
        private void tbcamera_Click(object sender, EventArgs e)
        {
            if (InitCamera() == true)
            {
                MvApi.CameraShowSettingPage(m_hCamera, 1);//1 show ; 0 hide
            }
            //dcamera.showSetting();
            ////fcamera = new frmcamera();
            ////fcamera.Owner = this;
            ////fcamera.Show();
        }
        

    private void tbcameraplay_Click(object sender, EventArgs e)
        {
            if (m_hCamera < 1)//还未初始化相机
            {
                if (InitCamera() == true)
                {
                    MvApi.CameraPlay(m_hCamera);
                    //BtnPlay.Text = "Pause";
                }
            }

            tbcameraplay.Checked = !tbcameraplay.Checked;
            
            if (tbcameraplay.Checked)
            { 
                foreach (ToolStripItem tb in mtools.Items)
                {
                    if (tb.Name != "tbcameraplay" && tb.Name != "tbcamera") tb.Enabled = false;
                }
                MvApi.CameraSetTriggerMode(m_hCamera, (int)emSdkSnapMode.CONTINUATION);
            }
            else {
                
                foreach (ToolStripItem tb in mtools.Items)
                {
                    tb.Enabled = true; 
                }
                MvApi.CameraSetTriggerMode(m_hCamera, (int)emSdkSnapMode.SOFT_TRIGGER);
                template.CopyTo(dcamera.himg);
                template.CopyTo(himgbak);
                pictureBox1.Invalidate();
                //frmmain_Shown(null,null);
                //MvApi.CameraSoftTrigger(m_hCamera);
                //tbrunonce_Click(null, null);
                //提示当前帧是否保存为模板
                //if (MessageBox.Show("是否将当前图像作为新模板?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                //{
                //    isSaveToTemplate = true;
                //    //MvApi.CameraSoftTrigger(m_hCamera);
                //    //Thread.Sleep(150);
                //    //initview();
                //    //btnnewproduct_Click(null, null);
                //}
                //else
                //{
                //    pictureBox1.Invalidate();
                //}
                //Thread.Sleep(50);
                //btnsaveproduct_Click(null, null);
                //btnloadproduct_Click(null, null);
            }

            //return;
            
            //if (m_hCamera < 1)//还未初始化相机
            //{
            //    if (InitCamera() == true)
            //    {
            //        MvApi.CameraPlay(m_hCamera);
            //        //BtnPlay.Text = "Pause";
            //    }
            //}
            //else//已经初始化
            //{
            //    if (tbcameraplay.Checked)
            //    {
            //        //m_bExitCaptureThread = false;
            //        MvApi.CameraPlay(m_hCamera);
            //        //tplay.Enabled = true;
            //    }
            //    else
            //    {
            //        tSdkFrameHead tFrameHead;
            //        IntPtr uRawBuffer;//由SDK中给RAW数据分配内存，并释放
            //        if (MvApi.CameraSnapToBuffer(m_hCamera, out tFrameHead, out uRawBuffer, 500) == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            //        {
            //            //此时，uRawBuffer指向了相机原始数据的缓冲区地址，默认情况下为8bit位宽的Bayer格式，如果
            //            //您需要解析bayer数据，此时就可以直接处理了，后续的操作演示了如何将原始数据转换为RGB格式
            //            //并显示在窗口上。

            //            //将相机输出的原始数据转换为RGB格式到内存m_ImageBufferSnapshot中
            //            MvApi.CameraImageProcess(m_hCamera, uRawBuffer, m_ImageBufferSnapshot, ref tFrameHead);
            //            //CameraSnapToBuffer成功调用后必须用CameraReleaseImageBuffer释放SDK中分配的RAW数据缓冲区
            //            //否则，将造成死锁现象，预览通道和抓拍通道会被一直阻塞，直到调用CameraReleaseImageBuffer释放后解锁。
            //            MvApi.CameraReleaseImageBuffer(m_hCamera, uRawBuffer);
            //            //更新抓拍显示窗口。

            //            double stime = Environment.TickCount;
            //            dcamera.himg = new Mat(tFrameHead.iHeight, tFrameHead.iWidth, MatType.CV_8UC1, m_ImageBufferSnapshot);//.CvtColor(ColorConversionCodes.RGB2BGR);
            //            //Cv2.ImWrite(".\\himgrgb.bmp",dcamera.himg);
            //            //Cv2.ImWrite(".\\himgbgr.bmp", dcamera.himg.CvtColor(ColorConversionCodes.RGB2BGR)); 
            //            Cv2.Flip(dcamera.himg, dcamera.himg, FlipMode.X);
            //            if (dcamera.himg.Type().Channels == 1)
            //            {
            //                Cv2.CvtColor(dcamera.himg, dcamera.himg, ColorConversionCodes.GRAY2BGR);
            //            } 
            //            himgbak = dcamera.getBackImg();
            //            MvApi.CameraPause(m_hCamera);
            //            double etime = Environment.TickCount;
            //            Console.WriteLine("solution1 cost time：{0}", etime - stime);
            //            //vcommon.viewscale = zscale =(float)phwin.Width / (float)himgbak.Width;
                        
            //            //vcommon.viewx = imgx = 0;
            //            //vcommon.viewy = imgy = 0; 
            //            //pictureBox1.Refresh();
            //            //phwin.Refresh();
            //            //btnnewproduct_Click(null, null);
            //        }
                    
            //        //tplay.Enabled = false;
            //    }
            //}

            //pictureBox1.Invalidate();
            //if (tbcameraplay.Checked)
            //    tplay.Enabled = true;
            //else
            //    tplay.Enabled = false;
            //if (tplay.Enabled == false)
            //    pictureBox1.Invalidate();
        }

        private void tplay_Tick(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            //dcamera.grabasync();
            //#HSystem.SetSystem("flush_graphic", "false");
            //#hwin.HalconWindow.ClearWindow();

            //Graphics dc = pictureBox1.CreateGraphics();
            //dc.Clear(Color.Black);
            ////显示图像
            //try
            //{
            //    if (dcamera != null)
            //        pictureBox1.Image = dcamera.himg.ToBitmap();
            //        //#hwin.HalconWindow.DispImage(dcamera.himg);
            //}
            //catch { }


            /*//显示十字线
            hwin.HalconWindow.SetColor("red");
            hwin.HalconWindow.SetLineStyle((new HTuple(3)).TupleConcat(1).TupleConcat(3).TupleConcat(1));
            hwin.HalconWindow.DispLine(hlrow, hlcol, hlrow1, hlcol1);
            hwin.HalconWindow.DispLine(vlrow, vlcol, vlrow1, vlcol1);


            //更新视图
            HSystem.SetSystem("flush_graphic", "true");
            hwin.HalconWindow.DispCross(0.0, 0.0, 100.0, 0.0);*/
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tbio_Click(object sender, EventArgs e)
        {
            fio = new frmio();
            fio.Owner = this;
            fio.ShowDialog();
        }
        private void tbrunonce_Click(object sender, EventArgs e)
        { 
            
             
            if (tbcameraplay.Checked) {
                tplay.Enabled = false;
                tbcameraplay.Checked = false;
            }

            if (dcamera.himg == null||dcamera.himg.Width==0||dcamera.himg.Height==0||template==null||template.Width==0||template.Height==0) {
                MessageBox.Show("模板图片为空，请先加载图片或点击\"相机预览\"->\"停止预览\"。");
                return;
            }
            if ((zscale !=
                (((float)phwin.Width / (float)dcamera.himg.Width))) || imgx != 0 || imgy != 0)
            {
                zoomall();
            }
            MvApi.CameraSoftTrigger(m_hCamera);
            //runonce(pictureBox1);
            

            ////显示测试区域
            //if (ckregion.Checked)
            //{
            //    foreach (roishape rs in rois.rois)
            //    {
            //        if (rs.num == rois.broi) continue;
            //        rs.showregion(dcamera.himg, true);
            //    }
            //}
        }

        private void frmmain_FormClosing(object sender, FormClosingEventArgs e)
        {
            vcommon.savedata();
            savemeasurename();
            if(MessageBox.Show("你确认要退出系统吗?","确认",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes){
                log.write("关闭软件");
                //关闭相机，释放资源
                if (m_hCamera > 0)
                {
//#if !USE_CALL_BACK //使用回调函数的方式则不需要停止线程
//                    m_bExitCaptureThread = true;
//                    while (m_tCaptureThread!=null && m_tCaptureThread.IsAlive)
//                    {
//                        Thread.Sleep(10);
//                    }
//#endif
                    MvApi.CameraUnInit(m_hCamera);
                    Marshal.FreeHGlobal(m_ImageBuffer);
                    Marshal.FreeHGlobal(m_ImageBufferSnapshot);
                    m_hCamera = 0;
                }

                fprint.Dispose();
                fzebra.Dispose();
                if (sw_csv != null)
                    sw_csv.Close();
                if (fs_csv != null)
                    fs_csv.Close();
                Application.ExitThread();
            }else{
                e.Cancel = true;
            }
        }

        private void tbrun_Click(object sender, EventArgs e)
        {
             
           
            //关闭播放，移动，放大镜功能
            btnpixelunit.Enabled = false;
            if (tbcameraplay.Checked)
            {
                tplay.Enabled = false;
                tbcameraplay.Checked = false;
            }

            tb_move.Checked = false;
            Cursor = Cursors.Default;
            tb_magic.Checked = false;
            tbdrawrect.Checked = false;
            foreach (ToolStripItem tb in mtools.Items) {
                if(tb.Name!="toolStripButton8") tb.Enabled = false;
            }
            //foreach (Control ctr in tabControl1.Controls)
            //{
            //    ctr.Enabled = false; 
            //}
            //tbrunonce.Enabled = true;
            tbrun.Enabled = true;
            tbrunstrop.Enabled = true;
            tbhelp.Enabled = true;
            tbrun.Checked = true;
            ptools.Width = 450;
            toolStripButton7_Click(null, null);
            tabControl1.SelectedIndex = 3;

             
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (btnstart.Enabled == false) {
                btnstart.Enabled = true;
                btnend_Click(null,null);
            }
            //恢复播放，移动，放大镜功能
            tb_move.Checked = false;
            Cursor = Cursors.Default;
            tb_magic.Checked = false;
            foreach(ToolStripItem tb in mtools.Items){
                tb.Enabled = true;
            }
            tbrun.Checked = false;
            btnpixelunit.Enabled = true;
        }



        private void ckresult_CheckedChanged(object sender, EventArgs e)
        {
            rois.isshowtext = ckresult.Checked;
            pictureBox1.Invalidate();
        }

        private void tbhelp_Click(object sender, EventArgs e)
        {
            frmabout fabout = new frmabout();
            fabout.ShowDialog();
        }

        private void tbcamera_MouseEnter(object sender, EventArgs e)
        {
            if (!Program.getversion())
            {
                slabeltips.ForeColor = Color.Red;
                slabeltips.Text = "请使用正版软件，盗版必纠！";
                return;
            }
            else
            {
                slabeltips.ForeColor = Color.Black;
                slabeltips.Text = (sender as ToolStripItem).ToolTipText;
            }
        }

        private void mtools_MouseLeave(object sender, EventArgs e)
        {
            if (!Program.getversion())
            {
                slabeltips.ForeColor = Color.Red;
                slabeltips.Text = "请使用正版软件，盗版必纠！";
                return;
            }
            slabeltips.Text = "";
        }

        private void frmmain_Shown(object sender, EventArgs e)
        {
            loadmeasurename();
            ondrawingstr = "phi";
            //if (vcommon.productname!="") {
                
            //}
            openfile(vcommon.productname);
            vcommon.viewscale = zscale;
            vcommon.viewx = imgx;
            vcommon.viewy = imgy;
            //dcamera.grabasync();
            //foreach (roishape rs in rois.rois)
            //{
            //    rois.srois.add(rs);
            //}
            showroidata();
            foreach (roishape rs in rois.rois)
            {
                if (rs.surfacecheck)
                {
                    if (rs.thminsurface > 0)
                        rs.getWhiteMask(dcamera.himg, himgbak);
                    if (rs.thmaxsurface > 0)
                        rs.getBlackMask(dcamera.himg, himgbak);
                }
            }
            pictureBox1.Invalidate();
        }

        private void tbsetting_Click(object sender, EventArgs e)
        {
            frmsetting fs = new frmsetting();
            fs.StartPosition = FormStartPosition.CenterScreen;
            fs.ShowDialog();
            pictureBox1.Invalidate();
        }

        

        private void tbalignleft_Click(object sender, EventArgs e)
        {
            rois.srois.alignleft(pictureBox1, dcamera.himg, himgbak, iw,ih);
            pictureBox1.Invalidate();
        }

        private void tbalignright_Click(object sender, EventArgs e)
        {
            rois.srois.alignright(pictureBox1, dcamera.himg, himgbak, iw, ih);
            pictureBox1.Invalidate();
        }

        private void tbalignmidh_Click(object sender, EventArgs e)
        {
            rois.srois.alignmidh(pictureBox1, dcamera.himg, himgbak, iw, ih);
            pictureBox1.Invalidate();
        }
        private void tbaligntop_Click(object sender, EventArgs e)
        {
            rois.srois.aligntop(pictureBox1, dcamera.himg, himgbak, iw, ih);
            pictureBox1.Invalidate();
        }

        private void tbalignbottom_Click(object sender, EventArgs e)
        {
            rois.srois.alignbot(pictureBox1, dcamera.himg, himgbak, iw, ih);
            pictureBox1.Invalidate();
        }

        private void tbalignmidv_Click(object sender, EventArgs e)
        {
            rois.srois.alignmidv(pictureBox1, dcamera.himg, himgbak, iw, ih);
            pictureBox1.Invalidate();
        }

        private void tbalignheight_Click(object sender, EventArgs e)
        {
            rois.srois.alignheight(pictureBox1, dcamera.himg, himgbak, iw, ih);
            pictureBox1.Invalidate();
        }

        private void tbalignwidth_Click(object sender, EventArgs e)
        {
            rois.srois.alignwidth(pictureBox1, dcamera.himg, himgbak, iw, ih);
            pictureBox1.Invalidate();
        }

        private void tbalignsize_Click(object sender, EventArgs e)
        {
            rois.srois.alignsamesize(pictureBox1, dcamera.himg, himgbak, iw, ih);
            showroidata();
            //pictureBox1.Invalidate();
        }

        private void tbalignsamespace_Click(object sender, EventArgs e)
        {
            rois.srois.alignsamegap(pictureBox1, dcamera.himg, himgbak, iw, ih);
            showroidata();
            //pictureBox1.Invalidate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            rois.copy(himgbak);
            pictureBox1.Invalidate();
        }

        //删除按钮
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            foreach(roishape rs in rois.srois.rois) mrois.delete(rs);
            rois.delete(pictureBox1, dcamera.himg, himgbak, iw,ih);
            if (rois.broi == -1) cbbase.SelectedIndex = -1;
            dgupdatemeasureall();
            pictureBox1.Invalidate();
            showroidata();
            
        }

        private void cbthway_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbthway.SelectedIndex == 0) {
                thmin.Enabled = true;
                thmax.Enabled = true;
            }
            if (cbthway.SelectedIndex == 1) {
                thmin.Value = 128;
                thmax.Value = 255;
                thmin.Enabled = false;
                thmax.Enabled = false;
            }
            if (cbthway.SelectedIndex == 2) {
                thmin.Value = 2;
                thmax.Value = 128;
                thmin.Enabled = false;
                thmax.Enabled = false;
            }
            if (!cbthway.Focused) return;
            drawregion();
        }

        private void cbpoint_Click(object sender, EventArgs e)
        {
            drawregion();
        }

        private void trowmin_TextChanged(object sender, EventArgs e)
        {
            if ((sender as TextBox).Focused == false) return;
            if (trowmin.Text.CompareTo(trowmax.Text)>0) trowmax.Text = trowmin.Text;
            if (tcolmin.Text.CompareTo(tcolmax.Text) > 0) tcolmax.Text = tcolmin.Text;
            if (twidthmin.Text.CompareTo(twidthmax.Text) > 0) twidthmax.Text = twidthmin.Text;
            if (theightmin.Text.CompareTo(theightmax.Text) > 0) theightmax.Text = theightmin.Text;
            if (tareamin.Text.CompareTo(tareamax.Text) > 0) tareamax.Text = tareamin.Text;
            drawregion();
        }

        private void ckareamax_Click(object sender, EventArgs e)
        {
            ckcombine.Checked = false;
            drawregion();
        }

        private void cbbase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cbbase.Focused) return;
            if (cbbase.SelectedIndex <= 0)
            {
                rois.broi = 0; rois.brow = 0; rois.bcol = 0;
            }
            else {
                int num = int.Parse(cbbase.Text);
                rois.broi = num;
                rois.getbasepoint();
            }
        }

        private void cbpoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cbpoint.Focused) return;
            drawregion();
        }

        private void frmmain_Resize(object sender, EventArgs e)
        {
            btnroi.Left = this.Width - 24;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //测量结果
            lbpass.Visible = true;
            lbng.Visible = false;
            truntime.Text = "";
            vcommon.qty = 0;
            vcommon.qtypass = 0;
            vcommon.qtyng = 0;
            vcommon.showstatistic();
            //测量内容
            rtresult.Clear();
            rtresult.AppendText("序号\t检测项\t检测内容\t标准值\t下公差\t上公差\t实测\t结果\r\n");
            rtresult.AppendText("====\t======\t============\t======\t======\t======\t======\t====\r\n");
            //视图显示
            //pictureBox1.Image = dcamera.himg.ToBitmap();
            //#hwin.HalconWindow.DispImage(dcamera.himg);
            rois.text1 = "";
            rois.text2 = "Data...";
            //rois.showtext(pictureBox1);
            //清空统计数据
            rtdatastatistic.Clear();
            foreach (clsmeasure cm in mrois.ilist)
            {
                cm.mnums = 0;
                cm.mngslarge = 0;
                cm.mngssmall = 0;
            }
            pictureBox1.Invalidate();
        }

        private void dgview_DoubleClick(object sender, EventArgs e)
        {
            //MessageBox.Show(dgview.SelectedRows[0].Index.ToString());
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            frmoffset foff = new frmoffset();
            foff.ShowDialog();
        }

        private void tmname_Leave(object sender, EventArgs e)
        {
            string tname = tmname.Text.Trim();
            //string tn = tmname.Text;
            //int id = tmname.SelectedIndex;
            //if (id <= 9) {
            //    tmname.Text = tempCombName;
            //}

            if (tmname.FindString(tname) < 0) tmname.Items.Add(tname);

        }

        private void tmname_KeyUp(object sender, KeyEventArgs e)
        {
            return;
            //if (e.KeyData == Keys.Delete) {
            //    string tn = tmname.Text;
            //    int tnindex = tmname.Items.IndexOf(tn);
            //    if (tnindex > 0) tmname.Items.RemoveAt(tnindex);
            //}
        }

        private void tmname_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.Delete || (e.KeyData == Keys.Back)) {
            //    string tn = tmname.Text;
            //    if (tn.Equals("W 值") || tn.Equals("S 值") || tn.Equals("E 值") || tn.Equals("F 值") || tn.Equals("P 值") || tn.Equals("P0 值") || tn.Equals("P2 值") || tn.Equals("D0 值") || tn.Equals("A0 值") || tn.Equals("B0 值")) {
            //        tmname.Text = tn;
            //        return;
            //    }
            //}
            //tmname.SelectedItem
             
            if (e.KeyData == Keys.Delete) {
                
                string tn = tmname.Text;
                int id = tmname.Items.IndexOf(tn);
                if (id > 9)
                {
                    tmname.Items.RemoveAt(id);
                }
                else {
                    e.Handled = true;
                }
            }
        }

        private void tmname_KeyPress(object sender, KeyPressEventArgs e)
        {
            int idx = tmname.SelectedIndex;
            Console.WriteLine(idx);
            if (idx <= 9&&idx>=0)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void loadmeasurename() {
            StreamReader sr = new StreamReader(Application.StartupPath + "\\mname.dat");
            string rstr = sr.ReadToEnd();
            sr.Close();
            string[] nstr = rstr.Split(',');
            tmname.Items.Clear();
            foreach (string str in nstr) tmname.Items.Add(str);
            tmname.SelectedIndex = 0;
            cbmtype.SelectedIndex = 0;
        }
        private void savemeasurename(){
            StreamWriter sw = new StreamWriter(Application.StartupPath + "\\mname.dat");
            string[] sstr = new string[tmname.Items.Count];
            tmname.Items.CopyTo(sstr,0);
            string rstr = string.Join(",", sstr);
            
            sw.WriteLine(rstr);
            sw.Flush();
            sw.Close();
        }

        private void tbexit_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch { }
        }

        private void trowmin_KeyUp(object sender, KeyEventArgs e)
        {
            //if ((sender as TextBox).Text.Length == 0) (sender as TextBox).Text = "0"; 
        }

        private void trowmax_KeyUp(object sender, KeyEventArgs e)
        {
            //if ((sender as TextBox).Text.Length == 0) (sender as TextBox).Text = "99999"; 
        }
        //================相机设置

        private void savescreen() {
            double stime1 = Environment.TickCount;


            


            string fpath = fprint.folder;
            if (fpath == "") return;
            if (!Directory.Exists(fpath)) Directory.CreateDirectory(fpath);
            
            //int btime = Environment.TickCount;
            if(pview.Width==0 || pview.Height==0) return;
            //HImage timg = new HImage("byte", pview.Width, pview.Height);
            //timg = hwin.HalconWindow.DumpWindowImage();
            //timg.WriteImage("bmp", 0, "tmp.bmp");
            //timg.Dispose();
            Bitmap bmp = new Bitmap(pview.Width, pview.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //绘制控件
            if (this.pview.InvokeRequired)
            {
                pview.Invoke(new MethodInvoker(
                   delegate ()
                   {
                       pview.DrawToBitmap(bmp, pview.ClientRectangle);
                   }));
            }
            else
            {
                pview.DrawToBitmap(bmp, pview.ClientRectangle);
            }
            
            //绘制测量图形
            //Cv2.ImWrite(".\\tmp.bmp", dcamera.himg);
            Bitmap ytbmp = new Bitmap(phwin.Width, phwin.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            bool ts3 = false;
            //绘制控件
            if (this.phwin.InvokeRequired)
            {
                phwin.Invoke(new MethodInvoker(
                   delegate ()
                   {
                       phwin.DrawToBitmap(ytbmp, pview.ClientRectangle);
                   }));
            }
            else
            {
                phwin.DrawToBitmap(ytbmp, pview.ClientRectangle);
            }
            if (this.tabControl1.InvokeRequired)
            {
                tabControl1.Invoke(new MethodInvoker(
                   delegate ()
                   {
                       if (tabControl1.SelectedIndex == 3)
                           ts3 = true;
                   }));
            }
            else
            {
                if (tabControl1.SelectedIndex == 3)
                    ts3 = true;
            }

            Bitmap tbmp = ytbmp;// new Bitmap(".\\tmp.bmp");
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(tbmp, 0, 0, phwin.Width, phwin.Height);
            //绘制测量结果输出richtextbox内容
            if (ts3)
            {
                Rectangle rect = new Rectangle();
                if (this.rtresult.InvokeRequired)
                {
                    rtresult.Invoke(new MethodInvoker(
                       delegate ()
                       {
                           rect = rtresult.RectangleToScreen(rtresult.ClientRectangle);
                       }));
                }
                else
                {
                    rect = rtresult.RectangleToScreen(rtresult.ClientRectangle);
                }
                if (this.pview.InvokeRequired)
                {
                    pview.Invoke(new MethodInvoker(
                       delegate ()
                       {
                           rect = pview.RectangleToClient(rect);
                       }));
                }
                else
                {
                    rect = pview.RectangleToClient(rect);
                }
                
                Graphics gr = rtresult.CreateGraphics();
                IntPtr dcg = g.GetHdc();
                IntPtr dcgr = gr.GetHdc();
                BitBlt(dcg, rect.Left, rect.Top, rect.Width, rect.Height, dcgr, 0, 0, 0x00CC0020);//srccopy
                g.ReleaseHdc(dcg);
                gr.ReleaseHdc(dcgr);
                gr.Dispose();
            }

            double etime1 = Environment.TickCount;
            Console.WriteLine("save screen cost time ：{0}", etime1 - stime1);

            bmp.Save(fpath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssff") + ".jpg");
            bmp.Dispose();
            tbmp.Dispose();
            g.Dispose();
            //int etime = Environment.TickCount;
            //tcolmin.Text = (etime - btime).ToString();

          
        }

        private void tscreen_Tick()
        {
            if (isCsvTitleUpdated)
            {
                if (sw_csv != null)
                    sw_csv.Close();
                if (fs_csv != null)
                    fs_csv.Close();
                initcsv();
                data = "时间";
                foreach (clsmeasure cm in mrois.ilist)
                {
                    data += ",";
                    data += cm.mname;
                }
                sw_csv.WriteLine(data);
                isCsvTitleUpdated = false;
            }
            data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            foreach (clsmeasure cm in mrois.ilist)
            {
                data += ",";
                data += cm.mvalue.ToString("f3");
            }
            sw_csv.WriteLine(data);
            //tscreen.Enabled = false;
            if (testresult=="NG") savescreen();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            fprint.photosview();
        }

        private void measure_log_Click(object sender, EventArgs e) {
            string fpath = "";
            if (vcommon.filepath == "") {
                fpath = Application.StartupPath + "\\csv";
                if (!Directory.Exists(fpath)) Directory.CreateDirectory(fpath);
            }
            else
            {
                fpath = vcommon.filepath;
            }
            System.Diagnostics.Process.Start("explorer.exe", fpath);
        }

        private void tb_move_Click(object sender, EventArgs e)
        {
            tb_magic.Checked = false;
            tb_move.Checked = !(tb_move.Checked);
            if (tb_move.Checked)
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (tbrun.Checked || tbcameraplay.Checked) return;
            //if (e.Delta > 0) toolStripButton6_Click(null, null);
            //else toolStripButton5_Click(null, null);
            if (e.Delta > 0) {
                zoomImageWithMouse(sender, e, 0.9);
            }
            else
            {
                zoomImageWithMouse(sender, e, 1.1);
            }
        }

        private void thminsurface_ValueChanged(object sender, EventArgs e)
        {
            
        }

        public void drawWhiteRegion() {

            
            foreach (roishape croi in rois.srois.rois)
            {
                //赋值
                croi.surfacecheck = cksurface.Checked;
                croi.surfacemaxcheck = cksurfaceareamax.Checked;
                croi.thminsurface = thminsurface.Value;
                croi.thmaxsurface = thmaxsurface.Value;
                //croi.grayThresh = bargraythresh.Value;
                //croi.stdsurface = bararea.Value * 1.0 / 100.0;
                
                croi.getWhiteMask(dcamera.himg, himgbak);
            }
            
            pictureBox1.Invalidate(); 
        }
         

        public void drawBlackRegion()
        {
            //template.CopyTo(himgbak);
            //template.CopyTo(dcamera.himg);
            foreach (roishape croi in rois.srois.rois)
            {
                //赋值
                croi.surfacecheck = cksurface.Checked;
                croi.surfacemaxcheck = cksurfaceareamax.Checked;
                croi.thminsurface = thminsurface.Value;
                croi.thmaxsurface = thmaxsurface.Value;
                //croi.grayThresh = bargraythresh.Value;
                //croi.stdsurface = bararea.Value * 1.0 / 100.0;
                
                croi.getBlackMask(dcamera.himg, himgbak);
            }
            pictureBox1.Invalidate();
        }


        private void stdsurface_ValueChanged(object sender, EventArgs e)
        {

        }
        private void cksurface_Click(object sender, EventArgs e)
        {
            ckshowsurface.Checked = cksurface.Checked;

            foreach (roishape croi in rois.srois.rois)
            {
                croi.surfacecheck = cksurface.Checked;
            }
            //drawsurface();
            //if (!cksurface.Checked) showroidata();
        }

        private void showsurface() {
            if (rois.croi == null) return;
            if (rois.srois.count == 0) return;
            
            foreach (roishape croi in rois.rois)
            {
                if (!croi.surfacecheck) continue;
                croi.measuresuface(dcamera.himg,himgbak, true,true);
            }
            pictureBox1.Invalidate();
        }

        private void drawsurface()
        {
            //if (rois.croi == null) return;
            //if (rois.srois.count == 0) return;
            
            foreach (roishape croi in rois.rois)
            {
                if (!croi.surfacecheck) continue;
                //赋值
                croi.surfacecheck = cksurface.Checked;
                croi.surfacemaxcheck = cksurfaceareamax.Checked;
                croi.thminsurface = thminsurface.Value;
                croi.thmaxsurface = thmaxsurface.Value; 
                croi.grayThresh = bargraythresh.Value;
                croi.minDefectArea = bararea.Value;
                croi.minDefectWidth = barwidth.Value;
                croi.minDefectHeight = barheight.Value;
                //croi.stdsurface = bararea.Value * 1.0 / 100.0;
                croi.measuresuface(dcamera.himg,himgbak, true,true);
            }
            pictureBox1.Invalidate();
        }

        private void ckshowsurface_Click(object sender, EventArgs e)
        {
            if (!cksurface.Checked) return;
            if (ckshowsurface.Checked) drawsurface();
            else showroidata();
        }

        private void cksurfaceareamax_Click(object sender, EventArgs e)
        {
            if (!cksurface.Checked) return;
            drawsurface();
        }

        private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void btnpixelunit_Click(object sender, EventArgs e)
        {
            //自动校准
            string mcon = "";
            int i = 0;
            foreach (clsmeasure cs in mrois.ilist)
            {
                if (cs.mresult.Equals("NG"))
                {
                    double std = cs.mstd;
                    double realv = cs.mvalue-cs.moffset;
                    cs.moffset = std - realv;
                } 
                //dgview.Rows.Add();
                //i = dgview.Rows.Count - 1;
                //if (cs.roi1 == null) mcon = "";
                //else mcon = cs.roi1.num.ToString("d3");
                //if (cs.roi2 == null) mcon += "> ";
                //else mcon += ">" + cs.roi2.num.ToString("d3");
                //dgview[0, i].Value = cs.mname;
                //dgview[1, i].Value = mcon + " " + cs.mtypename();
                //dgview[2, i].Value = cs.mstd;
                //dgview[3, i].Value = cs.mllimit;
                //dgview[4, i].Value = cs.mulimit;

            }
            dgupdatemeasureall();
            ////自动计算像素比例
            //double pvx = dcamera.xpixel;
            //double pvy = dcamera.ypixel;
            //if (pvx == 0) pvx = 1;
            //if (pvy == 0) pvy = 1;
            //double pvxsum = 0.0, pvysum = 0.0;
            //int pvxnum = 0, pvynum = 0;
            //foreach (clsmeasure cm in mrois.ilist)
            //{
            //    if (cm.mvalue == 0) cm.mvalue = 1;
            //    if (cm.mtype == 0)
            //    {
            //        pvxsum += cm.mstd / (cm.mvalue / pvx);
            //        pvxnum++;
            //    }
            //    if (cm.mtype == 1) {
            //        pvysum += cm.mstd / (cm.mvalue / pvy);
            //        pvynum++;
            //    }
            //}

            //if (pvxnum > 0) pvxsum = pvxsum / pvxnum;
            //if (pvynum > 0) pvysum = pvysum / pvynum;
            //if (pvxsum == 0) pvxsum = pvysum;
            //if (pvysum == 0) pvysum = pvxsum;
            //if (pvxsum == 0) pvxsum = 1;
            //if (pvysum == 0) pvysum = 1;
            //dcamera.xpixel = Math.Abs(pvxsum);
            //dcamera.ypixel = Math.Abs(pvysum);
            //runonce(pictureBox1);
        }

        private void ckdisplayng_Click(object sender, EventArgs e)
        {
            ckdisplayng.Checked = true;
            ckdisplayall.Checked = false;
            showmeasure();
        }

        private void ckdisplayall_Click(object sender, EventArgs e)
        {
            ckdisplayall.Checked = true;
            ckdisplayng.Checked = false;
            showmeasure();
        }

        private void showmeasure() {
            //pictureBox1.Image = dcamera.himg.ToBitmap();
            //#hwin.HalconWindow.DispImage(dcamera.himg);
            rtresult.Clear();
            rtresult.AppendText("序号\t检测项\t检测内容\t标准值\t下限\t上限\t实测\t结果\r\n");
            rtresult.AppendText("====\t======\t============\t======\t======\t======\t======\t====\r\n");
            int i = 1;
            string mcon = "";
            foreach (clsmeasure cm in mrois.ilist)
            {
                //显示区域
                if (ckdisplayall.Checked)
                {
                    foreach (roishape rs in rois.rois)
                    {
                        rois.showresult(iw,ih,rs);
                    }
                }
                if (ckdisplayng.Checked && cm.mresult == "NG")
                {
                    rois.showresult(iw,ih,cm.roi1);
                    if (cm.mtype < 3 || (cm.mtype == 3 && cm.roi2 != null)) rois.showresult(iw,ih,cm.roi2);
                }
                //输出结果
                if (cm.mresult == "NG") rtresult.SelectionColor = Color.Red;
                else rtresult.SelectionColor = Color.Black;
                if (cm.roi1 == null) mcon = "";
                else mcon = cm.roi1.num.ToString("d3");
                if (cm.roi2 == null) mcon += "> ";
                else mcon += ">" + cm.roi2.num.ToString("d3");
                mcon += cm.mtypename();

                if (ckdisplayall.Checked || (ckdisplayng.Checked && cm.mresult == "NG"))
                    rtresult.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\r\n", i, cm.mname, mcon, cm.mstd, cm.mllimit, cm.mulimit, cm.mvalue.ToString("f3"), cm.mresult));
                i++;
            }
            //表面检测
            foreach (roishape rs in rois.rois)
            {
                if (rs.surfacecheck)
                {
                    //#bool areack = rs.measuresuface(hwin.HalconWindow, dcamera.himg, false,true);
                    //#if (areack == false)
                    //#{
                    //#    mcon = rs.num.ToString("d3");
                    //#    rtresult.SelectionColor = Color.Red;
                    //#    rtresult.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\r\n", i, "表面检测", mcon, "    ", rs.stdsurface + "%", "   ", rs.msurface.ToString("f2") + "%", "NG"));
                    //#    i++;
                    //#}
                }
            }
            //输出文本
            //rois.showtext(pictureBox1);
            //显示原点
            //hwin.HalconWindow.SetColor("red");
            //hwin.HalconWindow.DispCross(0.0, 0.0, 100.0, 0.0);
        }

        private void ckcombine_Click(object sender, EventArgs e)
        {
            ckareamax.Checked = false;
            drawregion();
        }

        private void ckdataall_Click(object sender, EventArgs e)
        {
            ckdataall.Checked = true;
            ckdatang.Checked = false;
            showdatastatistic();
        }

        private void ckdatang_Click(object sender, EventArgs e)
        {
            ckdatang.Checked = true;
            ckdataall.Checked = false;
            showdatastatistic();
        }

        private void showdatastatistic(){
            //显示测量数据统计结果
            rtdatastatistic.Clear();
            rtdatastatistic.AppendText("序号\t检测项\t检测内容\t检测数\tNG数\tNG偏大\tNG偏小\r\n");
            rtdatastatistic.AppendText("====\t======\t============\t======\t======\t======\t======\r\n");
            int i = 1;
            string mcon = "";
            foreach (clsmeasure cm in mrois.ilist)
            {
                if (ckdatang.Checked && (cm.mngssmall + cm.mngslarge) == 0) continue;
                if (cm.roi1 == null) mcon = "";
                else mcon = cm.roi1.num.ToString("d3");
                if (cm.roi2 == null) mcon += "> ";
                else mcon += ">" + cm.roi2.num.ToString("d3");
                mcon += cm.mtypename();
                rtdatastatistic.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t", i, cm.mname, mcon, cm.mnums, cm.mngssmall + cm.mngslarge));
                rtdatastatistic.SelectionColor = Color.Red;
                rtdatastatistic.AppendText(string.Format("{0}\t{1}\r\n", cm.mngslarge, cm.mngssmall));
                rtdatastatistic.SelectionColor = Color.Black;
                i++;
            }
        }

        private void btnstart_Click(object sender, EventArgs e)
        {
            if (fprint.factory == "" || fprint.device == "" || fprint.worker == "") {
                MessageBox.Show("请设置产品追踪参数!");
                return;
            }
            fprint.pbegin();
            btnstart.Enabled = false; 
            //button4_Click(null, null);
        }

        private void toolStripButton10_Click_1(object sender, EventArgs e)
        {
            frmlogin fs = new frmlogin();
            fs.StartPosition = FormStartPosition.CenterScreen;
            fs.ShowDialog();
        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void thminsurface_Scroll(object sender, EventArgs e)
        {

        }
 


        private void bargraythresh_ValueChanged(object sender, MouseEventArgs e)
        {
            if (!bargraythresh.Focused) return;
            if (!ckshowsurface.Checked) ckshowsurface.Checked = true;
            //if (thmaxsurface.Value < thminsurface.Value) thmaxsurface.Value = thminsurface.Value;
            tbgraythresh.Text = bargraythresh.Value.ToString();
            if (thminsurface.Value == 0 && thmaxsurface.Value == 0) {
                MessageBox.Show("请先选择亮/暗区域");
                return;
            }
            foreach (roishape croi in rois.rois)
            {
                if (!croi.surfacecheck) continue;
                //赋值
                //croi.surfacecheck = cksurface.Checked;
                //croi.surfacemaxcheck = cksurfaceareamax.Checked;
                croi.thminsurface = thminsurface.Value;
                croi.thmaxsurface = thmaxsurface.Value;
                croi.grayThresh = bargraythresh.Value;
                //croi.stdsurface = bararea.Value * 1.0 / 100.0;
                croi.minDefectArea = bararea.Value;
                croi.minDefectHeight = barheight.Value;
                croi.minDefectWidth = barwidth.Value;
                //croi.getWhiteMask(dcamera.himg, himgbak);
            }
            MvApi.CameraSoftTrigger(m_hCamera);
            //pictureBox1.Invalidate();
        }

        private void bararea_ValueChanged(object sender, MouseEventArgs e)
        {
            if (!bararea.Focused) return;
            if (!ckshowsurface.Checked) ckshowsurface.Checked = true;
            //if (thmaxsurface.Value < thminsurface.Value) thmaxsurface.Value = thminsurface.Value;
            tbarea.Text = bararea.Value.ToString();
            if (thminsurface.Value == 0 && thmaxsurface.Value == 0)
            {
                MessageBox.Show("请先选择亮/暗区域");
                return;
            }
            foreach (roishape croi in rois.srois.rois)
            {
                if (!croi.surfacecheck) continue;
                //赋值
                //croi.surfacecheck = cksurface.Checked;
                //croi.surfacemaxcheck = cksurfaceareamax.Checked;
                croi.thminsurface = thminsurface.Value;
                croi.thmaxsurface = thmaxsurface.Value;
                croi.grayThresh = bargraythresh.Value;
                //croi.stdsurface = bararea.Value * 1.0 / 100.0;
                croi.minDefectArea = bararea.Value;
                croi.minDefectHeight = barheight.Value;
                croi.minDefectWidth = barwidth.Value;

                //croi.getWhiteMask(dcamera.himg, himgbak);
            }
            //MvApi.CameraSoftTrigger(m_hCamera);
            //pictureBox1.Refresh();
        }

        private void thminsurface_ValueChanged(object sender, MouseEventArgs e)
        {
            //if (!thminsurface.Focused) return;
            if (!ckshowsurface.Checked) ckshowsurface.Checked = true;
            //if (thmaxsurface.Value < thminsurface.Value) thmaxsurface.Value = thminsurface.Value;
            tthminsurface.Text = thminsurface.Value.ToString();
            drawWhiteRegion();
            //drawsurface();
        }

        private void thmaxsurface_MouseUp(object sender, MouseEventArgs e)
        {
            
            if (!thmaxsurface.Focused) return;
            if (!ckshowsurface.Checked) ckshowsurface.Checked = true;
            //if (thmaxsurface.Value < thminsurface.Value) thminsurface.Value = thmaxsurface.Value;
            tthmaxsurface.Text = thmaxsurface.Value.ToString();
            drawBlackRegion();
            //drawsurface();
        }

        private void barshrink_ValueChanged(object sender, EventArgs e)
        {
            tbshrink.Text = barshrink.Value.ToString();
            foreach (roishape cr in rois.srois.rois) {
                cr.shrinkPixel = (double)barshrink.Value;
            }
        }

        

        

        private void barwidth_ValueChanged(object sender, MouseEventArgs e)
        {
            if (!barwidth.Focused) return;
            if (!ckshowsurface.Checked) ckshowsurface.Checked = true;
            //if (thmaxsurface.Value < thminsurface.Value) thmaxsurface.Value = thminsurface.Value;
            tbwidth.Text = barwidth.Value.ToString();
            if (thminsurface.Value == 0 && thmaxsurface.Value == 0)
            {
                MessageBox.Show("请先选择亮/暗区域");
                return;
            }
            foreach (roishape croi in rois.srois.rois)
            {
                if (!croi.surfacecheck) continue;
                //赋值
                //croi.surfacecheck = cksurface.Checked;
                //croi.surfacemaxcheck = cksurfaceareamax.Checked;
                croi.thminsurface = thminsurface.Value;
                croi.thmaxsurface = thmaxsurface.Value;
                croi.grayThresh = bargraythresh.Value;
                //croi.stdsurface = bararea.Value * 1.0 / 100.0;
                croi.minDefectArea = bararea.Value;
                croi.minDefectHeight = barheight.Value;
                croi.minDefectWidth = barwidth.Value;

                //croi.getWhiteMask(dcamera.himg, himgbak);
            }
            pictureBox1.Invalidate();
        }

        private void barheight_ValueChanged(object sender, MouseEventArgs e)
        {
            if (!barheight.Focused) return;
            if (!ckshowsurface.Checked) ckshowsurface.Checked = true;
            //if (thmaxsurface.Value < thminsurface.Value) thmaxsurface.Value = thminsurface.Value;
            tbheight.Text = barheight.Value.ToString();
            if (thminsurface.Value == 0 && thmaxsurface.Value == 0)
            {
                MessageBox.Show("请先选择亮/暗区域");
                return;
            }
            foreach (roishape croi in rois.srois.rois)
            {
                if (!croi.surfacecheck) continue;
                //赋值
                //croi.surfacecheck = cksurface.Checked;
                //croi.surfacemaxcheck = cksurfaceareamax.Checked;
                croi.thminsurface = thminsurface.Value;
                croi.thmaxsurface = thmaxsurface.Value;
                croi.grayThresh = bargraythresh.Value;
                //croi.stdsurface = bararea.Value * 1.0 / 100.0;
                croi.minDefectArea = bararea.Value;
                croi.minDefectHeight = barheight.Value;
                croi.minDefectWidth = barwidth.Value;

                //croi.getWhiteMask(dcamera.himg, himgbak);
            }
            MvApi.CameraSoftTrigger(m_hCamera);
            pictureBox1.Invalidate();
        }


         

        private void btnend_Click(object sender, EventArgs e)
        {
            try { fprint.pend(); }
            catch { }
            btnstart.Enabled = true;
            if (sw_csv != null)
                sw_csv.Close();
            if (fs_csv != null)
                fs_csv.Close();
            isCsvTitleUpdated = true;
        }

        private void btnprint_Click(object sender, EventArgs e)
        {
            fprint.Owner = this;
            fprint.Show();
        }

        private void frmmain_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void ckregion_Click(object sender, EventArgs e)
        {
            showmeasure();
        }

        //int county = 0;
        //int county1 = 0;
        public void ImageCaptureCallback(CameraHandle hCamera, IntPtr pFrameBuffer, ref tSdkFrameHead pFrameHead, IntPtr pContext)
        {
            //county += 1;
            //Console.WriteLine("in callback:{0}",county);
            isRunOrRunOnceChecked = true;
            if (tbcameraplay.Checked)
            {
                //图像处理，将原始输出转换为RGB格式的位图数据，同时叠加白平衡、饱和度、LUT等ISP处理。
                MvApi.CameraImageProcess(hCamera, pFrameBuffer, m_ImageBuffer, ref pFrameHead);
                //叠加十字线、自动曝光窗口、白平衡窗口信息(仅叠加设置为可见状态的)。   
                MvApi.CameraImageOverlay(hCamera, m_ImageBuffer, ref pFrameHead);
                //调用SDK封装好的接口，显示预览图像
                MvApi.CameraDisplayRGB24(hCamera, m_ImageBuffer, ref pFrameHead);
                m_iDisplayedFrames++;
                //Console.WriteLine("m_iDisplayedFrames:", m_iDisplayedFrames);
                if (pFrameHead.iWidth != m_tFrameHead.iWidth || pFrameHead.iHeight != m_tFrameHead.iHeight)
                {
                    m_bEraseBk = true;
                    m_tFrameHead = pFrameHead;
                }
                isRunOrRunOnceChecked = false;
                return;
            }
            if (isSaveToTemplate)
            {   //保存为模板
                isSaveToTemplate = false;
                //图像处理，将原始输出转换为RGB格式的位图数据，同时叠加白平衡、饱和度、LUT等ISP处理。
                MvApi.CameraImageProcess(hCamera, pFrameBuffer, m_ImageBuffer, ref pFrameHead);
                //叠加十字线、自动曝光窗口、白平衡窗口信息(仅叠加设置为可见状态的)。   
                MvApi.CameraImageOverlay(hCamera, m_ImageBuffer, ref pFrameHead);
                ////调用SDK封装好的接口，显示预览图像
                //MvApi.CameraDisplayRGB24(hCamera, m_ImageBuffer, ref pFrameHead);

                //此时，uRawBuffer指向了相机原始数据的缓冲区地址，默认情况下为8bit位宽的Bayer格式，如果
                //您需要解析bayer数据，此时就可以直接处理了，后续的操作演示了如何将原始数据转换为RGB格式
                //并显示在窗口上。

                ////将相机输出的原始数据转换为RGB格式到内存m_ImageBufferSnapshot中
                //MvApi.CameraImageProcess(m_hCamera, pFrameBuffer, m_ImageBuffer, ref tFrameHead);
                ////CameraSnapToBuffer成功调用后必须用CameraReleaseImageBuffer释放SDK中分配的RAW数据缓冲区
                ////否则，将造成死锁现象，预览通道和抓拍通道会被一直阻塞，直到调用CameraReleaseImageBuffer释放后解锁。
                //MvApi.CameraReleaseImageBuffer(m_hCamera, pFrameBuffer);
                ////更新抓拍显示窗口。

                
                dcamera.himg = new Mat(pFrameHead.iHeight, pFrameHead.iWidth, MatType.CV_8UC1, m_ImageBuffer);//.CvtColor(ColorConversionCodes.RGB2BGR);
                                                                                                                      //Cv2.ImWrite(".\\himgrgb.bmp",dcamera.himg);
                                                                                                                      //Cv2.ImWrite(".\\himgbgr.bmp", dcamera.himg.CvtColor(ColorConversionCodes.RGB2BGR)); 
                Cv2.Flip(dcamera.himg, dcamera.himg, FlipMode.X);
                if (dcamera.himg.Type().Channels == 1)
                {
                    Cv2.CvtColor(dcamera.himg, dcamera.himg, ColorConversionCodes.GRAY2BGR);
                }
                //himgbak = dcamera.getBackImg();
                //himgbak = dcamera.getBackImg();
                //himgbak.CopyTo(template);
                //himgbak = dcamera.getBackImg();
                //himgbak.CopyTo(template);

                //pictureBox1.Invalidate();
                isRunOrRunOnceChecked = false;
                return;
            }
            if (tbrun.Checked&& isTriggered) {// 程序为run状态
                isTriggered = false;
                //county1 += 1;
                //Console.WriteLine("in triggered:{0}",county1);
                //double stime = Environment.TickCount;
                //图像处理，将原始输出转换为RGB格式的位图数据，同时叠加白平衡、饱和度、LUT等ISP处理。
                MvApi.CameraImageProcess(hCamera, pFrameBuffer, m_ImageBuffer, ref pFrameHead);
                //叠加十字线、自动曝光窗口、白平衡窗口信息(仅叠加设置为可见状态的)。   
                MvApi.CameraImageOverlay(hCamera, m_ImageBuffer, ref pFrameHead);

                dcamera.himg = new Mat(pFrameHead.iHeight, pFrameHead.iWidth, MatType.CV_8UC1, m_ImageBuffer);
                Cv2.Flip(dcamera.himg, dcamera.himg, FlipMode.X);
                if (dcamera.himg.Type().Channels == 1)
                {
                    Cv2.CvtColor(dcamera.himg, dcamera.himg, ColorConversionCodes.GRAY2BGR);
                }
                himgbak = dcamera.getBackImg();
                runonce(pictureBox1);
                //double etime = Environment.TickCount;
                //Console.WriteLine("solution1 cost time：{0}", etime - stime);
            }
            else // 程序为调试状态，调整缺陷大小过滤条件（暂与上run状态逻辑相同）
            {
                MvApi.CameraImageProcess(hCamera, pFrameBuffer, m_ImageBuffer, ref pFrameHead);
                //叠加十字线、自动曝光窗口、白平衡窗口信息(仅叠加设置为可见状态的)。   
                MvApi.CameraImageOverlay(hCamera, m_ImageBuffer, ref pFrameHead);

                dcamera.himg = new Mat(pFrameHead.iHeight, pFrameHead.iWidth, MatType.CV_8UC1, m_ImageBuffer);
                Cv2.Flip(dcamera.himg, dcamera.himg, FlipMode.X);
                if (dcamera.himg.Type().Channels == 1)
                {
                    Cv2.CvtColor(dcamera.himg, dcamera.himg, ColorConversionCodes.GRAY2BGR);
                }
                himgbak = dcamera.getBackImg();
                runonce(pictureBox1);
                //double etime = Environment.TickCount;
                //Console.WriteLine("solution1 cost time：{0}", etime - stime);
            }
            isRunOrRunOnceChecked = false;
        }


        private bool InitCamera()
        {
            CameraSdkStatus status;
            tSdkCameraDevInfo[] tCameraDevInfoList;
            IntPtr ptr;
            int i;
            CAMERA_SNAP_PROC pCaptureCallOld = null;
            if (m_hCamera > 0)
            {
                //已经初始化过，直接返回 true

                return true;
            }

            status = MvApi.CameraEnumerateDevice(out tCameraDevInfoList);
            if (status == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            {
                if (tCameraDevInfoList != null)//此时iCameraCounts返回了实际连接的相机个数。如果大于1，则初始化第一个相机
                {
                    status = MvApi.CameraInit(ref tCameraDevInfoList[0], -1, -1, ref m_hCamera);
                    if (status == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
                    {
                        //获得相机特性描述
                        MvApi.CameraGetCapability(m_hCamera, out tCameraCapability);

                        m_ImageBuffer = Marshal.AllocHGlobal(tCameraCapability.sResolutionRange.iWidthMax * tCameraCapability.sResolutionRange.iHeightMax * 3 + 1024);
                        m_ImageBufferSnapshot = Marshal.AllocHGlobal(tCameraCapability.sResolutionRange.iWidthMax * tCameraCapability.sResolutionRange.iHeightMax * 3 + 1024);

                        if (tCameraCapability.sIspCapacity.bMonoSensor != 0)
                        {
                            // 黑白相机输出8位灰度数据
                            MvApi.CameraSetIspOutFormat(m_hCamera, (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8);
                        }

                        ////初始化显示模块，使用SDK内部封装好的显示接口
                        MvApi.CameraDisplayInit(m_hCamera, pictureBox1.Handle);
                        MvApi.CameraSetDisplaySize(m_hCamera, Convert.ToInt32(phwin.Width), Convert.ToInt32(phwin.Height));
                        zscale = (float)phwin.Width / (float)tCameraCapability.sResolutionRange.iWidthMax;
                        //Console.WriteLine("pb.width:{0}", pictureBox1.Width);
                        //Console.WriteLine("dcamera.himg.Width*zscale:{0}", dcamera.himg.Width*zscale);
                        MvApi.CameraSetDisplaySize(m_hCamera, Convert.ToInt32(tCameraCapability.sResolutionRange.iWidthMax * zscale), Convert.ToInt32(tCameraCapability.sResolutionRange.iHeightMax * zscale));
                        //MvApi.CameraSetDisplayOffset(m_hCamera, imgx, imgy);

                        //MvApi.CameraSetDisplayMode

                        //设置抓拍通道的分辨率。
                        tSdkImageResolution tResolution;
                        tResolution.uSkipMode = 0;
                        tResolution.uBinAverageMode = 0;
                        tResolution.uBinSumMode = 0;
                        tResolution.uResampleMask = 0;
                        tResolution.iVOffsetFOV = 0;
                        tResolution.iHOffsetFOV = 0;
                        tResolution.iWidthFOV = tCameraCapability.sResolutionRange.iWidthMax;
                        tResolution.iHeightFOV = tCameraCapability.sResolutionRange.iHeightMax;
                        tResolution.iWidth = tResolution.iWidthFOV;
                        tResolution.iHeight = tResolution.iHeightFOV;
                        //tResolution.iIndex = 0xff;表示自定义分辨率,如果tResolution.iWidth和tResolution.iHeight
                        //定义为0，则表示跟随预览通道的分辨率进行抓拍。抓拍通道的分辨率可以动态更改。
                        //本例中将抓拍分辨率固定为最大分辨率。
                        tResolution.iIndex = 0xff;
                        tResolution.acDescription = new byte[32];//描述信息可以不设置
                        tResolution.iWidthZoomHd = 0;
                        tResolution.iHeightZoomHd = 0;
                        tResolution.iWidthZoomSw = 0;
                        tResolution.iHeightZoomSw = 0;

                        MvApi.CameraSetResolutionForSnap(m_hCamera, ref tResolution);

                        //让SDK来根据相机的型号动态创建该相机的配置窗口。
                        MvApi.CameraCreateSettingPage(m_hCamera, this.Handle, tCameraDevInfoList[0].acFriendlyName,/*SettingPageMsgCalBack*/null,/*m_iSettingPageMsgCallbackCtx*/(IntPtr)null, 0);

                        //两种方式来获得预览图像，设置回调函数或者使用定时器或者独立线程的方式，
                        //主动调用CameraGetImageBuffer接口来抓图。
                        //本例中仅演示了两种的方式,注意，两种方式也可以同时使用，但是在回调函数中，
                        //不要使用CameraGetImageBuffer，否则会造成死锁现象。

                        m_CaptureCallback = new CAMERA_SNAP_PROC(ImageCaptureCallback);
                        MvApi.CameraSetCallbackFunction(m_hCamera, m_CaptureCallback, m_iCaptureCallbackCtx, ref pCaptureCallOld);
 
                        //MvApi.CameraReadSN 和 MvApi.CameraWriteSN 用于从相机中读写用户自定义的序列号或者其他数据，32个字节
                        //MvApi.CameraSaveUserData 和 MvApi.CameraLoadUserData用于从相机中读取自定义数据，512个字节
                        return true;

                    }
                    else
                    {
                        m_hCamera = 0;
                        //StateLabel.Text = "Camera init error";
                        slabeltips.ForeColor = Color.Red;
                        slabeltips.Text = "Camera init error";
                        String errstr = string.Format("相机初始化错误，错误码{0},错误原因是", status);
                        String errstring = MvApi.CameraGetErrorString(status);
                        // string str1 
                        MessageBox.Show(errstr + errstring, "ERROR");
                        Environment.Exit(0);
                        return false;
                    }


                }
            }
            else
            {
                MessageBox.Show("没有找到相机，如果已经接上相机，可能是权限不够，请尝试使用管理员权限运行程序。");
                Environment.Exit(0);
            }

            return false;

        }
    }//class
}//namespace
