///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// MainForm.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using NateW.Ssm.Properties;

namespace NateW.Ssm.ApplicationLogic
{
    public partial class MainForm
    {
        private bool exceptionLogged;
            
        /// <summary>
        /// Invoke code on the appropriate thread for the UI.
        /// </summary>
        /// <param name="code">Code.</param>
        void IUserInterface.Invoke(ThreadStart code)
        {
            if (this.IsDisposed)
            {
                Trace("MainForm.Invoke called after MainForm.Dispose()");
                Trace("Delegate: " + code.Method.Name);
                Trace("StackTrace:");
                Trace(new StackTrace(true).ToString());
                return;
            }

            try
            {
                base.Invoke(code);
                exceptionLogged = false;
            }
            catch (ArgumentException exception)
            {
                if (!exceptionLogged)
                {
                    Trace("MainForm.Invoke threw: " + exception.Message);
                }

                exceptionLogged = true;
            }
        }

        /// <summary>
        /// Close the main window
        /// </summary>
        void IUserInterface.Close()
        {
            this.Close();
        }

        /// <summary>
        /// Set the title of the UI main window.
        /// </summary>
        /// <param name="title">Title text.</param>
        void IUserInterface.SetTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Display the given log mode in the UI.
        /// </summary>
        /// <param name="mode">New logging mode.</param>
        void IUserInterface.SetLogFilterType(LogFilterType type)
        {
            if (type == LogFilterType.AlwaysLog)
            {
                this.logAlways.Checked = true;
            }
            else if (type == LogFilterType.NeverLog)
            {
                this.logOff.Checked = true;
            }
            else if (type == LogFilterType.Defogger)
            {
                this.logDefogger.Checked = true;
            }
            else if (type == LogFilterType.ClosedLoop)
            {
                this.logClosedLoop.Checked = true;
            }
            else if (type == LogFilterType.OpenLoop)
            {
                this.logOpenLoop.Checked = true;
            }
            else if (type == LogFilterType.FullThrottle)
            {
                this.logFullThrottle.Checked = true;
            }
            else
            {
                throw new NotImplementedException("Unsupported LogFilterType: " + type.ToString());
            }
        }

        LogFilterType IUserInterface.GetLogFilterType()
        {
            if (this.logAlways.Checked)
            {
                return LogFilterType.AlwaysLog;
            }
            else if (this.logOff.Checked)
            {
                return LogFilterType.NeverLog;
            }
            else if (this.logDefogger.Checked)
            {
                return LogFilterType.Defogger;
            }
            else if (this.logClosedLoop.Checked)
            {
                return LogFilterType.ClosedLoop;
            }
            else if (this.logOpenLoop.Checked)
            {
                return LogFilterType.OpenLoop;
            }
            else if (this.logFullThrottle.Checked)
            {
                return LogFilterType.FullThrottle;
            }
            else
            {
                throw new NotImplementedException("Unknown log filter type.");
            }
        }

        /// <summary>
        /// Add a serial port to the UI.
        /// </summary>
        /// <param name="name">Serial port name.</param>
        void IUserInterface.AddSsmSerialPort(string name)
        {
            this.ssmSerialPorts.Items.Add(name);
        }

        /// <summary>
        /// Add a serial port to the UI.
        /// </summary>
        /// <param name="name">Serial port name.</param>
        void IUserInterface.AddPlxSerialPort(string name)
        {
            this.plxSerialPorts.Items.Add(name);
        }

        /// <summary>
        /// Indicate whether or not the serial port list contains the given port name.
        /// </summary>
        /// <remarks>
        /// This is invoked with the name of the port that was used the last 
        /// time the application was run.  That port may or may not exist this
        /// time.
        /// </remarks>
        /// <param name="name">Name of a serial port.</param>
        /// <returns>True if the port is in the list of known ports.</returns>
        bool IUserInterface.SsmSerialPortListContains(string name)
        {
            return this.ssmSerialPorts.Items.Contains(name);
        }

        /// <summary>
        /// Indicate whether or not the serial port list contains the given port name.
        /// </summary>
        /// <remarks>
        /// This is invoked with the name of the port that was used the last 
        /// time the application was run.  That port may or may not exist this
        /// time.
        /// </remarks>
        /// <param name="name">Name of a serial port.</param>
        /// <returns>True if the port is in the list of known ports.</returns>
        bool IUserInterface.PlxSerialPortListContains(string name)
        {
            return this.plxSerialPorts.Items.Contains(name);
        }

