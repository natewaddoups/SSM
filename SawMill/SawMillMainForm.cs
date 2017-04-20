using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Settings = NateW.Ssm.ApplicationUI.Properties.Settings;
using NateW.Ssm;
using NateW.Ssm.ApplicationLogic;

namespace NateW.Ssm.ApplicationUI
{
    /// <summary>
    /// Main form for the SawMill application
    /// </summary>
    public partial class SawMillMainForm : Form
    {
        private const string exitButtonData = "Exit";
        private const string minimizeButtonData = "Minimize";

        private CarToolBar mainToolBar;
        private CarToolBar toolBar;
        private List<SawMillScreen> screens;
        private SawMill sawMill;
        private Exception exception;
        private float toolbarSize = 40;

        /// <summary>
        /// Constructor
        /// </summary>
        public SawMillMainForm()
        {
            InitializeComponent();

            if (Settings.Default.CarPC)
            {
                this.FormBorderStyle = FormBorderStyle.None;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.MinimizeBox = true;
                this.MaximizeBox = true;
            }

            this.Paint += new PaintEventHandler(SawMillMainForm_Paint);
            this.Resize += new EventHandler(SawMillMainForm_Resize);
            this.Click += new EventHandler(SawMillMainForm_Click);
            this.Load += new EventHandler(SawMillMainForm_Load);
            this.KeyDown += new KeyEventHandler(SawMillMainForm_KeyDown);

            Microsoft.Win32.SystemEvents.PowerModeChanged += this.SystemEvents_PowerModeChanged;

            this.BackColor = Color.Black;
            
            this.screens = this.BuildToolbars();
        }

