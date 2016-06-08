using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Android_Installer
{
    public class EfiNotExistException : Exception
    {

    }

    public partial class Install : Form
    {

        public void unpack(string p)
        {
            Process pc = new Process();
            StreamWriter BatFile1 = new StreamWriter(@"Bin\1.bat", false, Encoding.GetEncoding(1251));
            BatFile1.WriteLine("@echo off > nul");
            BatFile1.WriteLine("@chcp 1251 > nul");
            BatFile1.WriteLine("Bin\\7z.exe e \"" + p + "\" -o" + boot + "\\Android *system* *install.img *initrd.img *kernel *ramdisk.img");
            if (checkBox2.Checked)
                BatFile1.Write(" *data.img");
			//BatFile1.Write(" >> \"" + Directory.GetCurrentDirectory() + @"\log.txt""");
            BatFile1.WriteLine("echo.");
            BatFile1.WriteLine(@"del Bin\1.bat");
            BatFile1.Close();

            pc.StartInfo.Verb = "runas";
            pc.StartInfo.FileName = @"Bin\1.bat";
            pc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pc.EnableRaisingEvents = true;
            pc.StartInfo.RedirectStandardOutput = true;
            pc.StartInfo.UseShellExecute = false;
            pc.StartInfo.CreateNoWindow = true;
            pc.Start();
            StreamReader srIncoming = pc.StandardOutput;
            string[] s6 = { srIncoming.ReadToEnd() };
            lw.Write(s6);
            pc.WaitForExit();

            if (File.Exists(boot + @"\Android\data.img") == false && checkBox2.Checked)
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"\Android\OS\data.img"))
                    File.Copy(Directory.GetCurrentDirectory() + @"\Android\OS\data.img", boot + @"\Android\data.img", true);
                else
                {
                    stopWatch.Stop();
                    Message M1 = new Message("Attention!", "Data not found\nPlease make data resize after install", "Ok", null, null, 1, 30);
                    M1.ShowDialog(this);
                    string[] s4 = { "Attention - Data not found!", "" };
                    lw.Write(s4);
                    stopWatch.Start();
                }
            }
                    
        }

        public void btnEnable()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            progressBar1.Visible = false;
            comboBox1.Enabled = true;
            textBox1.Enabled = true;
            progressBar1.Value = 0;
            timer1.Stop();
            st = 0;
            label3.Visible = false;
            stopWatch.Reset();
        }

        public void Finstall()
        {
            string[] s = { "Full install", "" };
            lw.Write(s);

            Process pc = new Process();
            StreamWriter BatFile1 = new StreamWriter(@"Bin\1.bat", false, Encoding.GetEncoding(1251));
            BatFile1.WriteLine("@echo off > nul");
            BatFile1.WriteLine("@chcp 1251 > nul");
            BatFile1.WriteLine(@"echo Disable Bitlocker");
            BatFile1.WriteLine(@"cd %WINDIR%\System32");
            BatFile1.WriteLine(@"cscript %WINDIR%\System32\manage-bde.wsf");
            BatFile1.WriteLine(@"manage-bde -off " + boot);
            BatFile1.WriteLine(@"echo.");
            BatFile1.WriteLine(@"del Bin\1.bat");
            BatFile1.Close();

            pc.StartInfo.Verb = "runas";
            pc.StartInfo.FileName = @"Bin\1.bat";
            pc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pc.EnableRaisingEvents = true;
            pc.StartInfo.RedirectStandardOutput = true;
            pc.StartInfo.UseShellExecute = false;
            pc.StartInfo.CreateNoWindow = true;
            pc.Start();
            StreamReader srIncoming = pc.StandardOutput;
            string[] s6 = { srIncoming.ReadToEnd() };
            lw.Write(s6);
            pc.WaitForExit();

            st = 30;

            if (Directory.Exists(boot + @"\Android"))
            {
                if (checkBox2.Checked == false)
                {
                    if (File.Exists(boot + @"\Android\data.img"))
                        File.Move(boot + @"\Android\data.img", boot + @"\data.img");

                    Directory.Delete(boot + @"\Android", true);
                    if (File.Exists(boot + @"\data.img"))
                        File.Move(boot + @"\data.img", boot + @"\Android\data.img");
                }
                else
                    Directory.Delete(boot + @"\Android", true);
            }

            st = 40;

            string ph = boot + @"\Android";

            if (pl != "default" && pl != "" && pl != null)
            {
                if (File.Exists(pl))
                    unpack(pl);
                else
                {
                    string[] s9 = { "File not exist, default android install", "" };
                    lw.Write(s9);
                }
            }
            else {
                if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS"))
                { CopyDirectory(Directory.GetCurrentDirectory() + @"\Android\OS", boot + @"\Android", true); }
                else
                {
                    stopWatch.Stop();
                    //MessageBox.Show("Error - OS not found!");
                    Message M1 = new Message("Error!", "OS not found", "Ok", null, null, 1, 30);
                    M1.ShowDialog(this);
                    string[] s4 = { "Error - OS not found!", "-----------------------------", "" };
                    lw.Write(s4);

                    btnEnable();

                    Close();
                    return;
                }
            }

            st = 70;

            instBoot(ph);

            st = 100;
            stopWatch.Stop();

            //MessageBox.Show("Success!");
            Message M = new Message("Success!", "Android install complete", "Ok", null, null, 1, 10);
            M.ShowDialog(this);
            string[] s3 = { "Runtime: " + label3.Text, "","Install successful", "-----------------------------", "" };
            lw.Write(s3);

            btnEnable();

            Close();
            return;
        }

        public void UpdateA()
        {
            string[] s = { "Update Android", "" };
            lw.Write(s);

            if (Directory.Exists(boot + @"\Android"))
            {
                if (checkBox2.Checked == false)
                {
                    if (File.Exists(boot + @"\Android\data.img"))
                        File.Move(boot + @"\Android\data.img", boot + @"\data.img");

                    Directory.Delete(boot + @"\Android", true);
                }
                else
                    Directory.Delete(boot + @"\Android", true);
            }

            if (Directory.Exists(boot + @"\Android") == false)
                Directory.CreateDirectory(boot + @"\Android");

            if (pl != "default" && pl != "" && pl != null)
            {
                if (File.Exists(pl))
                    unpack(pl);
                else
                {
                    string[] s9 = { "File not exist, default android update", "" };
                    lw.Write(s9);
                }
            }
            else {
                if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\OS"))
                { CopyDirectory(Directory.GetCurrentDirectory() + @"\Android\OS", boot + @"\Android", checkBox2.Checked); }
                else
                {
                    stopWatch.Stop();
                    //MessageBox.Show("Error - OS not found!");
                    Message M1 = new Message("Error!", "OS not found", "Ok", null, null, 1, 30);
                    M1.ShowDialog(this);
                    string[] s4 = { "Error - OS not found!", "-----------------------------", "" };
                    lw.Write(s4);

                    btnEnable();

                    return;
                }
            }

            if (File.Exists(boot + @"\data.img"))
            {
                File.Move(boot + @"\data.img", boot + @"\Android\data.img");
            }

            st = 40;

            if (Directory.Exists(p + @"\boot"))
            {
                var dir = new DirectoryInfo(p + @"\boot");
                foreach (FileInfo file0 in dir.GetFiles("grub.cfg", SearchOption.AllDirectories))
                {
                    grub = file0.FullName;


                    if (File.Exists(boot + @"\Android\system.img") || File.Exists(boot + @"\Android\system.sfs"))
                    {
                        var dir1 = new DirectoryInfo(boot + @"\Android");
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

            st = 100;
            stopWatch.Stop();

            //MessageBox.Show("Success!");
            Message M = new Message("Success!", "Android update complete", "Ok", null, null, 1, 10);
            M.ShowDialog(this);
            string[] s3 = { "Runtime: " + label3.Text, "","Update successful", "-----------------------------", "" };
            lw.Write(s3);

            btnEnable();

            Close();
        }

        public void UpdateB()
        {
            string[] s = { "Update bootloader", "" };
            lw.Write(s);

            string ph = boot + @"\Android";
            instBoot(ph);

            st = 100;
            stopWatch.Stop();

            //MessageBox.Show("Success!");
            Message M = new Message("Success!", "Bootloader update complete", "Ok", null, null, 1, 10);
            M.ShowDialog(this);
            string[] s3 = { "Runtime: " + label3.Text, "","Update successful", "-----------------------------", "" };
            lw.Write(s3);

            btnEnable();

            Close();
        }

        public void FindBoot()
        {
            var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");

            if (Directory.Exists(boot + @"\EFI"))
            {
                p = boot + @"\";

                Process efi = new Process();
                efi.StartInfo.Verb = "runas";
                efi.StartInfo.FileName = boot + @"\Windows\System32\cmd.exe";
                efi.StartInfo.Arguments = @"/c dir C:";
                efi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                efi.EnableRaisingEvents = true;
                efi.Start();
                efi.WaitForExit();

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
                MountS();

                if (Directory.Exists(@"S:\EFI"))
                {
                    p = @"S:\";
                }
                else
                {
                    //MessageBox.Show("Error - EFI not found!");
                    Message M1 = new Message("Error!", "EFI not found", "Ok", null, null, 1, 30);
                    M1.ShowDialog(this);
                    string[] s4 = { "Error - EFI not found!", "-----------------------------", "" };
                    lw.Write(s4);
                    Close();
                    throw new EfiNotExistException();
                    return;
                }
            }
        }

        public void MountS()
        {
            Process efi = new Process();
            StreamWriter BatFile2 = new StreamWriter(@"Bin\2.bat", false, Encoding.GetEncoding(1251));
            BatFile2.WriteLine("@echo off > nul");
            BatFile2.WriteLine("@chcp 1251 > nul");
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
            StreamReader srIncoming = efi.StandardOutput;
            string[] s = { srIncoming.ReadToEnd() };
            lw.Write(s);
            efi.WaitForExit();
        }

        public void instBoot(string path)
        {
            try {
                string n = @"rEFInd Boot Manager";
                string pt = @"\EFI\refind\refind_ia32.efi";
                
                if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\Bootloader"))
                {
                    DirectoryInfo dir;

                    if (pl.Contains("remix") || pl.Contains("Remix") || pl.Contains("REMIX"))
                    {
                        string[] st = { "RemixOS build founded" };
                        lw.Write(st);

                        if (File.Exists(Directory.GetCurrentDirectory() + @"\Android\Remix\grub.cfg"))
                        {
                            string[] st1 = { "RemixOS grub.cfg founded" };
                            lw.Write(st1);
                            dir = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\Android\Remix");
                        }
                        else
                            dir = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\Android\Bootloader");
                    }
                    else
                        dir = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\Android\Bootloader");

                    foreach (FileInfo file0 in dir.GetFiles("grub.cfg", SearchOption.AllDirectories))
                    {
                        grub = file0.FullName;

                        if (File.Exists(Directory.GetCurrentDirectory() + @"\Android\Remix\grub.cfg") && (pl.Contains("remix") || pl.Contains("Remix") || pl.Contains("REMIX")))
                        {
                            re = grub;
                        }

                        if (File.Exists(path + @"\system.img") || File.Exists(path + @"\system.sfs"))
                        {
                            var dir1 = new DirectoryInfo(path);
                            foreach (FileInfo file1 in dir1.GetFiles("system*", SearchOption.AllDirectories))
                            {
                                sys = file1.Name;
                                string str = string.Empty;
                                using (System.IO.StreamReader reader = new System.IO.StreamReader(grub, Encoding.ASCII, true))
                                {
                                    str = reader.ReadToEnd();
                                }
                                if (File.Exists(path + @"\system.img") && File.Exists(path + @"\system.sfs"))
                                {
                                    DateTime D1 = File.GetLastWriteTime(path + @"\system.img");
                                    DateTime D2 = File.GetLastWriteTime(path + @"\system.sfs");
                                    if (D1 > D2)
                                    {
                                        sys = "system.sfs";
                                        str = str.Replace("system.img", sys).Replace("/android", "/Android");
                                    }
                                    else
                                    {
                                        sys = "system.img";
                                        str = str.Replace("system.sfs", sys).Replace("/android", "/Android");
                                    }
                                }
                                else {
                                    str = str.Replace("system.img", sys).Replace("system.sfs", sys).Replace("/android", "/Android");
                                }

                                if ((pl.Contains("x64") || pl.Contains("X64"))&& (pl.Contains("remix") == false || pl.Contains("Remix") == false || pl.Contains("REMIX") == false))
                                    str = str.Replace("android_x86_64", "android_x86").Replace("android_x86", "android_x86_64");

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
                            //MessageBox.Show("Error - System not found!");
                            Message M1 = new Message("Error!", "System not found", "Ok", null, null, 1, 30);
                            M1.ShowDialog(this);
                            string[] s4 = { "Error - System not found!", "-----------------------------", "" };
                            lw.Write(s4);
                            Close();
                            return;
                        }
                    }

                    CopyDirectory(Directory.GetCurrentDirectory() + @"\Android\Bootloader", p,true);

                    if (File.Exists(Directory.GetCurrentDirectory() + @"\Android\Remix\grub.cfg") && (pl.Contains("remix") || pl.Contains("Remix") || pl.Contains("REMIX")))
                    {
                        var dr = new DirectoryInfo(p + "\\boot");
                        foreach (FileInfo fileD in dr.GetFiles("grub.cfg", SearchOption.AllDirectories))
                        {
                            File.Copy(re, fileD.FullName, true);
                        }
                    }

                    if (checkBox1.Checked)
                    {
                        n = textBox1.Text;
                        pt = comboBox1.SelectedItem.ToString();
                    }

                    Process ef = new Process();
                    StreamWriter BatFile3 = new StreamWriter(@"Bin\3.bat", false, Encoding.GetEncoding(1251));
                    BatFile3.WriteLine("@echo off > nul");
                    BatFile3.WriteLine("@chcp 1251 > nul");
                    BatFile3.WriteLine(@"echo Install booltloader");
                    BatFile3.WriteLine(@"echo Set path " + pt);
                    BatFile3.WriteLine(@"bcdedit /set {bootmgr} path " + pt);
                    BatFile3.WriteLine(@"echo Set description """ + n);
                    BatFile3.WriteLine(@"bcdedit /set {bootmgr} description """ + n);
                    BatFile3.WriteLine(@"echo.");
                    BatFile3.WriteLine(@"del Bin\3.bat");
                    BatFile3.Close();

                    ef.StartInfo.Verb = "runas";
                    ef.StartInfo.FileName = @"Bin\3.bat";
                    ef.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    ef.EnableRaisingEvents = true;
                    ef.StartInfo.RedirectStandardOutput = true;
                    ef.StartInfo.UseShellExecute = false;
                    ef.StartInfo.CreateNoWindow = true;
                    ef.Start();
                    StreamReader srIncoming = ef.StandardOutput;
                    string[] sl = { srIncoming.ReadToEnd() };
                    lw.Write(sl);
                    ef.WaitForExit();
                }
                else
                {
                    //MessageBox.Show("Error - Bootloader not found!");
                    Message M = new Message("Error!", "Bootloader not found", "Ok", null, null, 1, 30);
                    M.ShowDialog(this);
                    string[] s4 = { "Error - Bootloader not found!", "-----------------------------", "" };
                    lw.Write(s4);
                    Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error!\nMore: log.txt");
                Message M = new Message("Program error!", "More info in: log.txt", "Ok", null, null, 1, 30);
                M.ShowDialog(this);
                string[] s2 = { "Android install error", ex.ToString(), "-----------------------------", "" };
                lw.Write(s2);
                throw new EfiNotExistException();
            }
        }
        
        Stopwatch stopWatch = new Stopwatch();
        string name = "";
        Point last;
        string sys = "";
        string grub = "";
        string p = "";
        string re = "";
        int st = 0;
        string pl = "";
        LogWriter lw = new LogWriter();
        string boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");

        public void CopyDirectory(string strSource, string strDestination, bool ch)
        {
            if (!Directory.Exists(strDestination))
            {
                Directory.CreateDirectory(strDestination);
            }
            DirectoryInfo dirInfo = new DirectoryInfo(strSource);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo tempfile in files)
            {
                if (ch == false)
                {
                    if (tempfile.Name.EndsWith("data.img") == false)
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
                }
                else {
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
            }
            DirectoryInfo[] dirctororys = dirInfo.GetDirectories();
            foreach (DirectoryInfo tempdir in dirctororys)
            {
                CopyDirectory(Path.Combine(strSource, tempdir.Name), Path.Combine(strDestination, tempdir.Name), ch);
            }

        }

        public Install(string txt)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            groupBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            groupBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            groupBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            groupBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            label2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            progressBar1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            progressBar1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            radioButton1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            radioButton1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            radioButton2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            radioButton2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            radioButton3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            radioButton3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);

            radioButton1.Checked = true;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");

            if (Directory.Exists(boot + @"\Android"))
            {
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
            }

            pl = txt;
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
            try {
            if (Directory.Exists(boot + @"\android"))
            {
                var dir = new DirectoryInfo(boot);
                foreach (DirectoryInfo fldr in dir.GetDirectories("*ndroid", SearchOption.TopDirectoryOnly))
                {
                    name = fldr.Name;
                }

            }
                progressBar1.Visible = true;
                FindBoot();

                st = 20;

                label3.Visible = true;
                stopWatch.Start();
                timer1.Start();
                button1.Enabled = false;
                button2.Enabled = false;
                comboBox1.Enabled = false;
                textBox1.Enabled = false;

                if (radioButton1.Checked)
                {
                    new Thread(() => Finstall()).Start();
                }
                else
                {
                    if (radioButton2.Checked)
                    {
                        new Thread(() => UpdateA()).Start();
                    }
                    else
                    {
                        new Thread(() => UpdateB()).Start();
                    }
                }
         }

            catch (EfiNotExistException Exts)
            {
                string[] s = { "Android install canceled", "-----------------------------", "" };
                lw.Write(s);

                btnEnable();
                Close();
            }

            catch (Exception ex)
            {
                /*MessageBox.Show("Error!\nMore: log.txt");*/
                Message M = new Message("Program error!", "More info in: log.txt", "Ok", null, null, 1, 30);
                M.ShowDialog(this);
                string[] s2 = { "Android install error", ex.ToString(), "-----------------------------", "" };
                lw.Write(s2);

                btnEnable();
                Close();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked || radioButton3.Checked)
            {
                checkBox1.Visible = true;
                checkBox2.Visible = false;
                checkBox2.Checked = true;
                this.Height = 181;
            }
            else
            {
                checkBox1.Visible = false;
                checkBox1.Checked = false;
                checkBox2.Visible = true;
                checkBox2.Checked = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                this.Height = 265;
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
                    //MessageBox.Show("Error - Bootloader not found!");
                    Message M = new Message("Error!", "Bootloader not found", "Ok", null, null, 1, 30);
                    M.ShowDialog(this);
                    string[] s4 = { "Error - Bootloader not found!", "-----------------------------", "" };
                    lw.Write(s4);
                    Close();
                    return;
                }
            }
            else
                this.Height = 181;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < st)
                progressBar1.Value = st;

            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            label3.Text = elapsedTime;
        }
    }
}
