using System;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NateW.Ssm;

namespace NateW.Ssm.TreeHugger
{
    /// <summary>
    /// Main window
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Serial port for the ECU connection
        /// </summary>
        private SerialPort port;

        /// <summary>
        /// Logger to pull data from the ECU
        /// </summary>
        private SsmBasicLogger logger;

        /// <summary>
        /// Parameter database.
        /// </summary>
        ParameterDatabase database;

        /// <summary>
        /// Ensures atomic disposal of the stream
        /// </summary>
        private object disposeSync = new object();

        /// <summary>
        /// Used to schedule restarts
        /// </summary>
        private System.Windows.Forms.Timer timer;

        /// <summary>
        /// Whether the system has power
        /// </summary>
        private bool powerOn = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args">command-line arguments</param>
        public MainForm()
        {
            this.InitializeComponent();
            this.timer = new System.Windows.Forms.Timer();
            this.timer.Tick += this.OnTimerTick;
            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
        }

        /// <summary>
        /// Stop/start logging when the system hibernates and resumes
        /// </summary>
        void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            Trace.WriteLine("### PowerModeChanged: " + e.Mode);

            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Suspend:
                    this.powerOn = false;
                    SsmBasicLogger localLogger = this.logger;
                    if (localLogger != null)
                    {
                        Trace.WriteLine("SystemEvents_PowerModeChanged: stopping logger");
                        localLogger.BeginStopLogging(Logger_LoggingStopped, false);
                    }
                    break;

