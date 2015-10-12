using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading;

namespace Android_Installer
{
    public partial class Main : Form
    {
        Stopwatch stopWatch = new Stopwatch();
        Point last;
        LogWriter lw = new LogWriter();
        string p = "";
        int st = 0;

        public void btnEnable()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            progressBar1.Value = 0;
            progressBar1.Visible = false;
            timer1.Stop();
            st = 0;
            label2.Visible = false;
            stopWatch.Reset();
        }

        public void delete()
        {
            try
            {
                Message M = new Message("Attention!", "Are you sure want to delete Android?", "No", "Yes", null, 2, 0);
                M.ShowDialog(this);

                if (M.fk == 2)
                {
                    stopWatch.Start();

                    progressBar1.Visible = true;
                    
                    string[] s = { "Android delete", "-----------------------------" };
                    lw.Write(s);

                    var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");

                    Process ef = new Process();
                    StreamWriter BatFile3 = new StreamWriter(@"Bin\3.bat", false, Encoding.GetEncoding(1251));
                    BatFile3.WriteLine("chcp 1251");
                    BatFile3.WriteLine(@"echo Delete booltloader");
                    BatFile3.WriteLine(@"echo Set path \EFI\Microsoft\Boot\bootmgfw.efi");
                    BatFile3.WriteLine(@"bcdedit /set {bootmgr} path \EFI\Microsoft\Boot\bootmgfw.efi");
                    BatFile3.WriteLine(@"echo Set description ""Windows Boot Manager""");
                    BatFile3.WriteLine(@"bcdedit /set {bootmgr} description ""Windows Boot Manager""");
                    BatFile3.WriteLine(@"del Bin\3.bat");
                    BatFile3.Close();

                    ef.StartInfo.Verb = "runas";
                    ef.StartInfo.FileName = @"Bin\3.bat";
                    ef.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    ef.StartInfo.RedirectStandardOutput = true;
                    ef.StartInfo.UseShellExecute = false;
                    ef.StartInfo.CreateNoWindow = true;
                    ef.Start();
                    StreamReader srIncoming = ef.StandardOutput;
                    string[] s8 = { srIncoming.ReadToEnd() };
                    lw.Write(s8);
                    ef.WaitForExit();

                    st = (20);

                    Process efi = new Process();

                    if (Directory.Exists(boot + @"\EFI"))
                    {
                        p = boot + @"\";

                        efi.StartInfo.Verb = "runas";
                        efi.StartInfo.FileName = boot + @"\Windows\System32\cmd.exe";
                        efi.StartInfo.Arguments = @"/c dir C:\";
                        efi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        efi.EnableRaisingEvents = true;
                        efi.Start();
                        efi.WaitForExit();
                    }
                    else
                    {
                        StreamWriter BatFile2 = new StreamWriter(@"Bin\2.bat", false, Encoding.GetEncoding(1251));
                        BatFile2.WriteLine("echo off");
                        BatFile2.WriteLine("chcp 1251");
                        BatFile2.WriteLine(@"echo Try to mount S");
                        BatFile2.WriteLine(@"mountvol S: /S ");
                        BatFile2.WriteLine(@"dir S:\");
                        BatFile2.WriteLine(@"echo.");
                        BatFile2.WriteLine(@"del Bin\2.bat");
                        BatFile2.Close();

                        efi.StartInfo.Verb = "runas";
                        efi.StartInfo.FileName = @"Bin\2.bat";
                        efi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        efi.StartInfo.RedirectStandardOutput = true;
                        efi.StartInfo.UseShellExecute = false;
                        efi.StartInfo.CreateNoWindow = true;
                        efi.Start();
                        StreamReader srIncom = efi.StandardOutput;
                        string[] s2 = { srIncom.ReadToEnd() };
                        lw.Write(s2);
                        efi.WaitForExit();

                        if (Directory.Exists(@"S:\EFI"))
                        {
                            p = @"S:\";
                        }
                    }

                    st = (40);

                    if (Directory.Exists(p + @"boot\grub"))
                    {
                        Directory.Delete(p + @"boot\grub", true);

                        string[] s2 = { p + @"boot\grub - deleted" };
                        lw.Write(s2);
                    }

                    if (Directory.Exists(p + @"EFI\grub"))
                    {
                        Directory.Delete(p + @"EFI\grub", true);

                        string[] s4 = { p + @"EFI\grub - deleted" };
                        lw.Write(s4);
                    }
                    if (Directory.Exists(p + @"EFI\refind"))
                    {
                        Directory.Delete(p + @"EFI\refind", true);

                        string[] s5 = { p + @"EFI\refind - deleted" };
                        lw.Write(s5);
                    }
                    if (Directory.Exists(p + @"EFI\tools"))
                    {
                        Directory.Delete(p + @"EFI\tools", true);

                        string[] s6 = { p + @"EFI\tools - deleted" };
                        lw.Write(s6);
                    }

                    st = 60;

                    if (Directory.Exists(boot + @"Android"))
                    {
                        Directory.Delete(boot + @"Android", true);

                        string[] s7 = { boot + @"Android - deleted" };
                        lw.Write(s7);
                    }

                    st = (100);
                    stopWatch.Stop();

                    //MessageBox.Show("Success!");
                    Message M1 = new Message("Success!", "Delete complete", "Ok", null, null, 1, 0);
                    M1.ShowDialog(this);
                    string[] s3 = { "Runtime: " + label2.Text,"","Delete successful", "-----------------------------", "" };
                    lw.Write(s3);
                    btnEnable();
                    Close();
                }
                else
                {
                    stopWatch.Stop();
                    string[] s = { "Delete canceled", "-----------------------------", "" };
                    lw.Write(s);
                    btnEnable();
                }
            }
            catch (Exception ex)
            {
                stopWatch.Stop();
                //MessageBox.Show("Error!\nMore: log.txt");
                Message M = new Message("Program error!", "More info in: log.txt", "Ok", null, null, 1, 10);
                M.ShowDialog(this);
                string[] s2 = { "", "Data resize error", ex.ToString(), "-----------------------------", "" };
                lw.Write(s2);
                btnEnable();
            }
        }

        public Main()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] s = { "Android Install", "-----------------------------" };
            lw.Write(s);
            Install f3 = new Install();
            f3.ShowDialog(this);
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
            string[] s = { "Data Reisze", "-----------------------------" };
            lw.Write(s);
            Form f1 = new Resize();
            f1.ShowDialog(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label2.Visible = true;

            timer1.Start();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            new Thread(() => delete()).Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            string[] s3 = { "Program closed", "", "" };
            lw.Write(s3);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string[] s = { "Config editor", "-----------------------------" };
            lw.Write(s);
            Editor f4 = new Editor();
            f4.ShowDialog(this);
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
