///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// Lumberjack.cs
// Core application logic for Win32 and WinCE versions of the SSM logger
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using NateW.Ssm.ApplicationLogic.Properties;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Forms;

namespace NateW.Ssm.ApplicationLogic
{    
    public partial class Lumberjack : IDisposable
    {
        private const string WindowCaption = "Lumberjack";
        private static string configurationDirectory;
        private IUserInterface ui;
        private SsmRecordingLogger logger;
        private ParameterDatabase database;
        private ParameterSource ssmParameterSource;
        private ParameterSource plxParameterSource;
        private DateTime logStartTime = DateTime.Now;
        private bool currentProfileIsChanged;
        private bool restartLogging;
        private LogFilterType startMode = LogFilterType.Defogger;
        private string startProfile;
        private string startPort;
        private bool ignoreProfileSettingsChangeNotifications;
        private bool ignoreSerialPortChangeNotifications;
        
        /// <summary>
        /// Application settings.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public Settings Settings
        {
            get
            {
                return Settings.Default;
            }
        }

        /// <summary>
        /// ECU identifier string.
        /// </summary>
        public string EcuIdentifier
        {
            get
            {
                return this.logger.EcuIdentifier;
            }
        }

        /// <summary>
        /// Parameter database for the currently-connected ECU.
        /// </summary>
        public ParameterDatabase Database
        {
            get
            {
                return this.database;
            }
        }

        /// <summary>
        /// Time the current log started.
        /// </summary>
        public DateTime LogStartTime
        {
            get
            {
                return this.logStartTime;
            }
        }

        /// <summary>
        /// Where log files will be saved
        /// </summary>
        public string LoggingFolder
        {
            get
            {
                return this.Settings.LogFolderPath;
            }
        }

        /// <summary>
        /// If true, the current profile has changed since it was last saved.
        /// </summary>
        internal bool CurrentProfileIsChanged
        {
            get
            {
                return this.currentProfileIsChanged;
            }
        }

        /// <summary>
        /// Construct an instance of Lumberjack with the given UI.
        /// </summary>
        /// <param name="ui">User interface.</param>
        public Lumberjack(IUserInterface ui)
        {
            Trace("Lumberjack.Lumberjack");
            this.ui = ui;
            this.database = ParameterDatabase.GetInstance();
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~Lumberjack()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Dispose of a Lumberjack instance. (Public IDispose.Dispose implementation.)
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implements the Dispose pattern.
        /// </summary>
        /// <param name="disposing">True if disposing, false if finalizing.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.logger != null)
                {
                    this.logger.Dispose();
                    this.logger = null;
                }
            }
        }

        /// <summary>
        /// Set the logging mode to use upon startup.
        /// </summary>
        /// <param name="value">Logging mode to use upon start.</param>
        public void SetStartupLogFilterType(LogFilterType value)
        {
            this.startMode = value;
        }

        /// <summary>
        /// Set the logging profile to use upon startup.
        /// </summary>
        /// <param name="value">Logging profile to use upon startup.</param>
        public void SetStartupLoggingProfile(string value)
        {
            this.startProfile = value;
        }

        /// <summary>
        /// Set the serial port to use upon startup.
        /// </summary>
        /// <param name="value">Serial port to use upon startup.</param>
        public void SetStartupPort(string value)
        {
            this.startPort = value;
        }

        /// <summary>
        /// Set the current logging mode.
        /// </summary>
        /// <param name="value">Desired logging mode.</param>
        public void SetLogFilterType(LogFilterType type)
        {
            this.logger.SetLogFilterType(type);
        }

