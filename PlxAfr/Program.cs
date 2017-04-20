using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using NateW.Ssm;

namespace PlxAfr
{
    static class Program
    {
        private static string portName;

        public static string PortName { get { return portName; } }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                string ports = Environment.NewLine + "Available ports are:";
                foreach (string name in System.IO.Ports.SerialPort.GetPortNames())
                {
                    ports += Environment.NewLine + name;
                }
                MessageBox.Show("This must be run with one command-line argument, the name of the serial port." + ports);
                return;
            }

            string traceFileName = Path.GetTempPath();
            string timestamp = DateTime.Now.ToString("s", CultureInfo.InvariantCulture).Replace(':', '.');
            traceFileName = Path.Combine(traceFileName, "PlxAfr.Trace." + timestamp + ".txt");
            TextWriterTraceListener listener = new TextWriterTraceListener(traceFileName);
            Trace.Listeners.Add(listener);
            Trace.AutoFlush = true;
            Trace.WriteLine("Session started " + DateTime.Now.ToString());

            portName = args[0];

            using (new ExceptionRecorder("PlxAfr"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new PlxAfr());
            }
        }
    }
}
