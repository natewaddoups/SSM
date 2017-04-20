///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// Lumberjack.LoggingEventHandlers.cs
// SsmLogger callbacks
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Threading;
using NateW.Ssm.ApplicationLogic.Properties;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Forms;

namespace NateW.Ssm.ApplicationLogic
{
    public partial class Lumberjack
    {
        /// <summary>
        /// Does what it says
        /// </summary>
        /// <returns>you can probably guess</returns>
        private LogWriter CreateLogWriter()
        {
            this.logStartTime = DateTime.Now;
            string time = this.logStartTime.ToString("yyyyMMdd-hhmmss", CultureInfo.InvariantCulture);
            string name = CondenseFileName(Settings.LastProfilePath);
            string logFileName = string.Format(CultureInfo.InvariantCulture, "{0}-{1}.csv", time, name);
            //string logFileName =  + ".csv";
            string logFilePath = Path.Combine(Settings.Default.LogFolderPath, logFileName);
            Stream stream = File.OpenWrite(logFilePath);
            return LogWriter.GetInstance(stream, true);
        }

        internal static string CondenseFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "New";
            }

            string fileName = Path.GetFileNameWithoutExtension(path);
            if (string.IsNullOrEmpty(path))
            {
                return "New";
            }

            StringBuilder builder = new StringBuilder(fileName.Length);
            foreach(char existing in fileName)
            {
                if (existing != ' ')
                {
                    builder.Append(existing);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Handles LogStart events from the SSM logger
        /// </summary>
        private void OnLogStart(object sender, LogEventArgs args)
        {
            Trace("Lumberjack.LogStart");
            this.ui.Invoke(new ThreadStart(delegate
            {
                this.ui.RenderLogEntry(args);
            }));
        }

        /// <summary>
        /// Handles LogEntry events from the SSM logger
        /// </summary>
        private void OnLogEntry(object sender, LogEventArgs args)
        {
            this.ui.Invoke(new ThreadStart(delegate
                       {
                           this.ui.RenderLogEntry(args);
                       }));
        }

        /// <summary>
        /// Handles LogStop events from the SSM logger
        /// </summary>
        private void OnLogStop(object sender, EventArgs args)
        {
            if (this.restartLogging)
            {
                this.restartLogging = false;
                this.logger.StartLogging();
            }
        }

        /// <summary>
        /// Handles LogError events from the SSM logger
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void OnLogError(object sender, LogErrorEventArgs args)
        {
            Exception exception = args.Exception;

            if ((exception is SsmPacketFormatException) ||
                (exception is IOException))
            {
                IOException placeholder = new IOException("Lost connection with ECU.");
                this.ui.RenderError(placeholder);
            }
            else
            {
                this.ui.Invoke(new ThreadStart(delegate
                {
                    this.ui.RenderError(exception);
                }));
            }
        }
    }
}
