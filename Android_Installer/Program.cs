﻿using System;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Diagnostics;

namespace Android_Installer
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]

        static void Main(String[] args)
        {
            Directory.SetCurrentDirectory(Application.StartupPath);

            LogWriter lw = new LogWriter();

            int pid = Process.GetCurrentProcess().Id;
            Process[] proc = Process.GetProcesses();
            foreach (Process process in proc)
                if (process.ProcessName == "Android_Installer" && process.Id != pid)
                {
                    process.Kill();
                }

            ManagementObjectSearcher searcher8 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");

            string c = "";
            foreach (ManagementObject queryObj in searcher8.Get())
            {
                c = "CPU: " + queryObj["Name"];
                c += " Load: " + queryObj["LoadPercentage"] + "%";
            }

            string r = "";
            ManagementObjectSearcher searcher12 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject queryObj in searcher12.Get())
            {
                r = "RAM: " + Math.Round(Convert.ToDouble(queryObj["TotalVisibleMemorySize"]) / 1024) + "mb";
                r += " Free: " + Math.Round(Convert.ToDouble(queryObj["FreePhysicalMemory"]) / 1024) + "mb";
            }

            string[] s = { DateTime.Now.ToString("dd.MM.yy HH-mm-ss") + " Program Version: " + Application.ProductVersion, "-----------------------------", c, r, "-----------------------------", "" };
            lw.Write(s);



            if (Directory.Exists(@"Bin"))
            {
                AppDomain.CurrentDomain.AppendPrivatePath(@"Bin");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main(args));
            }
            else
            {
                string[] s4 = { "Error - Bin not found!", "-----------------------------", "", "Program closed", "", "" };
                lw.Write(s4);

                //MessageBox.Show("Error - Bin not found!");
                Message M1 = new Message("Error!", "Bin not found", "Ok", null, null, 1, 30);
                M1.ShowDialog();
                return;
            }

        }
    }
}