        /// <summary>
        /// Handle keypresses
        /// </summary>
        /// <remarks>
        /// Control-C: Copy the current screen's data to the clipboard
        /// </remarks>
        void SawMillMainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.C) && (e.Modifiers == Keys.Control))
            {
                string text = this.sawMill.GetClipboardText();
                if (!string.IsNullOrEmpty(text))
                {
                    Clipboard.SetData(DataFormats.UnicodeText, text);
                }
            }
        }

        /// <summary>
        /// Close
        /// </summary>
        void SawMill_CloseCallback(IAsyncResult asyncResult)
        {
            this.Invoke(new ThreadStart(delegate
            {
                this.WriteData(); 
                this.Close();
            }));
        }

        /// <summary>
        /// Load
        /// </summary>
        void SawMillMainForm_Load(object sender, EventArgs e)
        {
            this.SetBounds();

            string configurationDirectory = Path.Combine(
                            Environment.CurrentDirectory,
                            "Configuration");

            Trace.WriteLine("SawMill.SawMillMainForm_Load: SSM Port = " + Settings.Default.SsmPort);
            if (Settings.Default.SsmPort == "Mock ECU")
            {
                MessageBox.Show(
                    "Bogus data will be displayed." +
                    Environment.NewLine +
                    "If you want real data, change the \"Port\" setting in" +
                    Environment.NewLine +
                    "SawMill.exe.config to COM1 or COM2 or whatever.",
                    "SawMill");
            }

            while (this.sawMill == null)
            {
                Exception exception = null;

                try
                {
                    this.sawMill = SawMill.GetInstance(
                        configurationDirectory,
                        Settings.Default.SsmPort,
                        Settings.Default.PlxPort,
                        this.screens,
                        this.Update);
                }
                catch (SecurityException ex)
                {
                    exception = ex;
                }
                catch (IOException ex)
                {
                    exception = ex;
                }
                catch (UnauthorizedAccessException ex)
                {
                    exception = ex;
                }

                if (exception != null)
                {
                    string message = string.Format("Unable to start: {0}{1}({2})",
                        exception.Message,
                        Environment.NewLine,
                        exception.GetType().Name);

                    DialogResult result = MessageBox.Show(
                        message,
                        "SawMill",
                        MessageBoxButtons.RetryCancel);

                    if (result == DialogResult.Retry)
                    {
                        continue;
                    }

                    this.Close();
                    return;
                }                
            }

            this.sawMill.Exception += this.RenderException;
            this.ReadData();
            this.sawMill.BeginConnect(this.Connected, null);
        }

        /// <summary>
        /// Single Click
        /// </summary>
        void SawMillMainForm_Click(object sender, EventArgs e)
        {
            if (this.IsMinimized())
            {
                return;
            }

            Point mouse = this.PointToClient(new Point(MousePosition.X, MousePosition.Y));
            CarToolBarButton button = this.toolBar.HitTest(mouse.X, mouse.Y, this.GetToolbarBounds());

            if (button == null)
            {
                this.sawMill.SingleClick();
                return;
            }

            if (button.Data is SawMillScreen)
            {
                this.toolBar.Select(button);
                if (this.sawMill.CurrentScreen != button.Data)
                {
                    this.sawMill.SetCurrentScreen(button.Data as SawMillScreen);
                    using (Graphics graphics = this.CreateGraphics())
                    {
                        this.Repaint(graphics);
                    }
                }
            }
            else if (button.Data is CarToolBar)
            {
                if (button != this.toolBar.Buttons[0])
                {
                    this.toolBar.Select(button);
                }

                this.toolBar = button.Data as CarToolBar;
                this.DrawToolbar();
                foreach (CarToolBarButton toolBarButton in this.toolBar.Buttons)
                {
                    if (toolBarButton.Selected && toolBarButton.Data is SawMillScreen)
                    {
                        this.sawMill.SetCurrentScreen(toolBarButton.Data as SawMillScreen);
                        break;
                    }
                }
            }
            else if (button.Data is string)
            {
                if ((string)button.Data == exitButtonData)
                {
                    this.toolBar.Select(button);
                    this.DrawToolbar();
                    this.sawMill.BeginClose(this.SawMill_CloseCallback, null);
                }
                else if ((string)button.Data == minimizeButtonData)
                {
                    this.toolBar.Select(button);
                    this.DrawToolbar();

                    if (Settings.Default.BackButton == "SendToBack")
                    {
                        this.SendToBack();
                        this.toolBar.Select(null);
                        this.toolBar = this.mainToolBar;
                    }
                    else
                    {
                        this.WindowState = FormWindowState.Minimized;
                        this.toolBar.Select(null);
                        this.toolBar = this.mainToolBar;
                    }
                }
                this.DrawToolbar();
            }
        }

        /// <summary>
        /// Double-Click
        /// </summary>
        private void SawMillMainForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Point mouse = this.PointToClient(new Point(MousePosition.X, MousePosition.Y));
            CarToolBarButton button = this.toolBar.HitTest(mouse.X, mouse.Y, this.GetToolbarBounds());
            if (button != null)
            {
                return;
            }

            this.sawMill.DoubleClick();
        }

        /// <summary>
        /// Resize
        /// </summary>
        void SawMillMainForm_Resize(object sender, EventArgs e)
        {
            using (Graphics graphics = this.CreateGraphics())
            {
                this.Repaint(graphics);
            }
        }

        /// <summary>
        /// Paint
        /// </summary>
        void SawMillMainForm_Paint(object sender, PaintEventArgs e)
        {
            this.Repaint(e.Graphics);
        }

        /// <summary>
        /// Update SSM display
        /// </summary>
        private void Update(LogEventArgs args)
        {
            if (this.IsMinimized())
            {
                return;
            }

            this.exception = null;
            int width = this.ClientRectangle.Width;
            //int toolbarHeight = (int) (this.ClientRectangle.Height - toolbarSize);
            int height = this.ClientRectangle.Height - (int) toolbarSize;

            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Graphics windowGraphics = this.CreateGraphics())
            {
                this.sawMill.Update(args, graphics, width, height);
                windowGraphics.DrawImage(bitmap, 0, toolbarSize);
            }
        }

        /// <summary>
        /// Invoked when an exception happens in the logging code
        /// </summary>
        private void RenderException(object sender, ExceptionEventArgs args)
        {
            this.exception = args.Exception;

            this.Invoke(new ThreadStart(delegate
                {
                    using (Graphics graphics = this.CreateGraphics())
                    {
                        this.RenderException(graphics, this.exception);
                    }
                }));
        }

        /// <summary>
        /// Draw exception details
        /// </summary>
        private void RenderException(Graphics graphics, Exception localException)
        {
            using (Font font = new Font(
                FontFamily.GenericSansSerif,
                (float)System.Windows.Forms.SystemInformation.CaptionHeight,
                GraphicsUnit.Pixel))
            {
                RectangleF bounds;
                string message;

                // TODO: preserve the screen background so this can be painted
                // over last good data, when invoked via WM_PAINT
                /*  if (localException is SsmPacketFormatException)
                {
                    message = localException.Message;
                    bounds = new RectangleF(
                        0,
                        this.toolbarSize + 2,
                        this.ClientRectangle.Width / 2,
                        this.toolbarSize);
                }*/

                if (NateW.Ssm.SsmUtility.IsTransientException(localException))
                {
                    message = "DANGER TO MANIFOLD";// localException.Message;
                }
                else
                {
                    message = localException.ToString();
                }
                
                bounds = new RectangleF(
                    0,
                    this.toolbarSize,
                    this.ClientRectangle.Width,
                    this.ClientRectangle.Height);

                graphics.FillRectangle(
                    Brushes.Red,
                    bounds);

                if (localException != null)
                {
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    graphics.DrawString(
                        message,
                        font,
                        Brushes.White,
                        bounds,
                        format);
                }
            }
        }

        /// <summary>
        /// Returns a rectangle for the toolbar area
        /// </summary>
        private RectangleF GetToolbarBounds()
        {
            RectangleF toolBarRectangle = new RectangleF(
                this.ClientRectangle.Left,
                this.ClientRectangle.Top,
                this.ClientRectangle.Width,
                this.toolbarSize);
            return toolBarRectangle;
        }

        /// <summary>
        /// Returns a rectangle for the SawMill screen area
        /// </summary>
        private RectangleF GetScreenBounds()
        {
            RectangleF toolBarRectangle = new RectangleF(
                this.ClientRectangle.Left,
                this.toolbarSize,
                this.ClientRectangle.Width,
                this.ClientRectangle.Height - this.toolbarSize);
            return toolBarRectangle;
        }

        /// <summary>
        /// Draw the screen
        /// </summary>
        private void Repaint(Graphics graphics)
        {
            if (this.IsMinimized())
            {
                return;
            }

            RectangleF toolBarRectangle = GetToolbarBounds();
            RectangleF screenRectangle = this.GetScreenBounds();

            if (this.toolBar != null)
            {
                this.toolBar.Draw(graphics, toolBarRectangle);
            }
            else
            {
                screenRectangle = RectangleF.FromLTRB(
                    this.ClientRectangle.Left, 
                    this.ClientRectangle.Top, 
                    this.ClientRectangle.Right, 
                    this.ClientRectangle.Bottom);
            }

            graphics.FillRectangle(
                Brushes.Black,
                screenRectangle);

            Exception localException = this.exception;
            if (localException != null)
            {
                this.RenderException(graphics, localException);
            }
        }

        /// <summary>
        /// Invoked when SawMill connects to the ECU
        /// </summary>
        private void Connected(IAsyncResult asyncResult)
        {
            try
            {
                this.sawMill.EndConnect(asyncResult);
                this.sawMill.StartLogging();
            }
            catch (Exception ex)
            {
                if (ex is SsmPacketFormatException || ex is IOException || ex is UnauthorizedAccessException)
                {
                    Trace.WriteLine("Connect exception: " + ex.Message);
                }
                else
                {
                    Trace.WriteLine("Connect exception: " + ex.ToString());
                }

                if (DialogResult.Retry == MessageBox.Show(
                    "Is the Tactrix cable connected?" + Environment.NewLine + ex.Message, 
                    "Unable to start.", 
                    MessageBoxButtons.RetryCancel))
                {
                    this.sawMill.BeginConnect(this.Connected, null);
                }
                else
                {
                    this.Invoke(new ThreadStart(delegate { this.Close(); }));
                }
            }
        }

        /// <summary>
        /// For debugging
        /// </summary>
        private void DrawDebugString(string message)
        {
            RectangleF bounds = new RectangleF(
                    this.ClientRectangle.Left,
                    this.ClientRectangle.Top,
                    this.ClientRectangle.Width,
                    this.ClientRectangle.Height);

            using (Graphics graphics = this.CreateGraphics())
            using (Font font = new Font(
                FontFamily.GenericSansSerif,
                25,
                GraphicsUnit.Pixel))
            {
                graphics.DrawString(message, font, Brushes.Black, new PointF(0, bounds.Height / 2));
            }
        }

        /// <summary>
        /// Get parameters for a SixPack screen
        /// </summary>
        private IList<string> GetSixPackParameters(string set)
        {
            List<string> parameters = new List<string>(6);
            for (int i = 1; i <= 6; i++)
            {
                try
                {
                    string name = set + i.ToString();
                    string value = Settings.Default[name] as string;
                    parameters.Add(value);
                }
                catch (System.Configuration.SettingsPropertyNotFoundException)
                {
                }
            }
            return parameters;
        }

        /// <summary>
        /// Get the path to load/save screen data
        /// </summary>
        private string GetPersistencePath()
        {
            if ((Program.Args != null) && (Program.Args.Length > 0))
            {
                if (File.Exists(Program.Args[0]))
                {
                    return Program.Args[0];
                }
            }

            string path = Environment.GetEnvironmentVariable("USERPROFILE");
            string fileName = "SawMill.data";
            string result = Path.Combine(path, fileName);
            return result;
        }

        /// <summary>
        /// Read screen data from disk
        /// </summary>
        private void ReadData()
        {
            try
            {
                string path = this.GetPersistencePath();
                using (Stream stream = File.OpenRead(path))
                {
                    this.sawMill.ReadFrom(stream);
                }
            }
            catch (IOException exception)
            {
                Trace.WriteLine("SawMillMainForm.ReadData: " + exception.ToString());
            }
        }

        /// <summary>
        /// Save state when suspending, just in case.
        /// </summary>
        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Suspend)
            {
                this.WriteData();
            }
        }

        /// <summary>
        /// Write screen data to disk
        /// </summary>
        private void WriteData()
        {
            string path = this.GetPersistencePath();
            string oldPath = path + ".old";

            try
            {
                File.Delete(oldPath);
            }
            catch (UnauthorizedAccessException exception)
            {
                Trace.WriteLine("SawMillMainForm.WriteData Delete: " + exception.ToString());
            }
            catch (IOException exception)
            {
                Trace.WriteLine("SawMillMainForm.WriteData Delete: " + exception.ToString());
            }

            try
            {
                File.Copy(path, oldPath, true);
            }
            catch (UnauthorizedAccessException exception)
            {
                Trace.WriteLine("SawMillMainForm.WriteData Copy: " + exception.ToString());
            }
            catch (IOException exception)
            {
                Trace.WriteLine("SawMillMainForm.WriteData Copy: " + exception.ToString());
            }

            try
            {
                using (Stream stream = File.Create(path))
                {
                    this.sawMill.WriteTo(stream);
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                Trace.WriteLine("SawMillMainForm.WriteData Create/Write: " + exception.ToString());
            }
            catch (IOException exception)
            {
                Trace.WriteLine("SawMillMainForm.WriteData Create/Write: " + exception.ToString());
            }
        }


        /// <summary>
        /// Set the Form's position and size
        /// </summary>
        private void SetBounds()
        {
            int left = Settings.Default.WindowLeft;
            int top = Settings.Default.WindowTop;
            int width = Settings.Default.WindowWidth;
            int height = Settings.Default.WindowHeight;
            this.SetBounds(left, top, width, height);
        }

        /// <summary>
        /// Indicate whether the window is minimized (or just too small to paint properly)
        /// </summary>
        private bool IsMinimized()
        {
            if (this.ClientRectangle.Width < 10)
            {
                return true;
            }
            
            if (this.ClientRectangle.Height < (this.toolbarSize + 10))
            {
                return true;
            }

            return false;
        }

        private List<SawMillScreen> BuildToolbars()
        {
            this.mainToolBar = new CarToolBar();
            CarToolBar gaugeToolBar = new CarToolBar();
            CarToolBar scatterToolBar = new CarToolBar();
            CarToolBar heatMapToolBar = new CarToolBar();
            CarToolBar exitToolBar = new CarToolBar();

            int leftButtonWidth = 50;

            List<CarToolBarButton> mainButtons = new List<CarToolBarButton>();
            mainButtons.Add(new CarToolBarButton("<", exitToolBar, leftButtonWidth)); // placeholder for back button in other menus
            mainButtons.Add(new CarToolBarButton("Gauges", gaugeToolBar));
            mainButtons.Add(new CarToolBarButton("Scatter Plots", scatterToolBar));
            mainButtons.Add(new CarToolBarButton("Tables", heatMapToolBar));
            mainButtons[1].Selected = true;
            mainToolBar.Buttons = mainButtons;

            List<CarToolBarButton> gaugeButtons = new List<CarToolBarButton>();
            gaugeButtons.Add(new CarToolBarButton("<", mainToolBar, leftButtonWidth));
            gaugeButtons.Add(new CarToolBarButton("Main", new SixPackScreen("Main", GetSixPackParameters("Main"))));
            gaugeButtons.Add(new CarToolBarButton("Boost", new SixPackScreen("Boost", GetSixPackParameters("Boost"))));
            gaugeButtons.Add(new CarToolBarButton("Timing", new SixPackScreen("Timing", GetSixPackParameters("Timing"))));
            gaugeButtons.Add(new CarToolBarButton("Misfire", new SixPackScreen("Roughness", GetSixPackParameters("Roughness"))));
            gaugeButtons.Add(new CarToolBarButton("Trims", new SixPackScreen("Fuel Trim", GetSixPackParameters("FTrim"))));
            gaugeButtons.Add(new CarToolBarButton("A", new SixPackScreen("A", GetSixPackParameters("A"))));
            gaugeButtons.Add(new CarToolBarButton("B", new SixPackScreen("B", GetSixPackParameters("B"))));
            gaugeButtons[1].Selected = true;
            gaugeToolBar.Buttons = gaugeButtons;

            List<CarToolBarButton> scatterButtons = new List<CarToolBarButton>();
            scatterButtons.Add(new CarToolBarButton("<", mainToolBar, leftButtonWidth));
            scatterButtons.Add(new CarToolBarButton("MAF", new RpmMafScreen(Settings.Default.MaxRpm, Settings.Default.MaxMafGS)));
            scatterButtons.Add(new CarToolBarButton("Load", new RpmLoadScreen(Settings.Default.MaxRpm, Settings.Default.MaxLoadGRev)));
            scatterButtons.Add(new CarToolBarButton("AFR", new AfrScatterScreen(Settings.Default.MaxMafGS)));
            scatterButtons.Add(new CarToolBarButton("Compressor", new CompressorMapScreen(Settings.Default.MaxMafGS, Settings.Default.MaxAbsoluteBoostBar)));
            scatterButtons[1].Selected = true;
            scatterToolBar.Buttons = scatterButtons;

            List<CarToolBarButton> heatMapButtons = new List<CarToolBarButton>();
            heatMapButtons.Add(new CarToolBarButton("<", mainToolBar, leftButtonWidth));
            heatMapButtons.Add(new CarToolBarButton("Knock", new KnockScreen(Settings.Default.MaxRpm, Settings.Default.MaxLoadGRev)));
            heatMapButtons.Add(new CarToolBarButton("AFR", new AfrHeatMapScreen(Settings.Default.MaxRpm, Settings.Default.MaxLoadGRev)));
            heatMapButtons.Add(new CarToolBarButton("Boost", new BoostScreen(Settings.Default.MaxRpm)));
            heatMapButtons.Add(new CarToolBarButton("VE", new VEScreen(Settings.Default.MaxRpm, Settings.Default.MaxAbsoluteBoostBar)));
            heatMapButtons.Add(new CarToolBarButton("IVE", new IVEScreen(Settings.Default.MaxRpm, Settings.Default.MaxLoadGRev)));
            // TODO: boost targets (also need UI to choose min/max/avg)
            heatMapButtons[1].Selected = true;
            heatMapToolBar.Buttons = heatMapButtons;
            
            List<CarToolBarButton> exitButtons = new List<CarToolBarButton>();
            if (Settings.Default.CarPC)
            {
                exitButtons.Add(new CarToolBarButton("<", minimizeButtonData, leftButtonWidth));
                exitButtons.Add(new CarToolBarButton("Main Menu", mainToolBar));
            }
            else
            {
                exitButtons.Add(new CarToolBarButton("<", mainToolBar, leftButtonWidth));
                exitButtons.Add(new CarToolBarButton("Minimize", minimizeButtonData));
            }
            exitButtons.Add(new CarToolBarButton("Exit", exitButtonData));
            exitToolBar.Buttons = exitButtons;
                        
            this.toolBar = mainToolBar;

            List<SawMillScreen> screens = new List<SawMillScreen>();
            GetScreensFromToolbars(mainToolBar, mainToolBar, screens);
            return screens;
        }

        private void GetScreensFromToolbars(CarToolBar root, CarToolBar current, ICollection<SawMillScreen> screens)
        {
            foreach (CarToolBarButton button in current.Buttons)
            {
                if (button.Data is SawMillScreen)
                {
                    screens.Add(button.Data as SawMillScreen);
                }
                else if (button.Data is CarToolBar)
                {
                    if (button.Data != root)
                    {
                        this.GetScreensFromToolbars(root, button.Data as CarToolBar, screens);
                    }
                }
            }
        }

        private void DrawToolbar()
        {
            RectangleF toolbarRectangle = this.GetToolbarBounds();
            using (Graphics graphics = this.CreateGraphics())
            {
                this.toolBar.Draw(graphics, toolbarRectangle);
            }
        }
    }
}
