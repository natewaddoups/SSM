using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NateW.Ssm;

namespace NateW.Ssm.ApplicationLogic
{
    /// <summary>
    /// Renders log entries
    /// </summary>
    internal class SixPackPainter
    {
        private const int rows = 2;
        private const int columns = 3;

        /// <summary>
        /// Cached drawing objects
        /// </summary>
        private static Brush backgroundBrush = new SolidBrush(Configuration.GetColor(Colors.CommonBackground));
        private static Brush majorTextBrush = new SolidBrush(Configuration.GetColor(Colors.SixPackMajorText));
        private static Brush minorTextBrush = new SolidBrush(Configuration.GetColor(Colors.SixPackMinorText));
        private static Pen horizontalLinePen = new Pen(Configuration.GetColor(Colors.SixPackHorizontalLine));
        private static Pen verticalLinePen = new Pen(Configuration.GetColor(Colors.SixPackVerticalLines));

        private static Bitmap bitmap;
        private static Graphics graphics;

        private Graphics windowGraphics;
        private LogRow row;
        private int cellWidth;
        private int cellHeight;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <remarks>
        /// Will re-create cached Bitmap/Graphics if the window size has
        /// changed since the last iteration.
        /// </remarks>
        public SixPackPainter(
            Graphics windowGraphics, 
            int width, 
            int height, 
            LogRow row)
        {
            if ((bitmap == null) || (bitmap.Width != width) || (bitmap.Height != height))
            {
                this.CreateOffscreenGraphics(width, height);
            }

            this.cellWidth = width / columns;
            this.cellHeight = height / rows;
            this.windowGraphics = windowGraphics;
            this.row = row;
        }

        /// <summary>
        /// Render the log entry
        /// </summary>
        public void Paint()
        {
            this.DrawBackground();
            this.DrawCells();
            this.CopyToWindow();
        }

        /// <summary>
        /// Create resources for off-screen rendering
        /// </summary>
        private void CreateOffscreenGraphics(int width, int height)
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
            }

            bitmap = new Bitmap(width, height);

            if (graphics != null)
            {
                graphics.Dispose();
            }

            graphics = Graphics.FromImage(bitmap);
        }

        /// <summary>
        /// Erase the background and draw lines between the cells
        /// </summary>
        private void DrawBackground()
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            graphics.FillRectangle(backgroundBrush, 0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawLine(horizontalLinePen, 0, height / 2, width, height / 2);
            graphics.DrawLine(verticalLinePen, width / 3, 0, width / 3, height);
            graphics.DrawLine(verticalLinePen, (2 * width) / 3, 0, (2 * width) / 3, height);
        }

        /// <summary>
        /// Draw the cells
        /// </summary>
        private void DrawCells()
        {
            int firstColumnIndex = 0;
            this.DrawCell(this.row.Columns[firstColumnIndex + 0], 0, 0);
            this.DrawCell(this.row.Columns[firstColumnIndex + 1], 1, 0);
            this.DrawCell(this.row.Columns[firstColumnIndex + 2], 2, 0);

            this.DrawCell(this.row.Columns[firstColumnIndex + 3], 0, 1);
            this.DrawCell(this.row.Columns[firstColumnIndex + 4], 1, 1);
            this.DrawCell(this.row.Columns[firstColumnIndex + 5], 2, 1);
        }

        /// <summary>
        /// Copy the bitmap from off-screen to the window
        /// </summary>
        private void CopyToWindow()
        {
            this.windowGraphics.DrawImage(SixPackPainter.bitmap, 0, 0);
        }

        /// <summary>
        /// Draw a single cell
        /// </summary>
        private void DrawCell(LogColumn data, int column, int row)
        {
            int left = this.cellWidth * column;
            int right = left + this.cellWidth;
            int top = this.cellHeight * row;
            int bottom = top + this.cellHeight;
            RectangleF cell = new RectangleF(left, top, this.cellWidth, this.cellHeight);

            this.DrawTextAtTop(
                data.Parameter.Name,
                cell,
                minorTextBrush);

            this.DrawTextCentered(
                data.ValueAsString,
                cell,
                majorTextBrush);

            this.DrawTextAtBottom(
                data.Conversion.Units,
                cell,
                minorTextBrush);
        }

        /// <summary>
        /// Draw the text at the top of a cell
        /// </summary>
        private void DrawTextAtTop(
            string text,
            RectangleF cell,
            Brush brush)
        {
            RectangleF rectangle = new RectangleF(
                cell.Left, 
                cell.Top, 
                this.cellWidth, 
                this.cellHeight / 4);
            this.DrawText(text, brush, rectangle, 0.25f);
        }

        /// <summary>
        /// Draw the text in the center of a cell
        /// </summary>
        private void DrawTextCentered(
            string text,
            RectangleF cell,
            Brush brush)
        {
            RectangleF rectangle = new RectangleF(
                cell.Left,
                cell.Top + (this.cellHeight * 0.325f), 
                cell.Width, 
                this.cellHeight / 2);

            this.DrawText(text, brush, rectangle, 0.5f);
        }

        /// <summary>
        /// Draw the text at the bottom of a cell
        /// </summary>
        private void DrawTextAtBottom(
            string text,
            RectangleF cell,
            Brush brush)
        {
            RectangleF rectangle = new RectangleF(
                cell.Left,
                cell.Top + (this.cellHeight * 0.75f),//0.875f),
                cell.Width,
                this.cellHeight / 4);
            this.DrawText(text, brush, rectangle, 0.25f);
        }

        /// <summary>
        /// Draw text in the given rectangle
        /// </summary>
        private void DrawText(
            string text, 
            Brush brush,
            RectangleF rectangle, 
            float scale)
        {
            double height = rectangle.Height * scale;

            using (Font font = new Font(
                FontFamily.GenericSansSerif,
                (float)height,
                GraphicsUnit.Pixel))
            {
                StringFormat format = StringFormat.GenericDefault;
                format.Alignment = StringAlignment.Center;
                format.FormatFlags = StringFormatFlags.NoClip;

                graphics.DrawString(text, font, brush, rectangle, format);
            }
        }
    }
}