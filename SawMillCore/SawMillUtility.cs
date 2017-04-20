using System;
using System.Collections.Generic;
using System.Text;

namespace NateW.Ssm.ApplicationLogic
{
    internal static class SawMillUtility
    {
        public static float GetHeader(int row, int rows, float min, float max)
        {
            return (((max - min) / rows) * row) + min;
        }
    }
}
