﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using leanvision;

namespace ctmeasure
{
    public partial class frmsetting : Form
    {
        
        public frmsetting()
        {
            InitializeComponent();
            cbcolor1.SelectedIndex = 0;
            cbcolor2.SelectedIndex = 0;
            cbcolor3.SelectedIndex = 0;
            cbcolor4.SelectedIndex = 0;
            cbcolor5.SelectedIndex = 0;
            tfontsize.Text = "100";
            rbdata.Checked = false;
            rbstatistic.Checked = true;
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                ComboBox cbox = sender as ComboBox;
                string colorName = (string)cbox.Items[e.Index];
                
                Color color = Color.FromName(colorName);
                Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width / 4, e.Bounds.Height - 2);
                Brush brush = new SolidBrush(color);
                e.Graphics.DrawRectangle(new Pen(Color.Black), rect);
                rect.X += 1;
                rect.Y += 1;
                rect.Width -= 1;
                rect.Height -= 1;
                e.Graphics.FillRectangle(brush, rect);
                rect.Offset(rect.Width + 4, 0);
                e.Graphics.DrawString(colorName, e.Font, Brushes.Black, rect.Location);
            }
        }

        private void cbcolor1_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void frmsetting_Load(object sender, EventArgs e)
        {
            cbcolor1.SelectedIndex = cbcolor1.Items.IndexOf(vcommon.hcolor);
            cbcolor2.SelectedIndex = cbcolor2.Items.IndexOf(vcommon.hcoloractive);
            cbcolor3.SelectedIndex = cbcolor3.Items.IndexOf(vcommon.hcolorhandle);
            cbcolor4.SelectedIndex = cbcolor4.Items.IndexOf(vcommon.hcolorregion);
            cbcolor5.SelectedIndex = cbcolor5.Items.IndexOf(vcommon.hcolorselect);
            cbcolor6.SelectedIndex = cbcolor6.Items.IndexOf(vcommon.hcolortext);
            tfontsize.Text = vcommon.fontsize.ToString();
            rbdata.Checked = vcommon.hshowresult;
            rbstatistic.Checked = vcommon.hshowstatistic;
            tmove.Text = vcommon.posmove.ToString();
            txpixel.Text = Program.fmain.dcamera.xpixel.ToString();
            typixel.Text = Program.fmain.dcamera.ypixel.ToString();
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnok_Click(object sender, EventArgs e)
        {   
            vcommon.hcolor = vcommon.colorToPen(cbcolor1.Text);
            vcommon.hcoloractive = vcommon.colorToPen(cbcolor2.Text);
            vcommon.hcolorhandle = vcommon.colorToPen(cbcolor3.Text);
            vcommon.hcolorregion = vcommon.colorToPen(cbcolor4.Text);
            vcommon.hcolorselect = vcommon.colorToPen(cbcolor5.Text);
            vcommon.hcolortext = vcommon.colorToPen(cbcolor6.Text);
            vcommon.fontsize = int.Parse(tfontsize.Text);
            vcommon.hshowresult = rbdata.Checked;
            vcommon.hshowstatistic = rbstatistic.Checked;
            vcommon.posmove = int.Parse(tmove.Text);

            vcommon.picpath = tbpic.Text.ToString().Trim();
            vcommon.filepath = tbfile.Text.ToString().Trim();

            vcommon.savedata();
            Program.fmain.dcamera.xpixel = double.Parse(txpixel.Text);
            Program.fmain.dcamera.ypixel = double.Parse(typixel.Text);
            Program.fmain.dcamera.savedata();

            this.Close();
        }

        //private Pen colorToPen(string colorText)
        //{
        //    if (string.Equals(colorText,"red"))
        //    {
        //        return Pens.Red;
        //    }else if (string.Equals(colorText, "red"))
        //    {
        //        return Pens.Green;
        //    }
        //    return Pens.Blue;
        //}

        private void txpixel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && e.KeyChar != '.' && (e.KeyChar < '0' || e.KeyChar > '9')) e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf(".") > -1) e.Handled = true;
        }

        private void txpixel_Leave(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text.Trim() == "") (sender as TextBox).Text = "10.00";
        }

        private void tmove_Leave(object sender, EventArgs e)
        {
            if (tmove.Text.Trim() == "") tmove.Text = "5";
        }

        private void tmove_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar!=8 && (e.KeyChar < '0' || e.KeyChar > '9')) e.Handled = true;
        }

        private void tfontsize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar!=8 && (e.KeyChar < '0' || e.KeyChar > '9')) e.Handled = true;
        }

        private void tfontsize_Leave(object sender, EventArgs e)
        {
            if (tfontsize.Text.Trim() == "") tfontsize.Text = "5";
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void btnpic_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                tbpic.Text = dialog.SelectedPath;
               
            }
             
        }

        private void btnfile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                tbfile.Text = dialog.SelectedPath;
                
            }
        }
    }
}
