using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NateW.Ssm.ApplicationLogic
{
    public enum Colors
    {
        NormalButtonTop,
        SelectedButtonTop,
        NormalButtonBottom,
        SelectedButtonBottom,
        NormalButtonText,
        SelectedButtonText,
        LinesBetweenButtons,
        CommonBackground,
        CommonAxisLines,
        CommonAxisText,
        CommonAxisHighlight,
        SixPackMajorText,
        SixPackMinorText,
        SixPackHorizontalLine,
        SixPackVerticalLines,
        ScatterPlotForeground,
        ScatterPlotHighlightInner,
        ScatterPlotHighlightOuter,
        ScatterPlotText,
        HeatMapForeground,
        HeatMapHighlight,
        HeatMapCellText,
        HeatMapOverlayText,
    }

    public delegate string ValueGetter (string id);

    public class Configuration
    {
        private static Configuration instance;
        private static object creationLock = new object();

        private Dictionary<Colors, Color> colors;
        
        public Dictionary<Colors, Color> Colors { get { return this.colors; } }

        public ValueGetter GetValue;

        private Configuration()
        {
            this.colors = new Dictionary<Colors, Color>();
        }

        public static Configuration GetInstance()
        {
            if (instance == null)
            {
                lock (creationLock)
                {
                    if (instance == null)
                    {
                        instance = new Configuration();
                    }
                }
            }
            return instance;
        }

        public static string GetStringValue(string valueName)
        {
            Configuration configuration = Configuration.GetInstance();
            return configuration.GetValue(valueName);
        }

        public bool Validate()
        {
            bool result = true;
            result &= this.ValidateColors();
            return result;
        }

        public bool ValidateColors()
        {
            foreach (Colors colorId in Enum.GetValues(typeof(Colors)))
            {
                string name = "Color" + colorId.ToString();
                string colorString = GetValue(name);

                if (colorString == null)
                {
                    string message = "Configuration file does not contain " + name;
                    MessageBox.Show(message);
                    return false;
                }

                try
                {
                    Color color = System.Drawing.ColorTranslator.FromHtml(colorString);
                    this.colors[colorId] = color;
                }
                catch (Exception ex)
                {
                    string message = "Unable to parse " + name + " in configuration file: " +
                        Environment.NewLine + ex.Message;
                    MessageBox.Show(message);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Shorthand
        /// </summary>
        public static Color GetColor(Colors colorId)
        {
            return Configuration.GetInstance().Colors[colorId];
        }
    }
}
