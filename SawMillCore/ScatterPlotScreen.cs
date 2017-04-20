using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace NateW.Ssm.ApplicationLogic
{
    public class ScatterPlotScreen : SawMillScreen
    {
        // Was black
        private static Color backgroundColor = Configuration.GetColor(Colors.CommonBackground);

        // Was black
        private static Brush backgroundBrush = new SolidBrush(backgroundColor);

        // Was white
        private static Color foregroundColor = Configuration.GetColor(Colors.ScatterPlotForeground);

        // Was white
        private static Pen highlightInner = new Pen(Configuration.GetColor(Colors.ScatterPlotHighlightInner));

        // Was black
        private static Pen highlightOuter = new Pen(Configuration.GetColor(Colors.ScatterPlotHighlightOuter));

        // Was yellow
        private static Brush textBrush = new SolidBrush(Configuration.GetColor(Colors.ScatterPlotText));

        protected string xParameterName;
        protected string yParameterName;

        const int rows = 100;
        const int columns = 100;
        float[][] dataPoints;
        float xMax;
        float yMin;
        float yMax;
        float xDivision;
        float yDivision;
        string lastTrace;
        int lastX = -1;
        int lastY = -1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of this screen (for debugging only)</param>
        /// <param name="xMax">Maximum value on the X axis</param>
        /// <param name="xDivision">Where to put tick marks on the X axis</param>
        /// <param name="yMax">Maximum value on the Y axis</param>
        /// <param name="yDivision">Where to put tick marks on the Y axis</param>
        /// <param name="parameters">Parameters required by this screen</param>
        public ScatterPlotScreen(
            string name, 
            float xMax, 
            float xDivision,
            float yMin,
            float yMax,
            float yDivision,
            bool foregroundOnly,
            params string[] parameters)
            : base(name, foregroundOnly, parameters)
        {
            dataPoints = new float[columns][];
            for (int i = 0; i < columns; i++)
            {
                dataPoints[i] = new float[rows];
            }

            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
            this.xDivision = xDivision;
            this.yDivision = yDivision;

            List<string> errorMessages = new List<string>();
            try
            {
                this.xParameterName = Utility.GetParameterName(parameters[0]);
            }
            catch (ArgumentException ex)
            {
                errorMessages.Add(string.Format(this.GetType().Name + " X parameter: " + ex.Message));
            }

            try
            {
                this.yParameterName = Utility.GetParameterName(parameters[1]);
            }
            catch (ArgumentException ex)
            {
                errorMessages.Add(string.Format(this.GetType().Name + " Y parameter: " + ex.Message));
            }

            if (errorMessages.Count != 0)
            {
                string combined = string.Join(Environment.NewLine, errorMessages.ToArray());
                MessageBox.Show(combined, "Check configuration.");
                throw new Exception(string.Join("; ", errorMessages.ToArray()));
            }
        }

        /// <summary>
        /// Add data to the plot
        /// </summary>
        public override void AddData(LogRow data)
        {
            double xValue;
            double yValue;
            string xName;
            string yName;
            this.GetDataPoint(data, out xValue, out yValue, out xName, out yName);
            this.AddDataPoint(xValue, yValue, xName, yName);
        }

        /// <summary>
        /// Paint the screen
        /// </summary>
        public override void Paint(Graphics graphics, float width, float height)
        {
                       
            // TODO: cache/reuse bitmap
            using (Bitmap bitmap = new Bitmap((int)width, (int)height))
            using (Graphics innerGraphics = Graphics.FromImage(bitmap))
            {
                this.DrawBackground(graphics, width, height);
                GraphPainter graphPainter = new GraphPainter(graphics, width, height, this.xMax, this.yMin, this.yMax, this.xDivision, this.yDivision);
                this.DrawDataPoints(innerGraphics, graphPainter.ContentWidth, graphPainter.ContentHeight);
                graphPainter.Paint(bitmap);
            }
        }

        public override void DoubleClick()
        {
            if (MessageBox.Show("Reset this screen?", this.Name, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int x = 0; x < this.dataPoints.Length; x++)
                {
                    for (int y = 0; y < this.dataPoints[x].Length; y++)
                    {
                        this.dataPoints[x][y] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Get a data point to add to the plot
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xValue"></param>
        /// <param name="yValue"></param>
        /// <param name="xName"></param>
        /// <param name="yName"></param>
        protected virtual void GetDataPoint(
            LogRow data, 
            out double xValue, 
            out double yValue,
            out string xName,
            out string yName)
        {
            xValue = data.Columns[0].ValueAsDouble;
            yValue = data.Columns[1].ValueAsDouble;
            xName = data.Columns[0].Conversion.Units;
            yName = data.Columns[1].Conversion.Units;
        }

        /// <summary>
        /// Add a data point to the plot
        /// </summary>
        private void AddDataPoint(double xValue, double yValue, string xName, string yName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(xName);
            builder.Append(": ");
            builder.Append(xValue.ToString("0.00"));
            builder.AppendLine();
            builder.Append(yName);
            builder.Append(": ");
            builder.Append(yValue.ToString("0.00"));
            this.lastTrace = builder.ToString();

            int xIndex = (int)((xValue * columns) / this.xMax);
            int yIndex = (int)(((yValue - yMin) * rows) / (this.yMax - yMin));

            if ((xIndex < 0) || (xIndex >= columns) ||
                (yIndex < 0) || (yIndex >= rows))
            {
                this.lastX = -1;
                this.lastY = -1; 
                return;
            }

            this.lastX = xIndex;
            this.lastY = yIndex;

            float current = this.dataPoints[xIndex][yIndex];
            float difference = 1f - current;
            difference /= 10;
            current += difference;
            this.dataPoints[xIndex][yIndex] = current;
        }

        /// <summary>
        /// Draw the background
        /// </summary>
        private void DrawBackground(Graphics graphics, float width, float height)
        {
            graphics.FillRectangle(backgroundBrush, 0, 0, width, height);
        }

        /// <summary>
        /// Get X and Y positions for a given row and column
        /// </summary>
        private void GetXY(
            int column, 
            int row, 
            float height,
            float cellWidth, 
            float cellHeight, 
            out float x, 
            out float y)
        {
            x = column * cellWidth;
            y = height - ((row + 1) * cellHeight);
        }

        /// <summary>
        /// Draw all data points
        /// </summary>
        private void DrawDataPoints(Graphics graphics, float width, float height)
        {
            float cellWidth = width / columns;
            float cellHeight = height / rows;

            float x;
            float y;

            for (int xIndex = 0; xIndex < columns; xIndex++)
            {
                for (int yIndex = 0; yIndex < rows; yIndex++)
                {
                    this.GetXY(xIndex, yIndex, height, cellWidth, cellHeight, out x, out y);
                    float thisCell = this.dataPoints[xIndex][yIndex];
                    
                    // TODO: cache brushes
                    Brush brush = new SolidBrush(Color.FromArgb(
                        (int)Utility.Interpolate(backgroundColor.R, foregroundColor.R, thisCell),
                        (int)Utility.Interpolate(backgroundColor.G, foregroundColor.G, thisCell),
                        (int)Utility.Interpolate(backgroundColor.B, foregroundColor.B, thisCell)));

                    graphics.FillRectangle(
                        brush,
                        (float)x,
                        (float)y,
                        (float)cellWidth,
                        (float)cellHeight);

                    brush.Dispose();
                }
            }

            if ((this.lastX != -1) && (this.lastY != -1))
            {
                this.GetXY(this.lastX, this.lastY, height, cellWidth, cellHeight, out x, out y);
                graphics.DrawEllipse(
                    highlightInner,
                    x - cellWidth,
                    y - cellHeight,
                    3 * cellWidth,
                    3 * cellHeight);
                graphics.DrawEllipse(
                    highlightOuter,
                    (x - cellWidth) - 3,
                    (y - cellHeight) - 3,
                    (3 * cellWidth) + 6,
                    (3 * cellHeight) + 6);
            }

            if (this.lastTrace == null)
            {
                return;
            }

            using (Font font = new Font(
                FontFamily.GenericSansSerif,
                (float)30,
                GraphicsUnit.Pixel))
            {
                graphics.DrawString(this.lastTrace, font, textBrush, width / 20, 0);
            }
        }

        private float GetRowHeader(int y)
        {
            return SawMillUtility.GetHeader(y, rows, this.yMin, this.yMax);
        }

        private float GetColumnHeader(int x)
        {
            return SawMillUtility.GetHeader(x, columns, 0f, this.xMax);
        }

        private string tableHeader = "ScatterPlotVersion: 1, Rows: {0}, Columns: {1}, xMin: {2}, xMax: {3}, yMin: {4}, yMax: {5}";

        /// <summary>
        /// Write data to text stream
        /// </summary>
        public override void WriteTo(StreamWriter writer)
        {
            string header = string.Format(tableHeader, rows, columns, 0f, this.xMax, this.yMin, this.yMax);
            writer.WriteLine(header);

            FloatTableSerializer serializer = FloatTableSerializer.GetInstance(writer);
            serializer.Write(this.dataPoints, GetRowHeader, GetColumnHeader);
        }

        /// <summary>
        /// Read data from text stream
        /// </summary>
        public override void ReadFrom(StreamReader reader)
        {
            string expectedHeader = string.Format(tableHeader, rows, columns, 0f, this.xMax, this.yMin, this.yMax);
            string actualHeader = reader.ReadLine();

            if (expectedHeader == actualHeader)
            {
                FloatTableSerializer serializer = FloatTableSerializer.GetInstance(reader);
                serializer.Read(this.dataPoints);
            }
        }
    }
}
