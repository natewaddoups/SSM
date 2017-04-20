using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using NateW.Ssm;

namespace NateW.Ssm.ApplicationLogic
{
    public delegate void Update(LogEventArgs args);
    public class SawMill
    {
        private SsmLogger logger;
        private Update update;
        private IList<SawMillScreen> screens;
        private SawMillScreen currentScreen;
        private ParameterDatabase database;
        
        /// <summary>
        /// Gets/sets the currently active screen
        /// </summary>
        public SawMillScreen CurrentScreen
        {
            get { return this.currentScreen; }
            set { this.currentScreen = value; }
        }

        /// <summary>
        /// Reports exceptions encountered during logging
        /// </summary>
        public event EventHandler<ExceptionEventArgs> Exception;

        /// <summary>
        /// Constructor
        /// </summary>
        private SawMill(
            string configurationDirectory,
            string ssmPortName,
            string plxPortName,
            IList<SawMillScreen> screens,
            Update update)
        {
            this.update = update;
            this.screens = screens;
            this.database = ParameterDatabase.GetInstance();
            
            this.CreateLogger(configurationDirectory, ssmPortName);
            
            if (!string.IsNullOrEmpty(plxPortName))
            {
                ExternalSensors externalSensors = ExternalSensors.GetInstance();
                externalSensors.SetPlxSerialPort(plxPortName);
                
                ParameterSource plxParameters = PlxParameterSource.GetInstance();
                this.database.Add(plxParameters);
            }
        }

        #region - Public API -

        /// <summary>
        /// Factory
        /// </summary>
        /// <returns></returns>
        public static SawMill GetInstance(
            string configurationDirectory,
            string ssmPortName,
            string plxPortName,
            IList<SawMillScreen> screens,
            Update update)
        {
            SawMill instance = new SawMill(
                configurationDirectory, 
                ssmPortName, 
                plxPortName, 
                screens, 
                update);
            return instance;
        }

        /// <summary>
        /// Begins an asyncrhonous operation to connect to the ECU, get the 
        /// ECU ID, and load the supported parameters from the database.
        /// </summary>
        public IAsyncResult BeginConnect(AsyncCallback callback, object asyncState)
        {
            return this.logger.BeginConnect(callback, asyncState);
        }

        /// <summary>
        /// Complete the BeginConnect asynchronous operation
        /// </summary>
        public void EndConnect(IAsyncResult asyncResult)
        {
            ParameterSource source = this.logger.EndConnect(asyncResult);
            this.database.Add(source);
        }

        /// <summary>
        /// Stop logging
        /// </summary>
        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            Trace.WriteLine("SawMill.BeginClose");
            return this.logger.BeginStopLogging(callback, state);
        }

        /// <summary>
        /// Complements BeginClose
        /// </summary>
        public void EndClose(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SawMill.EndClose");
            this.logger.EndStopLogging(asyncResult);
        }

        /// <summary>
        /// Start logging
        /// </summary>
        public void StartLogging()
        {
            if (this.currentScreen == null)
            {
                this.SetCurrentScreen(this.screens[0]);
            }

            this.logger.StartLogging();
        }

        /// <summary>
        /// Switch to a new screen
        /// </summary>
        public void SetCurrentScreen(SawMillScreen screen)
        {
            this.currentScreen = screen;
            LogProfile profile = this.GenerateProfile();
            profile.UserData = this.currentScreen;
            this.logger.SetProfile(profile, this.database);
        }

        public string GetClipboardText()
        {
            string result = string.Empty;
            if (this.currentScreen != null)
            {
                result = this.currentScreen.GetClipboardText();
            }

            return result;
        }

        public void SingleClick()
        {
            if (this.currentScreen != null)
            {
                this.currentScreen.SingleClick();
            }
        }

        public void DoubleClick()
        {
            if (this.currentScreen != null)
            {
                this.currentScreen.DoubleClick();
            }
        }

        public void WriteTo(Stream stream)
        {
            //try
            {
                SawMillWriter writer = SawMillWriter.GetInstance(stream);
                writer.Write(this.screens);                
            }
            //catch (Exception ex)
            {
//                Trace.WriteLine("SawMill.WriteTo(stream): " + ex.ToString());
            }
        }

        public void ReadFrom(Stream stream)
        {
            try
            {
                SawMillReader reader = SawMillReader.GetInstance(stream);
                reader.Read(this.screens);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("SawMill.ReadFrom(stream): " + ex.ToString());
            }
        }

        public void Update(LogEventArgs args, Graphics graphics, int width, int height)
        {
            SawMillScreen updateScreen = args.UserData as SawMillScreen;
            foreach (SawMillScreen screen in this.screens)
            {
                if (screen.ForegroundOnly)
                {
                    continue;
                }
                screen.AddData(args.Row);
            }

            if (updateScreen.ForegroundOnly)
            {
                updateScreen.AddData(args.Row);
            }

            updateScreen.Paint(graphics, width, height);
        }

        #endregion

        #region - Event Handlers -

        /// <summary>
        /// 
        /// </summary>
        private void OnLogEntry(object sender, LogEventArgs args)
        {
            this.update(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnLogError(object sender, LogErrorEventArgs args)
        {
            this.ReportException(args.Exception);            
        }

        /// <summary>
        /// 
        /// </summary>
        void OnLoggerException(object sender, ExceptionEventArgs args)
        {
            Trace.WriteLine("SawMill.OnLoggerException");
            this.ReportException(args.Exception);            
        }


        /// <summary>
        /// 
        /// </summary>
        private void ReportException(Exception exception)
        {
            EventHandler<ExceptionEventArgs> handler = this.Exception;
            if (handler != null)
            {
                handler(this, new ExceptionEventArgs(exception));
            }
        }

        #endregion

        #region - Private Methods -

        /// <summary>
        /// 
        /// </summary>
        private void CreateLogger(string configurationDirectory, string ssmPortName)
        {
            this.logger = SsmLogger.GetInstance(configurationDirectory, ssmPortName);
            this.logger.LogEntry += this.OnLogEntry;
            this.logger.LogError += this.OnLogError;
            this.logger.Exception += this.OnLoggerException;
        }

        private LogProfile GenerateProfile()
        {
            LogProfile profile = LogProfile.CreateInstance();

            this.AddScreenParameters(profile, this.currentScreen);
            
            foreach (SawMillScreen screen in this.screens)
            {
                if ((screen.ForegroundOnly) || (screen == this.currentScreen))
                {
                    continue;
                }

                AddScreenParameters(profile, screen);
            }
            profile.Add("P200", "g/rev", this.database);
            return profile;
        }

        private void AddScreenParameters(LogProfile profile, SawMillScreen screen)
        {
            if (this.database == null)
            {
                throw new InvalidOperationException("Logger must be connected and database must be initialized before calling AddScreenParameters");
            }

            LogProfile screenProfile = screen.GetProfile(this.database);
            foreach (LogColumn column in screenProfile.Columns)
            {
                Parameter parameter = column.Parameter;
                IList<Conversion> conversions = screenProfile.GetConversions(parameter);
                foreach (Conversion conversion in conversions)
                {
                    profile.Add(parameter, conversion);
                }
            }
        }

        #endregion
    }
}