        /// <summary>
        /// Get the name of the currently selected serial port
        /// </summary>
        /// <returns>name of the currently selected serial port</returns>
        string IUserInterface.GetSelectedSsmSerialPort()
        {
            string portName = null;
            this.ssmSerialPorts.Invoke(new MethodInvoker(delegate
            {
                portName = (string)this.ssmSerialPorts.SelectedItem;
            }));
            return portName;
        }

        /// <summary>
        /// Get the name of the currently selected serial port
        /// </summary>
        /// <returns>name of the currently selected serial port</returns>
        string IUserInterface.GetSelectedPlxSerialPort()
        {
            string portName = null;
            this.plxSerialPorts.Invoke(new MethodInvoker(delegate
            {
                portName = (string)this.plxSerialPorts.SelectedItem;
            }));
            return portName;
        }

        /// <summary>
        /// Select the given serial port in the UI
        /// </summary>
        /// <remarks>
        /// Will be invoked to select the port that was in use in the last
        /// session.
        /// Consider: Have this return a boolean, get rid of the 
        /// SerialPortListContains method.
        /// </remarks>
        /// <param name="name">name of the port to highlight in the UI</param>
        void IUserInterface.SelectSsmSerialPort(string name)
        {
            this.ssmSerialPorts.SelectedItem = name;
        }

        /// <summary>
        /// Select the given serial port in the UI
        /// </summary>
        /// <remarks>
        /// Will be invoked to select the port that was in use in the last
        /// session.
        /// Consider: Have this return a boolean, get rid of the 
        /// SerialPortListContains method.
        /// </remarks>
        /// <param name="name">name of the port to highlight in the UI</param>
        void IUserInterface.SelectPlxSerialPort(string name)
        {
            this.plxSerialPorts.SelectedItem = name;
        }

        /// <summary>
        /// Add a profile to the list shown in the UI
        /// </summary>
        /// <param name="path">path to the profile</param>
        void IUserInterface.AddProfile(string path)
        {
            foreach (PathDisplayAdaptor adaptor in this.profiles.Items)
            {
                if (adaptor.Equals(path))
                {
                    return;
                }
            }

            PathDisplayAdaptor newAdaptor = new PathDisplayAdaptor(path);
            this.profiles.Items.Add(newAdaptor);
        }

        /// <summary>
        /// Select a profile in the UI
        /// </summary>
        /// <param name="path">path to the profile</param>
        void IUserInterface.SelectProfile(string path)
        {
            for (int i = 0; i < this.profiles.Items.Count; i++)
            {
                PathDisplayAdaptor adaptor = (PathDisplayAdaptor) this.profiles.Items[i];
                if (adaptor.Equals(path))
                {
                    this.profiles.SelectedIndex = i;
                    return;
                }
            }
        }

        /// <summary>
        /// Get the path of the currently-selected profile
        /// </summary>
        /// <returns>path to the profile file</returns>
        string IUserInterface.GetSelectedProfile()
        {
            PathDisplayAdaptor adaptor = (PathDisplayAdaptor) this.profiles.SelectedItem;
            if (adaptor == null)
            {
                return null;
            }

            return adaptor.Path;
        }

        /// <summary>
        /// Get the list of profiles currently shown in the UI
        /// </summary>
        IEnumerable<PathDisplayAdaptor> IUserInterface.Profiles
        {
            get
            {
                foreach (PathDisplayAdaptor adaptor in this.profiles.Items)
                {
                    yield return adaptor;
                }
            }
        }

        /// <summary>
        /// Ask the user if they want to save the given profile before 
        /// creating or opening a new profile/
        /// </summary>
        /// <param name="path">path to the modified profile</param>
        /// <returns>yes=save and continue; no=don't save, just continue; cancel=don't continue</returns>
        DialogResult IUserInterface.PromptToSaveProfileBeforeChanging(string path)
        {
            DialogResult result = MessageBox.Show(
                                   this,
                                   "The logging profile has changed.  Would you like to save it before continuing?",
                                   path,
                                   MessageBoxButtons.YesNoCancel);
            return result;
        }

