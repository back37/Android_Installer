using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Android_Installer
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]

        static void Main()
        {
            LogWriter lw = new LogWriter();

            string[] s = { DateTime.Now.ToString("dd.MM.yy HH-mm-ss") + " Program Version: " + Application.ProductVersion, "" };
            lw.Write(s);

            if (Directory.Exists(@"Bin"))
            {
                AppDomain.CurrentDomain.AppendPrivatePath(@"Bin");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
            else
            {
                string[] s4 = { "Error - Bin not found!", "-----------------------------", "","Program closed","","" };
                lw.Write(s4);

                MessageBox.Show("Error - Bin not found!");
                return;
            }
        }
    }
}
