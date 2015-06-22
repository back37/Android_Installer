using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Drawing;

namespace Android_Installer
{
    public partial class Form1 : Form
    {
        Point last;

        public Form1()
        {
            InitializeComponent();
        }

        public void CopyDirectory(string strSource, string strDestination)
        {
            if (!Directory.Exists(strDestination))
            {
                Directory.CreateDirectory(strDestination);
            }
            DirectoryInfo dirInfo = new DirectoryInfo(strSource);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo tempfile in files)
            {
                if (File.Exists(strDestination + "\\" + tempfile.Name) == false)
                {
                    tempfile.CopyTo(Path.Combine(strDestination, tempfile.Name));
                }
                else
                {
                    File.Delete(strDestination + "\\" + tempfile.Name);
                    tempfile.CopyTo(Path.Combine(strDestination, tempfile.Name));
                }
            }
            DirectoryInfo[] dirctororys = dirInfo.GetDirectories();
            foreach (DirectoryInfo tempdir in dirctororys)
            {
                CopyDirectory(Path.Combine(strSource, tempdir.Name), Path.Combine(strDestination, tempdir.Name));
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var win = Environment.ExpandEnvironmentVariables(@"%WINDIR%");
            var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");
            string p = "";

            Process pc = new Process();
            StreamWriter BatFile1 = new StreamWriter(win + @"\temp.bat", false, Encoding.GetEncoding(866));
            BatFile1.WriteLine(@"echo off");
            BatFile1.WriteLine(@"cd %WINDIR%\System32");
            BatFile1.WriteLine(@"cscript manage-bde.wsf");
            BatFile1.WriteLine(@"manage-bde -off " + boot);
            BatFile1.WriteLine(@"del %WINDIR%\temp.bat");
            BatFile1.Close();
            pc.StartInfo.Verb = "runas";
            pc.StartInfo.FileName = win + @"\temp.bat";
            //pc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pc.Start();
            pc.WaitForExit();

            if (Directory.Exists(boot + @"\EFI"))
            {
                p = @"C:\";
            }
            else
            {
                Process efi = new Process();
                StreamWriter BatFile2 = new StreamWriter(win + @"\temp.bat", false, Encoding.GetEncoding(866));
                BatFile2.WriteLine(@"mountvol S: /S");
                BatFile2.WriteLine(@"del %WINDIR%\temp.bat");
                BatFile2.Close();
                efi.StartInfo.Verb = "runas";
                efi.StartInfo.FileName = win + @"\temp.bat";
                efi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                efi.Start();
                efi.WaitForExit();

                if (Directory.Exists(@"S:\EFI"))
                {
                    p = @"S:\";
                }
                else
                {
                    MessageBox.Show("EFI not found");
                    return;
                }
            }
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\Bootloader"))
            {   
                CopyDirectory(Directory.GetCurrentDirectory() + @"\Android\Bootloader", p);

                Process ef = new Process();
                StreamWriter BatFile3 = new StreamWriter(win + @"\temp.bat", false, Encoding.GetEncoding(866));
                BatFile3.WriteLine(@"bcdedit /set {bootmgr} path \EFI\refind\refind_ia32.efi");
                BatFile3.WriteLine(@"bcdedit /set {bootmgr} description ""rEFInd Boot Manager""");
                BatFile3.Close();
                ef.StartInfo.Verb = "runas";
                ef.StartInfo.FileName = win + @"\temp.bat";
                ef.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ef.Start();
                ef.WaitForExit();
            }
            else { MessageBox.Show("Bootloader not found"); return; }

            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS"))
            {CopyDirectory(Directory.GetCurrentDirectory() + @"\Android\OS", boot + @"\Android");}
            else { MessageBox.Show("OS not found"); return; }

            MessageBox.Show("Success!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                last = MousePosition;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point cur = MousePosition;
                int dx = cur.X - last.X;
                int dy = cur.Y - last.Y;
                Point loc = new Point(Location.X + dx, Location.Y + dy);
                Location = loc;
                last = cur;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form f1 = new Form2();
            f1.ShowDialog();
        }
    }
}
