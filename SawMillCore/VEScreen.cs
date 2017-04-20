using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NateW.Ssm.ApplicationLogic
{
    public class VEScreen : HeatMapScreen
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public VEScreen(
            double maxRpm, 
            double maxAbsoluteBoostBar)
            : base(
            "VE", 
            Configuration.GetStringValue("ParameterRpm"), 
            Configuration.GetStringValue("ParameterMap"),
            Configuration.GetStringValue("ParameterLoad"),
            (float) maxRpm, 
            (float) maxAbsoluteBoostBar,
            0f,
            false)
        {
        }

        /// <summary>
        /// Get a data point for the heat map
        /// </summary>
        /// <remarks>
        /// 1.20 grams/liter at 70F/20C
        /// 3.0 g/2rev at 100% VE
        /// 1.5 g/rev at 100% VE
        /// </remarks>
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
