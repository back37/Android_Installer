using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Android_Installer
{
    public partial class Editor : Form
    {

        string p = "";
        LogWriter lw = new LogWriter();
        Point last;
        int y = 0;
        int c = 0;

        public Editor()
        {
            InitializeComponent();
            var boot = Environment.ExpandEnvironmentVariables(@"%SystemDrive%");
            try
            {

                if (Directory.Exists(boot + @"\Android"))
                {

                    if (Directory.Exists(boot + @"\EFI"))
                    {
                        p = boot + @"\";
                    }
                    else
                    {
                        Process efi = new Process();
                        StreamWriter BatFile2 = new StreamWriter(@"Bin\2.bat", false, Encoding.GetEncoding(866));
                        BatFile2.WriteLine("chcp 1251");
                        BatFile2.WriteLine(@"echo Try to mount S >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
                        BatFile2.WriteLine(@"mountvol S: /S >> """ + Directory.GetCurrentDirectory() + @"\log.txt""");
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
                            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\Bootloader"))
                            {
                                p = Directory.GetCurrentDirectory() + @"\Android\Bootloader";
                            }
                            else
                            {
                                MessageBox.Show("Bootloader not found");
                                return;
                                string[] s4 = { "Bootloader not found","" };
                                lw.Write(s4);
                                Close();
                            }
                        }
                    }
                }
                else
                {
                    if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Android\Bootloader"))
                    {
                        p = Directory.GetCurrentDirectory() + @"\Android\Bootloader";
                    }
                    else
                    {
                        MessageBox.Show("Bootloader not found");
                        return;
                        string[] s4 = { "", "Bootloader not found" };
                        lw.Write(s4);
                        Close();
                    }
                }

                var dir1 = new DirectoryInfo(p + @"\boot");
                foreach (FileInfo file1 in dir1.GetFiles("*.cfg", SearchOption.AllDirectories))
                {
                    string str = file1.FullName;
                    str = str.Replace(p, "");
                    comboBox1.Items.Add(str);
                }
                foreach (FileInfo file1 in dir1.GetFiles("*.conf", SearchOption.AllDirectories))
                {
                    string str = file1.FullName;
                    str = str.Replace(p, "");
                    comboBox1.Items.Add(str);
                }
                dir1 = new DirectoryInfo(p + @"\EFI");
                foreach (FileInfo file1 in dir1.GetFiles("*.cfg", SearchOption.AllDirectories))
                {
                    string str = file1.FullName;
                    str = str.Replace(p, "");
                    comboBox1.Items.Add(str);
                }
                foreach (FileInfo file1 in dir1.GetFiles("*.conf", SearchOption.AllDirectories))
                {
                    string str = file1.FullName;
                    str = str.Replace(p, "");
                    comboBox1.Items.Add(str);
                }

                comboBox1.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!\nMore: log.txt");
                string[] s2 = { "", "Config editor error", ex.ToString(), "" };
                lw.Write(s2);
                Close();
            }
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void выделитьВсёToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                richTextBox1.Copy();
            }
            if (e.Control && e.KeyCode == Keys.X)
            {
                richTextBox1.Cut();
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                richTextBox1.Cut();
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{DEL}");
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (richTextBox1.Modified == true)
            {
                DialogResult result = MessageBox.Show(null, "Save changes?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        {
                            string[] s = { comboBox1.SelectedItem.ToString() + " - changes saved", "Config editor closed", "-----------------------------" };
                            lw.Write(s);

                            using (StreamWriter sw = new StreamWriter(p + @"\" + comboBox1.SelectedItem.ToString(), false, Encoding.ASCII))
                            {
                                sw.Write(richTextBox1.Text);
                                sw.Close();
                            }
                            break;
                        }
                    case DialogResult.No:
                        {
                            string[] s = { comboBox1.SelectedItem.ToString() + " - changes cancelled", "Config editor closed", "-----------------------------" };
                            lw.Write(s);

                            break;
                        }
                    case DialogResult.Cancel:
                        {
                            string[] s = { "Closing cancelled" };
                            lw.Write(s);

                            return;
                        }
                }
            }
            else
            {
                string[] s = { "Config editor closed", "-----------------------------" };
                lw.Write(s);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(p + @"\" + comboBox1.SelectedItem.ToString(), false, Encoding.ASCII))
            {
                sw.Write(richTextBox1.Text);
                sw.Close();
            }

            System.IO.StreamReader streamReader;
            streamReader = new System.IO.StreamReader(p + @"\" + comboBox1.SelectedItem.ToString(), Encoding.ASCII, true);
            while (streamReader.EndOfStream == false)
            {
                richTextBox1.Text = streamReader.ReadToEnd();
            }
            streamReader.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (y == 0)
            {
                this.WindowState = FormWindowState.Maximized;
                y++;
                button2.Image = Properties.Resources.Maxi;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                y--;
                button2.Image = Properties.Resources.Normal;
            }

        }

        private void Editor_Resize(object sender, EventArgs e)
        {
            button1.Left = this.Width - 48;
            button2.Left = this.Width - 82;
            button3.Left = this.Width - 116;
        }

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                last = MousePosition;
            }
        }

        private void richTextBox1_MouseMove(object sender, MouseEventArgs e)
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.Modified == true)
                {
                    DialogResult result = MessageBox.Show(null, "Save changes?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            {
                                string[] s = { comboBox1.Items[c].ToString() + " - changes saved", };
                                lw.Write(s);

                                using (StreamWriter sw = new StreamWriter(p + @"\" + comboBox1.Items[c].ToString(), false, Encoding.ASCII))
                                {
                                    sw.Write(richTextBox1.Text);
                                    sw.Close();
                                }
                                break;
                            }
                        case DialogResult.No:
                            {
                                string[] s = { comboBox1.Items[c].ToString() + " - changes cancelled", };
                                lw.Write(s);

                                break;
                            }
                    }
                }
                c = comboBox1.SelectedIndex;

                string[] s1 = { comboBox1.SelectedItem.ToString() + " - selected", };
                lw.Write(s1);

                System.IO.StreamReader streamReader;
                streamReader = new System.IO.StreamReader(p + @"\" + comboBox1.SelectedItem.ToString(), Encoding.ASCII, true);
                while (streamReader.EndOfStream == false)
                {
                    richTextBox1.Text = streamReader.ReadToEnd();
                }
                streamReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!\nMore: log.txt");
                string[] s2 = { "", "Config editor error", ex.ToString(),"" };
                lw.Write(s2);
                Close();
            }
        }
    }
}
