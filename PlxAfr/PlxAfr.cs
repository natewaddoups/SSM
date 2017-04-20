using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using NateW.Ssm;
using NSFW.PlxSensors;

namespace PlxAfr
{
    public partial class PlxAfr : Form
    {
        private PlxSensors sensors;
        private Timer timer;

        public PlxAfr()
        {
            InitializeComponent();
            this.TopMost = true;
        }

        private void PlxAfr_Load(object sender, EventArgs e)
        {
            if (Debugger.IsAttached && Environment.GetEnvironmentVariable("COMPUTERNAME") == "MONOLITH")
            {
                this.Left = -1000;
            }

            if (Program.PortName == "AFR")
            {
                this.timer = new Timer();
                this.timer.Interval = 500;
                this.timer.Tick += delegate(object s, EventArgs ea) { this.DrawAfr(14.7); };
                this.timer.Enabled = true;
                return;
            }

            if (Program.PortName == "ex")
            {
                try
                {
                    throw new Exception("Sample message.");
                }
                catch (Exception ex)
                {
                    this.timer = new Timer();
                    this.timer.Interval = 500;
                    this.timer.Tick += delegate(object s, EventArgs ea) { this.DrawException(ex); };
                    this.timer.Enabled = true;
                }
                return;
            }

            try
            {
                this.sensors = PlxSensors.GetInstance(Program.PortName);
                this.sensors.ValueReceived += this.PlxValueReceived;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void PlxValueReceived(object sender, PlxSensorEventArgs args)
        {
            double value = this.sensors.GetValue(args.SensorId, PlxSensorUnits.WidebandAfrGasoline147);
            this.Invoke((MethodInvoker)delegate { this.DrawAfr(value); });
        }

        private static Font afrFont;
        private static int afrFontHeight;

        private void DrawAfr(double value)
        {
            using (Graphics graphics = this.CreateGraphics())
            {
                graphics.FillRectangle(Brushes.Black, this.ClientRectangle);

                if (afrFontHeight != this.Height)
                {
                    afrFontHeight = this.Height;

                    if (afrFont != null)
                    {
                        afrFont.Dispose();
                    }
                    afrFont = new Font(FontFamily.GenericSansSerif, this.Height / 2, FontStyle.Regular, GraphicsUnit.Pixel);
                }

                graphics.DrawString(value.ToString("0.00"), afrFont, Brushes.White, new RectangleF(0, 0, this.Width, this.Height));
            }
        }

        private void DrawException(Exception ex)
        {
            using (Graphics graphics = this.CreateGraphics())
            using (Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                graphics.FillRectangle(Brushes.Red, this.ClientRectangle);
                graphics.DrawString(ex.ToString(), font, Brushes.White, new RectangleF(0, 0, this.Width, this.Height));
            }
        }
    }
}
