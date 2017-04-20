using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using NateW.Ssm;
using Microsoft.Win32;

namespace NateW.Ssm.ApplicationLogic
{
    public abstract class SawMillScreen
    {
        private string name;
        private IEnumerable<string> parameters;
        private LogProfile profile;
        private bool foregroundOnly;

        public IEnumerable<string> Parameters
        {
            get { return this.parameters; }
        }

        public bool ForegroundOnly
        {
            get { return this.foregroundOnly; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public override string ToString()
        {
            return this.name;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        protected SawMillScreen(
            string name, 
            bool foregroundOnly,
            IEnumerable<string> parameters)
        {
            this.name = name;
            this.parameters = parameters;
            this.foregroundOnly = foregroundOnly;
        }

        /// <summary>
        /// Create a LogProfile, using parameters passed to ctor and the given SsmLogger's database.
        /// </summary>
        public LogProfile GetProfile(ParameterDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            if (this.profile != null)
            {
                return this.profile;
            }

            this.profile = LogProfile.CreateInstance();
            foreach (string combined in this.parameters)
            {
                string[] idAndUnits = combined.Split(',');

                if (idAndUnits.Length != 2)
                {
                    continue;
                }

                string id = idAndUnits[0];
                string units = idAndUnits[1];

                try
                {
                    profile.Add(id, units, database);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Parameter " + id + " is not supported for your car." + Environment.NewLine +
                        "The " + this.name + " screen will not work.");
                }
            }
            if (this.profile.UserData != null)
            {
                System.Diagnostics.Debugger.Break();
            }
            this.profile.UserData = this;

            return this.profile;
        }

        /// <summary>
        /// Copy the screen data to the clipboard
        /// </summary>
        public virtual string GetClipboardText()
        {
            return string.Empty;
        }

        /// <summary>
        /// Double-Click (behavior determined by derived classes)
        /// </summary>
        public virtual void DoubleClick()
        {
        }

        /// <summary>
        /// Single-Click (behavior determined by derived classes)
        /// </summary>
        public virtual void SingleClick()
        {
        }

        /// <summary>
        /// Add data from a log entry
        /// </summary>
        public abstract void AddData(LogRow data);

        /// <summary>
        /// Paint the screen
        /// </summary>
        public abstract void Paint(Graphics graphics, float width, float height);

        /// <summary>
        /// Write data to text stream
        /// </summary>
        public abstract void WriteTo(StreamWriter writer);

        /// <summary>
        /// Read data from text stream
        /// </summary>
        public abstract void ReadFrom(StreamReader reader);
    }
}
