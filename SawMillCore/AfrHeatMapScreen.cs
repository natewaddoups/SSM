using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NateW.Ssm.ApplicationLogic
{
    public class AfrHeatMapScreen : HeatMapScreen
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AfrHeatMapScreen(
            double maxRpm, 
            double maxLoadGs)
            : base(
            "AFR", 
            Configuration.GetStringValue("ParameterRpm"), 
            Configuration.GetStringValue("ParameterLoad"),
            Configuration.GetStringValue("ParameterActualAfr"),
            (float) maxRpm, 
            (float) maxLoadGs,
            0f,
            false)
        {
            this.cellFontScale = 0.4f;
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
            LogColumn x = data.GetColumn(this.xParameterName);
            LogColumn y = data.GetColumn(this.yParameterName);
            LogColumn z = data.GetColumn(this.zParameterName);

            xValue = (float) x.ValueAsDouble;
            yValue = (float) y.ValueAsDouble;
            zValue = (float) z.ValueAsDouble;

            zName = z.Conversion.Units;
            return true;
        }
    }
}
