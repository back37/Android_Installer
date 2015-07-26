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
    public partial class Main : Form
    {
        Point last;
        LogWriter lw = new LogWriter();
        string p = "";

        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] s = { "Android Install", "-----------------------------" };
            lw.Write(s);
            Install f3 = new Install();
            f3.ShowDialog();
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
            f1.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you sure want to delete Android?", "Attention!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string[] s = { "Android delete", "-----------------------------" };
                    lw.Write(s);

                    var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");

                    Process ef = new Process();
                    StreamWriter BatFile3 = new StreamWriter(@"Bin\3.bat", false, Encoding.GetEncoding(1251));
                    BatFile3.WriteLine("chcp 1251");
                    BatFile3.WriteLine(@"echo Delete booltloader >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile3.WriteLine(@"echo Set path \EFI\Microsoft\Boot\bootmgfw.efi >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile3.WriteLine(@"bcdedit /set {bootmgr} path \EFI\Microsoft\Boot\bootmgfw.efi >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile3.WriteLine(@"echo Set description ""Windows Boot Manager"" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile3.WriteLine(@"bcdedit /set {bootmgr} description ""Windows Boot Manager"" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile3.WriteLine(@"del Bin\3.bat");
                    BatFile3.Close();
                    ef.StartInfo.Verb = "runas";
                    ef.StartInfo.FileName = @"Bin\3.bat";
                    ef.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    ef.Start();
                    ef.WaitForExit();

                    if (Directory.Exists(boot + @"\EFI"))
                    {
                        p = boot + @"\";
                    }
                    else
                    {
                        Process efi = new Process();
                        StreamWriter BatFile2 = new StreamWriter(@"Bin\2.bat", false, Encoding.GetEncoding(1251));
                        BatFile2.WriteLine("chcp 1251");
                        BatFile2.WriteLine(@"echo Try to mount S >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                        BatFile2.WriteLine(@"mountvol S: /S ");
                        BatFile2.WriteLine(@"dir S:\ >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                        BatFile2.WriteLine(@"echo.>> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                        BatFile2.WriteLine(@"del Bin\2.bat");
                        BatFile2.Close();
                        efi.StartInfo.Verb = "runas";
                        efi.StartInfo.FileName = @"Bin\2.bat";
                        efi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        efi.Start();
                        efi.WaitForExit();

                        if (Directory.Exists(@"S:\EFI"))
                        {
                            p = @"S:\";
                        }
                    }

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

                    if (Directory.Exists(boot + @"Android"))
                    {
                        Directory.Delete(boot + @"Android", true);

                        string[] s7 = { boot + @"Android - deleted" };
                        lw.Write(s7);
                    }

                    MessageBox.Show("Success!");
                    string[] s3 = { "Delete successful", "-----------------------------", "" };
                    lw.Write(s3);
                }
                else
                {
                    string[] s = { "Delete canceled", "-----------------------------", "" };
                    lw.Write(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!\nMore: log.txt");
                string[] s2 = { "", "Data resize error", ex.ToString(), "-----------------------------", "" };
                lw.Write(s2);
                Close();
            }
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
            f4.ShowDialog();
        }
    }
}
