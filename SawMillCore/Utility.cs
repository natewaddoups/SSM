using System;
using System.Collections.Generic;
using System.Text;

namespace NateW.Ssm.ApplicationLogic
{
    internal static class Utility
    {
        public static string GetParameterName(string parameterAndUnits)
        {
            string[] parts = parameterAndUnits.Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException(
                    string.Format("The value \"{0}\" should be of the form \"X12,units\""));
            }
            return parts[0];
        }

        public static double Interpolate(double a, double b, double mix)
        {
            double result = a + (mix * (b - a));
            return result;
        }
    }
}
