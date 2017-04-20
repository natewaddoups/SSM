using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;

namespace NateW.Ssm.ApplicationLogic
{
    public class SixPackScreen : SawMillScreen
    {
        private LogRow data;

        public SixPackScreen(string name, IList<string> parameters)
            : base(name, true, parameters)
        {
        }

        public override void AddData(LogRow data)
        {
            this.data = data;
        }

        public override void Paint(Graphics graphics, float width, float height)
        {
            SixPackPainter painter = new SixPackPainter(graphics, (int) width, (int) height, this.data);
            painter.Paint();
        }

        /// <summary>
        /// Copy the screen data to the clipboard
        /// </summary>
        public override string GetClipboardText()
        {
            StringBuilder builder = new StringBuilder(1000);
            for (int i = 0; i < 6; i++)
            {
                string line = string.Format("{0}: {1} {2}",
                    this.data.Columns[i].Parameter.Name,
                    this.data.Columns[i].ValueAsString,
                    this.data.Columns[i].Conversion.Units);
                builder.AppendLine(line);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Write data to text stream
        /// </summary>
        public override void WriteTo(StreamWriter writer)
        {
        }

        /// <summary>
        /// Read data from text stream
        /// </summary>
        public override void ReadFrom(StreamReader reader)
        {
        }
    }
}
