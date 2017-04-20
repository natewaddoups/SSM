using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NateW.Ssm.ApplicationLogic
{
    public class RpmLoadScreen : ScatterPlotScreen
    {
        public RpmLoadScreen(double maxRpm, double maxLoad)
            : base(
            "RPM / Load", 
            (float)maxRpm, 
            1000, 
            0f, 
            (float)maxLoad, 
            0.5f, 
            false, 
            Configuration.GetStringValue("ParameterRpm"),
            Configuration.GetStringValue("ParameterLoad"))
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
