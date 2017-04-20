using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using NateW.Ssm.ApplicationLogic;

namespace NateW.Ssm.ApplicationUI
{
    /// <summary>
    /// Buttons for CarToolBar
    /// </summary>
    class CarToolBarButton
    {
        /// <summary>
        /// Cached drawing objects
        /// </summary>
        private static Brush selectedTextBrush = new SolidBrush(Configuration.GetColor(Colors.SelectedButtonText));
        private static Brush normalTextBrush = new SolidBrush(Configuration.GetColor(Colors.NormalButtonText));
        private static Pen buttonSeparatorPen = new Pen(Configuration.GetColor(Colors.LinesBetweenButtons));

        /// <summary>
        /// Text to draw on the button
        /// </summary>
        private string label;
        
        /// <summary>
        /// User-supplied data for this button
        /// </summary>
        private object data;

        /// <summary>
        /// Indicates whether the button should be drawn as 'selected'
        /// </summary>
        private bool selected;

        /// <summary>
        /// If zero, has no effect.  If nonzero, the button must be this width regardless of toolbar width
        /// </summary>
        private int width;

        /// <summary>
        /// User-supplied data for this button
        /// </summary>
        public object Data { get { return this.data; } }

        /// <summary>
        /// If zero, has no effect.  If nonzero, the button must be this width regardless of toolbar width
        /// </summary>
        public int Width { get { return this.width; } }

        /// <summary>
        /// Indicates whether the button should be drawn as 'selected'
        /// </summary>
        public bool Selected
        { 
            get { return this.selected; }
            set { this.selected = value; }
        }

        /// <summary>
        /// Constructor for auto-sized button
        /// </summary>
        public CarToolBarButton(string label, object data) : this (label, data, 0)
        {
        }

        /// <summary>
        /// Constructor for button with fixed width
        /// </summary>
        public CarToolBarButton(string label, object data, int width)
        {
            this.label = label;
            this.data = data;
            this.width = width;
        }

        /// <summary>
        /// Returns button label, to aid debugging
        /// </summary>
        public override string ToString()
        {
            return this.label;
        }

