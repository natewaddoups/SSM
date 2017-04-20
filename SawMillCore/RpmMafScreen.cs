using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NateW.Ssm.ApplicationLogic
{
    public class RpmMafScreen : ScatterPlotScreen
    {
        public RpmMafScreen(double maxRpm, double maxMafGS)
            : base(
            "RPM / MAF", 
            (float)maxRpm, 
            1000, 
            0f, 
            (float)maxMafGS, 
            50f, 
            false,
            Configuration.GetStringValue("ParameterRpm"),
            Configuration.GetStringValue("ParameterMaf"))
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
            xValue = x.ValueAsDouble;
            yValue = y.ValueAsDouble;
            xName = x.Conversion.Units;
            yName = y.Conversion.Units;
        }
    }
}
