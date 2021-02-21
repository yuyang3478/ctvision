using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using leanvision;
//using HalconDotNet;
using MVSDK;//使用SDK接口
//using Snapshot;
using CameraHandle = System.Int32;
using MvApi = MVSDK.MvApi;
using System.Runtime.InteropServices;

namespace ctmeasure
{
    public partial class frmcamera : Form
    {
        private clscamera dcamera;
        public frmcamera()
        {
            InitializeComponent();
        }

        private void frmcamera_Load(object sender, EventArgs e)
        {
            dcamera = Program.fmain.dcamera;
            //列举相机
            tSdkCameraDevInfo[] tCameraDevInfoList = dcamera.listcameras();
            string cname = "";
            if (tCameraDevInfoList.Length == 1)
            {
                cname = System.Text.Encoding.Default.GetString(tCameraDevInfoList[0].acFriendlyName); //cameras.ToString().Replace("\"", "");
                cbcameras.Items.Add(cname);
                //if (cname.IndexOf("MindVision")>-1) 
            }
            else {
                for (int i = 0; i < tCameraDevInfoList.Length; i++)
                {
                    cname = System.Text.Encoding.Default.GetString(tCameraDevInfoList[i].acFriendlyName);
                    cbcameras.Items.Add(cname);
                    //if (cname.IndexOf("MindVision") > -1) 
                }
            }
            cbcameras.SelectedIndex = cbcameras.Items.IndexOf(cname);
            try
            {
                tbexposuretime.Value = (int)dcamera.exposuretime;
                tbgain.Value = (int)dcamera.gain;
                tbgamma.Value = (int)dcamera.gamma;
                tbcontrast.Value = dcamera.contrast;
                texposuretime.Text = dcamera.exposuretime.ToString();
                tgain.Text = dcamera.gain.ToString();
                tgamma.Text = dcamera.gamma.ToString();
                tcontrast.Text = dcamera.contrast.ToString();
            }
            catch { }
            txpixel.Text = dcamera.xpixel.ToString();
            typixel.Text = dcamera.ypixel.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dcamera.xpixel = double.Parse(txpixel.Text);
            dcamera.ypixel = double.Parse(typixel.Text);
            dcamera.savedata();
            dcamera.cameraopen();
            this.Close();
        }

        private void tplay_Tick(object sender, EventArgs e)
        {

        }

        private void frmcamera_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void btnplay_Click(object sender, EventArgs e)
        {

        }

        private void cbcameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cbcameras.Focused) return;
            dcamera.cname = cbcameras.Text;
        }

        private void tbexposuretime_ValueChanged(object sender, EventArgs e)
        {
            if (!(sender as TrackBar).Focused) return;
            if (sender.Equals(tbexposuretime))
            {
                texposuretime.Text = tbexposuretime.Value.ToString();
                dcamera.exposuretime = tbexposuretime.Value;
            }
            if (sender.Equals(tbgain))
            {
                tgain.Text = tbgain.Value.ToString();
                dcamera.gain = tbgain.Value;
            }
            if (sender.Equals(tbgamma))
            {
                tgamma.Text = tbgamma.Value.ToString();
                dcamera.gamma = tbgamma.Value;
            }
            if (sender.Equals(tbcontrast)) {
                tcontrast.Text = tbcontrast.Value.ToString();
                dcamera.contrast = tbcontrast.Value;
            }
            dcamera.cameraparamset();
            dcamera.grabasync();
        }

        private void txpixel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && e.KeyChar != '.' && (e.KeyChar < '0' || e.KeyChar > '9')) e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf(".") > -1) e.Handled = true;
        }

        private void txpixel_Leave(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text.Trim() == "") (sender as TextBox).Text = "10.00";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            dcamera.cameraparam();
            tbexposuretime.Value = (int)dcamera.exposuretime;
            tbgain.Value = (int)dcamera.gain;
            tbgamma.Value = (int)dcamera.gamma;
            tbcontrast.Value = dcamera.contrast;
            texposuretime.Text = dcamera.exposuretime.ToString();
            tgain.Text = dcamera.gain.ToString();
            tgamma.Text = dcamera.gamma.ToString();
            tcontrast.Text = dcamera.contrast.ToString();
        }
    }
}