        /// <summary>
        /// Draw the button within the given bounds
        /// </summary>
        public void Draw(Graphics graphics, RectangleF bounds)
        {
            FontStyle style = selected ? FontStyle.Bold : FontStyle.Regular;
            //float scale = selected ? 0.95f : 0.8f;
            float scale = 0.7f;

            using (GraphicsPath path = new GraphicsPath())
            using (Font font = new Font(
                FontFamily.GenericSansSerif,
                bounds.Height * scale,
                style,
                GraphicsUnit.Pixel))
            {
                Color buttonTop = this.selected ?
                    Configuration.GetColor(Colors.SelectedButtonTop) :
                    Configuration.GetColor(Colors.NormalButtonTop);

                Color buttonBottom = this.selected ?
                    Configuration.GetColor(Colors.SelectedButtonBottom) :
                    Configuration.GetColor(Colors.NormalButtonBottom);

                using (Brush brush = new LinearGradientBrush(
                    bounds,
                    buttonTop,
                    buttonBottom,
                    90f))
                {
                    graphics.FillRectangle(brush, bounds);
                }

                if (!selected)
                {
                    graphics.DrawLine(Pens.Black, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
                    graphics.DrawLine(Pens.Black, bounds.Left + 1, bounds.Top, bounds.Left + 1, bounds.Bottom);
                    graphics.DrawLine(Pens.Black, bounds.Right - 2, bounds.Top, bounds.Right - 2, bounds.Bottom);
                    graphics.DrawLine(Pens.Black, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
                }

                StringFormat format = StringFormat.GenericDefault;
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.FormatFlags = StringFormatFlags.NoClip;

                graphics.DrawString(
                    this.label,
                    font,
                    selected ? selectedTextBrush : normalTextBrush,
                    new RectangleF(
                        bounds.X,
                        bounds.Y,
                        bounds.Width,
                        bounds.Height),
                    format);
            }
        }
    }

    /// <summary>
    /// ToolBar optimized for CarPC UI
    /// </summary>
    class CarToolBar
    {
        /// <summary>
        /// See ForEachButton
        /// </summary>
        /// <returns>true to continue iterating, false to stop iterating</returns>
        private delegate bool ButtonRectangleOperation(CarToolBarButton button, RectangleF buttonRectangle);

        /// <summary>
        /// Buttons
        /// </summary>
        private IList<CarToolBarButton> buttons;

        /// <summary>
        /// Buttons
        /// </summary>
        public IList<CarToolBarButton> Buttons
        {
            get { return this.buttons; }
            set { this.buttons = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CarToolBar()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CarToolBar(IList<CarToolBarButton> buttons)
        {
            this.buttons = buttons;
        }

        /// <summary>
        /// Draw the toolbar within the given bounds
        /// </summary>
        public void Draw(Graphics graphics, RectangleF bounds)
        {
            this.ForEachButton(bounds, delegate(CarToolBarButton button, RectangleF buttonRectangle)
            {
                button.Draw(graphics, buttonRectangle);
                return true;
            });
        }

        /// <summary>
        /// Indicate which button (if any) lies under the given X/Y.
        /// </summary>
        public CarToolBarButton HitTest(int x, int y, RectangleF bounds)
        {
            this.ThrowIfNotReady();

            if (x < bounds.Left)
            {
                return null;
            }

            if (x > bounds.Right)
            {
                return null;
            }

            if (y > bounds.Bottom)
            {
                return null;
            }

            if (y < bounds.Top)
            {
                return null;
            }

            PointF point = new PointF(x, y);
            CarToolBarButton result = null;
            this.ForEachButton(bounds, delegate(CarToolBarButton button, RectangleF buttonRectangle)
            {
                if (buttonRectangle.Contains(point))
                {
                    result = button;
                    return false;
                }
                return true;
            });

            return result;
        }

        /// <summary>
        /// Select the given button
        /// </summary>
        public void Select(CarToolBarButton selected)
        {
            this.ThrowIfNotReady();

            foreach (CarToolBarButton button in this.buttons)
            {
                button.Selected = (button == selected);
            }
        }

        /// <summary>
        /// Iterate over the buttons, operating on each button with its bounding rectangle
        /// </summary>
        private void ForEachButton(RectangleF bounds, ButtonRectangleOperation operation)
        {
            this.ThrowIfNotReady();

            int numberOfFixedWidthButtons = 0;
            int sumOfFixedWidthButtons = 0;
            for (int buttonIndex = 0; buttonIndex < buttons.Count; buttonIndex++)
            {
                int buttonWidth = this.buttons[buttonIndex].Width;
                if (buttonWidth > 0)
                {
                    numberOfFixedWidthButtons++;
                    sumOfFixedWidthButtons += buttonWidth;
                }
            }

            float defaultButtonWidth = (bounds.Width - sumOfFixedWidthButtons) / (this.buttons.Count - numberOfFixedWidthButtons);
            float buttonPosition = bounds.Left;

            for (int buttonIndex = 0; buttonIndex < buttons.Count; buttonIndex++)
            {
                CarToolBarButton button = this.buttons[buttonIndex];
                bool isFixedWidth = button.Width > 0;
                float buttonWidth = isFixedWidth ? button.Width : defaultButtonWidth;

                float left = buttonPosition;
                float right = left + buttonWidth;
                float top = bounds.Top;
                float bottom = bounds.Bottom;
                RectangleF buttonRectangle = new RectangleF(left, top, right - left, bottom - top);

                if (!operation(button, buttonRectangle))
                    break;

                buttonPosition += buttonWidth;
            }
        }

        /// <summary>
        /// Throw an exception if the toolbar is not ready for use.
        /// </summary>
        private void ThrowIfNotReady()
        {
            if (this.buttons == null)
            {
                throw new InvalidOperationException("CarToolBar has no buttons.");
            }
        }
    }
}