        /// <summary>
        /// Ask the user where logs should be saved
        /// </summary>
        /// <param name="path">folder to save logs to</param>
        /// <returns>ok/cancel</returns>
        DialogResult IUserInterface.PromptForLoggingFolder(out string path)
        {
            //string initialPath = Environment.CurrentDirectory;

            Shell32.ShellClass shell = new Shell32.ShellClass();
            Shell32.Folder2 folder = (Shell32.Folder2)shell.BrowseForFolder(
                this.Handle.ToInt32(),
                "Select Folder...",
                0, // Options
                this.lumberjack.Settings.LogFolderPath
            );
            path = folder.Self.Path;
            this.folderLabel.Text = path;
            
            /*
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckPathExists = true;
            dialog.InitialDirectory = Settings.Default.LogFolderPath;
            dialog.ValidateNames = true;
            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                string path = dialog.FileName;
                path = Path.GetDirectoryName(path);
                Settings.Default.LogFolderPath = path;
            }
            */
            //Environment.CurrentDirectory = initialPath;
            return DialogResult.OK; // TODO: really?
        }

        /// <summary>
        /// Display the path to the folder that logs will be saved in
        /// </summary>
        /// <param name="path">path where logs will be saved</param>
        void IUserInterface.ShowLogFolder(string path)
        {
            this.folderLabel.Text = "Save logs to: " + path;
        }

        /// <summary>
        /// Enable/disable the save button
        /// </summary>
        void IUserInterface.SetSaveButtonState(bool enabled)
        {
            this.saveButton.Enabled = enabled;
        }

        /// <summary>
        /// Show a log entry in the UI
        /// </summary>
        /// <param name="args">log entry</param>
        void IUserInterface.RenderLogEntry(LogEventArgs args)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (tabs.SelectedTab == this.controlTab)
            {
                this.RenderEntryToStatusControl(args);
            }
            else if (tabs.SelectedTab == this.dashboardTab)
            {
                this.RenderEntryToDashboard(args);
            }
        }

        /// <summary>
        /// Show why logging just failed.
        /// </summary>
        /// <remarks>
        /// Users may not understand, but it will be helpful for diagnosing 
        /// problems reported in the field.
        /// </remarks>
        /// <param name="ex">what went wrong</param>
        void IUserInterface.RenderError(Exception ex)
        {
            Trace("LogError");

            if (this.IsDisposed)
            {
                return;
            }

            StringBuilder builder = new StringBuilder();
            do
            {
                builder.AppendLine(ex.ToString());
                builder.AppendLine("Inner Exception: ");
                ex = ex.InnerException;
            } while (ex != null);
            builder.Append("(no inner exception)");

            this.statusText.Text = builder.ToString();
        }

        /// <summary>
        /// Update the UI to show that an ECU has been connected or 
        /// disconnected.
        /// </summary>
        /// <param name="connected">true if an ECU has been connected, false
        /// if an ECU has been disconnected.</param>
        void IUserInterface.ConnectionStateChanged(bool connected)
        {
            Trace("ConnectionStateChanged " + connected);

            if (connected)
            {
                this.logAlways.Enabled = true;
                string ecuIdentifier = this.lumberjack.EcuIdentifier;
                this.ecuIdentifierLabel.Text = "Connected to " + ecuIdentifier;
                this.statusText.Text = "Connected.";
            }
            else
            {
                this.ecuIdentifierLabel.Text = "Not connected";
            }

            // Should be able to preserve log filter type when changing ports...
            //this.logOff.Checked = true;

            this.connectButton.Visible = !connected;
            this.ecuIdentifierLabel.Visible = connected;

            this.profiles.Enabled = connected;
            this.openButton.Enabled = connected;
            this.saveAsButton.Enabled = connected;

            this.logAlways.Enabled = connected;
            this.logDefogger.Enabled = connected;
            this.logClosedLoop.Enabled = connected;
            this.logOpenLoop.Enabled = connected;
            this.logFullThrottle.Enabled = connected;
        }

