///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// IUserInterface.cs
// Abstracts the Win32 and CE versions of the Lumberjack UI
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NateW.Ssm.ApplicationLogic
{
    public interface IUserInterface
    {
        /// <summary>
        /// Set the title of the main window
        /// </summary>
        /// <param name="title">title string</param>
        void SetTitle(string title);

        /// <summary>
        /// Show the new log filter type
        /// </summary>
        /// <param name="mode">new log filter type</param>
        void SetLogFilterType(LogFilterType filterType);

        /// <summary>
        /// Return the current log filter type.
        /// </summary>
        LogFilterType GetLogFilterType();

        /// <summary>
        /// Add the given serial port to the UI's SSM port list.
        /// </summary>
        /// <param name="name">name of a serial port</param>
        void AddSsmSerialPort(string name);

        /// <summary>
        /// Add the given serial port to the UI's PLX port list.
        /// </summary>
        /// <param name="name">name of a serial port</param>
        void AddPlxSerialPort(string name);

        /// <summary>
        /// Indicate whether the given port is in the list in the UI
        /// </summary>
        /// <remarks>
        /// The core code is trying to figure out if the last selected port
        /// (from a previous session) is still available.  
        /// Might be better to have the core code maintain the port list, 
        /// but then again, the UI needs to maintain it anyway...
        /// </remarks>
        /// <param name="name">name of a serial port</param>
        /// <returns>true if the given port is in the list</returns>
        bool SsmSerialPortListContains(string name);

        /// <summary>
        /// Indicate whether the given port is in the list in the UI
        /// </summary>
        /// <remarks>
        /// The core code is trying to figure out if the last selected port
        /// (from a previous session) is still available.  
        /// Might be better to have the core code maintain the port list, 
        /// but then again, the UI needs to maintain it anyway...
        /// </remarks>
        /// <param name="name">name of a serial port</param>
        /// <returns>true if the given port is in the list</returns>
        bool PlxSerialPortListContains(string name);
        
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
        void SelectSsmSerialPort(string name);

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
        void SelectPlxSerialPort(string name);

        /// <summary>
        /// Get the name of the currently selected SSM serial port.
        /// </summary>
        /// <returns>name of the currently selected serial port</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetSelectedSsmSerialPort();

        /// <summary>
        /// Get the name of the currently selected PXL serial port.
        /// </summary>
        /// <returns>name of the currently selected serial port</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetSelectedPlxSerialPort();
        
        /// <summary>
        /// Add a profile to the list shown in the UI
        /// </summary>
        /// <param name="path">path to the profile</param>
        void AddProfile(string path);

        /// <summary>
        /// Select a profile in the UI
        /// </summary>
        /// <param name="path">path to the profile</param>
        void SelectProfile(string path);

        /// <summary>
        /// Get the path of the currently-selected profile
        /// </summary>
        /// <returns>path to the profile file</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetSelectedProfile();

        /// <summary>
        /// Get the list of profiles currently shown in the UI
        /// </summary>
        IEnumerable<PathDisplayAdaptor> Profiles { get; }

        /// <summary>
        /// Ask the user if they want to save the given profile before 
        /// creating or opening a new profile.
        /// </summary>
        /// <param name="path">path to the modified profile</param>
        /// <returns>yes=save and continue; no=don't save, just continue; cancel=don't continue</returns>
        DialogResult PromptToSaveProfileBeforeChanging(string path);

        /// <summary>
        /// Display the path to the folder that logs will be saved in
        /// </summary>
        /// <param name="path">path where logs will be saved</param>
        void ShowLogFolder(string path);

        /// <summary>
        /// Enable/disable the save button
        /// </summary>
        void SetSaveButtonState(bool enabled);

        /// <summary>
        /// Update the UI to show that an ECU has been connected or 
        /// disconnected.
        /// </summary>
        /// <param name="connected">true if an ECU has been connected, false
        /// if an ECU has been disconnected.</param>
        void ConnectionStateChanged(bool connected);

        /// <summary>
        /// Show a log entry in the UI
        /// </summary>
        /// <param name="args">log entry</param>
        void RenderLogEntry(LogEventArgs args);

        /// <summary>
        /// Show why logging just failed.
        /// </summary>
        /// <remarks>
        /// Users may not understand, but it will be helpful for diagnosing 
        /// problems reported in the field.
        /// </remarks>
        /// <param name="ex">what went wrong</param>
        void RenderError(Exception ex);

        /// <summary>
        /// Show an open-file dialog, get the path the users chooses.
        /// </summary>
        /// <param name="path">path to the file to be opened</param>
        /// <returns>ok/cancel</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        DialogResult ShowOpenDialog(out string path);

        /// <summary>
        /// Show a save-as dialog, get the path the user chooses.
        /// </summary>
        /// <param name="path">path to save the file as</param>
        /// <returns>ok/cancel</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        DialogResult ShowSaveAsDialog(out string path);

        /// <summary>
        /// Ask the user where logs should be saved
        /// </summary>
        /// <param name="path">folder to save logs to</param>
        /// <returns>ok/cancel</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        DialogResult PromptForLoggingFolder(out string path);

        /// <summary>
        /// Show the list of parameters available for logging
        /// </summary>
        /// <param name="database">parameter database</param>
        void PopulateParameterList(ParameterDatabase database);

        /// <summary>
        /// Get log profile settings from the UI
        /// </summary>
        /// <param name="profile">log profile</param>
        void GetNewProfileSettings(LogProfile profile);

        /// <summary>
        /// Show log profiles settings in the UI
        /// </summary>
        /// <param name="profile">log profile</param>
        void ShowNewProfileSettings(LogProfile profile);

        /// <summary>
        /// Close the main window
        /// </summary>
        void Close();

        /// <summary>
        /// Invoke a delegate on a UI thread
        /// </summary>
        /// <param name="code"></param>
        void Invoke(ThreadStart code);
    }
}
