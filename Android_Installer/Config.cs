using System;
using System.Drawing;
using System.Windows.Forms;

namespace Android_Installer
{
    public partial class Config : Form
    {
        Point last;
        LogWriter lw = new LogWriter();
        public string text = "default";
        public Boolean ch = true;

        public Config(Boolean en, string txt)
        {
            InitializeComponent();

            if (en == true)
                radioButton1.Checked = true;
            else
                radioButton2.Checked = true;
            textBox1.Text = txt;
            groupBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            groupBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
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

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                button1.Visible = true;
                textBox1.Visible = true;
                this.Height = 121;
                textBox1.Undo();
            }
            else
            {
                button1.Visible = false;
                textBox1.Visible = false;
                this.Height = 88;
                textBox1.Text = "default";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Undo();

            string[] s = { "Changes canceled", "-----------------------------", "" };
            lw.Write(s);

            text = textBox1.Text;
            ch = radioButton1.Checked;

            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] s = { "Changes saved", "Android: " + textBox1.Text, "-----------------------------", "" };
            lw.Write(s);

            text = textBox1.Text;
            ch = radioButton1.Checked;

            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }
    }
}
