using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NateW.Ssm.ApplicationLogic
{
    /// <summary>
    /// Inverse volumetric efficiency
    /// </summary>
    /// <remarks>
    /// How much boost does it take to reach a given load?
    /// </remarks>
    public class IVEScreen : HeatMapScreen
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public IVEScreen(
            double maxRpm, 
            double maxLoad)
            : base(
            "IVE", 
            Configuration.GetStringValue("ParameterRpm"), 
            Configuration.GetStringValue("ParameterLoad"), 
            Configuration.GetStringValue("ParameterMap"), 
            (float) maxRpm, 
            (float) maxLoad,
            0f,
            false)
        {
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