        /// <summary>
        /// Call this when the main form loads.
        /// </summary>
        public void Load()
        {
            Trace("Lumberjack.Load");
            this.Settings.Reload();

            // Suppress change notifications while the UI is initialized
            this.ignoreSerialPortChangeNotifications = true;
            
            this.InitializeLogFolderPath();
            this.InitializeProfileList();
            this.InitializeSerialPortList();

            if (this.startPort != null)
            {
                this.Settings.DefaultSsmPort = this.startPort;
                this.startPort = null;
            }

            string lastPort = this.Settings.DefaultSsmPort;
            if (!this.ui.SsmSerialPortListContains(lastPort))
            {
                lastPort = NateW.Ssm.MockEcuStream.PortName;
            }

            // This causes a change notification, which causes Lumberjack to connect to the ECU.
            this.ignoreSerialPortChangeNotifications = false;
            this.ui.SelectSsmSerialPort(lastPort);

            lastPort = this.Settings.DefaultPlxPort;
            if (!this.ui.PlxSerialPortListContains(lastPort))
            {
                lastPort = ExternalSensors.NullSerialPortName;
            }
            this.ui.SelectPlxSerialPort(lastPort);

/*            if (this.startProfile != null)
            {
                this.Settings.LastProfilePath = this.startProfile;
                this.startProfile = null;
            }

            if (!string.IsNullOrEmpty(this.Settings.LastProfilePath))
            {
                // This causes a change notification which causes Lumberjack to start logging
                // If no profile can be selected, logging will start when the users chooses a parameter
                this.ignoreProfileSettingsChangeNotifications = false;
                this.ui.SelectProfile(this.Settings.LastProfilePath);
            }
            */
            this.SetTitle();
        }

        /// <summary>
        /// Call this when the main form closes.
        /// </summary>
        /// <param name="cancel">If true, the Close should be aborted.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        public void Closing(ref bool cancel)
        {
            Trace("Lumberjack.Closing");
            if (this.logger != null && this.logger.IsLogging)
            {
                this.logger.BeginStopLogging(this.ContinueClosing, null);
                cancel = true;
                return;
            }
                
            if (this.currentProfileIsChanged && !PromptToSaveChangedProfile())
            {
                cancel = true;
                this.logger.StartLogging();
            }
            else
            {
                this.SaveProfileList();
                this.Settings.Save();
            }
        }

        /// <summary>
        /// Call this when the user selects a new SSM serial port.
        /// </summary>
        public void ChangeSsmSerialPort()
        {
            if (ignoreSerialPortChangeNotifications)
            {
                Trace("Lumberjack.ChangeSerialPort: Ignored");
                return;
            }

            Trace("Lumberjack.ChangeSerialPort");
            this.BeginReopenEcuInterface();
        }

        /// <summary>
        /// Call this when the user selects a new PLX serial port.
        /// </summary>
        public void ChangePlxSerialPort()
        {
            if (this.plxParameterSource == null)
            {
                this.plxParameterSource = PlxParameterSource.GetInstance();
            }

            string portName = this.ui.GetSelectedPlxSerialPort();
            portName = portName ?? ExternalSensors.NullSerialPortName;
            this.Settings.DefaultPlxPort = portName;
            this.Settings.Save();

            // TODO: Make ParameterDatabase a singleton; ExternalSensors should add/remove sources
            ExternalSensors.GetInstance().SetPlxSerialPort(portName);

            if (portName == ExternalSensors.NullSerialPortName)
            {
                this.database.Remove(this.plxParameterSource);
            }
            else
            {
                this.database.Add(this.plxParameterSource);
            }

            this.ui.PopulateParameterList(this.database);
        }


        /// <summary>
        /// Call this if the user wants to reconnect to the ECU.
        /// </summary>
        /// <remarks>Shouldn't be necessary - remove before "gamma" release.</remarks>
        public void Reconnect()
        {
            Trace("Lumberjack.Reconnect");
            this.BeginReopenEcuInterface();
        }

        /// <summary>
        /// Call this when the user wants to create a new profile.
        /// </summary>
        public void ProfileNew()
        {
            Trace("Lumberjack.ProfileNew");
            LogProfile empty = LogProfile.CreateInstance();
            this.logger.SetProfile(empty);

            this.Settings.LastProfilePath = null;

            this.ShowNewProfileSettings();
            this.ui.SelectProfile(null);
            this.ui.SetSaveButtonState(false);
            this.currentProfileIsChanged = false;
            this.SetTitle();
        }

        /// <summary>
        /// Call this when the user wants to open a new profile.
        /// </summary>
        public void ProfileOpen()
        {
            Trace("Lumberjack.ProfileOpen");
            if (!this.SaveProfileIfChanged())
            {
                return;
            }

            string newPath;
            DialogResult result = this.ui.ShowOpenDialog(out newPath);
            if (result == DialogResult.OK && !string.IsNullOrEmpty(newPath))
            {
                this.ui.AddProfile(newPath);
                this.ui.SelectProfile(newPath);
            }
        }

