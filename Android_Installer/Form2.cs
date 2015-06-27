using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Android_Installer
{
    public partial class Form2 : Form
    {
        Point last;

        public Form2()
        {
            InitializeComponent();

            var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");
            DriveInfo Drive = new System.IO.DriveInfo(boot);
            double free = Drive.AvailableFreeSpace;
            long size = 1048576;

            if (File.Exists(boot + @"\Android\data.img"))
            {
                System.IO.FileInfo file = new System.IO.FileInfo(boot + @"\Android\data.img");
                size = file.Length;
            }
            else
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"\Android\OS\data.img"))
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(Directory.GetCurrentDirectory() + @"\Android\OS\data.img");
                    size = file.Length;
                }
            }

            trackBar1.Maximum = Convert.ToInt32((free / 1024 / 1024) + (Convert.ToDouble(size) / 1024 / 1024) - 2048);
            trackBar1.Value = Convert.ToInt32(Convert.ToDouble(size) / 1024 / 1024);
            label1.Text = "Data size: " + trackBar1.Value + " mb";
            trackBar1.TickFrequency = trackBar1.Maximum / 10;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = "Data size: " + trackBar1.Value + " mb";
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

        private void button1_Click(object sender, EventArgs e)
        {
            var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");

            if (File.Exists(boot + @"\Android\data.img"))
            {
                File.Delete(boot + @"\Android\data.img");
            }
            else
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"\Android\OS\data.img"))
                {
                    File.Delete(Directory.GetCurrentDirectory() + @"\Android\OS\data.img");
                }
            }

            Process efi = new Process();
            StreamWriter BatFile2 = new StreamWriter(@"Bin\temp.bat", false, Encoding.GetEncoding(866));
            BatFile2.WriteLine("chcp 1251");
            BatFile2.WriteLine(@"echo %date% %time% >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
            BatFile2.WriteLine(@"echo Data Resize >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
            BatFile2.WriteLine(@"echo ----------------------------- >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
            BatFile2.WriteLine("cd \"" + Directory.GetCurrentDirectory() + @"\Bin"" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
            BatFile2.WriteLine(@"tfile.exe data.img 1024 >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
            BatFile2.WriteLine(@"mke2fs.exe -F data.img >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
            BatFile2.WriteLine(@"resize2fs.exe -p data.img " + (trackBar1.Value * 256) + @" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
            BatFile2.WriteLine(@"echo ----------------------------- >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
            BatFile2.WriteLine(@"del temp.bat");
            BatFile2.Close();
            efi.StartInfo.Verb = "runas";
            efi.StartInfo.FileName = @"Bin\temp.bat";
            efi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            efi.Start();
            efi.WaitForExit();

            if (Directory.Exists(boot + @"\Android"))
            {
                File.Move(Directory.GetCurrentDirectory() + @"\Bin\data.img", boot + @"\Android\data.img");
            }
            else
            {
                if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS"))
                {
                    File.Move(Directory.GetCurrentDirectory() + @"\Bin\data.img", Directory.GetCurrentDirectory() + @"\Android\OS\data.img");
                }
                else
                {
                    if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS") == false)
                        Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Android\OS");

                    File.Move(Directory.GetCurrentDirectory() + @"\Bin\data.img", Directory.GetCurrentDirectory() + @"\Android\OS\data.img");
                }
            }

            MessageBox.Show("Success!");
        }
    }
}
