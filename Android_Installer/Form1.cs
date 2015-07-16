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
        LogWriter lw = new LogWriter();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] s = { "Android Install", "-----------------------------" };
            lw.Write(s);
            Form3 f3 = new Form3();
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
            Form f1 = new Form2();
            f1.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you sure want to delete Android?", "Attention!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string[] s = { "", "Android delete", "-----------------------------", "Started" };
                    lw.Write(s);

                    var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");

                    Process ef = new Process();
                    StreamWriter BatFile3 = new StreamWriter(@"Bin\3.bat", false, Encoding.GetEncoding(866));
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

                    if (Directory.Exists(boot + @"\Android"))
                        Directory.Delete(boot + @"\Android", true);

                    MessageBox.Show("Success!");
                    string[] s3 = { "", "Delete successful" };
                    lw.Write(s3);
                }
                else
                {
                    string[] s = { "", "Android delete", "Canceled" };
                    lw.Write(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!\nMore: log.txt");
                string[] s2 = { "", "Data resize error", ex.ToString(),"" };
                lw.Write(s2);
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
