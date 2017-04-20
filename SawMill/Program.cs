using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using NateW.Ssm.ApplicationLogic;
using Settings = NateW.Ssm.ApplicationUI.Properties.Settings;

namespace NateW.Ssm.ApplicationUI
{
    /// <summary>
    /// SawMill application's entry point class
    /// </summary>
    static class Program
    {
        private static string[] args;

        public static string[] Args { get { return args; } }

        /// <summary>
        /// The main entry point for SawMill.
        /// </summary>
        [STAThread]
        static void Main(string[] _args)
        {
            args = _args;
            string traceFileName = Path.GetTempPath();
            string timestamp = DateTime.Now.ToString("s", CultureInfo.InvariantCulture).Replace(':', '.');
            traceFileName = Path.Combine(traceFileName, "SawMill.Trace." + timestamp + ".txt");
            TextWriterTraceListener listener = new TextWriterTraceListener(traceFileName);
            Trace.Listeners.Add(listener);
            Trace.AutoFlush = true;
            Trace.WriteLine("Session started " + DateTime.Now.ToString());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            using (new ExceptionRecorder("SawMill"))
            {
                // This forces loading of settings
                object unused = Settings.Default["MaxRpm"];
                Configuration.GetInstance().GetValue = GetConfigurationValue;
                if (!Configuration.GetInstance().Validate())
                {
                    return;
                }

                SawMillMainForm mainForm = new SawMillMainForm();
                Application.Run(mainForm);
            }
        }

        private static string GetConfigurationValue(string name)
        {
            SettingsPropertyValue spv = Settings.Default.PropertyValues[name];
            if (spv == null)
            {
                return null;
            }

            string value = spv.SerializedValue.ToString();
            return value;   
        }
    }
}
