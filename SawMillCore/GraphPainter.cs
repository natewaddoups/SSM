using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NateW.Ssm;

namespace NateW.Ssm.ApplicationLogic
{
    /// <summary>
    /// Draws axes and tick marks for a graph
    /// </summary>
    internal class GraphPainter
    {
        // Was black
        private static readonly Color backgroundColor = Configuration.GetColor(Colors.CommonBackground);
        
        // Was black
        private static readonly Brush backgroundBrush = new SolidBrush(backgroundColor);

        // Was white
        private static readonly Color lineColor = Configuration.GetColor(Colors.CommonAxisLines);

        // Was white
        private static readonly Pen linePen = new Pen(lineColor);

        // Was white
        private static readonly Color axisTextColor = Configuration.GetColor(Colors.CommonAxisText);

        // Was white
        private static readonly Brush axisTextBrush = new SolidBrush(axisTextColor);


        private const float tickLength = 5;
        private Graphics graphics;
        private float width;
        private float height;
        float xMax;
        float yMin;
        float yMax;
        float xDivision;
        float yDivision;
        float marginLeft;
        float marginBottom;
        float marginTop;
        float marginRight;
        float contentWidth;
        float contentHeight;

        /// <summary>
        /// Width of the area available for graph content
        /// </summary>
        public float ContentWidth
        {
            get { return this.contentWidth; }
        }

        /// <summary>
        /// Height of the area available for graph content
        /// </summary>
        public float ContentHeight
        {
            get { return this.contentHeight; }
        }
        
        /// <summary>
        /// Contructor
        /// </summary>
        public GraphPainter(
            Graphics graphics, 
            float width, 
            float height, 
            float xMax,
            float yMin,
            float yMax,
            float xDivision,
            float yDivision)
        {
            this.graphics = graphics;
            this.width = width;
            this.height = height;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
            this.xDivision = xDivision;
            this.yDivision = yDivision;
            this.marginLeft = 50;
            this.marginTop = 15;
            this.marginRight = 15;
            this.marginBottom = 50;
            this.contentHeight = height - (this.marginTop + this.marginBottom);
            this.contentWidth = width - (this.marginLeft + this.marginRight);
        }

        /// <summary>
        /// Draw the axes and copy the given content into the graph
        /// </summary>
        /// <param name="content"></param>
        public void Paint(Image content)
        {
            this.DrawAxes();

            using (Font font = new Font(
                FontFamily.GenericSansSerif,
                (float)this.marginBottom / 4,
                GraphicsUnit.Pixel))
            {
                this.DrawXTicks(font);
                this.DrawYTicks(font);
                this.graphics.DrawImage(
                    content, 
                    new Rectangle(
                        (int) this.marginLeft + 1, 
                        (int) this.marginTop + 1,
                        (int) this.contentWidth - 1,
                        (int) this.contentHeight - 1),
                    0, 0, this.contentWidth, this.contentHeight, GraphicsUnit.Pixel);
            }
        }

        /// <summary>
        /// Draw X and Y axes
        /// </summary>
        private void DrawAxes()
        {
            this.graphics.DrawLine(
                linePen, 
                this.marginLeft, 
                this.marginTop, 
                this.marginLeft, 
                this.marginTop + this.contentHeight);
            
            this.graphics.DrawLine(
                linePen, 
                this.marginLeft, 
                this.marginTop + this.contentHeight, 
                this.marginLeft + this.contentWidth, 
                this.marginTop + this.contentHeight);
        }

        /// <summary>
        /// Draw ticks on the X axis
        /// </summary>
        private void DrawXTicks(Font font)
        {
            float ticks = this.xMax / this.xDivision;
            float tickSpacing = this.contentWidth / ticks;
            for (int tick = 0; tick <= (int) ticks; tick++)
            {
                float yTop = this.marginTop + this.contentHeight;
                float yBottom = yTop + tickLength;
                float x = this.marginLeft + (tick * tickSpacing);
                this.graphics.DrawLine(linePen, x, yTop, x, yBottom);
                
                float tickValue = tick * this.xDivision;
                string tickText;
                if (this.xDivision > 1)
                {
                    tickText = tickValue.ToString("0");
                }
                else
                {
                    tickText = tickValue.ToString("0.00");
                }
                this.graphics.DrawString(tickText, font, axisTextBrush, x, yTop + tickLength);
            }
        }

        /// <summary>
        /// Draw ticks on the Y axis
        /// </summary>
        /// <param name="font"></param>
        private void DrawYTicks(Font font)
        {
            float ticks = (this.yMax - this.yMin) / this.yDivision;
            float tickSpacing = this.contentHeight / ticks;
            for (int tick = 0; tick <= (int)ticks; tick++)
            {
                float xLeft = this.marginLeft - tickLength;
                float xRight = this.marginLeft;
                float y = this.marginTop + this.contentHeight - (tick * tickSpacing);
                this.graphics.DrawLine(linePen, xLeft, y, xRight, y);

                float tickValue = (tick * this.yDivision) + this.yMin;
                string tickText;
                if (this.yDivision > 1)
                {
                    tickText = tickValue.ToString("0");
                }
                else
                {
                    tickText = tickValue.ToString("0.00");
                }
                this.graphics.DrawString(tickText, font, axisTextBrush, 0, y - (font.Height / 2));
            }
        }
   }
}