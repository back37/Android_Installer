using System;
using System.Drawing;
using System.Windows.Forms;

namespace Android_Installer
{
    public partial class About : Form
    {
        LogWriter lw = new LogWriter();
        Point last;

        public About()
        {
            InitializeComponent();

            label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            tabPage1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            tabPage1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            tabPage2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            tabPage2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            tabPage3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            tabPage3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            richTextBoxEx1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            richTextBoxEx1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            richTextBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            richTextBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            richTextBox3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            richTextBox3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);

            label1.Text = "Android installer: " + Application.ProductVersion;

            richTextBoxEx1.AppendText(Environment.NewLine);
            richTextBoxEx1.InsertLink("GitHub Repository", "https://github.com/back37/Android_Installer");
            richTextBoxEx1.AppendText(Environment.NewLine);
            richTextBoxEx1.InsertLink("YouTube channel", "http://www.youtube.com/playlist?list=PLXBIG1-uprGOYHQoFXdeHRmHAZRZyTeua");
            richTextBoxEx1.AppendText(Environment.NewLine);
            richTextBoxEx1.InsertLink("Changelog and other", "https://translate.google.ru/translate?sl=ru&tl=en&js=y&prev=_t&hl=ru&ie=UTF-8&u=http%3A%2F%2F4pda.ru%2Fforum%2Findex.php%3Fs%3D%26showtopic%3D611095%26view%3Dfindpost%26p%3D41005660&edit-text=&act=url");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] s = { "About closed", "-----------------------------","" };
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

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            string uri = e.LinkText.Split('#')[1];
            System.Diagnostics.Process.Start(uri);
        }
    }
}
