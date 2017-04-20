using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace NateW.Ssm.ApplicationLogic
{
    public class AfrScatterScreen : ScatterPlotScreen
    {
        private static bool supported;
        private string desiredAfrParameterName;

        public AfrScatterScreen(double maxMaf)
            : base(
            "AFR", 
            (float)maxMaf, // xMax
            50, // xDivision
            0.8F, // yMin
            1.2F, // yMax
            0.05F, // yDivision
            true, // foreground only
            Configuration.GetStringValue("ParameterMaf"), 
            Configuration.GetStringValue("ParameterActualAfr"), 
            Configuration.GetStringValue("ParameterDesiredAfr"))
        {
            supported = true;

            try
            {
                this.desiredAfrParameterName = Utility.GetParameterName(Configuration.GetStringValue("ParameterDesiredAfr"));
            }
            catch (ArgumentException ex)
            {
                string message = "Desired AFR parameter: " + ex.Message;
                MessageBox.Show(message, "Check configuration.");
                throw new Exception(message);
            }
        }

        protected override void GetDataPoint(
            LogRow data,
            out double xValue,
            out double yValue,
            out string xName,
            out string yName)
        {
            xName = "MAF";
            yName = "Sensor / Target";
            xValue = 0;
            yValue = 0;

            if (!supported)
            {
                return;
            }

            try
            {
                LogColumn xColumn = data.GetColumn(this.xParameterName);
                xValue = xColumn.ValueAsDouble;
                LogColumn ySensorColumn = data.GetColumn(this.yParameterName);
                LogColumn yTargetColumn = data.GetColumn(this.desiredAfrParameterName);
                yValue = ySensorColumn.ValueAsDouble / yTargetColumn.ValueAsDouble;
            }
            catch (InvalidOperationException)
            {
                supported = false;
            }
        }

    }
}
