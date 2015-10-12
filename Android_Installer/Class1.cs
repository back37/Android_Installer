using System;
using System.Text;

namespace Android_Installer
{
    class LogWriter
    {
        public void Write(string [] s)
        {
            if (System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory()))
            {
                string str = string.Empty;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt", true, Encoding.GetEncoding(1251)))
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        file.WriteLine(s[i]);
                    }
                }

                using (System.IO.StreamReader reader = new System.IO.StreamReader("log.txt", Encoding.GetEncoding(1251), true))
                {
                    str = reader.ReadToEnd();
                }
                str = str.Replace("\n", Environment.NewLine);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt", false, Encoding.GetEncoding(1251)))
                {
                    file.Write(str);
                }

            }
        }
    }
}
