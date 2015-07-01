using System;
using System.Collections.Generic;
using System.Linq;
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
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt", true))
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        file.WriteLine(s[i]);
                    }
                }
            }
        }
    }
}
