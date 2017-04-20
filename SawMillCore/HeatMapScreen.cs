using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace NateW.Ssm.ApplicationLogic
{
    public class HeatMapScreen : SawMillScreen
    {
        protected class Cell
        {
            public static readonly Cell Null = new Cell();
            public float Maximum;
            public float Average;
            public float Minimum;
            public long Hits;

            public void Reset()
            {
                this.Maximum = 0;
                this.Average = 0;
                this.Minimum = 0;
                this.Hits = 0;
            }
        }

        protected enum DisplayValue
        {
            Average,
            Maximum,
            Minimum,
            Hits,
            Undefined
        }

        // Was black
        private static Color backgroundColor = Configuration.GetColor(Colors.CommonBackground);

        // Was black
        private static Brush backgroundBrush = new SolidBrush(backgroundColor);

        // Was dark blue
        private static Color axisHighlightColor = Configuration.GetColor(Colors.CommonAxisHighlight);

        // Was dark blue
        private static Brush axisHighlightBrush = new SolidBrush(axisHighlightColor);

        // Was white
        private static Brush axisTextBrush = new SolidBrush(Configuration.GetColor(Colors.CommonAxisText));

        // Was white
        private static Color foregroundColor = Configuration.GetColor(Colors.HeatMapForeground);

        // Was red
        private static Color highlightColor = Configuration.GetColor(Colors.HeatMapHighlight);

        // Was red
        private static Brush highlightBrush = new SolidBrush(highlightColor);

        // Was white
        private static Brush cellTextBrush = new SolidBrush(Configuration.GetColor(Colors.HeatMapCellText));

        // Was yellow
        private static Brush overlayBrush = new SolidBrush(Configuration.GetColor(Colors.HeatMapOverlayText));

        protected int columns = 15;
        protected int rows = 15;
        protected Cell[][] dataPoints;
        protected float xMax;
        protected float yMin;
        protected float yMax;
        protected double maxCellValue = double.MinValue;
        protected double minCellValue = double.MaxValue;
        protected int lastX = -1;
        protected int lastY = -1;
        protected string lastTrace;
        protected string xParameterName;
        protected string yParameterName;
        protected string zParameterName;
        protected float cellFontScale = 0.5f;
        protected DisplayValue displayValue = DisplayValue.Undefined;

        public HeatMapScreen(
            string name, 
            string xParameter,
            string yParameter,
            string zParameter,
            //string switchParameter,
            float xScale,
            float yMax,
            float yMin,
            bool foregroundOnly)
            : base(name, foregroundOnly, new string[] { xParameter, yParameter, zParameter})//switchParameter })
        {
            dataPoints = new Cell[columns][];
            for (int i = 0; i < columns; i++)
            {
                dataPoints[i] = new Cell[rows];
                for (int j = 0; j < rows; j++)
                {
                    dataPoints[i][j] = new Cell();
                }
            }

            this.xMax = xScale;
            this.yMin = yMin;
            this.yMax = yMax;

            List<string> errorMessages = new List<string>();
            try
            {
                this.xParameterName = Utility.GetParameterName(xParameter);
            }
            catch (ArgumentException ex)
            {
                errorMessages.Add(string.Format(this.GetType().Name + " X parameter: " + ex.Message));
            }

            try
            {
                this.yParameterName = Utility.GetParameterName(yParameter);
            }
            catch (ArgumentException ex)
            {
                errorMessages.Add(string.Format(this.GetType().Name + " Y parameter: " + ex.Message));
            }

            try
            {
                this.zParameterName = Utility.GetParameterName(zParameter);
            }
            catch (ArgumentException ex)
            {
                errorMessages.Add(string.Format(this.GetType().Name + " Z parameter: " + ex.Message));
            }

            if (errorMessages.Count != 0)
            {
                string combined = string.Join(Environment.NewLine, errorMessages.ToArray());
                MessageBox.Show(combined, "Check configuration.");
                throw new Exception(string.Join("; ", errorMessages.ToArray()));
            }
        }

        public override void AddData(LogRow data)
        {
            float xValue;
            float yValue;
            float zValue;
            string zName;
            if (this.GetDataPoint(data, out xValue, out yValue, out zValue, out zName))
            {
                this.AddDataPoint(xValue, yValue, zValue, zName);
            }
        }

        public override void SingleClick()
        {
            int temp = (int)this.displayValue;
            this.displayValue = (DisplayValue)temp + 1;
            if (this.displayValue >= DisplayValue.Undefined)
            {
                this.displayValue = DisplayValue.Average;
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
                        Cell cell = this.dataPoints[x][y];
                        cell.Reset();
                    }
                }
            }
        }

        public override void Paint(Graphics graphics, float width, float height)
        {
            this.DrawBackground(graphics, width, height);
            this.DrawDataPoints(graphics, width, height);
        }
        
        protected virtual bool GetDataPoint(
            LogRow data,
            out float xValue,
            out float yValue,
            out float zValue,
            out string zName)
        {
            xValue = (float) data.Columns[0].ValueAsDouble;
            yValue = (float) data.Columns[1].ValueAsDouble;
            zValue = (float) data.Columns[2].ValueAsDouble;
            zName = data.Columns[2].Conversion.Units;
            return true;
        }

        protected bool TryGetCell(float xValue, float yValue, out int xIndex, out int yIndex, out Cell cell)
        {
            xIndex = (int)((xValue * columns) / this.xMax);
            yIndex = (int)(((yValue - yMin) * rows) / (this.yMax - yMin));

            // 100% throttle went off the edge of the boost table...
            if (xValue == this.xMax) xIndex = columns - 1;
            if (yValue == this.yMax) yIndex = rows - 1;
            if (yValue == this.yMin) yIndex = 0;

            if ((xIndex < 0) || (xIndex >= columns) ||
                (yIndex < 0) || (yIndex >= rows))
            {
                this.lastX = -1;
                this.lastY = -1;
                cell = Cell.Null;
                return false;
            }

            this.lastX = xIndex;
            this.lastY = yIndex;

            cell = this.dataPoints[xIndex][yIndex];
            return true;
        }

        private void AddDataPoint(float xValue, float yValue, float zValue, string zName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(zName);
            builder.Append(": ");
            builder.Append(zValue.ToString("0.00", CultureInfo.InvariantCulture));
            this.lastTrace = builder.ToString();

            int xIndex;
            int yIndex;
            Cell current;
            if (!this.TryGetCell(xValue, yValue, out xIndex, out yIndex, out current))
            {
                return;
            }
            
            if (current.Hits == 0)
            {
                current.Average = zValue;
                current.Minimum = zValue;
                current.Maximum = zValue;
            }
            else
            {
                float difference = zValue - current.Average;
                difference /= 10;
                current.Average += difference;
            }

            current.Maximum = Math.Max(current.Maximum, zValue);
            current.Minimum = Math.Min(current.Minimum, zValue);
            current.Hits++;
        }

        private void DrawBackground(Graphics graphics, float width, float height)
        {
            graphics.FillRectangle(backgroundBrush, 0, 0, width, height);
        }

        protected virtual string GetCellText(Cell cell)
        {
            switch (this.displayValue)
            {
                case DisplayValue.Maximum:
                    return cell.Maximum.ToString("0.00");

                case DisplayValue.Minimum:
                    return cell.Minimum.ToString("0.00");

                case DisplayValue.Hits:
                    return Math.Log10(cell.Hits).ToString("0.00");

                case DisplayValue.Average:
                default:
                    return cell.Average.ToString("0.00");
            }
        }

        private void DrawDataPoints(Graphics graphics, float width, float height)
        {
            double cellWidth = width / (columns + 1);
            double cellHeight = height / (rows + 1);

            using (Font largeFont = new Font(
                FontFamily.GenericSansSerif,
                (float)cellHeight * this.cellFontScale,
                FontStyle.Regular,
                GraphicsUnit.Pixel))
            using (Font smallFont = new Font(
                FontFamily.GenericSansSerif,
                (float)cellHeight * 0.4f,
                FontStyle.Regular,
                GraphicsUnit.Pixel))
            {
                for (int column = 0; column < columns + 1; column++)
                {
                    for (int row = 0; row < rows + 1; row++)
                    {
                        double x = column * cellWidth;
                        double y = height - ((row + 1d) * cellHeight);

                        int xIndex = column - 1;
                        int yIndex = row - 1;

                        RectangleF rectangle = new RectangleF(
                            (float)x,
                            (float)y,
                            (float)cellWidth,
                            (float)cellHeight);

                        //if ((column == 0) && (row == 0))
                        //{
                        //}
                        //else 
                        if (column == 0)
                        {
                            float yValue = SawMillUtility.GetHeader(row, rows, yMin, yMax);
                            StringFormat format = new StringFormat();
                            format.Alignment = StringAlignment.Far;
                            format.LineAlignment = StringAlignment.Center;
                            graphics.DrawString(yValue.ToString("0.00"), smallFont, axisTextBrush, rectangle, format);
                        }
                        else if (row == 0)
                        {
                            float xValue = SawMillUtility.GetHeader(column, columns, 0f, xMax);
                            StringFormat format = new StringFormat();
                            format.Alignment = StringAlignment.Center;
                            format.LineAlignment = StringAlignment.Center;

                            if (column % 2 == 1)
                            {
                                graphics.FillRectangle(axisHighlightBrush, rectangle);
                            }

                            graphics.DrawString(xValue.ToString("0"), smallFont, axisTextBrush, rectangle, format);
                        }
                        else
                        {
                            Cell thisCell = this.dataPoints[xIndex][yIndex];
                            double thisCellValue = thisCell.Average;

                            this.maxCellValue = Math.Max(thisCellValue, this.maxCellValue);
                            this.minCellValue = Math.Min(thisCellValue, this.minCellValue);
                            double range = this.maxCellValue - this.minCellValue;
                            double ratio = range == 0 ?
                                0 :
                                (thisCellValue - this.minCellValue) / range;

                            Brush brush;

                            if (thisCell.Hits == 0)
                            {
                                brush = backgroundBrush;
                            }
                            else if ((xIndex == this.lastX) && (yIndex == this.lastY))
                            {
                                brush = highlightBrush;
                            }
                            else
                            {
                                brush = new SolidBrush(Color.FromArgb(
                                    (int)Utility.Interpolate(backgroundColor.R, foregroundColor.R, ratio),
                                    (int)Utility.Interpolate(backgroundColor.G, foregroundColor.G, ratio),
                                    (int)Utility.Interpolate(backgroundColor.B, foregroundColor.B, ratio)));
                            }

                            graphics.FillRectangle(
                                brush,
                                rectangle);

                            if (thisCell.Hits > 0)
                            {
                                StringFormat format = new StringFormat();
                                format.Alignment = StringAlignment.Far;
                                format.LineAlignment = StringAlignment.Center;
                                string cellText = this.GetCellText(thisCell);
                                graphics.DrawString(cellText, largeFont, cellTextBrush, rectangle, format);
                            }

                            if ((brush != highlightBrush) && (brush != backgroundBrush))
                            {
                                brush.Dispose();
                            }
                        }
                    }
                }
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
                switch (this.displayValue)
                {
                    case DisplayValue.Undefined:
                        break;
                    case DisplayValue.Average:
                        this.lastTrace += Environment.NewLine + "Average";
                        break;
                    case DisplayValue.Maximum:
                        this.lastTrace += Environment.NewLine + "Maximum";
                        break;
                    case DisplayValue.Minimum:
                        this.lastTrace += Environment.NewLine + "Minimum";
                        break;
                    case DisplayValue.Hits:
                        this.lastTrace += Environment.NewLine + "Hits";
                        break;
                }

                graphics.DrawString(this.lastTrace, font, overlayBrush, width / 10, 15);
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

        private string tableHeader = "HeatMapVersion: 1, Rows: {0}, Columns: {1}, xMin: {2}, xMax: {3}, yMin: {4}, yMax: {5}";

        private delegate float GetValue(int x, int y);

        private float[][] CreateTable(GetValue getValue)
        {
            float[][] table = new float[columns][];
            for (int i = 0; i < columns; i++)
            {
                table[i] = new float[rows];
            }

            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    table[column][row] = getValue(column, row);
                }
            }

            return table;
        }

        /// <summary>
        /// Copy the screen data to the clipboard
        /// </summary>
        public override string GetClipboardText()
        {
            StringBuilder builder = new StringBuilder(1000);
            for (int row = rows; row >= 0; row--)
            {
                for (int column = 0; column < columns + 1; column++)
                {
                    if (column == 0)
                    {
                        float yValue = SawMillUtility.GetHeader(row, rows, yMin, yMax);
                        builder.Append(yValue.ToString("0.00"));
                        
                    }
                    else if (row == 0)
                    {
                        float xValue = SawMillUtility.GetHeader(column, columns, 0f, xMax);
                        builder.Append(xValue.ToString("0"));
                    }
                    else
                    {
                        int xIndex = column - 1;
                        int yIndex = row - 1;
                        Cell cell = this.dataPoints[xIndex][yIndex];
                        string cellText = this.GetCellText(cell);
                        builder.Append(cellText);
                    }

                    if (column == columns)
                    {
                        builder.AppendLine();
                    }
                    else
                    {
                        builder.Append('\t');
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Write data to text stream
        /// </summary>
        public override void WriteTo(StreamWriter writer)
        {
            string header = string.Format(tableHeader, rows, columns, 0f, this.xMax, this.yMin, this.yMax);
            writer.WriteLine(header);

            float[][] averages = CreateTable(delegate (int x, int y) { return this.dataPoints[x][y].Average; });
            float[][] maximums = CreateTable(delegate(int x, int y) { return this.dataPoints[x][y].Maximum; });
            float[][] minimums = CreateTable(delegate(int x, int y) { return this.dataPoints[x][y].Minimum; });
            float[][] hits = CreateTable(delegate(int x, int y) { return this.dataPoints[x][y].Hits; });
            
            FloatTableSerializer serializer = FloatTableSerializer.GetInstance(writer);
            serializer.Write(averages, GetRowHeader, GetColumnHeader);
            serializer.Write(maximums, GetRowHeader, GetColumnHeader);
            serializer.Write(minimums, GetRowHeader, GetColumnHeader);
            serializer.Write(hits, GetRowHeader, GetColumnHeader);
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
                float[][] averages = CreateTable(delegate(int x, int y) { return 0; });
                float[][] maximums = CreateTable(delegate(int x, int y) { return 0; });
                float[][] minimums = CreateTable(delegate(int x, int y) { return 0; });
                float[][] hits = CreateTable(delegate(int x, int y) { return 0; });

                FloatTableSerializer serializer = FloatTableSerializer.GetInstance(reader);
                serializer.Read(averages);
                serializer.Read(maximums);
                serializer.Read(minimums);
                serializer.Read(hits);

                for (int column = 0; column < columns; column++)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        this.dataPoints[column][row].Average = averages[column][row];
                        this.dataPoints[column][row].Maximum = maximums[column][row];
                        this.dataPoints[column][row].Minimum = minimums[column][row];
                        this.dataPoints[column][row].Hits = (long) hits[column][row];
                    }
                }
            }
        }
    }
}
