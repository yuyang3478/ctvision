using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace ctmeasure
{
    public partial class frmzebra : Form
    {
        private Dictionary<string, string> dini = new Dictionary<string, string>();

        public string tpldata { get; set; }
        public string printdata { get; set; }
        public string ftpl { get; set; }
        public string printername { get; set; }
        
        public frmzebra()
        {
            InitializeComponent();
            tpldata = "";
            printdata = "";
            ftpl = "";
            printername = "";
            loaddata();
        }

        private void frmzebra_Load(object sender, EventArgs e)
        {

        }

        private void loaddata()
        {
            String fn = Application.StartupPath + "\\printer.dat";
            if (!File.Exists(fn)) return;
            StreamReader sr = new StreamReader(fn);
            dini.Clear();
            string[] strs = new string[2] { "", "" };
            string rstr = sr.ReadLine();
            while (rstr != null)
            {
                strs = rstr.Split('=');
                if (strs.Length > 1) dini.Add(strs[0], strs[1]);
                rstr = sr.ReadLine();
            }
            sr.Close();

            ftpl = dini["tpl"];
            printername = dini["printer"];
            if (ftpl != "") loadtpl();
        }

        private void loadtpl()
        {
            string fn = ftpl.Trim();
            if (fn == "") return;
            if (!File.Exists(fn)) return;
            StreamReader sr = new StreamReader(fn);
            tpldata = sr.ReadToEnd();
            sr.Close();
        }

        private void savedata()
        {
            string fn = Application.StartupPath + "\\printer.dat";
            dini["tpl"] = ftpl;
            dini["printer"] = printername;
            StreamWriter fw = new StreamWriter(fn);
            foreach (string key in dini.Keys)
            {
                fw.WriteLine(string.Format("{0}={1}", key, dini[key]));
            }
            fw.Flush();
            fw.Close();
        }

        public void print()
        {
            if (printdata == "") return;
            if (printername == "") return;
            Printer.SendStringToPrinter(printername, printdata);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dlgprint.ShowDialog() == DialogResult.OK)
            {
                printername = dlgprint.PrinterSettings.PrinterName;
                tprinter.Text = printername;
                savedata();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dlgopen.ShowDialog() == DialogResult.OK)
            {
                ftpl = dlgopen.FileName;
                loadtpl();
                txtprint.Text = tpldata;
                savedata();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            savedata();
            this.Close();
        }

        private void frmzebra_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                txtprint.Text = tpldata;
                ttpl.Text = ftpl;
                tprinter.Text = printername;
            }
        }

        private void frmzebra_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }

    //指定斑马打印机名称来打印，不区分并口，USB等
    public static class Printer
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);


        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }


        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
    }

}
