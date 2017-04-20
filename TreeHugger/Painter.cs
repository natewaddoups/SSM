using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NateW.Ssm;

namespace NateW.Ssm.TreeHugger
{
    /// <summary>
    /// Renders log entries
    /// </summary>
    internal class Painter
    {
        private const int rows = 2;
        private const int columns = 3;

        private static Bitmap bitmap;
        private static Graphics graphics;

        private Graphics windowGraphics;
        private LogRow row;
        int cellWidth;
        int cellHeight;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <remarks>
        /// Will re-create cached Bitmap/Graphics if the window size has
        /// changed since the last iteration.
        /// </remarks>
        public Painter(
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

            graphics.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawLine(Pens.White, 0, height / 2, width, height / 2);
            graphics.DrawLine(Pens.Gray, width / 3, 0, width / 3, height);
            graphics.DrawLine(Pens.Gray, (2 * width) / 3, 0, (2 * width) / 3, height);
        }

        /// <summary>
        /// Draw the cells
        /// </summary>
        private void DrawCells()
        {
            this.DrawCell(this.row.Columns[0], 0, 0);
            this.DrawCell(this.row.Columns[1], 1, 0);
            this.DrawCell(this.row.Columns[2], 2, 0);

            this.DrawCell(this.row.Columns[3], 0, 1);
            this.DrawCell(this.row.Columns[4], 1, 1);
            this.DrawCell(this.row.Columns[5], 2, 1);
        }

        /// <summary>
        /// Copy the image from off-screen to the window
        /// </summary>
        private void CopyToWindow()
        {
            this.windowGraphics.DrawImage(Painter.bitmap, 0, 0);
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
                Brushes.Gray);

            this.DrawTextCentered(
                data.ValueAsString,
                cell,
                Brushes.White);

            this.DrawTextAtBottom(
                data.Conversion.Units,
                cell,
                Brushes.Gray);
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
            this.DrawText(text, brush, rectangle, 0.3f);
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
                cell.Top + (this.cellHeight * 0.875f),
                cell.Width,
                this.cellHeight / 4);
            this.DrawText(text, brush, rectangle, 0.3f);
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

            Font font = new Font(
                FontFamily.GenericSansSerif, 
                (float) height, 
                GraphicsUnit.Pixel);

            StringFormat format = StringFormat.GenericDefault;
            format.Alignment = StringAlignment.Center;
            format.FormatFlags = StringFormatFlags.NoClip;

            graphics.DrawString(text, font, brush, rectangle, format);
        }
    }
}