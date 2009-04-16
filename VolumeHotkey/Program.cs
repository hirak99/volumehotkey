using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace VolumeHotkey {
    static class Program {
        public static bool IsAppAlreadyRunning() {
            Process currentProcess = Process.GetCurrentProcess();
            return (Process.GetProcessesByName(currentProcess.ProcessName).Length > 1);
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            if (IsAppAlreadyRunning()) return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            new Form1();
            Application.Run();
        }
    }
}
