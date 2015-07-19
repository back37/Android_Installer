using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Android_Installer
{
    public partial class Form3 : Form
    {
        string name = "";
        Point last;
        string sys = "";
        string grub = "";
        LogWriter lw = new LogWriter();

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

        public Form3()
        {
            InitializeComponent();

            radioButton1.Checked = true;
            radioButton2.Enabled = false;
            var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");

            if (Directory.Exists(boot + @"\Android"))
                radioButton2.Enabled = true;
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
            string[] s = { "Android install canceled","-----------------------------","" };
            lw.Write(s);

            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try {var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");
            if (Directory.Exists(boot + @"\android"))
            {
                var dir = new DirectoryInfo(boot);
                foreach (DirectoryInfo fldr in dir.GetDirectories("*ndroid", SearchOption.TopDirectoryOnly))
                {
                    name = fldr.Name;
                }

            }

            string p = "";

            if (Directory.Exists(boot + @"\EFI"))
            {
                p = boot + @"\";
                if (Directory.Exists(boot + @"\Boot"))
                {
                    Directory.Move(boot + @"\Boot", boot + @"\BootS");
                    Directory.Move(boot + @"\BootS", boot + @"\boot");

                    string[] s9 = { "Rename Boot to boot" };
                    lw.Write(s9);
                }
            }
            else
            {
                Process efi = new Process();
                StreamWriter BatFile2 = new StreamWriter(@"Bin\2.bat", false, Encoding.GetEncoding(866));
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
                else
                {
                    MessageBox.Show("Error - EFI not found!");
                    string[] s4 = { "Error - EFI not found!", "-----------------------------", "" };
                    lw.Write(s4);
                    Close();
                    return;
                }
            }


            if (radioButton1.Checked)
            {
                string[] s = { "Full install", "" };
                lw.Write(s);

                string n = @"rEFInd Boot Manager";
                string pt = @"\EFI\refind\refind_ia32.efi";

                Process pc = new Process();
                StreamWriter BatFile1 = new StreamWriter(@"Bin\1.bat", false, Encoding.GetEncoding(866));
                BatFile1.WriteLine("chcp 1251");
                BatFile1.WriteLine(@"echo Disable Bitlocker >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                BatFile1.WriteLine(@"cd %WINDIR%\System32");
                BatFile1.WriteLine(@"cscript %WINDIR%\System32\manage-bde.wsf >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                BatFile1.WriteLine(@"manage-bde -off " + boot + @" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                BatFile1.WriteLine(@"echo.>> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                BatFile1.WriteLine(@"del Bin\1.bat");
                BatFile1.Close();
                pc.StartInfo.Verb = "runas";
                pc.StartInfo.FileName = @"Bin\1.bat";
                pc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                pc.Start();
                pc.WaitForExit();

                if (Directory.Exists(boot + @"\android"))
                {
                    Directory.Delete(boot + @"\android", true);
                }

                if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\Bootloader"))
                {
                    var dir = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\Android\Bootloader");
                    foreach (FileInfo file0 in dir.GetFiles("grub.cfg", SearchOption.AllDirectories))
                    {
                        grub = file0.FullName;

                        if (File.Exists(Directory.GetCurrentDirectory() + @"\Android\OS\system.img") || File.Exists(Directory.GetCurrentDirectory() + @"\Android\OS\system.sfs"))
                        {
                            var dir1 = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\Android\OS");
                            foreach (FileInfo file1 in dir1.GetFiles("system*", SearchOption.AllDirectories))
                            {
                                sys = file1.Name;
                                string str = string.Empty;
                                using (System.IO.StreamReader reader = new System.IO.StreamReader(grub, Encoding.ASCII, true))
                                {
                                    str = reader.ReadToEnd();
                                }
                                str = str.Replace("system.img", sys).Replace("system.sfs", sys).Replace("/android", "/Android");
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(grub, false, Encoding.ASCII))
                                {
                                    file.Write(str);
                                }

                                string[] s1 = { ".cfg path: " + grub, "System name: " + sys, "" };
                                lw.Write(s1);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error - System not found!"); 
                            string[] s4 = { "Error - System not found!", "-----------------------------", "" };
                            lw.Write(s4);
                            Close();
                            return;
                        }
                    }

                    CopyDirectory(Directory.GetCurrentDirectory() + @"\Android\Bootloader", p);

                    if (checkBox1.Checked)
                    {
                        n = textBox1.Text;
                        pt = comboBox1.SelectedItem.ToString();
                    }
                    Process ef = new Process();
                    StreamWriter BatFile3 = new StreamWriter(@"Bin\3.bat", false, Encoding.GetEncoding(866));
                    BatFile3.WriteLine("chcp 1251");
                    BatFile3.WriteLine(@"echo Install booltloader >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile3.WriteLine(@"echo Set path " + pt + @" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile3.WriteLine(@"bcdedit /set {bootmgr} path " + pt + @" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile3.WriteLine(@"echo Set description """ + n + @""" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile3.WriteLine(@"bcdedit /set {bootmgr} description """ + n + @"""" + @" >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile3.WriteLine(@"echo.>> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                    BatFile3.WriteLine(@"del Bin\3.bat");
                    BatFile3.Close();
                    ef.StartInfo.Verb = "runas";
                    ef.StartInfo.FileName = @"Bin\3.bat";
                    ef.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    ef.Start();
                    ef.WaitForExit();
                }
                else
                {
                    MessageBox.Show("Error - Bootloader not found!"); 
                    string[] s4 = { "Error - Bootloader not found!", "-----------------------------", "" };
                    lw.Write(s4);
                    Close();
                    return;
                }

                if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS"))
                { CopyDirectory(Directory.GetCurrentDirectory() + @"\Android\OS", boot + @"\Android"); }
                else
                {
                    MessageBox.Show("Error - OS not found!"); 
                    string[] s4 = { "Error - OS not found!", "-----------------------------", "" };
                    lw.Write(s4);
                    Close();
                    return;
                }

                MessageBox.Show("Success!");
                string[] s3 = { "Install successful", "-----------------------------", "" };
                lw.Write(s3);
                Close();
                return;
            }
            else
            {
                string[] s = { "Update", "" };
                lw.Write(s);

                if (Directory.Exists(p + @"\boot"))
                {
                    var dir = new DirectoryInfo(p + @"\boot");
                    foreach (FileInfo file0 in dir.GetFiles("grub.cfg", SearchOption.AllDirectories))
                    {
                        grub = file0.FullName;


                        if (File.Exists(Directory.GetCurrentDirectory() + @"\Android\OS\system.img") || File.Exists(Directory.GetCurrentDirectory() + @"\Android\OS\system.sfs"))
                        {
                            var dir1 = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\Android\OS");
                            foreach (FileInfo file1 in dir1.GetFiles("system*", SearchOption.AllDirectories))
                            {
                                sys = file1.Name;

                                string str = string.Empty;
                                using (System.IO.StreamReader reader = new System.IO.StreamReader(grub, Encoding.ASCII, true))
                                {
                                    str = reader.ReadToEnd();
                                }
                                str = str.Replace("system.img", sys).Replace("system.sfs", sys).Replace("/android", "/" + name).Replace("/Android", "/" + name);

                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(grub, false, Encoding.ASCII))
                                {
                                    file.Write(str);
                                }

                                string[] s1 = { ".cfg path: " + grub, "System name: " + sys, "" };
                                lw.Write(s1);
                            }
                        }
                    }
                }

                if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS"))
                { CopyDirectory(Directory.GetCurrentDirectory() + @"\Android\OS", boot + @"\Android"); }
                else
                {
                    MessageBox.Show("Error - OS not found!"); 
                    string[] s4 = { "Error - OS not found!", "-----------------------------", "" };
                    lw.Write(s4);
                    return;
                }

                MessageBox.Show("Success!");
                string[] s3 = { "Update successful", "-----------------------------", "" };
                lw.Write(s3);
                Close();
            }
         }
            catch (Exception ex)
            {
                MessageBox.Show("Error!\nMore: log.txt");
                string[] s2 = { "Android install error", ex.ToString(),"" };
                lw.Write(s2);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                checkBox1.Visible = true;
                this.Height = 151;
            }
            else
            {
                checkBox1.Visible = false;
                checkBox1.Checked = false;
                this.Height = 131;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                this.Height = 235;
                if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\Bootloader"))
                {
                    var dir1 = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\Android\Bootloader");
                    foreach (FileInfo file1 in dir1.GetFiles("*.efi", SearchOption.AllDirectories))
                    {
                        string str = file1.FullName;
                        str = str.Replace(Directory.GetCurrentDirectory() + @"\Android\Bootloader", "");
                        comboBox1.Items.Add(str);
                        if (str.EndsWith("refind_ia32.efi"))
                            comboBox1.SelectedItem = str;
                    }
                }
                else
                {
                    MessageBox.Show("Error - Bootloader not found!");
                    string[] s4 = { "Error - Bootloader not found!", "-----------------------------", "" };
                    lw.Write(s4);
                    Close();
                    return;
                }
            }
            else
                this.Height = 151;
        }
    }
}