        /// <summary>
        /// Show an open-file dialog, get the path the users chooses.
        /// </summary>
        /// <param name="path">path to the file to be opened</param>
        /// <returns>ok/cancel</returns>
        DialogResult IUserInterface.ShowOpenDialog(out string path)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = SsmBasicLogger.DefaultProfileExtension;
            dialog.Filter = SsmBasicLogger.FileDialogFilterString;
            DialogResult result = dialog.ShowDialog(this);
            path = dialog.FileName;
            return result;
        }

        /// <summary>
        /// Show a save-as dialog, get the path the user chooses.
        /// </summary>
        /// <param name="path">path to save the file as</param>
        /// <returns>ok/cancel</returns>
        DialogResult IUserInterface.ShowSaveAsDialog(out string path)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = SsmBasicLogger.DefaultProfileExtension;
            dialog.Filter = SsmBasicLogger.FileDialogFilterString;
            DialogResult result = dialog.ShowDialog(this);
            path = dialog.FileName;
            return result;
        }

        /// <summary>
        /// Show the list of parameters available for logging
        /// </summary>
        /// <param name="database">parameter database</param>
        void IUserInterface.PopulateParameterList(ParameterDatabase database)
        {
            Trace("PopulateParameterList");

            this.parameterGrid.Rows.Clear();

            foreach (Parameter parameter in database.Parameters)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.parameterGrid);
                row.Cells[(int)GridColumns.Enabled].Value = false;
                row.Cells[(int)GridColumns.Parameter].Value = parameter;

                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)row.Cells[(int)GridColumns.Conversions];
                cell.DisplayMember = "Units";                
                foreach (Conversion conversion in parameter.Conversions)
                {                    
                    DataGridViewComboBoxCell.ObjectCollection items = cell.Items;
                    items.Add(conversion);
                }
                Conversion defaultConversion = parameter.Conversions[0];
                cell.Value = defaultConversion;

                this.parameterGrid.Rows.Add(row);
            }

            this.parameterGrid.Sort(this.parameterGrid.Columns[1], ListSortDirection.Ascending);
        }

        /// <summary>
        /// Get log profile settings from the UI
        /// </summary>
        /// <param name="profile">log profile</param>
        void IUserInterface.GetNewProfileSettings(LogProfile profile)
        {
            Trace("MainForm.GetNewProfileSettings");

            foreach (DataGridViewRow row in this.parameterGrid.Rows)
            {
                if ((bool)row.Cells[(int)GridColumns.Enabled].Value)
                {
                    DataGridViewComboBoxCell conversionCell = (DataGridViewComboBoxCell)row.Cells[(int)GridColumns.Conversions];
                    Conversion conversion = null;
                    foreach (Conversion candidate in conversionCell.Items)
                    {
                        // The cell.Value can be a string.  WTF?
                        if ((candidate.Units == conversionCell.Value as string) ||
                            (candidate == conversionCell.Value as Conversion))
                        {
                            conversion = candidate;
                            break;
                        }
                    }

                    if (conversion != null)
                    {
                        Parameter parameter = (Parameter)row.Cells[(int)GridColumns.Parameter].Value;
                        profile.Add(parameter, conversion);
                    }
                    else
                    {
                        // TODO: apologize to user?
                    }
                }
            }
        }

        /// <summary>
        /// Show log profiles settings in the UI
        /// </summary>
        /// <param name="profile">log profile</param>
        void IUserInterface.ShowNewProfileSettings(LogProfile profile)
        {
            Trace("ShowNewProfileSettings");
                        
            foreach (DataGridViewRow row in this.parameterGrid.Rows)
            {
                row.Cells[(int)GridColumns.Enabled].Value = false;

                foreach (LogColumn column in profile.Columns)
                {
                    Parameter parameter = column.Parameter;

                    Parameter rowParameter = (Parameter)row.Cells[(int)GridColumns.Parameter].Value;
                    if (rowParameter.Id == parameter.Id)
                    {
                        row.Cells[(int)GridColumns.Enabled].Value = true;
                        DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)row.Cells[(int)GridColumns.Conversions];
                        Conversion selectedConversion = profile.GetConversions(parameter)[0];
                        string selectedUnits = selectedConversion.Units;
                        foreach (Conversion conversion in cell.Items)
                        {
                            if (conversion.Units == selectedUnits)
                            {
                                cell.Value = conversion;
                            }
                        }
                    }
                }
            }
        }
    }
}