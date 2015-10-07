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
    public partial class Resize : Form
    {
        Point last;
        LogWriter lw = new LogWriter();
        int tb = 0;

        public Resize()
        {
            try
            {
                InitializeComponent();

                var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");
                DriveInfo Drive = new System.IO.DriveInfo(boot);
                double free = Drive.AvailableFreeSpace;
                long size = 1024;

                if (File.Exists(boot + @"\Android\data.img"))
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(boot + @"\Android\data.img");
                    size = file.Length;

                    string[] s = { "Data exists", file.FullName };
                    lw.Write(s);
                }
                else
                {
                    if (File.Exists(Directory.GetCurrentDirectory() + @"\Android\OS\data.img"))
                    {
                        System.IO.FileInfo file = new System.IO.FileInfo(Directory.GetCurrentDirectory() + @"\Android\OS\data.img");
                        size = file.Length;

                        string[] s = { "Data exists", file.FullName };
                        lw.Write(s);
                    }
                    else
                    {
                        string[] s = { "Data not exists" };
                        lw.Write(s);
                        size = 1073741824;
                    }
                }

                int sz = Convert.ToInt32(Convert.ToDouble(size) / 1024 / 1024);
                if (sz < 1)
                    sz = 1;

                trackBar1.Maximum = Convert.ToInt32((free / 1024 / 1024) + sz - 512);
                trackBar1.Value = sz;
                label1.Text = "Data size: " + trackBar1.Value + " mb";
                trackBar1.TickFrequency = trackBar1.Maximum / 10;

                tb = trackBar1.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!\nMore: log.txt");
                string[] s2 = { "Data resize error", ex.ToString(), "-----------------------------", "" };
                lw.Write(s2);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = "Data size: " + trackBar1.Value + " mb";
            tb = trackBar1.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] s = { "Android resize canceled", "-----------------------------","" };
            lw.Write(s);

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
            try
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

                string[] s = { "Set size", trackBar1.Value.ToString() + " mb", "" };
                lw.Write(s);
                if (radioButton1.Checked)
                {
                    Process efi = new Process();
                    StreamWriter BatFile2 = new StreamWriter(@"Bin\temp.bat", false, Encoding.GetEncoding(1251));
                    BatFile2.WriteLine("chcp 1251");
                    BatFile2.WriteLine(@"echo Data Resize >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile2.WriteLine("cd \"" + Directory.GetCurrentDirectory() + @"\Bin"" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile2.WriteLine(@"tfile.exe data.img 1 >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile2.WriteLine(@"mke2fs.exe -F data.img >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile2.WriteLine(@"resize2fs.exe -p data.img " + (trackBar1.Value) * 1024 + @" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile2.WriteLine(@"echo.>> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
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
                }
                else
                {
                    Process efi = new Process();
                    string str = string.Empty;
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(@"Bin\CreateEXT4.bat", true))
                    {
                        str = reader.ReadToEnd();
                    }
                    double ds = tb * 1024 * 1024;
                    ds = Convert.ToDouble(ds.ToString().Replace("-", ""));
                    string rs = "";
                    if (Directory.Exists(boot + @"\Android"))
                    {
                        rs = "\"" + boot + @"\Android\%~n1.img" + "\"";
                    }
                    else
                    {
                        if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS"))
                        {
                            rs = "\"" + Directory.GetCurrentDirectory() + @"\Android\OS\%~n1.img" + "\"";
                        }
                        else
                        {
                            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS") == false)
                                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Android\OS");

                            rs = "\"" + Directory.GetCurrentDirectory() + @"\Android\OS\%~n1.img" + "\"";
                        }
                    }
                    str = str.Replace("size", ds.ToString()).Replace("path", "\"" + Directory.GetCurrentDirectory() + @"\log.txt""").Replace("pl%~n1.img", rs);

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"Bin\temp.bat", false))
                    {
                        file.Write(str);
                    }
                    efi.StartInfo.Verb = "runas";
                    efi.StartInfo.FileName = @"Bin\temp.bat";
                    efi.StartInfo.Arguments = @"data";
                    efi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    efi.Start();
                    efi.WaitForExit();
                }

                MessageBox.Show("Success!");
                string[] s3 = { "Resize successful", "-----------------------------","" };
                lw.Write(s3);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!\nMore: log.txt");
                string[] s2 = { "Data resize error", ex.ToString(), "-----------------------------", "" };
                lw.Write(s2);
            }
        }
    }
}
