using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NateW.Ssm;

namespace NateW.Ssm.ApplicationLogic
{
    internal class MockUserInterface : IUserInterface
    {        
        public bool SsmSerialPortListContainsResult = false;
        public bool PlxSerialPortListContainsResult = false;
        public string GetSelectedSsmSerialPortResult = MockEcuStream.PortName;
        public string GetSelectedPlxSerialPortResult = MockEcuStream.PortName;
        public string GetSelectedProfileResult = @"..\..\..\Configuration\Turbo dynamics.profile";
        public IEnumerable<PathDisplayAdaptor> ProfilesResult = new PathDisplayAdaptor[0];

        public DialogResult PromptToSaveProfileBeforeChangingResult = DialogResult.OK;
        public string ShowOpenFileDialogPath = "bogus.profile";
        public DialogResult ShowOpenFileDialogResult = DialogResult.OK;
        public string ShowSaveAsFileDialogPath = "bogus.profile";
        public DialogResult ShowSaveAsFileDialogResult = DialogResult.OK;
        public string PromptForLoggingFolderPath = "bogus.directory";
        public DialogResult PromptForLoggingFolderResult = DialogResult.OK;
        public LogProfile Profile = null;

        private DateTime created;
        private Lumberjack lumberjack;
        private StringBuilder builder = new StringBuilder();

        public Lumberjack Lumberjack
        {
            set
            {
                this.lumberjack = value;
            }
        }

        public MockUserInterface()
        {
            this.created = DateTime.Now;
            this.RecordEvent("MockUserInterface Constructor");
        }

        private void RecordEvent(string message)
        {
            this.builder.AppendLine(message);
            Console.WriteLine(message);
        }

        public override string ToString()
        {
            return this.builder.ToString();
        }

        public void Invoke(ThreadStart code)
        {
            this.RecordEvent("Invoke: " + code.Method.Name);
            code();
        }

        public void Close()
        {
            this.RecordEvent("Close");
        }

        public void SetTitle(string title)
        {
            this.RecordEvent("SetTitle: " + title);
        }

        public void SetLogMode(LogFilterType mode)
        {
            this.RecordEvent("SetLogMode: " + mode.ToString());
        }

        public void SetLogFilterType(LogFilterType type)
        {
            this.RecordEvent("SetLogFilterType: " + type.ToString());
        }

        public LogFilterType GetLogFilterType()
        {
            this.RecordEvent("GetLogFilterTyp");
            return LogFilterType.AlwaysLog;
        }

        /// <summary>
        /// Add the given serial port to the UI's SSM port list.
        /// </summary>
        /// <param name="name">name of a serial port</param>
        public void AddSsmSerialPort(string name)
        {
            this.RecordEvent("AddSsmSerialPort: " + name);
        }

        /// <summary>
        /// Add the given serial port to the UI's PLX port list.
        /// </summary>
        /// <param name="name">name of a serial port</param>
        public void AddPlxSerialPort(string name)
        {
            this.RecordEvent("AddPlxSerialPort: " + name);
        }

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
        public bool SsmSerialPortListContains(string name)
        {
            return this.SsmSerialPortListContainsResult;
        }

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
        public bool PlxSerialPortListContains(string name)
        {
            return this.PlxSerialPortListContainsResult;
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
        public void SelectSsmSerialPort(string name)
        {
            this.RecordEvent("SelectSsmSerialPort: " + name);
            this.GetSelectedSsmSerialPortResult = name;
            this.lumberjack.ChangeSsmSerialPort();
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
        public void SelectPlxSerialPort(string name)
        {
            this.RecordEvent("SelectPlxSerialPort: " + name);
            this.GetSelectedPlxSerialPortResult = name;
            this.lumberjack.ChangePlxSerialPort();
        }

        /// <summary>
        /// Get the name of the currently selected SSM serial port.
        /// </summary>
        /// <returns>name of the currently selected serial port</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetSelectedSsmSerialPort()
        {
            return this.GetSelectedSsmSerialPortResult;
        }

        /// <summary>
        /// Get the name of the currently selected PXL serial port.
        /// </summary>
        /// <returns>name of the currently selected serial port</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetSelectedPlxSerialPort()
        {
            return this.GetSelectedPlxSerialPortResult;
        }

        public void AddProfile(string path)
        {
            this.RecordEvent("AddProfile: " + path);
        }

        public void SelectProfile(string path)
        {
            this.RecordEvent("SelectProfile: " + path);
        }

        public string GetSelectedProfile()
        {
            return this.GetSelectedProfileResult;
        }

        public IEnumerable<PathDisplayAdaptor> Profiles
        {
            get
            {
                this.RecordEvent("get_Profiles");
                return ProfilesResult;
            }
        }

        public DialogResult PromptToSaveProfileBeforeChanging(string path)
        {
            this.RecordEvent("PromptToSaveProfileBeforeChanging: " + path);
            return PromptToSaveProfileBeforeChangingResult;
        }

        public void ShowLogFolder(string path)
        {
            this.RecordEvent("ShowLogFolder: " + path);
        }

        public void SetSaveButtonState(bool enabled)
        { 
            this.RecordEvent("SaveButtonEnabled: " + enabled);
        }

        public void ConnectionStateChanged(bool connected)
        {
            this.RecordEvent("ConnectionStateChanged: " + connected);
        }

        public void RenderLogEntry(LogEventArgs args)
        {
            Thread.Sleep(3);
            StringBuilder parameters = new StringBuilder();
            foreach(LogColumn column in args.Row.Columns)
            {
                if (parameters.Length != 0)
                {
                    parameters.Append(", ");
                }
                parameters.Append(column.Parameter.Name);
            }

            this.RecordEvent("RenderLogEntry: " + parameters.ToString() + " & mui created: " + this.created.Millisecond);
        }

        public void RenderError(Exception exception)
        {
            this.RecordEvent("RenderLogError: " + exception.Message);
        }

        public DialogResult ShowOpenDialog(out string path)
        {
            this.RecordEvent("ShowOpenFileDialog");
            path = ShowOpenFileDialogPath;
            return ShowOpenFileDialogResult;
        }

        public DialogResult ShowSaveAsDialog(out string path)
        {
            this.RecordEvent("ShowSaveAsFileDialog");
            path = ShowSaveAsFileDialogPath;
            return ShowSaveAsFileDialogResult;
        }

        public DialogResult PromptForLoggingFolder(out string path)
        {
            this.RecordEvent("PromptForLoggingFolder");
            path = PromptForLoggingFolderPath;
            return PromptForLoggingFolderResult;
        }

        public void PopulateParameterList(ParameterDatabase database)
        {
            this.RecordEvent("PopulateParameterList");
        }

        public void GetNewProfileSettings(LogProfile profile)
        {
            this.RecordEvent("GetNewProfileSettings");
        }

        public void ShowNewProfileSettings(LogProfile profile)
        {
            this.RecordEvent("ShowNewProfileSettings");
        }
    }
}