                case Microsoft.Win32.PowerModes.Resume:
                    this.powerOn = true;
                    this.DisplayResume();
                    this.ScheduleRestart("System resumed", TimeSpan.FromSeconds(10));
                    break;
            }
        }

        /// <summary>
        /// Invoked once before the form is displayed
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.SetBounds();
            this.Start("Startup");
        }

        /// <summary>
        /// Close the main form if the user double-clicks on it.
        /// </summary>
        private void MainForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Invoked before the window closes
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Trace.WriteLine("MainForm_FormClosing");

            SsmBasicLogger localLogger = this.logger;
            if (localLogger != null)
            {
                Trace.WriteLine("MainForm_FormClosing: stopping logger.");
                localLogger.BeginStopLogging(Logger_LoggingStopped, true);
                e.Cancel = true;
            }
            else
            {
                Trace.WriteLine("MainForm_FormClosing: allowing close");                
            }

            Trace.WriteLine("MainForm_FormClosing: returning.");
        }
        
        /// <summary>
        /// Invoked after the logger has stopped.
        /// </summary>
        private void Logger_LoggingStopped(IAsyncResult asyncResult)
        {
            Trace.WriteLine("MainForm.Logger_LoggingStopped");

            this.logger.EndStopLogging(asyncResult);

            this.Invoke(
                new ThreadStart(delegate
                {
                    this.logger = null;
            
                    Trace.WriteLine("Logger_LoggingStopped: Releasing serial port.");
                    this.ReleaseSerialPort();

                    if ((bool) asyncResult.AsyncState)
                    {
                        Trace.WriteLine("Logger_LoggingStopped: Closing form.");
                        this.Close();
                    }
                }));
        }

        /// <summary>
        /// Carry out a scheduled restart
        /// </summary>
        private void OnTimerTick(object sender, EventArgs args)
        {
            Trace.WriteLine("OnTimerTick");
            lock (this.timer)
            {
                this.timer.Stop();
                if (!this.powerOn)
                {
                    Trace.WriteLine("OnTimerTick: system suspending, will not attempt restart.");
                    return;
                }

                if ((this.logger != null) && this.logger.IsLogging)
                {
                    Trace.WriteLine("OnTimerTick: logger is already running.");
                    return;
                }

                string context = (string)this.timer.Tag;
                Trace.WriteLine("OnTimerTick: Attempting restart.  Context: " + context);
                this.Start(context);
            }
        }

        /// <summary>
        /// Invoked by the logger when a new packet arrives
        /// </summary>
        private void OnLogEntry(object sender, LogEventArgs args)
        {
            if (!this.powerOn)
            {
                Trace.WriteLine("OnLogEntry: system suspending.");
                this.logger.BeginStopLogging(Logger_LoggingStopped, false);
            }

            this.Invoke(
                new ThreadStart(delegate
                {
                    this.DisplayLogEntry(args.Row);
                }));
        }

        /// <summary>
        /// Draw the log entry on the screen
        /// </summary>
        private void DisplayLogEntry(LogRow row)
        {
            int width = this.ClientRectangle.Width;
            int height = this.ClientRectangle.Height;

            using (Graphics graphics = this.CreateGraphics())
            {
                Painter painter = new Painter(graphics, width, height, row);
                painter.Paint();
            }
        }

        /// <summary>
        /// Handles I/O errors while logging
        /// </summary>
        private void OnLogError(object sender, LogErrorEventArgs args)
        {
            Trace.WriteLine("LogError");

            this.Invoke(
                new ThreadStart(delegate
                {
                    this.HandleLogError(args.Exception);
                }));
        }

        /// <summary>
        /// Handles I/O errors while logging
        /// </summary>
        private void HandleLogError(Exception exception)
        {
            Trace.WriteLine("HandleLogError: " + exception.Message);

            if ((exception is IOException) ||
                (exception is ObjectDisposedException) ||
                (exception is System.UnauthorizedAccessException))
            {
                this.Restart("Logging interrupted", exception);
                return;
            }

            Trace.WriteLine("LogError raised unexpected exception: " + exception.ToString());

            DialogResult userAction = MessageBox.Show(
                "Exception during logging: " + exception.ToString(),
                "Please send a screenshot.",
                MessageBoxButtons.RetryCancel);

            if (userAction == DialogResult.Cancel)
            {
                this.Close();
            }
            else
            {
                this.ScheduleRestart("logging", TimeSpan.FromSeconds(1));
            }
        }

        /// <summary>
        /// Draw an exception message on the screen.
        /// </summary>
        private void DisplayException(Exception exception)
        {
            Trace.WriteLine("DisplayException: " + exception.ToString());
            Graphics graphics = this.CreateGraphics();

            Font font = new Font(
                FontFamily.GenericSansSerif, 
                (float) System.Windows.Forms.SystemInformation.CaptionHeight, 
                GraphicsUnit.Pixel);

            RectangleF clientRectangle = new RectangleF(
                    0,
                    0,
                    this.ClientRectangle.Width,
                    this.ClientRectangle.Height);

            graphics.FillRectangle(
                Brushes.Red, 
                clientRectangle);

            graphics.DrawString(
                exception.ToString(),
                font,
                Brushes.White,
                clientRectangle);
        }

        /// <summary>
        /// Indicate that we're paused due to system coming out of hibernation
        /// </summary>
        private void DisplayResume()
        {
            Graphics graphics = this.CreateGraphics();

            Font font = new Font(
                FontFamily.GenericSansSerif,
                (float)System.Windows.Forms.SystemInformation.CaptionHeight,
                GraphicsUnit.Pixel);

            RectangleF clientRectangle = new RectangleF(
                    0,
                    0,
                    this.ClientRectangle.Width,
                    this.ClientRectangle.Height);

            graphics.FillRectangle(
                Brushes.Blue,
                clientRectangle);

            graphics.DrawString(
                "Power restored....",
                font,
                Brushes.White,
                clientRectangle);
        }

        /// <summary>
        /// Get a data stream and start logging
        /// </summary>
        private void Start(string context)
        {
            Trace.WriteLine("Start: " + context);
            context = "Start";
            try
            {
                Stream stream = this.GetDataStream();
                if (stream == null)
                {
                    throw new IOException("Unable to open SSM stream.");
                }

                this.InitializeLogger(stream);
                this.StartLogging();
            }
            catch (SsmPacketFormatException exception)
            {
                // Unable to do the init handshake with the ECU (car turned off)
                this.Restart(context, exception);
            }
            catch (IOException exception)
            {
                // Serial port failed (car turned off, PC hibernated)
                this.Restart(context, exception);
            }
            catch (UnauthorizedAccessException exception)
            {
                // Serial port disappeared (car turned off, PC hibernated)
                this.Restart(context, exception);
            }
            catch (Exception exception)
            {
                Trace.WriteLine("Start raised unexpected exception: " + exception.ToString());

                DialogResult userAction = MessageBox.Show(
                    string.Format("Exception during {0}: {1}{2}",
                        context, 
                        Environment.NewLine, 
                        exception.ToString()),
                    "Please send a screenshot.",
                    MessageBoxButtons.RetryCancel);

                if (userAction == DialogResult.Cancel)
                {
                    this.Close();
                }
                else
                {
                    this.Restart(context, exception);
                }
            }
        }

        /// <summary>
        /// What to do when startup fails
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        private void Restart(string context, Exception exception)
        {
            Trace.WriteLine("Restart: " + context);

            this.DisplayException(exception);
            this.ScheduleRestart(context, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Get the stream from which to read ECU data
        /// </summary>
        private Stream GetDataStream()
        {
            Trace.WriteLine("GetDataStream");

            string portName = ConfigurationManager.AppSettings["Port"];

            if (portName == "Mock ECU")
            {
                MessageBox.Show(
                    "Bogus data will be displayed." +
                    Environment.NewLine +
                    "If you want real data, change the \"Port\" setting in" +
                    Environment.NewLine +
                    "TreeHugger.exe.config to COM1 or COM2 or whatever.");
                MockEcuStream.Image = new EcuImage2F12785206();
                return MockEcuStream.CreateInstance();
            }

            Stream stream = null;
            Exception exception = null;
            try
            {
                if (this.port == null)
                {
                    Trace.WriteLine("Creating port.");
                    this.port = new SerialPort(portName, 4800, Parity.None, 8);
                    this.port.ReadTimeout = 500;
                    this.port.WriteTimeout = 500;
                }

                if (!this.port.IsOpen)
                {
                    Trace.WriteLine("Port not open.");
                    this.port.Open();
                    stream = this.port.BaseStream;
                    Trace.WriteLine("Port opened, returning stream.");
                    return stream;
                }

                Trace.WriteLine("Port already created, draining input queue...");
                int bytesToRead = 0;
                while ((bytesToRead = this.port.BytesToRead) > 0)
                {
                    Trace.WriteLine(bytesToRead.ToString() + " bytes in queue, reading...");
                    byte[] buffer = new byte[bytesToRead];
                    this.port.Read(buffer, 0, buffer.Length);
                    Trace.WriteLine("Read completed.");
                    Thread.Sleep(500);
                }
                stream = this.port.BaseStream;
                Trace.WriteLine("Input queue drained, returning stream.");
                return stream;
            }
            catch (System.TimeoutException ex)
            {
                exception = ex;
            }
            catch (System.IO.IOException ex)
            {
                exception = ex;
            }
            catch (System.UnauthorizedAccessException ex)
            {
                exception = ex;
            }

            this.DisplayException(exception);
            Thread.Sleep(1000);
            return null;
        }

        /// <summary>
        /// Close port, release stream
        /// </summary>
        private void ReleaseSerialPort()
        {
            Trace.WriteLine("ReleaseSerialPort");
            SerialPort localPort = this.port;
            if (localPort != null)
            {
                this.port = null;
                Trace.WriteLine("ReleaseStream: closing port");
                localPort.Close();

                Trace.WriteLine("ReleaseStream: disposing port");
                localPort.Dispose();
            }
            else
            {
                Trace.WriteLine("ReleaseStream: there is no port.");
            }

            Trace.WriteLine("ReleaseStream: returning");
        }

        /// <summary>
        /// Initialize the logger
        /// </summary>
        private void InitializeLogger(Stream stream)
        {
            Trace.WriteLine("InitializeLogger");

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            string configurationDirectory = Path.Combine(
                Environment.CurrentDirectory,
                "Configuration");

            SsmBasicLogger localLogger = SsmBasicLogger.GetInstance(configurationDirectory, stream);
            localLogger.LogEntry += this.OnLogEntry;
            localLogger.LogError += this.OnLogError;

            Trace.WriteLine("Connecting to ECU");
            IAsyncResult asyncResult = localLogger.BeginConnect(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            ParameterSource source = localLogger.EndConnect(asyncResult);
            this.database = ParameterDatabase.GetInstance();
            this.database.Add(source);
            Trace.WriteLine("Connected to ECU");
            this.logger = localLogger;
        }

        /// <summary>
        /// Start logging
        /// </summary>
        private void StartLogging()
        {
            LogProfile profile = this.CreateLogProfile();
            this.logger.SetProfile(profile, database);

            if (this.powerOn)
            {
                this.logger.StartLogging();
            }
        }

        /// <summary>
        /// Create a log profile from the application's configuration settings
        /// </summary>
        private LogProfile CreateLogProfile()
        {
            Trace.WriteLine("CreateLogProfile");

            LogProfile profile = LogProfile.CreateInstance();

            for (int i = 0; i < 6; i++)
            {
                string combined = ConfigurationManager.AppSettings[i.ToString()];
                string[] idAndUnits = combined.Split(',');

                if (idAndUnits.Length != 2)
                {
                    throw new ApplicationException("Parameter " + i.ToString() +
                        "is not of the form \"P12,units\"." + Environment.NewLine +
                        "Please fix this in TreeHugger.exe.config and try again");
                }

                string id = idAndUnits[0];
                string units = idAndUnits[1];

                this.AddParameter(profile, id, units);
            }

            return profile;
        }

        /// <summary>
        /// Add a parameter to the log profile
        /// </summary>
        private void AddParameter(LogProfile profile, string id, string units)
        {
            foreach (SsmParameter parameter in this.database.Parameters)
            {
                if (parameter.Id == id)
                {
                    foreach (Conversion conversion in parameter.Conversions)
                    {
                        if (conversion.Units == units)
                        {
                            profile.Add(parameter, conversion);
                            return;
                        }
                    }

                    throw new ApplicationException(
                        "Conversion " + units + " not found for parameter " + id);
                }
            }

            throw new ApplicationException(
                "Parameter " + id + " not found.");
        }

        /// <summary>
        /// Set a timer that will cause a restart 
        /// </summary>
        private void ScheduleRestart(string context, TimeSpan timeSpan)
        {
            Trace.WriteLine("Scheduling restart from: " + context);

            lock (this.timer)
            {
                if (this.timer.Enabled)
                {
                    MessageBox.Show("Concurrent calls to ScheduleRestart.", "Please send a screenshot");
                    return;
                }

                this.timer.Tag = context;
                this.timer.Interval = (int) timeSpan.TotalMilliseconds;
                this.timer.Start();
            }
        }

        /// <summary>
        /// Set the Form's position and size
        /// </summary>
        private void SetBounds()
        {
            int left = int.Parse(ConfigurationManager.AppSettings["Left"]);
            int top = int.Parse(ConfigurationManager.AppSettings["Top"]);
            int width = int.Parse(ConfigurationManager.AppSettings["Width"]);
            int height = int.Parse(ConfigurationManager.AppSettings["Height"]);
            this.SetBounds(left, top, width, height);
        }
    }
}