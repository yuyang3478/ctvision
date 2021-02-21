using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;
using System.Threading;
using leanvision;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace ctmeasure
{
    public partial class frmmain : Form
    {
        //log
        public clslog log;
        //主图像
        public HImage himg;
        //视图控制: 放大, 缩小, 平移
        private Rectangle vrect;
        private double zscale = 1.0;
        private int iw = 1, ih = 1;
        private double movex,movey,movex1,movey1,motionx,motiony;
        private bool mouseleftpress = false;
        private bool mousemidpress = false;
        private bool onselect = false;
        private bool ondrawing = false;
        public string ondrawingstr;
        //放大镜视图, magicsize-范围内的图放大3倍显示
        private HWindow magicwindow;
        private int magicsize; //视图半径尺寸
        private Rectangle magicrect=new Rectangle();//视图rect, x-row,y-col,width-row1,height-col1
        private double hlrow, hlcol, hlrow1, hlcol1, vlrow, vlcol, vlrow1, vlcol1;

        //camera
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

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        public frmmain()
        {
            InitializeComponent();
        }

        private void frmmain_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + " V" + Application.ProductVersion;
            HSystem.SetSystem("help_dir", "");
            HSystem.SetSystem("do_low_error", "false");
            hwin.HalconWindow.SetColor("red");
            hwin.HalconWindow.SetLineWidth(1);
            hwin.HalconWindow.SetDraw("margin");
            mver = "lip";

            //log
            log = new clslog();
            log.write("打开软件");

            //camera
            dcamera = new clscamera();
            //IO
            dio = new clsio();

            //roi
            rois = new roimanager();
            rois.hw = hwin.HalconWindow;
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


            initview();
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

        //======================================================halwin 视图控制
        //功能： 放大，缩小，100%，全屏，初始化
        private void hwinrepaint() {
            HSystem.SetSystem("flush_graphic", "false");
            hwin.HalconWindow.ClearWindow();
            hwin.HalconWindow.SetLineStyle(new HTuple());

            //显示图像
            try
            {
                if (dcamera != null) hwin.HalconWindow.DispImage(dcamera.himg);
            }
            catch { }

            //更新roi
            if (tplay.Enabled == false)
            {
                try
                {
                    if (rois != null) rois.paintroi();
                }
                catch { }
            }

            //更新视图
            HSystem.SetSystem("flush_graphic", "true");
            hwin.HalconWindow.DispCross(0.0, 0.0, 100.0, 0.0);
        }

        private void hwin_Paint(object sender, PaintEventArgs e)
        {
            //hwinrepaint();
        }
        private void hwin_Resize(object sender, EventArgs e)
        {
            //保持比例不变
            vrect.Width = (int)(hwin.Width / zscale);
            vrect.Height = (int)(hwin.Height / zscale);
            if (vrect.Width == 0 || vrect.Height == 0) return;
            hwin.ImagePart = vrect;
            hwinrepaint();
        }

        private void initview(){//100%显示
            tabControl1.SelectedIndex = 2;
            if (dcamera.iw == 0) dcamera.iw = pictureBox1.Width;
            if (dcamera.ih == 0) dcamera.ih = pictureBox1.Height;
            iw = dcamera.iw; ih = dcamera.ih;
            
            zscale = vcommon.viewscale;
            vrect = pictureBox1.Bounds;
            hlrow = (vrect.Top + vrect.Bottom) / 2; hlcol = vrect.Left;
            hlrow1 = hlrow; hlcol1 = vrect.Right;
            vlrow = vrect.Top; vlcol = (vrect.Left + vrect.Right) / 2;
            vlrow1 = vrect.Bottom; vlcol1 = vlcol;
            zoomimageto(zscale);
            zoommove(-vcommon.viewx, -vcommon.viewy);
        }

        private void zoomall(){
            vrect.X = 0;
            vrect.Y = 0;
            if ((hwin.Width*1.0 / iw) < (hwin.Height*1.0 / ih))
            {
                zscale = hwin.Width*1.0 / iw;
                vrect.Width = iw;
                vrect.Height = (int)(hwin.Height / zscale);
            }
            else {
                zscale = hwin.Height*1.0 / ih;
                vrect.Height = ih;
                vrect.Width = (int)(hwin.Width / zscale);
            }
            vcommon.viewscale = zscale;
            vcommon.viewx = vrect.X;
            vcommon.viewy = vrect.Y;
            hwin.ImagePart = vrect;
            hwinrepaint();
            slabelzoom.Text = "zoom: " + zscale.ToString("f2");
        }

        private void zoomin() { zoomimage(1.1); }
        private void zoomout() { zoomimage(0.9); }
        //从当前比例再缩放scale比例
        private void zoomimage(double scale) {
            zscale /= scale;
            if (zscale <= 0.1) { zscale = 0.1; return; }
            if (zscale >= 10) { zscale = 10; return; }
            vcommon.viewscale = zscale;
            vrect.X = (int)(movex1 - (movex1 - vrect.X) * scale);
            vrect.Y = (int)(movey1 - (movey1 - vrect.Y) * scale);
            vrect.Width = (int)(vrect.Width * scale);
            vrect.Height = (int)(vrect.Height * scale);
            vcommon.viewx = vrect.X;
            vcommon.viewy = vrect.Y;
            hwin.ImagePart = vrect;
            hlcol = vrect.Left; hlcol1 = vrect.Right;
            vlrow = vrect.Top; vlrow1 = vrect.Bottom;
            hwinrepaint();
            slabelzoom.Text = "zoom: " + zscale.ToString("f2");
        }
        //缩放至scale比例
        private void zoomimageto(double scale)
        {
            vrect.Width = (int)(pictureBox1.Width / scale);
            vrect.Height = (int)(pictureBox1.Height / scale);
            hwin.ImagePart = vrect;
            zscale = scale;
            vcommon.viewscale = zscale;
            hlcol = vrect.Left; hlcol1 = vrect.Right;
            vlrow = vrect.Top; vlrow1 = vrect.Bottom;
            hwinrepaint();
            slabelzoom.Text = "zoom: " + zscale.ToString("f2");
        }

        private void zoommove(double dx,double dy){
            vrect = hwin.ImagePart;
            vrect.X -= (int)Math.Round(dx);
            vrect.Y -= (int)Math.Round(dy);
            hwin.ImagePart = vrect;
            vcommon.viewx = vrect.X;
            vcommon.viewy = vrect.Y;
            hlcol = vrect.Left; hlcol1 = vrect.Right;
            vlrow = vrect.Top; vlrow1 = vrect.Bottom;
            hwinrepaint();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            zoomall();
            showroidata();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            zoomout();
            if(tplay.Enabled==false) showroidata();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            zoomin();
            if(tplay.Enabled==false) showroidata();
        }

        //=======================================halwin 视图控制

        //=======================================halwin 交互操作begin
        //新建元素： 点击新建， 点击窗口并拖动， 放开鼠标完成
        //选择元素： 点击元素框内， 点击空白并拖动
        //删除元素： 选择roi， 按delete键
        private void hwin_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) mouseleftpress = true;
            movex = e.X; movey = e.Y;
            if (e.Button == MouseButtons.Middle) { tb_move.Checked = true; mousemidpress = true; return; }
            if (tbrun.Checked) return;
            if (!mouseleftpress) return;
            //检查是否选中roi
            hwinrepaint();
            rois.mousedown(e.X, e.Y);
            if (ondrawing) { rois.action = "ondraw"; return; }
            if (rois.action!="") return;

            //新建或选中roi对象中
            if (tb_move.Checked) return; 
            if (tb_magic.Checked) { showmagicwindow(e.X, e.Y); return; }
            onselect = true;
            rois.action = "onselect";
        }

        private void hwin_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (!hwin.Focused) hwin.Focus();
            movex1=e.X;movey1=e.Y;
            slabelxy.Text = string.Format("XY: {0},{1}", e.X.ToString("f0"), e.Y.ToString("f0"));

            if (!mouseleftpress&&!mousemidpress) return;
            //视图平移
            if (tb_move.Checked){
                motionx = e.X - movex;
                motiony = e.Y - movey;
                if (((int)motionx != 0) || ((int)motiony != 0))
                {
                    zoommove(motionx, motiony);
                    movex = e.X - motionx;
                    movey = e.Y - motiony;
                }
            }

            if (tbrun.Checked) return;

            //roi平移
            if (mouseleftpress && rois.action != "")
            {
                hwinrepaint();
                rois.mousemove(e.X, e.Y);
                return;
            }
            //放大镜
            if (tb_magic.Checked)
            {
                movemagicwindow(e.X, e.Y);
                //子窗口mousemove事件在父窗口响应
                //Console.WriteLine(string.Format("{0},{1}", e.X, e.Y));
            }
        }

        private void hwin_HMouseUp(object sender, HMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle) tb_move.Checked = false;
            if (tbrun.Checked) return;
            mouseleftpress = false;
            mousemidpress = false;
            if (e.Button == MouseButtons.Right) {
                rois.action = "";
                ondrawing = false;
                tbdrawrect.Checked = false;
                rois.srois.clear();
                rois.croi = null;
                hwinrepaint();
                showroidata();
                tb_magic.Checked = false;
                tb_move.Checked = false;
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
            rois.mouseup(e.X, e.Y);
            hwinrepaint();
            showroidata();
        }

        private void hwin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                foreach (roishape rs in rois.srois.rois) mrois.delete(rs);
                rois.delete();
                if (rois.broi == -1) cbbase.SelectedIndex = -1;
                dgupdatemeasureall();
                hwinrepaint();
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
                rois.srois.moveup();
                hwinrepaint();
                showroidata();
            }
            if (e.KeyData == Keys.Down)
            {
                rois.srois.movedown();
                hwinrepaint();
                showroidata();
            }
            if (e.KeyData == Keys.Left)
            {
                rois.srois.moveleft();
                hwinrepaint();
                showroidata();
            }
            if (e.KeyData == Keys.Right)
            {
                rois.srois.moveright();
                hwinrepaint();
                showroidata();
            }
        }
        //========================================================halwin 视图交互end

        //========================================================放大镜视图begin
        //点击放大镜， 点击窗口移动
        private void showmagicwindow(double cx, double cy){
            //cx,cy 鼠标点坐标
            if (magicwindow != null) magicwindow.Dispose();
            HOperatorSet.SetSystem("border_width", 10);
            magicwindow = new HWindow();
            magicsize = 70;
            magicwindow.OpenWindow(0,0,magicsize*2,magicsize*2,hwin.HalconID, "visible","");
            magicwindowpaint(cx, cy); 
        }
        private void movemagicwindow(double cx, double cy) {
            if (magicwindow == null) return;
            magicwindowpaint(cx, cy);
        }
        private void deletemagicwindow() {
            if (magicwindow == null) return;
            magicwindow.Dispose();
        }
        private void magicwindowpaint(double cx, double cy) {
            //重新确定窗口位置, mousemove时位置变化
            magicwindow.SetWindowExtents((int)((cy - vrect.Y) * zscale - magicsize), (int)((cx - vrect.X) * zscale - magicsize), magicsize * 2, magicsize * 2);
            //按缩放比例设置窗口区域
            magicrect.X = (int)(cx - magicsize / 2.0);
            magicrect.Y = (int)(cy - magicsize / 2.0);
            magicrect.Width = (int)(cx + magicsize / 2.0);
            magicrect.Height = (int)(cy + magicsize / 2.0);
            magicwindow.SetPart(magicrect.Y, magicrect.X, magicrect.Height, magicrect.Width);
            //绘制图像
            HSystem.SetSystem("flush_graphic", "false");
            magicwindow.ClearWindow();
            //显示图像
            magicwindow.DispImage(dcamera.himg);
            //显示roi
            if (rois != null) rois.paintroi(magicwindow);
            //更新视图
            HSystem.SetSystem("flush_graphic", "true");
            magicwindow.DispCross(0.0, 0.0, 100.0, 0.0);
        }
        private void tb_magic_Click(object sender, EventArgs e)
        {
            tb_move.Checked = false;
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
                dcamera.loadphoto(dlgopen.FileName);
                initview();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            dlgsave.FileName = "";
            dlgsave.Filter = "photo files|*.jpg";
            dlgsave.InitialDirectory = "";
            dlgsave.DefaultExt = ".jpg";
            if (dlgsave.ShowDialog() == DialogResult.OK) {
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
            if (!sender.Equals(tb_move)) tb_move.Checked = false;
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
            if (rois.croi == null) return;
            if (rois.srois.count == 0) return;
            hwinrepaint();
            foreach (roishape croi in rois.srois.rois)
            {
                croi.getregion(hwin.HalconWindow, dcamera.himg);
                croi.showregion(hwin.HalconWindow, true);
            }        
        }

        //重新计算并显示
        private void drawregion(){
            if (rois.croi == null) return;
            if (rois.srois.count == 0) return;
            hwinrepaint();
            
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
                

                croi.getregion(hwin.HalconWindow, dcamera.himg);
                croi.showregion(hwin.HalconWindow, true);
            }
        }


        private void showroidata() {
            if (rois.croi == null)
            {
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
            stdsurface.Value = Convert.ToInt32(rois.croi.stdsurface*10);
            tstdsurface.Text = (stdsurface.Value*1.0/10.0).ToString();

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
            stdsurface.Value = 0;
            tstdsurface.Text = "0";
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
            hwin.Focus();
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
            tmstd.Text = "0.0";
            tmllimit.Text = "0.0";
            tmulimit.Text = "0.0";
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
            tmoffset.Text = mroi.moffset.ToString();
        }

        private void dgview_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgview.SelectedRows.Count == 0) return;
            dgshowmeasure();
        }

        
        //自动运行检测
        public void run() {
            if (tbrun.Checked == false) return;
            runonce();
            //显示测试区域
            //if (ckregion.Checked)
            //{
            //    foreach (roishape rs in rois.rois)
            //    {
            //        if (rs.num == rois.broi) continue;
            //        rs.showregion(hwin.HalconWindow,true);
            //    }
            //}
        }
        //手动运行一次
        public void runonce() {
            double stime = Environment.TickCount;
            
            //初始化
            lbpass.Visible = false;
            lbng.Visible = false;
            dcamera.grabimg();
            rois.text1 = "";
            rois.text2 = "";
            rois.srois.clear();
            rois.croi = null;
            //计算
            rois.run();
            //测量
            mrois.run();
            
            testresult = "OK";
            rtresult.Clear();
            rtresult.AppendText("序号\t检测项\t检测内容\t标准值\t下公差\t上公差\t实测\t结果\r\n");
            rtresult.AppendText("====\t======\t============\t======\t======\t======\t======\t====\r\n");
            int i=1;
            string rstr = "";
            string mcon = "";
            foreach (clsmeasure cm in mrois.ilist) {
                //显示区域
                if (ckdisplayall.Checked) {
                    foreach (roishape rs in rois.rois) {
                        rois.showresult(rs);
                    }
                }
                if (ckdisplayng.Checked && cm.mresult == "NG") {
                    rois.showresult(cm.roi1);
                    if (cm.mtype < 3 || (cm.mtype == 3 && cm.roi2!=null)) rois.showresult(cm.roi2);
                }
                //输出结果
                if (cm.mresult == "NG") rtresult.SelectionColor= Color.Red;
                else rtresult.SelectionColor = Color.Black;
                if (cm.roi1 == null) mcon = "";
                else mcon = cm.roi1.num.ToString("d3");
                if (cm.roi2 == null) mcon += "> ";
                else mcon += ">" + cm.roi2.num.ToString("d3");
                mcon += cm.mtypename();
                
                if(ckdisplayall.Checked || (ckdisplayng.Checked && cm.mresult=="NG"))
                    rtresult.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\r\n",i,cm.mname,mcon,cm.mstd,cm.mllimit,cm.mulimit,cm.mvalue.ToString("f3"),cm.mresult));
                
                rstr += string.Format("{0} : {1}", cm.mname, cm.mvalue.ToString("f3")) + "\r\n";
                i++;
                if(cm.mresult=="NG") testresult="NG";
            }
            //表面检测
            foreach (roishape rs in rois.rois) {
                if (rs.surfacecheck) {
                    bool areack = rs.measuresuface(hwin.HalconWindow, dcamera.himg, false,true);
                    if (areack == false) {
                        testresult = "NG";
                        mcon = rs.num.ToString("d3");
                        rtresult.SelectionColor = Color.Red;
                        rtresult.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\r\n", i, "表面检测", mcon, "    ", rs.stdsurface+"%","   ", rs.msurface.ToString("f2")+"%", "NG"));
                        i++;
                    }
                }
            }

            //结果输出
            if (testresult == "NG")
            {
                lbng.Visible = true;
                //if(tbrun.Checked) dio.sendng();
                vcommon.qtyng++;
            }
            else {
                lbpass.Visible = true;
                //if(tbrun.Checked) dio.sendok();
                vcommon.qtypass++;
            }
            vcommon.qty++;
            vcommon.showstatistic();
            //时间
            double etime = Environment.TickCount;
            truntime.Text = (etime - stime).ToString();
            
            //显示测量数据统计结果
            rtdatastatistic.Clear();
            rtdatastatistic.AppendText("序号\t检测项\t检测内容\t检测数\tNG数\tNG偏大\tNG偏小\r\n");
            rtdatastatistic.AppendText("====\t======\t============\t======\t======\t======\t======\r\n");
            i = 1;
            foreach (clsmeasure cm in mrois.ilist){
                if (ckdatang.Checked && (cm.mngssmall + cm.mngslarge) == 0) continue;
                if (cm.roi1 == null) mcon = "";
                else mcon = cm.roi1.num.ToString("d3");
                if (cm.roi2 == null) mcon += "> ";
                else mcon += ">" + cm.roi2.num.ToString("d3");
                mcon += cm.mtypename();
                rtdatastatistic.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t", i, cm.mname, mcon, cm.mnums, cm.mngssmall+cm.mngslarge));
                rtdatastatistic.SelectionColor = Color.Red;
                rtdatastatistic.AppendText(string.Format("{0}\t{1}\r\n", cm.mngslarge, cm.mngssmall));
                rtdatastatistic.SelectionColor = Color.Black;
                i++;
            }

            //测量区显示测量结果
            //rois.text1 = testresult;
            if (vcommon.hshowresult) rois.text2 = rstr;
            else rois.text2 = string.Format("检测数： {0}\r\nPASS数： {1}\r\nNG数： {2}\r\n用时(ms)： {3}", tqty.Text, tqtypass.Text, tqtyng.Text, truntime.Text);
            rois.showtext();
            //显示原点
            hwin.HalconWindow.SetColor("red");
            hwin.HalconWindow.DispCross(0.0, 0.0, 100.0, 0.0);
            //NG图片保存
            tscreen.Enabled = true;
            //PLC信号输出
            if (tbrun.Checked){
               if (testresult == "NG") dio.sendng();
               else dio.sendok();
            }
        }
        //===========检测内容

        //===========产品加载，保存
        private void btnsaveproduct_Click(object sender, EventArgs e)
        {
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
                FileStream fs = new FileStream(fn, FileMode.Create, FileAccess.Write);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, data);
                fs.Flush();
                fs.Close();
                data.Clear();
                data = null;
                tpart.Text = Path.GetFileName(fn);
                vcommon.productname = fn;
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
                data.Clear();
                data = null;
            }
            //文件名
            tpart.Text = Path.GetFileName(fn);
            vcommon.productname = fn;
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

            hwinrepaint();
        }

        private void openfile(string fn) {
            //加载数据
            if (!Program.getversion()) return;
            tpart.Text = "";
            if (fn == "") return;
            if (!File.Exists(fn)) fn=Application.StartupPath+"\\default.lvd";
            
            ArrayList data;
            FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            data = (ArrayList)bf.Deserialize(fs);
            fs.Close();
            tpart.Text = Path.GetFileName(fn);
            vcommon.productname = fn;
            
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

            hwinrepaint();
        }

        private void btnnewproduct_Click(object sender, EventArgs e)
        {
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
            hwinrepaint();
        }
        //================产品管理

        //================相机设置
        private void tbcamera_Click(object sender, EventArgs e)
        {
            fcamera = new frmcamera();
            fcamera.Owner = this;
            fcamera.Show();
        }

        private void tbcameraplay_Click(object sender, EventArgs e)
        {
            if (dcamera.iscameraopen() == false)
            {
                MessageBox.Show("相机未连接！");
                return;
            }
            tbcameraplay.Checked = !tbcameraplay.Checked;
            if (tbcameraplay.Checked) tplay.Enabled = true;
            else tplay.Enabled = false;
            if (tplay.Enabled == false) hwinrepaint();
        }

        private void tplay_Tick(object sender, EventArgs e)
        {
            dcamera.grabasync();
            HSystem.SetSystem("flush_graphic", "false");
            hwin.HalconWindow.ClearWindow();
            //显示图像
            try{
                if (dcamera != null) hwin.HalconWindow.DispImage(dcamera.himg);
            }
            catch { }
            //显示十字线
            hwin.HalconWindow.SetColor("red");
            hwin.HalconWindow.SetLineStyle((new HTuple(3)).TupleConcat(1).TupleConcat(3).TupleConcat(1));
            hwin.HalconWindow.DispLine(hlrow, hlcol, hlrow1, hlcol1);
            hwin.HalconWindow.DispLine(vlrow, vlcol, vlrow1, vlcol1);


            //更新视图
            HSystem.SetSystem("flush_graphic", "true");
            hwin.HalconWindow.DispCross(0.0, 0.0, 100.0, 0.0);
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
            btnpixelunit.Enabled = true;
            runonce();
            //显示测试区域
            //if (ckregion.Checked)
            //{
            //    foreach (roishape rs in rois.rois)
            //    {
            //        if (rs.num == rois.broi) continue;
            //        rs.showregion(hwin.HalconWindow,true);
            //    }
            //}
        }

        private void frmmain_FormClosing(object sender, FormClosingEventArgs e)
        {
            vcommon.savedata();
            savemeasurename();
            if(MessageBox.Show("你确认要退出系统吗?","确认",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes){
                log.write("关闭软件");
                fprint.Dispose();
                fzebra.Dispose();
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
            tb_magic.Checked = false;
            tbdrawrect.Checked = false;
            foreach (ToolStripItem tb in mtools.Items) {
                if(tb.Name!="toolStripButton8") tb.Enabled = false;
            }
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
            //恢复播放，移动，放大镜功能
            tb_move.Checked = false;
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
            hwinrepaint();
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
            if (vcommon.productname!="") {
                openfile(vcommon.productname);
            }
            dcamera.grabasync();
            hwinrepaint();
        }

        private void tbsetting_Click(object sender, EventArgs e)
        {
            frmsetting fs = new frmsetting();
            fs.ShowDialog();
            hwinrepaint();
        }

        

        private void tbalignleft_Click(object sender, EventArgs e)
        {
            rois.srois.alignleft();
            hwinrepaint();
        }

        private void tbalignright_Click(object sender, EventArgs e)
        {
            rois.srois.alignright();
            hwinrepaint();
        }

        private void tbalignmidh_Click(object sender, EventArgs e)
        {
            rois.srois.alignmidh();
            hwinrepaint();
        }
        private void tbaligntop_Click(object sender, EventArgs e)
        {
            rois.srois.aligntop();
            hwinrepaint();
        }

        private void tbalignbottom_Click(object sender, EventArgs e)
        {
            rois.srois.alignbot();
            hwinrepaint();
        }

        private void tbalignmidv_Click(object sender, EventArgs e)
        {
            rois.srois.alignmidv();
            hwinrepaint();
        }

        private void tbalignheight_Click(object sender, EventArgs e)
        {
            rois.srois.alignheight();
            hwinrepaint();
        }

        private void tbalignwidth_Click(object sender, EventArgs e)
        {
            rois.srois.alignwidth();
            hwinrepaint();
        }

        private void tbalignsize_Click(object sender, EventArgs e)
        {
            rois.srois.alignsamesize();
            hwinrepaint();
        }

        private void tbalignsamespace_Click(object sender, EventArgs e)
        {
            rois.srois.alignsamegap();
            hwinrepaint();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            rois.copy();
            hwinrepaint();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            foreach(roishape rs in rois.srois.rois) mrois.delete(rs);
            rois.delete();
            if (rois.broi == -1) cbbase.SelectedIndex = -1;
            dgupdatemeasureall();
            hwinrepaint();
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
                thmin.Value = 0;
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
            hwin.HalconWindow.DispImage(dcamera.himg);
            rois.text1 = "";
            rois.text2 = "Data...";
            rois.showtext();
            //清空统计数据
            rtdatastatistic.Clear();
            foreach (clsmeasure cm in mrois.ilist)
            {
                cm.mnums = 0;
                cm.mngslarge = 0;
                cm.mngssmall = 0;
            }
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
            if (tmname.FindString(tname) < 0) tmname.Items.Add(tname);
        }

        private void tmname_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete) {
                string tn = tmname.Text;
                int tnindex = tmname.Items.IndexOf(tn);
                if (tnindex > 0) tmname.Items.RemoveAt(tnindex);
            }
        }

        private void tmname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete) {
                string tn = tmname.Text;
                int id = tmname.Items.IndexOf(tn);
                if (id > 9) tmname.Items.RemoveAt(id);
            }
        }
        private void loadmeasurename() {
            StreamReader sr = new StreamReader(Application.StartupPath + "\\mname.dat");
            string rstr = sr.ReadToEnd();
            sr.Close();
            string[] nstr = rstr.Split(',');
            tmname.Items.Clear();
            foreach (string str in nstr) tmname.Items.Add(str);
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
            string fpath = fprint.folder;
            if (fpath == "") return;
            if (!Directory.Exists(fpath)) Directory.CreateDirectory(fpath);
            
            //int btime = Environment.TickCount;
            if(pview.Width==0 || pview.Height==0) return;
            HImage timg = new HImage("byte", pview.Width, pview.Height);
            timg = hwin.HalconWindow.DumpWindowImage();
            timg.WriteImage("bmp", 0, "tmp.bmp");
            timg.Dispose();
            Bitmap bmp = new Bitmap(pview.Width, pview.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //绘制控件
            pview.DrawToBitmap(bmp, pview.ClientRectangle);
            //绘制测量图形
            Bitmap tbmp = new Bitmap("tmp.bmp");
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(tbmp, 0, 0, hwin.Width, hwin.Height);
            //绘制测量结果输出richtextbox内容
            if (tabControl1.SelectedIndex == 3)
            {
                Rectangle rect = rtresult.RectangleToScreen(rtresult.ClientRectangle);
                rect = pview.RectangleToClient(rect);
                Graphics gr = rtresult.CreateGraphics();
                IntPtr dcg = g.GetHdc();
                IntPtr dcgr = gr.GetHdc();
                BitBlt(dcg, rect.Left, rect.Top, rect.Width, rect.Height, dcgr, 0, 0, 0x00CC0020);//srccopy
                g.ReleaseHdc(dcg);
                gr.ReleaseHdc(dcgr);
                gr.Dispose();
            }
            bmp.Save(fpath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssff") + ".bmp");
            bmp.Dispose();
            tbmp.Dispose();
            g.Dispose();
            //int etime = Environment.TickCount;
            //tcolmin.Text = (etime - btime).ToString();
        }

        private void tscreen_Tick(object sender, EventArgs e)
        {
            tscreen.Enabled = false;
            if (testresult=="NG") savescreen();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            fprint.photosview();
        }

        private void tb_move_Click(object sender, EventArgs e)
        {
            tb_magic.Checked = false;
            tb_move.Checked = !(tb_move.Checked);
        }

        private void hwin_HMouseWheel(object sender, HMouseEventArgs e)
        {
            if (e.Delta > 0) toolStripButton6_Click(null, null);
            else toolStripButton5_Click(null, null);
        }

        private void thminsurface_ValueChanged(object sender, EventArgs e)
        {
            if (!thminsurface.Focused) return;
            if (!ckshowsurface.Checked) ckshowsurface.Checked = true;
            if (thmaxsurface.Value < thminsurface.Value) thmaxsurface.Value = thminsurface.Value;
            tthminsurface.Text = thminsurface.Value.ToString();
            drawsurface();
        }

        private void thmaxsurface_ValueChanged(object sender, EventArgs e)
        {
            if (!thmaxsurface.Focused) return;
            if (!ckshowsurface.Checked) ckshowsurface.Checked = true;
            if (thmaxsurface.Value < thminsurface.Value) thminsurface.Value = thmaxsurface.Value;
            tthmaxsurface.Text = thmaxsurface.Value.ToString();
            drawsurface();
        }

        private void stdsurface_ValueChanged(object sender, EventArgs e)
        {
            if (!stdsurface.Focused) return;
            if (!ckshowsurface.Checked) ckshowsurface.Checked = true;
            tstdsurface.Text = (stdsurface.Value*1.0/100.0).ToString();
            drawsurface();
        }
        private void cksurface_Click(object sender, EventArgs e)
        {
            ckshowsurface.Checked = cksurface.Checked;
            drawsurface();
            if (!cksurface.Checked) showroidata();
        }

        private void showsurface() {
            if (rois.croi == null) return;
            if (rois.srois.count == 0) return;
            hwinrepaint();
            foreach (roishape croi in rois.srois.rois)
            {
                croi.measuresuface(hwin.HalconWindow, dcamera.himg, true,true);
            }        
        }

        private void drawsurface()
        {
            if (rois.croi == null) return;
            if (rois.srois.count == 0) return;
            hwinrepaint();
            foreach (roishape croi in rois.srois.rois)
            {
                //赋值
                croi.surfacecheck = cksurface.Checked;
                croi.surfacemaxcheck = cksurfaceareamax.Checked;
                croi.thminsurface = thminsurface.Value;
                croi.thmaxsurface = thmaxsurface.Value;
                croi.stdsurface = stdsurface.Value*1.0/100.0;
                croi.measuresuface(hwin.HalconWindow, dcamera.himg, true,true);
            }
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


        private void btnpixelunit_Click(object sender, EventArgs e)
        {
            //自动计算像素比例
            double pvx = dcamera.xpixel;
            double pvy = dcamera.ypixel;
            if (pvx == 0) pvx = 1;
            if (pvy == 0) pvy = 1;
            double pvxsum = 0.0, pvysum = 0.0;
            int pvxnum = 0, pvynum = 0;
            foreach (clsmeasure cm in mrois.ilist)
            {
                if (cm.mvalue == 0) cm.mvalue = 1;
                if (cm.mtype == 0)
                {
                    pvxsum += cm.mstd / (cm.mvalue / pvx);
                    pvxnum++;
                }
                if (cm.mtype == 1) {
                    pvysum += cm.mstd / (cm.mvalue / pvy);
                    pvynum++;
                }
            }

            if (pvxnum > 0) pvxsum = pvxsum / pvxnum;
            if (pvynum > 0) pvysum = pvysum / pvynum;
            if (pvxsum == 0) pvxsum = pvysum;
            if (pvysum == 0) pvysum = pvxsum;
            if (pvxsum == 0) pvxsum = 1;
            if (pvysum == 0) pvysum = 1;
            dcamera.xpixel = Math.Abs(pvxsum);
            dcamera.ypixel = Math.Abs(pvysum);
            runonce();
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
            hwin.HalconWindow.DispImage(dcamera.himg);
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
                        rois.showresult(rs);
                    }
                }
                if (ckdisplayng.Checked && cm.mresult == "NG")
                {
                    rois.showresult(cm.roi1);
                    if (cm.mtype < 3 || (cm.mtype == 3 && cm.roi2 != null)) rois.showresult(cm.roi2);
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
                    bool areack = rs.measuresuface(hwin.HalconWindow, dcamera.himg, false,true);
                    if (areack == false)
                    {
                        mcon = rs.num.ToString("d3");
                        rtresult.SelectionColor = Color.Red;
                        rtresult.AppendText(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\r\n", i, "表面检测", mcon, "    ", rs.stdsurface + "%", "   ", rs.msurface.ToString("f2") + "%", "NG"));
                        i++;
                    }
                }
            }
            //输出文本
            rois.showtext();
            //显示原点
            hwin.HalconWindow.SetColor("red");
            hwin.HalconWindow.DispCross(0.0, 0.0, 100.0, 0.0);
        }

        private void ckcombine_Click(object sender, EventArgs e)
        {
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
            btnprint.Enabled = false;
            button4_Click(null, null);
        }

        private void btnend_Click(object sender, EventArgs e)
        {
            try { fprint.pend(); }
            catch { }
            btnstart.Enabled = true;
            btnprint.Enabled = true;
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
    }//class
}//namespace
