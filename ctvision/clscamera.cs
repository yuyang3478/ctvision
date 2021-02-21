using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
//using HalconDotNet;
using ctmeasure;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

using MVSDK;//使用SDK接口
//using Snapshot;
using CameraHandle = System.Int32;
using MvApi = MVSDK.MvApi;
using System.Runtime.InteropServices;

using OpenCvSharp;    //添加相应的引用即可
using OpenCvSharp.Extensions;

using System.Diagnostics;
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
        private bool hfgon=false;
        [NonSerialized]
        public Mat himg;// = new Mat("C:\\Users\\24981\\Desktop\\ct\\testimg.bmp");//, ImreadModes.Grayscale// new Mat(new OpenCvSharp.Size(300,300),MatType.CV_8UC1);//=new HImage("byte",3000,3000);
        [NonSerialized]
        private string ctype = "";
        [NonSerialized]
        public int iw = 0, ih = 0;
        [NonSerialized]
        CameraSdkStatus status;
        [NonSerialized]
        tSdkCameraDevInfo[] tCameraDevInfoList;
        [NonSerialized]
        public CameraHandle m_hCamera = 0;             // 句柄
        [NonSerialized]
        protected IntPtr m_ImageBuffer;             // 预览通道RGB图像缓存
        [NonSerialized]
        protected IntPtr m_ImageBufferSnapshot;     // 抓拍通道RGB图像缓存
        [NonSerialized]
        protected Thread m_tCaptureThread;          //图像抓取线程
        [NonSerialized]
        protected tSdkCameraCapbility tCameraCapability;  // 相机特性描述
        [NonSerialized]
        protected bool m_bExitCaptureThread = false;//采用线程采集时，让线程退出的标志
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
            return;
            loaddata();
        }

        public bool cameraopen() {
            return false;

#if USE_CALL_BACK
                        CAMERA_SNAP_PROC pCaptureCallOld = null;
#endif
            if (m_hCamera > 0) return true;
            if ((tCameraDevInfoList!=null)&&(tCameraDevInfoList.Length>0))
            {
                //已经初始化过，直接返回 true
                return true;
            }
            status = MvApi.CameraEnumerateDevice(out tCameraDevInfoList);
            //hfgon = true;
            if (status == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            {
                //hfgon = true;
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
                        //try { if (exposuretime > 0) MvApi.CameraSetExposureTime(m_hCamera, exposuretime);  } catch { }
                        //try { if (gain > 0) MvApi.CameraSetGain(m_hCamera, Convert.ToInt32(gain / 100), Convert.ToInt32(gain / 100), Convert.ToInt32(gain / 100)); } catch { }
                        //try { if (contrast > 0) MvApi.CameraSetContrast(m_hCamera, contrast); } catch { }
                        //try { if (gamma > 0) MvApi.CameraSetGamma(m_hCamera, Convert.ToInt32(gamma)); } catch { }

                        //两种方式来获得预览图像，设置回调函数或者使用定时器或者独立线程的方式，
                        //主动调用CameraGetImageBuffer接口来抓图。
                        //本例中仅演示了两种的方式,注意，两种方式也可以同时使用，但是在回调函数中，
                        //不要使用CameraGetImageBuffer，否则会造成死锁现象。
#if USE_CALL_BACK
                                                m_CaptureCallback = new CAMERA_SNAP_PROC(ImageCaptureCallback);
                                                MvApi.CameraSetCallbackFunction(m_hCamera, m_CaptureCallback, m_iCaptureCallbackCtx, ref pCaptureCallOld);
#else //如果需要采用多线程，使用下面的方式
                        m_bExitCaptureThread = false;
                        //m_tCaptureThread = new Thread(new ThreadStart(CaptureThreadProc));
                        //m_tCaptureThread.Start();
                        return true;
#endif

                    }
                    else
                    {
                        m_hCamera = 0;
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

            //InitCamera();
            //if (cname == "") return;


            //hfgon = false;
            //try
            //{
            //    hfg = new HFramegrabber();
            //    //open_framegrabber ('GigEVision', 0, 0, 0, 0, 0, 0, 'default', -1, 'default', -1, 'false', 'default', 'xiangji1', 0, -1, AcqHandle)
            //    hfg.OpenFramegrabber("GigEVision", 0, 0, 0, 0, 0, 0, "default", -1, "default", -1, "false", "default", cname, 0, -1);

            //    Thread.Sleep(300);
            //    try
            //    {
            //        if (cname.IndexOf("MindVision") > -1)
            //        {
            //            HOperatorSet.SetFramegrabberParam(hfg, "AcquisitionMode", "SingleFrame");
            //            HOperatorSet.SetFramegrabberParam(hfg, "AcquisitionFrameRate", "Mid");
            //        }
            //        try { if (exposuretime > 0) hfg.SetFramegrabberParam("ExposureTime", exposuretime); } catch { }
            //        try { if (gain > 0) hfg.SetFramegrabberParam("Gain", gain / 100); } catch { }
            //        try { if (contrast > 0) hfg.SetFramegrabberParam("Contrast", contrast); } catch { }
            //        try { if (gamma > 0) hfg.SetFramegrabberParam("Gamma", gamma); } catch { }
            //    }
            //    catch { }

            //    hfg.GrabImageStart(-1);
            //    //#himg = hfg.GrabImageAsync(-1);
            //    //#string ctype = "";
            //    //#himg.GetImagePointer1(out ctype, out iw, out ih);

            //    hfgon = true;
            //}
            //catch
            //{
            //    //MessageBox.Show("相机未连接！");
            //}
        }

        public Mat loadphoto(string filephoto) {
            //#himg.Dispose();
            Mat imgback = new Mat();
            himg = new Mat(filephoto);//ReadImage(filephoto);, ImreadModes.Grayscale
            if (himg != null)
                iw = himg.Width;
                ih = himg.Height;
                himg.CopyTo(imgback);
            return imgback;
        }


        public void savephoto(string filename)
        {
            Cv2.ImWrite(filename, himg);//保存到桌面
            //himg.WriteImage("jpeg", 0, filename);
        }
        public void cameraparam() {
            if (!cameraopen()) return;
            try
            {
                double ept = 0.0;
                int rgain = 0;
                int ggain = 0;
                int bgain = 0;
                int ctt = 0;
                int gm = 0;
                MvApi.CameraGetExposureTime(m_hCamera,ref ept);
                MvApi.CameraGetGain(m_hCamera,ref rgain,ref ggain,ref bgain);
                MvApi.CameraGetContrast(m_hCamera, ref ctt);
                MvApi.CameraGetGamma(m_hCamera, ref gm);
                exposuretime = ept;
                gain = rgain;
                contrast = ctt;
                gamma = gm;
                //exposuretime = hfg.GetFramegrabberParam("ExposureTime");
                //gain = hfg.GetFramegrabberParam("Gain")*100;
                //gamma = hfg.GetFramegrabberParam("Gamma");
                //contrast = hfg.GetFramegrabberParam("Contrast");
            }
            catch { }
        }

        public void cameraparamset() {
            if (!cameraopen()) return;
            try { if (exposuretime > 0) MvApi.CameraSetExposureTime(m_hCamera, exposuretime); } catch { }
            try { if (gain > 0) MvApi.CameraSetGain(m_hCamera, Convert.ToInt32(gain / 100), Convert.ToInt32(gain / 100), Convert.ToInt32(gain / 100)); } catch { }
            try { if (contrast > 0) MvApi.CameraSetContrast(m_hCamera, contrast); } catch { }
            try { if (gamma > 0) MvApi.CameraSetGamma(m_hCamera, Convert.ToInt32(gamma)); } catch { }
            //try{if (exposuretime > 0) hfg.SetFramegrabberParam("ExposureTime", exposuretime);}catch{}
            //try{if (gain > 0) hfg.SetFramegrabberParam("Gain", gain / 100);}catch{}
            //try{if (contrast > 0) hfg.SetFramegrabberParam("Contrast", contrast);}catch{}
            //try{if (gamma > 0) hfg.SetFramegrabberParam("Gamma", gamma);}catch { }
        }

        public void cameraclose(){
            //HOperatorSet.CloseAllFramegrabbers();
            //hfgon=false;
            if (m_hCamera > 0)
            {
#if !USE_CALL_BACK //使用回调函数的方式则不需要停止线程
                m_bExitCaptureThread = true;
                while (m_tCaptureThread.IsAlive)
                {
                    Thread.Sleep(10);
                }
#endif
                MvApi.CameraUnInit(m_hCamera);
                Marshal.FreeHGlobal(m_ImageBuffer);
                Marshal.FreeHGlobal(m_ImageBufferSnapshot);
                m_hCamera = 0;
            }
        }

        public tSdkCameraDevInfo[] listcameras(){
            //return null;
            return tCameraDevInfoList;
            //HTuple dinfo,dnames;
            //HOperatorSet.InfoFramegrabber("GigEVision", "device", out dinfo, out dnames);
            //return dnames;
        }

        public Mat grabasync() {
            //if (!hfgon) cameraopen();
            if (!cameraopen()) return himg; //status != CameraSdkStatus.CAMERA_STATUS_SUCCESS
            try
            {
                //himg.Dispose();
                //himg = hfg.GrabImageAsync(-1);
                //himg = hfg.GrabImage();
                //Program.fmain.hwin.HalconWindow.DispImage(himg);
            }
            catch {}
            return himg;
        }

        public Mat grabimg(PictureBox pb)
        { 
            if (!cameraopen()) 
                return himg;
            try
            {
               

                tSdkFrameHead tFrameHead;
                IntPtr uRawBuffer;//由SDK中给RAW数据分配内存，并释放


                if (m_hCamera <= 0)
                {
                    return himg;//相机还未初始化，句柄无效
                }
                if (himg!=null)
                    himg.Dispose();
                if (MvApi.CameraSnapToBuffer(m_hCamera, out tFrameHead, out uRawBuffer, 500) == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
                {
                    //此时，uRawBuffer指向了相机原始数据的缓冲区地址，默认情况下为8bit位宽的Bayer格式，如果
                    //您需要解析bayer数据，此时就可以直接处理了，后续的操作演示了如何将原始数据转换为RGB格式
                    //并显示在窗口上。

                    //将相机输出的原始数据转换为RGB格式到内存m_ImageBufferSnapshot中
                    MvApi.CameraImageProcess(m_hCamera, uRawBuffer, m_ImageBufferSnapshot, ref tFrameHead);
                    //CameraSnapToBuffer成功调用后必须用CameraReleaseImageBuffer释放SDK中分配的RAW数据缓冲区
                    //否则，将造成死锁现象，预览通道和抓拍通道会被一直阻塞，直到调用CameraReleaseImageBuffer释放后解锁。
                    MvApi.CameraReleaseImageBuffer(m_hCamera, uRawBuffer);
                    ////更新抓拍显示窗口。
                    //m_DlgSnapshot.UpdateImage(ref tFrameHead, m_ImageBufferSnapshot);
                    //m_DlgSnapshot.Show();

                    //// TODO 待调整
                    //pb.Width = tFrameHead.iWidth;
                    //pb.Height = tFrameHead.iHeight;
                    //Stopwatch timer = Stopwatch.StartNew();
                    //pb.Image = MvApi.CSharpImageFromFrame(m_ImageBufferSnapshot, ref tFrameHead);
                    //timer.Stop();
                    //TimeSpan timespan = timer.Elapsed;

                    //himg = BitmapConverter.ToMat(new System.Drawing.Bitmap(pb.Image));


                    //void* dst =  himg.Ptr(0, 0).ToPointer();
                    //btnTabuSearch.Text = String.Format("{0:00}:{1:00}:{2:00}", timespan.Minutes, timespan.Seconds, timespan.Milliseconds / 10);
                    //Console.WriteLine(String.Format("{0:00}:{1:00}:{2:00}", timespan.Minutes, timespan.Seconds, timespan.Milliseconds));
                    //OpenCvSharp.Extensions.ToMat(pb.Image, himg);
                    //OpenCvSharp.Extensions.
                    //himg = new Mat(m_ImageBufferSnapshot);
                    return himg;
                }

                //himg = hfg.GrabImage();
                //Program.fmain.hwin.HalconWindow.DispImage(himg);
            }
            catch { }
            return himg;
        }
        public Mat getBackImg()
        {
            Mat imgback = new Mat();
            if (himg!=null)
                himg.CopyTo(imgback);
            return imgback;
        }
        public bool iscameraopen() { 
            return cameraopen();
        }
         

        ///camera保存
        public void savedata()
        {
            string fn = Environment.CurrentDirectory + "\\camera1.bin";
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
             
        }

         
    }
}
