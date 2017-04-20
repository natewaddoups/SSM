using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NateW.Ssm.ApplicationLogic
{
    public class BoostScreen : HeatMapScreen
    {
        private static bool supported;

        public BoostScreen(double maxRpm)
            : base(
            "Boost", 
            Configuration.GetStringValue("ParameterRpm"), // X
            Configuration.GetStringValue("ParameterThrottle"), // Y
            ///"E90,psi relative sea level", // Target boost
            Configuration.GetStringValue("ParameterMrp"), // Manifold relative pressure
            (float) maxRpm, // X scale
            100, // Y max
            0, // Y min
            false) // always
        {
            supported = true;
            this.columns = 12;
        }

        protected override bool GetDataPoint(
            LogRow data,
            out float xValue,
            out float yValue,
            out float zValue,
            out string zName)
        {
            xValue = 0;
            yValue = 0;
            zValue = 0;
            zName = "Not supported";

            if (!supported)
            {
                return false;
            }

            try
            {
                LogColumn xColumn = data.GetColumn("P8");
                LogColumn yColumn = data.GetColumn("P13");
                LogColumn zColumn = data.GetColumn("P25");
                xValue = (float) xColumn.ValueAsDouble;
                yValue = (float) yColumn.ValueAsDouble;
                zValue = (float) zColumn.ValueAsDouble;
                zName = zColumn.Conversion.Units;
            }
            catch (InvalidOperationException)
            {
                supported = false;
                return false;
            }
            return true;
        }

    }
}
