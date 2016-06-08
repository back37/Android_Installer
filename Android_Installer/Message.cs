using System;
using System.Drawing;
using System.Windows.Forms;

namespace Android_Installer
{
    public partial class Message : Form
    {
        public int fk = 0;
        int Timer = new int();
        int f = new int();
        string d = null;
        Point last;

        public Message(String A, String B, String D, String E, String G, int I, int J)
        {
            InitializeComponent();

            groupBox1.MouseDown += new MouseEventHandler(this.Form1_MouseDown);
            groupBox1.MouseMove += new MouseEventHandler(this.Form1_MouseMove);
            label1.MouseDown += new MouseEventHandler(this.Form1_MouseDown);
            label1.MouseMove += new MouseEventHandler(this.Form1_MouseMove);
            label3.MouseDown += new MouseEventHandler(this.Form1_MouseDown);
            label3.MouseMove += new MouseEventHandler(this.Form1_MouseMove);


            Timer = f = J;

            label1.Text = A;
            label3.Text = B;

            button1.Text = d = D;
            button2.Text = E;
            button3.Text = G;

            switch (I)
            {
                case 1:
                    {
                        button1.Visible = true;
                        button2.Visible = false;
                        button3.Visible = false;
                        break;
                    }
                case 2:
                    {
                        button1.Visible = true;
                        button2.Visible = true;
                        button3.Visible = false;
                        break;
                    }
                case 3:
                    {
                        button1.Visible = true;
                        button2.Visible = true;
                        button3.Visible = true;
                        break;
                    }
            }

            if (Timer > 0)
            {
                button1.Text += " (" + f.ToString() + ")";
                f--;
                timer1.Start();
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Timer > 0)
            {
                if (f != 0)
                {
                    button1.Text = d + " (" + f.ToString() + ")";
                    f--;
                }
                else
                {
                    timer1.Stop();
                    fk = 1;
                    Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fk = 1;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fk = 2;
            Close();
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

        private void button3_Click(object sender, EventArgs e)
        {
            fk = 3;
            Close();
        }
    }
}
