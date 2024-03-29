﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Management;
using System.IO;

namespace ctmeasure
{
    
    public static class Apphelper
    {
        //给要创建的注册表设置一个变量，使用更方便
        //private string RegistFileName = "logInfo";

        public static bool verifyInstall(string name, string password) {
            //验证登录
            string ds = GetCPUSerialNumber();
            if (ds == null || ds.Equals("")) {
                MessageBox.Show("读取硬件信息失败。\n");
            }
            string n_p = ds + name.Trim() + password.Trim();
            string hash = GetSha1Hash(n_p);
            //string rhash = "8191DFA2A060AA510742DBF0F3E09BD303EBCEB0";
             
            if ((!name.Equals("serversalessong"))||(!password.Equals("Szamxkjsfp780208"))) {
                MessageBox.Show("用户名或密码错误！\n 不能在未授权主机中运行。\n");
                //StreamWriter sw = new StreamWriter("c:\\info.txt");
                //sw.WriteLine(hash);
                ////sw.WriteLine(s); 
                //sw.Flush();
                //sw.Close();

                return false;
                
            }else// if (!IsRegistryKeyExist())
            {//保存数据到注册表
                CreateRegistFile();
                WriteValue(name.Trim(),password.Trim(), hash);
                return true;
            }
            //string getval = getValue();

            //using (StreamWriter sw = new StreamWriter("c:\\info.txt"))
            //{
            //    sw.WriteLine("----");
            //    sw.WriteLine(getval);
            //    sw.WriteLine("----");
            //}
            
        }

        /// <summary>
        /// 创建一个test注册表项,下面包含OpenLog，和SaveLog两个子项
        /// </summary>
        private static void CreateRegistFile()
        {  //SOFTWARE在LocalMachine分支下    
           //使用CreateSubKey()在SOFTWARE下创建子项test
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftWare = hklm.CreateSubKey(@"SOFTWARE\logInfo");
            hklm.Close();
            hkSoftWare.Close();
        }

        //判断注册表项是否存在
        public static bool IsRegistryKeyExist()
        {
            string sKeyName = "logInfo";
            string[] sKeyNameColl;
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftWare = hklm.OpenSubKey(@"SOFTWARE");
            sKeyNameColl = hkSoftWare.GetSubKeyNames(); //获取SOFTWARE下所有的子项
            //using (StreamWriter sw = new StreamWriter("c:\\info.txt"))
            //{
            foreach (string sName in sKeyNameColl)
            {
                if (sName == sKeyName)
                {
                    hklm.Close();
                    hkSoftWare.Close();
                    return true;
                }
            }
            //}
            hklm.Close();
            hkSoftWare.Close();
            return false;
        }

        private static bool WriteValue(string name,string passwd,string hash)
        {
            //主要用到了SetValue()，表示在test下创建名称为Name，值为RegistryTest的键值。第三个参数表示键值类型，省略时，默认为字符串
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftWare = hklm.OpenSubKey(@"SOFTWARE\logInfo", true);
            hkSoftWare.SetValue("name", name, RegistryValueKind.String);
            hkSoftWare.SetValue("passwd", passwd, RegistryValueKind.String);
            hkSoftWare.SetValue("hash", hash, RegistryValueKind.String);
            hklm.Close();
            hkSoftWare.Close();
            return true;
        }

        public static string getHashValue()
        {
            //主要用到了GetValue(),获得名称为"Name"的键值
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftWare = hklm.OpenSubKey(@"SOFTWARE\logInfo", true);
            string sValue = hkSoftWare.GetValue("hash").ToString();
            hklm.Close();
            hkSoftWare.Close();
            return sValue;
        }

        public static string getNameValue()
        {
            //主要用到了GetValue(),获得名称为"Name"的键值
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftWare = hklm.OpenSubKey(@"SOFTWARE\logInfo", true);
            
            string sValue = hkSoftWare.GetValue("name").ToString();
            hklm.Close();
            hkSoftWare.Close();
            return sValue;
        }

        public static string getPassValue()
        {
            //主要用到了GetValue(),获得名称为"Name"的键值
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftWare = hklm.OpenSubKey(@"SOFTWARE\logInfo", true);
            string sValue = hkSoftWare.GetValue("passwd").ToString();
            hklm.Close();
            hkSoftWare.Close();
            return sValue;
        }

        public static bool deleteValue() {
            //主要用到了DeleteValue(),表示删除名称为"Name"的键值，第二个参数表示是否抛出异常
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftWare = hklm.OpenSubKey(@"SOFTWARE\logInfo", true);
            if (IsRegistryValueNameExist("hash"))  hkSoftWare.DeleteValue("hash", true);
            if (IsRegistryValueNameExist("name"))  hkSoftWare.DeleteValue("name", true);
            if (IsRegistryValueNameExist("passwd"))  hkSoftWare.DeleteValue("passwd", true);
            hklm.Close();
            hkSoftWare.Close();
            return true;
        }

        //判断键值是否存在
        public static bool IsRegistryValueNameExist(string sValueName)
        {
            string[] sValueNameColl;
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkTest = hklm.OpenSubKey(@"SOFTWARE\logInfo");
            sValueNameColl = hkTest.GetValueNames(); //获取test下所有键值的名称
            foreach (string sName in sValueNameColl)
            {
                if (sName == sValueName)
                {
                    hklm.Close();
                    hkTest.Close();
                    return true;
                }
            }
            hklm.Close();
            hkTest.Close();
            return false;
        }

        private static void deleteItem()
        {
            //主要用到了DeleteSubKey(),删除test项
            RegistryKey hklm = Registry.LocalMachine;
            hklm.DeleteSubKey(@"SOFTWARE\test", true);  //为true时，删除的注册表不存在时抛出异常；当为false时不抛出异常。
            hklm.Close();
        }
        
        public static string GetHardDiskSN()
        {
            //创建ManagementObjectSearcher对象
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            String strHardDiskID = null;//存储磁盘序列号
            //调用ManagementObjectSearcher类的Get方法取得硬盘序列号
            foreach (ManagementObject mo in searcher.Get())
            { 
                strHardDiskID = mo["SerialNumber"].ToString().Trim();//记录获得的磁盘序列号
                break;
            }
            return strHardDiskID;
        }

        //获取CPU序列号
        public static string GetCPUSerialNumber()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Processor");
                string sCPUSerialNumber = "";
                foreach (ManagementObject mo in searcher.Get())
                {
                    sCPUSerialNumber = mo["ProcessorId"].ToString().Trim();
                    break;
                }
                return sCPUSerialNumber;
            }
            catch
            {
                return "";
            }
        }

        public static string GetSha1Hash(string input)
        {
            byte[] inputBytes = Encoding.Default.GetBytes(input);

            SHA1 sha = new SHA1CryptoServiceProvider();

            byte[] result = sha.ComputeHash(inputBytes);

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                sBuilder.Append(result[i].ToString("x2"));
            }

            return sBuilder.ToString().ToUpper();
        }

        public static bool VerifySha1Hash(string input, string hash)
        {
            string hashOfInput = GetSha1Hash(input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
