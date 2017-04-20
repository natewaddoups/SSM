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
    public sealed partial class MainForm : Form, IUserInterface
    {
        /// <summary>
        /// Lumberjack application logic.
        /// </summary>
        private Lumberjack lumberjack;

        /// <summary>
        /// Bitmap for double-buffering the 'dashboard' display.
        /// </summary>
        private Bitmap backbuffer;

        /// <summary>
        /// Height of a line of text on the 'dashboard' display.
        /// </summary>
        private float lineHeight = 30;

        /// <summary>
        /// Grid column enumeration.
        /// </summary>
        private enum GridColumns : int
        {
            Enabled,
            Parameter,
            Conversions
        }

        /// <summary>
        /// Set the logging mode to use upon startup.
        /// </summary>
        /// <param name="value">Logging mode to use upon start.</param>
        public void SetStartMode(LogFilterType type)
        {
            this.lumberjack.SetStartupLogFilterType(type);
        }

        /// <summary>
        /// Set the logging profile to use upon startup.
        /// </summary>
        /// <param name="value">Logging profile to use upon startup.</param>
        public void SetStartProfile(string value)
        {
            this.lumberjack.SetStartupLoggingProfile(value);
        }

        /// <summary>
        /// Set the serial port to use upon startup.
        /// </summary>
        /// <param name="value">Serial port to use upon startup.</param>
        public void SetStartPort(string value)
        {
            this.lumberjack.SetStartupPort(value);
        }

        /// <summary>
        /// Create a new instance of the Form object.
        /// </summary>
        public MainForm()
        {
            this.lumberjack = new Lumberjack(this);
            this.InitializeComponent();
        }

        #region GUI Event Handlers

        /// <summary>
        /// Invoked when the form loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.RunUnderExceptionHandler(
                delegate
                {
                    Trace("MainForm_Load");
                    if (string.Compare(Environment.GetEnvironmentVariable("ComputerName"), "Monolith", true) == 0)
                    {
                        this.lumberjack.Settings.Reset();
                    }

                    this.lumberjack.Load();

                    try
                    {
                        if (this.lumberjack.Settings.MainFormMaximized)
                        {
                            this.WindowState = FormWindowState.Maximized;
                        }
                        else
                        {
                            // TODO: is there a way to test if this setting exists w/o try/catch? 
                            Rectangle rect = this.lumberjack.Settings.MainFormRectangle;
                            this.Left = rect.Left;
                            this.Top = rect.Top;
                            this.Width = rect.Width;
                            this.Height = rect.Height;
                        }
                    }
                    catch (System.NullReferenceException)
                    {
                    }
                });
        }

        /// <summary>
        /// Invoked when the form is about to close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Trace("MainForm_FormClosing");

            bool maximized = this.WindowState == FormWindowState.Maximized;
            this.lumberjack.Settings.MainFormMaximized = maximized;
            if (!maximized)
            {
                Rectangle rect = new Rectangle(this.Left, this.Top, this.Width, this.Height);
                this.lumberjack.Settings.MainFormRectangle = rect;
            }

            bool cancel = false;
            this.lumberjack.Closing(ref cancel);
            e.Cancel = cancel;
        }

        /// <summary>
        /// Invoked when the form has closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Trace("MainForm_FormClosed");
        }

        /// <summary>
        /// Invoked when the user has selected a new SSM serial port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ssmSerialPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            RunUnderExceptionHandler(
                delegate
                {
                    string portName = "(none)";
                    if (this.ssmSerialPorts.SelectedItem != null)
                    {
                        portName = this.ssmSerialPorts.SelectedItem.ToString();
                    }

                    Trace("ssmSerialPorts_SelectedIndexChanged: " + portName);

                    this.lumberjack.ChangeSsmSerialPort();
                });
        }

        /// <summary>
        /// Invoked when the user has selected a new PLX serial port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plxSerialPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            RunUnderExceptionHandler(
                delegate
                {
                    string portName = "(none)";
                    if (this.plxSerialPorts.SelectedItem != null)
                    {
                        portName = this.plxSerialPorts.SelectedItem.ToString();
                    }

                    Trace("plxSerialPorts_SelectedIndexChanged: " + portName);

                    this.lumberjack.ChangePlxSerialPort();
                });
        }

        /// <summary>
        /// Invoked when the user has selected a profile from the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void profiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            Trace("profiles_SelectedIndexChanged");

            this.RunUnderExceptionHandler(delegate
            {
                this.lumberjack.SelectedProfileChanged();
            });
        }

        private void logOff_CheckedChanged(object sender, EventArgs e)
        {
            Trace("logOff_CheckedChanged: " + this.logOff.Checked);
            if (this.logOff.Checked)
            {
                this.lumberjack.SetLogFilterType(LogFilterType.NeverLog);
            }
        }

        private void logAlways_CheckedChanged(object sender, EventArgs e)
        {
            Trace("logAlways_CheckedChanged: " + this.logAlways.Checked);
            if (this.logAlways.Checked)
            {
                this.lumberjack.SetLogFilterType(LogFilterType.AlwaysLog);
            }
            return;
        }

        private void logDefogger_CheckedChanged(object sender, EventArgs e)
        {
            Trace("logDefogger_CheckedChanged: " + this.logDefogger.Checked);

            if (this.logDefogger.Checked)
            {
                this.lumberjack.SetLogFilterType(LogFilterType.Defogger);
            }
            return;
        }

        private void logOpenLoop_CheckedChanged(object sender, EventArgs e)
        {
            Trace("logOpenLoop_CheckedChanged: " + this.logOpenLoop.Checked);

            if (this.logOpenLoop.Checked)
            {
                this.lumberjack.SetLogFilterType(LogFilterType.OpenLoop);
            }
            return;
        }

        private void logClosedLoop_CheckedChanged(object sender, EventArgs e)
        {
            Trace("logClosedLoop_CheckedChanged: " + this.logClosedLoop.Checked);

            if (this.logClosedLoop.Checked)
            {
                this.lumberjack.SetLogFilterType(LogFilterType.ClosedLoop);
            }
            return;
        }

        private void logFullThrottle_CheckedChanged(object sender, EventArgs e)
        {
            Trace("logFullThrottle_CheckedChanged: " + this.logFullThrottle.Checked);

            if (this.logFullThrottle.Checked)
            {
                this.lumberjack.SetLogFilterType(LogFilterType.FullThrottle);
            }
            return;
        }

        private void parameterGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Trace("parameterGrid_CellValueChanged");
            this.lumberjack.SelectedProfileSettingsChanged();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            this.RunUnderExceptionHandler(delegate
            {
                Trace("connectButton_Click");
                this.lumberjack.Reconnect();
            });
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            Trace("newButton_Click");
            this.lumberjack.ProfileNew();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            this.RunUnderExceptionHandler(delegate
            {
                Trace("openButton_Click");
                this.lumberjack.ProfileOpen();
            });
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Trace("SaveButton_Click");
            this.lumberjack.ProfileSave();
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            this.RunUnderExceptionHandler(delegate
            {
                Trace("saveAsButton_Click");
                this.lumberjack.ProfileSaveAs();
            });
        }

        private void folderButton_Click(object sender, EventArgs e)
        {
            Trace("FolderButton_Click");
            this.lumberjack.SetLoggingFolder();
        }

        private void openLogFolderButton_Click(object sender, EventArgs e)
        {
            Process.Start(this.lumberjack.LoggingFolder);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Run the given code under an exception handler that will display any errors.
        /// </summary>
        /// <param name="operation">Code.</param>
        private void RunUnderExceptionHandler(VoidVoid operation)
        {
            try
            {
                operation();
                this.SetError(operation.Method.Name, null);
            }
            catch (Exception ex)
            {
                this.SetError(operation.Method.Name, ex);
            }
        }

        /// <summary>
        /// Display the given exception and indicate what was happening when it was thrown.
        /// </summary>
        /// <param name="action">What was happening.</param>
        /// <param name="ex">What was thrown.</param>
        private void SetError(string action, Exception ex)
        {
            this.statusText.Invoke(new ThreadStart(
                delegate
                {
                    //Trace("MainForm.SetError");
                    
                    if (ex == null)
                    {
                        Trace("MainForm.SetError: " + action + " succeeded");
                        this.statusText.Text =
                            action +
                            Environment.NewLine +
                            "Succeeded";
                    }
                    else
                    {
                        Trace("MainForm.SetError: " + action + " " + ex.ToString());
                        if (ex.InnerException != null)
                        {
                            Trace("MainForm.SetError InnerException: " + action + " " + ex.InnerException.ToString());
                        }

                        this.statusText.Text =
                            action +
                            Environment.NewLine +
                            ex.ToString();
                    }
                }));
        }
        
        /// <summary>
        /// Render a log entry to the status window.
        /// </summary>
        /// <param name="args">Log entry.</param>
        private void RenderEntryToStatusControl(LogEventArgs args)
        {
            StringBuilder builder = new StringBuilder(200);
            DateTime now = DateTime.Now;
            builder.Append(now.ToString("yyyy-MM-dd T hh:mm:ss:fff"));
            builder.AppendLine();

            foreach (LogColumn column in args.Row.Columns)
            {
                builder.Append(column.Parameter.Name);
                builder.Append(": ");
                builder.Append(column.ValueAsString);
                builder.AppendLine();
            }
            this.statusText.Text = builder.ToString();
        }

        /// <summary>
        /// Render a log entry to the dashboard window.
        /// </summary>
        /// <param name="args">Log entry.</param>
        private void RenderEntryToDashboard(LogEventArgs args)
        {
            if ((this.backbuffer == null) ||
                (this.backbuffer.Width != this.canvas.ClientSize.Width) ||
                (this.backbuffer.Height != this.canvas.ClientSize.Height))
            {
                this.backbuffer = new Bitmap(this.canvas.ClientSize.Width, this.canvas.ClientSize.Height);
            }

            Brush backgroundBrush = Brushes.Black;
            Brush textBrush = Brushes.LightGray;
            Color highlightColor = Color.FromArgb(32, 32, 32);
            using (Brush highlightBrush = new SolidBrush(highlightColor))
            using (Graphics offscreen = Graphics.FromImage(this.backbuffer))
            {   
                Font font = new Font(new FontFamily("Arial"), lineHeight - 2, GraphicsUnit.Pixel);
                const int pseudoParameterCount = 2;
                int lines = args.Row.Columns.Count + pseudoParameterCount;                
                this.lineHeight = Math.Min (lineHeight, (float)this.backbuffer.Height / (float) lines);
                SizeF valueSize = offscreen.MeasureString("XXXXXXX", font);
                float divider = this.Width - valueSize.Width;
                float maxWidth = 0;

                offscreen.FillRectangle(backgroundBrush, 0, 0, this.backbuffer.Width, this.backbuffer.Height);

                for (int i = 0; i < lines; i++)
                {
                    float top = i * lineHeight;
                        
                    if (i % 2 == 1)
                    {
                        offscreen.FillRectangle(
                            highlightBrush,
                            0,
                            top, 
                            this.backbuffer.Width, 
                            lineHeight);
                    }

                    string name;
                    string value;
                    
                    if (i == 0)
                    {
                        DateTime now = DateTime.Now;
                        name = "Clock Time";
                        value = now.ToString("hh:mm");
                    }
                    else if (i == 1)
                    {
                        TimeSpan elapsedTime = DateTime.Now.Subtract(this.lumberjack.LogStartTime);
                        name = "Elapsed Time";
                        value = ((int)elapsedTime.TotalMilliseconds).ToString();
                    }
                    else
                    {
                        int column = i - pseudoParameterCount;
                        name = args.Row.Columns[column].Parameter.Name;
                        value = args.Row.Columns[column].ValueAsString;
                    }

                    SizeF nameSize = offscreen.MeasureString(name, font);
                    offscreen.DrawString(
                        name,
                        font,
                        textBrush, 
                        divider - (nameSize.Width + 2),
                        top + 1);

                    offscreen.DrawString(
                        value,
                        font,
                        textBrush,
                        divider + 1,
                        top + 1);
                    maxWidth = Math.Max(maxWidth, nameSize.Width + valueSize.Width);
                }

                float newLineHeight = lineHeight * (backbuffer.Width / maxWidth);
                lineHeight += (float) 0.1 * (newLineHeight - lineHeight);

                using (Graphics graphics = this.canvas.CreateGraphics())
                {
                    graphics.DrawImageUnscaled(this.backbuffer, 0, 0);
                }
            }
        }

        /// <summary>
        /// Trace a message to the debug console.
        /// </summary>
        /// <param name="s"></param>
        private void Trace(string s)
        {
            Debug.WriteLine(s);
        }

        #endregion

        private void removeButton_Click(object sender, EventArgs e)
        {
            object selected = this.profiles.SelectedItem;
            if (selected != null)
            {
                this.profiles.Items.Remove(selected);
            }
            this.lumberjack.Settings.Save();            
        }
    }
}