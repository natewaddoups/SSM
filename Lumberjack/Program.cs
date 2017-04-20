///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// Program.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace NateW.Ssm.ApplicationLogic
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string traceFileName = Path.GetTempPath();
            string timestamp = DateTime.Now.ToString("s", CultureInfo.InvariantCulture).Replace(':', '.');
            traceFileName = Path.Combine(traceFileName, "Lumberjack.Trace." + timestamp + ".txt");
            TraceListener listener = new BetterTraceListener(traceFileName);
            Trace.Listeners.Add(listener);
            Trace.AutoFlush = true;
            Trace.WriteLine("Session started " + DateTime.Now.ToString());

            using (new ExceptionRecorder("Lumberjack"))
            {
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    MainForm form = new MainForm();

                    if (!Program.ParseArgs(form, args))
                    {
                        Usage();
                        return;
                    }

                    if (args.Length == 3)
                    {

                        form.SetStartProfile(args[0]);
                    }

                    Application.Run(form);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString(), "Fatal Error");
                }
            }
        }

        /// <summary>
        /// Parse command line arguments and initialize the form as appropriate.
        /// </summary>
        /// <param name="form">New and not-yet-displayed main form.</param>
        /// <param name="args">Command line argumetns.</param>
        /// <returns>True if arguments are well-formed.</returns>
        private static bool ParseArgs(MainForm form, string[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            else if (args.Length == 3)
            {
                if (string.Compare(args[0], "always", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    form.SetStartMode(LogFilterType.AlwaysLog);
                    form.SetStartPort(args[1]);
                    form.SetStartProfile(args[2]);
                    return true;
                }
                else if (string.Compare(args[0], "defogger", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    form.SetStartMode(LogFilterType.Defogger);
                    form.SetStartPort(args[1]);
                    form.SetStartProfile(args[2]);
                    return true;
                }
                else if (string.Compare(args[0], "openloop", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    form.SetStartMode(LogFilterType.Defogger);
                    form.SetStartPort(args[1]);
                    form.SetStartProfile(args[2]);
                    return true;
                }
                else if (string.Compare(args[0], "closedloop", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    form.SetStartMode(LogFilterType.Defogger);
                    form.SetStartPort(args[1]);
                    form.SetStartProfile(args[2]);
                    return true;
                }
                else if (string.Compare(args[0], "fullthrottle", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    form.SetStartMode(LogFilterType.Defogger);
                    form.SetStartPort(args[1]);
                    form.SetStartProfile(args[2]);
                    return true;
                }
                return false;
            }

            return false;
        }

        /// <summary>
        /// Show command-line usage instructions.
        /// </summary>
        private static void Usage()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Usage:");
            builder.AppendLine("Lumberjack.exe");
            builder.AppendLine("Open the window, wait for you to choose a port and profile");
            builder.AppendLine();
            
            builder.AppendLine("Lumberjack.exe always <COMx> <whatever.profile>");
            builder.AppendLine("Start logging with the given profile and COM port");
            builder.AppendLine();

            builder.AppendLine("Lumberjack.exe defogger <COMx> <whatever.profile>");
            builder.AppendLine("Defogger-controlled logging with the given profile and COM port");
            builder.AppendLine();
            MessageBox.Show(builder.ToString());
        }

        private class BetterTraceListener : TraceListener
        {
            private TextWriter writer;
            public BetterTraceListener(string path)
            {
                this.writer = new StreamWriter(path);
            }

            public override void Write(string s)
            {
                this.writer.Write(s);
                this.writer.Flush();
            }

            public override void WriteLine(string message)
            {
                this.writer.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}, {1}, {2}",
                        DateTime.Now.ToString("hh:mm:ss.ffff"),
                        System.Threading.Thread.CurrentThread.ManagedThreadId.ToString("0000"),
                        message));                
                this.writer.Flush();
            }
        }
    }
}