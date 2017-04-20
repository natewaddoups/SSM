///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// Lumberjack.Private.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NateW.J2534;
using NateW.Ssm.ApplicationLogic.Properties;

namespace NateW.Ssm.ApplicationLogic
{
    public partial class Lumberjack
    {
        /// <summary>
        /// Prompt user to save profile if it has changed.
        /// </summary>
        /// <returns>False if the user doesn't want to save, otherwise true.</returns>
        private bool SaveProfileIfChanged()
        {
            Trace("Lumberjack.SaveProfileIfChanged");
            if (this.currentProfileIsChanged)
            {
                if (!PromptToSaveChangedProfile())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Load a logging profile from the given path.
        /// </summary>
        /// <param name="newProfilePath">Path to the desired profile.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void LoadProfile(string newProfilePath)
        {
            Trace("Lumberjack.LoadProfile");
            if (string.IsNullOrEmpty(newProfilePath))
            {
                return;
            }

            this.ui.Invoke(delegate
            {
                try
                {
                    LogProfile newProfile = LogProfile.Load(newProfilePath, this.database);
                    this.logger.SetProfile(newProfile);
                    this.Settings.LastProfilePath = newProfilePath;
                    this.Settings.Save();
                    Trace("Lumberjack.ContinueLoadProfile: setting ignore");
                    this.ignoreProfileSettingsChangeNotifications = true;
                    this.ShowNewProfileSettings();
                    this.ignoreProfileSettingsChangeNotifications = false;
                    Trace("Lumberjack.ContinueLoadProfile: clearing ignore");
                    this.ui.SetSaveButtonState(true);
                    this.currentProfileIsChanged = false;
                    this.SetTitle();
                    this.logger.StartLogging();
                }
                catch (Exception exception)
                {
                    Trace("Exception thrown during ContinueLoadProfile on UI thread");
                    Trace(exception.ToString());
                }
            });
        }

        /// <summary>
        /// Save the list of known profiles, so that they may be reloaded next time.
        /// </summary>
        private void SaveProfileList()
        {
            StringCollection paths = this.Settings.RecentProfiles;
            if (paths == null)
            {
                paths = new StringCollection();
            }
            paths.Clear();

            foreach (PathDisplayAdaptor adaptor in this.ui.Profiles)
            {
                paths.Add(adaptor.Path);
            }

            this.Settings.RecentProfiles = paths;
            this.Settings.Save();
        }

        /// <summary>
        /// Initialize the list of available serial ports.
        /// </summary>
        private void InitializeSerialPortList()
        {
            ignoreSerialPortChangeNotifications = true;

            if (SsmUtility.OpenPort20Exists())
            {
                Trace("Lumberjack.OpenPort20Exists: OpenPort 2.0 found.");
                this.ui.AddSsmSerialPort(SsmUtility.OpenPort20DisplayName);
            }
            else
            {
                Trace("Lumberjack.OpenPort20Exists: OpenPort 2.0 not found.");
            }

            this.ui.AddSsmSerialPort(MockEcuStream.PortName);
            this.ui.AddPlxSerialPort(ExternalSensors.NullSerialPortName);
            foreach (string name in System.IO.Ports.SerialPort.GetPortNames())
            {
                this.ui.AddSsmSerialPort(name);
                this.ui.AddPlxSerialPort(name);
            }
            ignoreSerialPortChangeNotifications = false;
        }

        /// <summary>
        /// Initialize the list of known profiles.
        /// </summary>
        private void InitializeProfileList()
        {
            if (this.Settings.RecentProfiles != null)
            {
                foreach (string path in this.Settings.RecentProfiles)
                {
                    if (File.Exists(path))
                    {
                        this.ui.AddProfile(path);
                    }
                }
            }
        }

        /// <summary>
        /// Initialize the path to the log-file folder.
        /// </summary>
        private void InitializeLogFolderPath()
        {
            string path = this.Settings.LogFolderPath;
            if (string.IsNullOrEmpty(path))
            {
                path = Environment.GetEnvironmentVariable("HOMEPATH");
                this.Settings.LogFolderPath = path;
            }

            this.ui.ShowLogFolder(path);
        }

        /// <summary>
        /// Update the UI to show the new profile.
        /// </summary>
        private void ShowNewProfileSettings()
        {
            Trace("Lumberjack.ShowNewProfileSettings");

            bool changed = this.currentProfileIsChanged;
            this.ui.ShowNewProfileSettings(this.logger.CurrentProfile);
            this.currentProfileIsChanged = changed;
        }

        /// <summary>
        /// Set the main window title based on the current application state.
        /// </summary>
        private void SetTitle()
        {
            StringBuilder builder = new StringBuilder(100);
            builder.Append(WindowCaption);
            builder.Append(" - ");
            
            if (!string.IsNullOrEmpty(this.Settings.LastProfilePath))
            {
                builder.Append(new PathDisplayAdaptor(this.Settings.LastProfilePath).ToString());
            }
            else
            {
                builder.Append("new profile");
            }

            if (this.currentProfileIsChanged)
            {
                builder.Append('*');
            }
            string title = builder.ToString();
            this.ui.SetTitle(title);
            Trace("Lumberjack.SetTitle: " + title);
        }

        /// <summary>
        /// Close and then reopen the ECU interface.
        /// </summary>
        private void BeginReopenEcuInterface()
        {
            Trace("Lumberjack.BeginCloseEcuInterface");

            this.ui.ConnectionStateChanged(false);

            if (this.logger == null)
            {
                // Skip the close, go straight to the open.
                ContinueReopenEcuInterface(null);
                return;
            }

            // Consider combining this with ConnectionStateChanged(false)
            if (this.ssmParameterSource != null)
            {
                this.database.Remove(this.ssmParameterSource);
                this.ssmParameterSource = null;
            }

            this.ui.Invoke(new ThreadStart(
            delegate
            {
                this.ui.PopulateParameterList(this.database);
            }));

            this.logger.BeginStopLogging(ContinueReopenEcuInterface, null);
        }

        /// <summary>
        /// Open a new ECU interface
        /// </summary>
        private void ContinueReopenEcuInterface(IAsyncResult unused)
        {
            // The logger will be null the first time down this path
            if (this.logger != null)
            {
                Trace("Lumberjack.ContinueReopenEcuInterface: disconnecting logger");
                this.logger.Disconnect();
                this.logger = null;
            }
            else
            {
                Trace("Lumberjack.ContinueReopenEcuInterface: no logger to disconnect");
            }
            
            string portName = string.Empty;
            this.ui.Invoke(delegate()
            {
                portName = this.ui.GetSelectedSsmSerialPort();
            });

            if (!string.IsNullOrEmpty(portName))
            {
                this.OpenEcuInterface(portName);
            }
        }

        /// <summary>
        /// Second half of the MainForm_Closing handler
        /// </summary>
        private void ContinueClosing(IAsyncResult asyncResult)
        {
            this.ui.Invoke(delegate { this.ui.Close(); });
        }

        /// <summary>
        /// Open the ECU interface.
        /// </summary>
        /// <param name="name">Name of the serial port to use for ECU communication.</param>
        private void OpenEcuInterface(string name)
        {
            Trace("Lumberjack.OpenEcuInterface: " + name);

            if (name == SsmUtility.OpenPort20DisplayName)
            {
                name = SsmUtility.OpenPort20PortName;
                Trace("Lumberjack.OpenEcuInterface: rewrote name to: " + name);
            }

            string configurationDirectory = Lumberjack.GetConfigurationDirectory();

            this.logger = SsmRecordingLogger.GetInstance(database, configurationDirectory, name, this.CreateLogWriter);
            this.logger.LogStart += this.OnLogStart;
            this.logger.LogEntry += this.OnLogEntry;
            this.logger.LogStop += this.OnLogStop;
            this.logger.LogError += this.OnLogError;

            this.logger.BeginConnect(ConnectCallback, name);
        }

        /// <summary>
        /// Callback to be invoked when the ECU connection is established.
        /// </summary>
        /// <param name="asyncResult">Async result.</param>
        private void ConnectCallback(IAsyncResult asyncResult)
        {
            Trace("Lumberjack.ConnectCallback");

            if (this.logger == null)
            {
                return;
            }

            Exception exception = null;
            try
            {
                this.ssmParameterSource = this.logger.EndConnect(asyncResult);
                this.database.Add(this.ssmParameterSource);
            }
            catch (IOException ex)
            {
                exception = ex;
            }
            catch (UnauthorizedAccessException ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                this.ui.Invoke(new ThreadStart(
                    delegate
                    {
                        this.ui.RenderError(exception);
                        this.ui.ConnectionStateChanged(false);
                    }));
                return;
            }

            try
            {
                this.ui.Invoke(new ThreadStart(
                delegate
                {
                    this.ui.PopulateParameterList(this.database);
                    this.ui.ConnectionStateChanged(true);

                    this.LoadSelectedProfile();

                    this.Settings.DefaultSsmPort = (string)asyncResult.AsyncState;
                    this.Settings.Save();
                    this.currentProfileIsChanged = false;

                    LogFilterType filterType = this.ui.GetLogFilterType();
                    this.SetLogFilterType(filterType);
                }));
            }
            catch (IOException ex)
            {
                this.ui.Invoke(new ThreadStart(
                    delegate
                    {
                        this.ui.RenderError(ex);
                    }));
            }
        }

        /// <summary>
        /// Load the currently selected logging profile.
        /// </summary>
        private void LoadSelectedProfile()
        {
            Trace("Lumberjack.LoadSelectedProfile");
            if ((this.logger.CurrentProfile != null) && this.currentProfileIsChanged)
            {
                string oldPath = new PathDisplayAdaptor(this.Settings.LastProfilePath).ToString();
                DialogResult dialogResult = this.ui.PromptToSaveProfileBeforeChanging(oldPath);

                if (dialogResult == DialogResult.Cancel)
                {
                    this.ui.SelectProfile(this.Settings.LastProfilePath);
                    return;
                }
                else if (dialogResult == DialogResult.Yes)
                {
                    this.logger.CurrentProfile.Save(this.Settings.LastProfilePath);
                }
            }

            string profilePath = this.ui.GetSelectedProfile();
            Trace("Lumberjack.LoadSelectedProfile: " + profilePath);
            this.logger.BeginStopLogging(ContinueLoadSelectedProfile, profilePath);
        }

        /// <summary>
        /// Second half of LoadSelectedProfile
        /// </summary>
        private void ContinueLoadSelectedProfile(IAsyncResult asyncResult)
        {
            Trace("Lumberjack.ContinueLoadSelectedProfile");
            string profilePath = (string) asyncResult.AsyncState;

            this.ui.Invoke(delegate
            {
                Trace("Lumberjack.ContinueLoadSelectedProfile (UI thread)");
                LogProfile profile;
                if (profilePath == null)
                {
                    Trace("Lumberjack.ContinueLoadSelectedProfile: creating trivial profile");
                    profile = LogProfile.CreateInstance();
/*                    foreach (SsmParameter parameter in this.logger.Database.Parameters)
                    {
                        // Add the RPM parameter and nothing else.
                        if (parameter.Id == "P8")
                        {
                            profile.Add(parameter, parameter.Conversions[0]);
                            break;
                        }
                    }
 */ 
                }
                else
                {
                    Trace("Lumberjack.ContinueLoadSelectedProfile: loading " + profilePath);
                    profile = LogProfile.Load(profilePath, this.database);
                }

                this.logger.SetProfile(profile);
                this.logger.StartLogging();
                this.Settings.LastProfilePath = profilePath;
                this.Settings.Save();

                this.ignoreProfileSettingsChangeNotifications = true;
                this.ui.ShowNewProfileSettings(this.logger.CurrentProfile);                
                this.ui.SelectProfile(this.Settings.LastProfilePath);
                this.ignoreProfileSettingsChangeNotifications = false;

                this.currentProfileIsChanged = false;
                this.ui.SetSaveButtonState(true);
                this.SetTitle();
            });
        }

        /// <summary>
        /// Prompt to save the profile.
        /// </summary>
        /// <remarks>
        /// Assumes that the profile has been checked for changes, and it has changed.
        /// </remarks>
        /// <returns>False if the user wants to cancel whatever lead to this.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool PromptToSaveChangedProfile()
        {
            Trace("Lumberjack.PromptToSaveChangedProfile");

            string oldPath = this.Settings.LastProfilePath;
            if (string.IsNullOrEmpty(oldPath))
            {
                oldPath = Path.Combine(Environment.CurrentDirectory, "New Profile.profile");
            }
            DialogResult saveResult = this.ui.PromptToSaveProfileBeforeChanging(oldPath);

            if (saveResult == DialogResult.Cancel)
            {
                return false;
            }
            else if (saveResult == DialogResult.Yes)
            {
                this.ProfileSaveAs();
            }

            return true;
        }

        /// <summary>
        /// Show a dialog to explain why the profile couldn't be saved.
        /// </summary>
        private static void UnableToSaveProfile(Exception exception)
        {
            if (exception is IOException)
            {
                MessageBox.Show("Unable to save profile:" +
                    Environment.NewLine +
                    exception.Message,
                    WindowCaption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                MessageBox.Show("Unable to save profile:" +
                    Environment.NewLine +
                    exception.Message +
                    Environment.NewLine +
                    exception.GetType().Name,
                    WindowCaption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

                Trace("Unexpected exception saving profile!");
                Trace(exception.ToString());
            }
        }

        /// <summary>
        /// Write a debugging trace message.
        /// </summary>
        /// <param name="message">Debug trace message.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private static void Trace(string message)
        {
            System.Diagnostics.Trace.WriteLine(message);
        }
    }
}
