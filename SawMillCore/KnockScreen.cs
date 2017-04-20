using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Drawing;

namespace NateW.Ssm.ApplicationLogic
{
    public class KnockScreen : HeatMapScreen
    {
        private float lastCount = -1;

        /// <summary>
        /// Constructor
        /// </summary>
        public KnockScreen(
            double maxRpm,
            double maxLoad)
            : base(
            "Knock",
            Configuration.GetStringValue("ParameterRpm"),
            Configuration.GetStringValue("ParameterLoad"),
            Configuration.GetStringValue("ParameterKnockSum"),
            (float)maxRpm,
            (float)maxLoad,
            0f,
            false)
        {
        }

        /// <summary>
        /// Add the given data to the heat map
        /// </summary>
        /// <param name="data">A row of data from the logger</param>
        public override void AddData(LogRow data)
        {
            float xValue;
            float yValue;
            float zValue;
            string zName;
            this.GetDataPoint(data, out xValue, out yValue, out zValue, out zName);

            StringBuilder builder = new StringBuilder();
            builder.Append(zName);
            builder.Append(": ");
            builder.Append(zValue.ToString("0", CultureInfo.InvariantCulture));
            this.lastTrace = builder.ToString();

            if (this.lastCount == -1)
            {
                this.lastCount = zValue;
            }
            
            float difference = 0;
            
            if (zValue < this.lastCount)
            {
                // The knock counter has incremented, but wrapped
                difference = 1;
            }
            
            if (zValue > this.lastCount)
            {
                difference = zValue - this.lastCount;
            }

            this.lastCount = zValue;
            this.AddDataPoint(xValue, yValue, difference);
        }

        /// <summary>
        /// Get a data point for the heat map
        /// </summary>
        protected override bool GetDataPoint(
            LogRow data,
            out float xValue,
            out float yValue,
            out float zValue,
            out string zName)
        {
            try
            {
                LogColumn x = data.GetColumn(this.xParameterName);
                LogColumn y = data.GetColumn(this.yParameterName);
                LogColumn z = data.GetColumn(this.zParameterName);

                xValue = (float)x.ValueAsDouble;
                yValue = (float)y.ValueAsDouble;
                zValue = (float)z.ValueAsDouble;

                zName = z.Conversion.Units;
            }
            catch (InvalidOperationException)
            {
                xValue = -1;
                yValue = -1;
                zValue = 0;
                zName = "Disabled";
            }
            return true;
        }

        /// <summary>
        /// Increment the knock count at the appropriate cell
        /// </summary>
        /// <param name="xValue"></param>
        /// <param name="yValue"></param>
        /// <param name="increment"></param>
        private void AddDataPoint(float xValue, float yValue, float increment)
        {
            int xIndex;
            int yIndex;
            Cell current;
            if (!this.TryGetCell(xValue, yValue, out xIndex, out yIndex, out current))
            {
                return;
            }

            current.Average += increment;
            current.Maximum += increment;
            current.Hits++;
        }

        public override void SingleClick()
        {
            if (this.displayValue == DisplayValue.Undefined)
            {
                this.displayValue = DisplayValue.Hits;
            }
            else
            {
                this.displayValue = DisplayValue.Undefined;
            }
        }

        /// <summary>
        /// Get the text for the given Cell
        /// </summary>
        /// <param name="cell">Cell</param>
        /// <returns>Text</returns>
        protected override string GetCellText(Cell cell)
        {
            return cell.Average.ToString("0", CultureInfo.InvariantCulture);
        }
    }
}
