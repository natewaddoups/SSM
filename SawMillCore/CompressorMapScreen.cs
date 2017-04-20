using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NateW.Ssm.ApplicationLogic
{
    public class CompressorMapScreen : ScatterPlotScreen
    {
        const double gsToLbMin = 1d / 453.6d * 60d;
        public CompressorMapScreen(
            double maxMaf, 
            double maxBoostBar)
            : base(
            "Compressor Map", 
            (float) (maxMaf * gsToLbMin), 
            5f,
            1f,
            (float)maxBoostBar,
            0.25f,
            false, // foreground only
            Configuration.GetStringValue("ParameterMaf"),
            Configuration.GetStringValue("ParameterMap"))
        {
        }

        protected override void GetDataPoint(
            LogRow data, 
            out double xValue, 
            out double yValue,
            out string xName,
            out string yName)
        {
            LogColumn x = data.GetColumn(this.xParameterName);
            LogColumn y = data.GetColumn(this.yParameterName);
            xValue = x.ValueAsDouble / 453.6d * 60d; // g/s to lb/min
            yValue = y.ValueAsDouble; // TODO: calc from actual atmos pressure
            xName = "lb/min";
            yName = "PR";
        }
    }
}
