using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Android_Installer
{
    public partial class Resize : Form
    {
        Stopwatch stopWatch = new Stopwatch();
        Point last;
        LogWriter lw = new LogWriter();
        long tb = 0;
        int st = 0;
        string boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");

        public void btnEnable()
        {
            button1.Enabled = true;
            button3.Enabled = true;
            progressBar1.Visible = false;
            progressBar1.Value = 0;
            timer1.Stop();
            st = 0;
            label2.Visible = false;
            stopWatch.Reset();
        }

        public Resize()
        {
            try
            {
                InitializeComponent();

                label1.MouseDown += new MouseEventHandler(this.Form1_MouseDown);
                label1.MouseMove += new MouseEventHandler(this.Form1_MouseMove);
                label2.MouseDown += new MouseEventHandler(this.Form1_MouseDown);
                label2.MouseMove += new MouseEventHandler(this.Form1_MouseMove);
                progressBar1.MouseDown += new MouseEventHandler(this.Form1_MouseDown);
                progressBar1.MouseMove += new MouseEventHandler(this.Form1_MouseMove);
                radioButton1.MouseDown += new MouseEventHandler(this.Form1_MouseDown);
                radioButton1.MouseMove += new MouseEventHandler(this.Form1_MouseMove);
                radioButton2.MouseDown += new MouseEventHandler(this.Form1_MouseDown);
                radioButton2.MouseMove += new MouseEventHandler(this.Form1_MouseMove);

                CheckForIllegalCrossThreadCalls = false;

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
                //MessageBox.Show("Error!\nMore: log.txt");
                Message M = new Message("Program error!", "More info in: log.txt", "Ok", null, null, 1, 30);
                M.ShowDialog(this);
                string[] s2 = { "Data resize error", ex.ToString(), "-----------------------------", "" };
                lw.Write(s2);
            }
        }

        public void start()
        {
            Process efi = new Process();
            
            efi.StartInfo.Verb = "runas";
            efi.StartInfo.FileName = @"Bin\temp.bat";
            efi.StartInfo.Arguments = @"data";
            efi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            efi.EnableRaisingEvents = true;
            try
            {
                efi.Start();
                efi.WaitForExit();
            }
            finally
            {
                efi.Close();
            }
        }
        public void resize_proc()
        {
            int er = 0;
            try
            {
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

                st = (10);

                string[] s = { "Set size: " + trackBar1.Value.ToString() + "mb" };
                lw.Write(s);

                if (radioButton1.Checked)
                {
                    string[] s5 = { "Data format: ext3","" };
                    lw.Write(s5);

                    if (Directory.Exists("Bin\\data") == false)
                        Directory.CreateDirectory("Bin\\data");

                    if (File.Exists("Bin\\cygwin1.dll.ext3"))
                    {
                        if (File.Exists("Bin\\cygwin1.dll"))
                            File.Move("Bin\\cygwin1.dll", "Bin\\cygwin1.dll.ext4");

                        File.Move("Bin\\cygwin1.dll.ext3", "Bin\\cygwin1.dll");
                    }

                    StreamWriter BatFile2 = new StreamWriter(@"Bin\temp.bat", false, Encoding.GetEncoding(1251));
                    BatFile2.WriteLine(@"@echo off > nul");
                    BatFile2.WriteLine("@chcp 1251 > nul");
                    BatFile2.WriteLine(@"echo Data Resize >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile2.WriteLine("cd \"" + Directory.GetCurrentDirectory() + @"\Bin"" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile2.WriteLine(@"tfile.exe data.img 1 >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile2.WriteLine(@"mke2fs.exe -F data.img >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile2.WriteLine(@"resize2fs.exe -p data.img " + (trackBar1.Value) * 1024 + @" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile2.WriteLine(@"echo.>> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile2.WriteLine(@"del temp.bat");
                    BatFile2.Close();

                    st = (40);
                    

                    Thread newThread = new Thread(start);
                    newThread.Start();
                    newThread.Join();

                    st = (70);

                    if (Directory.Exists(boot + @"\Android"))
                    {
                        File.Move(Directory.GetCurrentDirectory() + @"\Bin\data.img", boot + @"\Android\data.img");

                        FileInfo file = new FileInfo(boot + @"\Android\data.img");
                        long size = file.Length;
                        int sz = Convert.ToInt32(Convert.ToDouble(size) / 1024 / 1024);
                        if (sz < trackBar1.Value)
                            er = 1;
                    }
                    else
                    {
                        if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS"))
                        {
                            File.Move(Directory.GetCurrentDirectory() + @"\Bin\data.img", Directory.GetCurrentDirectory() + @"\Android\OS\data.img");

                            FileInfo file = new FileInfo(Directory.GetCurrentDirectory() + @"\Android\OS\data.img");
                            long size = file.Length;
                            int sz = Convert.ToInt32(Convert.ToDouble(size) / 1024 / 1024);
                            if (sz < trackBar1.Value)
                                er = 1;
                        }
                        else
                        {
                            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS") == false)
                                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Android\OS");

                            File.Move(Directory.GetCurrentDirectory() + @"\Bin\data.img", Directory.GetCurrentDirectory() + @"\Android\OS\data.img");

                            FileInfo file = new FileInfo(Directory.GetCurrentDirectory() + @"\Android\OS\data.img");
                            long size = file.Length;
                            int sz = Convert.ToInt32(Convert.ToDouble(size) / 1024 / 1024);
                            if (sz < trackBar1.Value)
                                er = 1;
                        }
                    }
                    st = (100);
                }
                else
                {
                    string[] s5 = { "Data format: ext4","" };
                    lw.Write(s5);

                    if (Directory.Exists("Bin\\data") == false)
                        Directory.CreateDirectory("Bin\\data");

                    if (File.Exists("Bin\\cygwin1.dll.ext4"))
                    {
                        if (File.Exists("Bin\\cygwin1.dll"))
                            File.Move("Bin\\cygwin1.dll", "Bin\\cygwin1.dll.ext3");

                        File.Move("Bin\\cygwin1.dll.ext4", "Bin\\cygwin1.dll");
                    }

                    Process efi = new Process();
                    string str = string.Empty;
                    using (StreamReader reader = new StreamReader(@"Bin\CreateEXT4.bat", true))
                    {
                        str = reader.ReadToEnd();
                    }
                    long ds = tb * 1024 * 1024;
                    ds = Convert.ToInt64(ds.ToString().Replace("-", ""));
                    string rs = "";

                    st = (20);

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

                    st = (40);

                    using (StreamWriter file = new StreamWriter(@"Bin\temp.bat", false))
                    {
                        file.Write(str);
                    }
                    
                    st = (60);

                    Thread newThread = new Thread(start);
                    newThread.Start();
                    newThread.Join();

                    if (Directory.Exists(boot + @"\Android"))
                    {
                        if (File.Exists(boot + @"\Android\data.img"))
                        {
                            FileInfo file = new FileInfo(boot + @"\Android\data.img");
                            long size = file.Length;
                            int sz = Convert.ToInt32(Convert.ToDouble(size) / 1024 / 1024);
                            if (sz < trackBar1.Value)
                                er = 1;
                        }
                    }
                    else
                    {
                        if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS"))
                        {
                            if (File.Exists(Directory.GetCurrentDirectory() + @"\Android\OS\data.img"))
                            {
                                FileInfo file = new FileInfo(Directory.GetCurrentDirectory() + @"\Android\OS\data.img");
                                long size = file.Length;
                                int sz = Convert.ToInt32(Convert.ToDouble(size) / 1024 / 1024);
                                if (sz < trackBar1.Value)
                                    er = 1;
                            }
                        }
                        else
                        {
                            er = 1;
                        }
                    }

                    st = (100);
                }
                stopWatch.Stop();
                if (Directory.Exists("Bin\\data"))
                    Directory.Delete("Bin\\data",true);
                //MessageBox.Show("Success!");
                if (er != 1)
                {
                    Message M1 = new Message("Success!", "Resize complete", "Ok", null, null, 1, 10);
                    M1.ShowDialog(this);
                    string[] s3 = { "", "Runtime: " + label2.Text, "", "Resize successful", "-----------------------------", "" };
                    lw.Write(s3);

                    Close();
                }
                else
                {
                    Message M = new Message("Attention!", "New data size is wrong\nMore info in: log.txt", "Ok", null, null, 1, 10);
                    M.ShowDialog(this);
                    string[] s2 = { "Data resize problem", "Something went wrong", "-----------------------------", "" };
                    lw.Write(s2);
                }
                progressBar1.Value = 0;
                btnEnable();
            }
            catch (Exception ex)
            {
                stopWatch.Stop();
                //MessageBox.Show("Error!\nMore: log.txt");
                Message M = new Message("Program error!", "More info in: log.txt", "Ok", null, null, 1, 30);
                M.ShowDialog(this);
                string[] s2 = { "Data resize error", ex.ToString(), "-----------------------------", "" };
                lw.Write(s2);
                btnEnable();
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
            label2.Visible = true;
            stopWatch.Start();
            
            timer1.Start();
            progressBar1.Visible = true;
            button1.Enabled = false;
            button3.Enabled = false;
            new Thread(() => resize_proc()).Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < st)
                progressBar1.Value = st;

            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            label2.Text = elapsedTime;
        }
    }
}