        /// <summary>
        /// Call this when the user wants to save the current profile.
        /// </summary>
        public void ProfileSave()
        {
            Trace("Lumberjack.ProfileSave");
            if (string.IsNullOrEmpty(this.Settings.LastProfilePath))
            {
                this.ProfileSaveAs();
                return;
            }

            this.logger.CurrentProfile.Save(this.Settings.LastProfilePath);
            this.currentProfileIsChanged = false;
            this.SetTitle();
        }

        /// <summary>
        /// Call this when the user wants to save the current profile to a new file.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void ProfileSaveAs()
        {
            Trace("Lumberjack.ProfileSaveAs");
            string newPath;

            while (true)
            {
                DialogResult result = this.ui.ShowSaveAsDialog(out newPath);
                if (result == DialogResult.Cancel)
                {
                    break;
                }

                if (result == DialogResult.OK && !string.IsNullOrEmpty(newPath))
                {
                    try
                    {
                        this.logger.CurrentProfile.Save(newPath);
                        break;
                    }
                    catch (IOException exception)
                    {
                        UnableToSaveProfile(exception);
                        continue;
                    }
                    catch (Exception exception)
                    {
                        UnableToSaveProfile(exception);
                        continue;
                    }
                }
            }

            this.currentProfileIsChanged = false;

            this.ui.AddProfile(newPath);
            this.ui.SelectProfile(newPath);
            this.ui.SetSaveButtonState(true);
            this.SetTitle();
        }

        /// <summary>
        /// Call this when the user selects a new profile.
        /// </summary>
        public void SelectedProfileChanged()
        {
            Trace("Lumberjack.SelectedProfileChanged");
            if (!this.SaveProfileIfChanged())
            {
                return;
            }

            string profilePath = this.ui.GetSelectedProfile();
            if (profilePath == null)
            {
                this.ui.SetSaveButtonState(false);
            }

            this.LoadProfile(profilePath);
        }

        /// <summary>
        /// Call this after the user has changed the set of parameters to log.
        /// </summary>
        public void SelectedProfileSettingsChanged()
        {
            if (this.logger == null)
            {
                Trace("Lumberjack.SelectedProfileSettingsChanged: this.logger is null - assuming this is startup time");
                return;
            }

            if (this.ignoreProfileSettingsChangeNotifications)
            {
                Trace("Lumberjack.SelectedProfileSettingsChanged: ignoring");
                return;
            }
            Trace("Lumberjack.SelectedProfileSettingsChanged: creating new profile");            

            LogProfile profile = LogProfile.CreateInstance();
            this.ui.GetNewProfileSettings(profile);
            this.ui.SetSaveButtonState(true);
            this.logger.SetProfile(profile);
            this.logger.StartLogging();

            this.currentProfileIsChanged = true;
            this.SetTitle();
        }
        
        /// <summary>
        /// Call this when the user wants to select a new folder to store logs in.
        /// </summary>
        public void SetLoggingFolder()
        {
            Trace("Lumberjack.SetLoggingFolder");
            string path;
            DialogResult result = this.ui.PromptForLoggingFolder(out path);
            if (result == DialogResult.OK)
            {
                this.Settings.LogFolderPath = path;
                this.Settings.Save();
            }
        }

        /// <summary>
        /// Directory with canned profiles & SSM database
        /// </summary>
        /// <remarks>For test use only</remarks>
        /// <returns>Directory with canned profiles & SSM database</returns>
        internal static string GetConfigurationDirectory()
        {
            Trace("Lumberjack.GetConfigurationDirectory");
            if (Lumberjack.configurationDirectory == null)
            {
                string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                if (assemblyPath.Contains("TestResults"))
                {
                    string[] parts = assemblyPath.Split(Path.DirectorySeparatorChar);
                    assemblyPath = string.Join(Path.DirectorySeparatorChar.ToString(), parts, 0, parts.Length - 3);
                }
                assemblyPath = Path.GetDirectoryName(assemblyPath);
                Lumberjack.configurationDirectory = Path.Combine(assemblyPath, "Configuration");
            }
            return Lumberjack.configurationDirectory;
        }
    }
}
