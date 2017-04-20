using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NateW.Ssm.TreeHugger
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (new ExceptionRecorder("TreeHugger"))
            {
                string traceConfig = ConfigurationManager.AppSettings["Trace"];
                bool traceEnabled = true;

                if (!bool.TryParse(traceConfig, out traceEnabled) || traceEnabled)
                {
                    string traceFileName = Path.GetTempPath();
                    string timestamp = DateTime.Now.ToString("s", CultureInfo.InvariantCulture).Replace(':', '.');
                    traceFileName = Path.Combine(traceFileName, "TreeHugger.Trace." + timestamp + ".txt");
                    TextWriterTraceListener listener = new TextWriterTraceListener(traceFileName);
                    Trace.Listeners.Add(listener);
                    Trace.AutoFlush = true;
                }

                Trace.WriteLine("Session started " + DateTime.Now.ToString());

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}