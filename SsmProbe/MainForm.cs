using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

using NateW.Ssm;

namespace SsmProbe
{
    public partial class MainForm : Form
    {
        private int[] supportedBaudRates = new int[] { 4800, 9600, 14400, 28800, 57600, 115200 };
        private SerialPort port;
        private Stream stream;
        private Timer timer;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            this.FillSerialPortList(builder);
            this.FillBitRateList();
            this.FillDeviceList();
            this.FillCommandByteList();
            this.FillPadList();
            this.summary.Text = builder.ToString();

            this.timer = new Timer();
            this.timer.Interval = 250;
            this.timer.Tick += this.TimerTick;
        }

        private void FillSerialPortList(StringBuilder builder)
        {
            this.serialPortList.Items.Add(string.Empty);

            this.serialPortList.Items.Add(SsmUtility.MockEcuDisplayName);

            if (SsmUtility.OpenPort20Exists())
            {
                this.serialPortList.Items.Add(SsmUtility.OpenPort20DisplayName);
            }

            foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                this.serialPortList.Items.Add(portName);
                builder.AppendLine("Added serial port " + portName);
            }
        }
        
        private void FillBitRateList()
        {
            foreach (int bitRate in supportedBaudRates)
            {
                this.bitRates.Items.Add(bitRate.ToString(CultureInfo.InvariantCulture));
            }
            this.bitRates.SelectedItem = supportedBaudRates[0].ToString(CultureInfo.InvariantCulture);
        }

        private void FillDeviceList()
        {
            for (byte b = 0; b < 255; b++)
            {
                string deviceId = b.ToHex();

                if (b == 0x10)
                {
                    deviceId = deviceId + " (ECU)";
                }

                if (b == 0xF0)
                {
                    deviceId = deviceId + " (Diagnostic Tool)";
                }

                this.device.Items.Add(deviceId);

                if (b == 0x10)
                {
                    this.device.SelectedItem = deviceId;
                }
            }
        }

        private void FillCommandByteList()
        {
            Dictionary<byte, string> ecuCommands = new Dictionary<byte, string>();

            foreach (SsmCommand value in Enum.GetValues(typeof(SsmCommand)))
            {
                if (value.ToString().EndsWith("Request", StringComparison.OrdinalIgnoreCase))
                {
                    //this.commandByte.Items.Add(value);
                    ecuCommands[(byte)value] = value.ToString();
                }
            }

            for (byte b = 0; b < 255; b++)
            {
                string commandId = b.ToHex();

                if (ecuCommands.ContainsKey(b))
                {
                    commandId = commandId + " " + ecuCommands[b];
                }

                this.commandByte.Items.Add(commandId);

                if (b == (byte)SsmCommand.ReadAddressesRequest)
                {
                    this.commandByte.SelectedItem = (commandId);
                }
            }            
        }

        private void FillPadList()
        {
            this.extraByte.Items.Add(true.ToString());
            this.extraByte.Items.Add(false.ToString());
            this.extraByte.SelectedItem = true.ToString();
        }

        private void serialPortList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.stream != null)
            {
                this.stream.Dispose();
                this.stream = null;
            }

            if (this.port != null)
            {
                this.port.Dispose();
                this.port = null;
            }

            try
            {
                if ((this.serialPortList.SelectedItem == null) ||
                    this.serialPortList.SelectedItem.ToString() == string.Empty)
                {
                    this.summary.Text = "No serial port selected";
                    return;
                }

                string portName = this.serialPortList.SelectedItem.ToString();
                int baudRate = int.Parse(this.bitRates.SelectedItem.ToString());
                this.stream = SsmUtility.GetDataStream(
                    portName,
                    baudRate,
                    ref this.port,
                    delegate(string line) { });

                this.summary.Text = "Opened " + portName;
            }
            catch (Exception ex)
            {
                this.summary.Text = ex.ToString();
            }
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            if (this.stream == null)
            {
                this.summary.Text = "Choose a port first.";
                return;
            }

            if (this.commandByte.SelectedItem == null)
            {
                this.summary.Text = "Select a command byte first.";
                return;
            }

            /*SsmDirection direction = SsmDirection.ToEcu;
            SsmCommand command = (SsmCommand) this.commandByte.SelectedItem;
            string payloadString = this.payload.Text;
            byte[] payloadBytes = StringToBytes(payloadString);
            SsmInterface ecu = SsmInterface.GetInstance(this.stream);
            SsmPacket probe = SsmPacket.CreateArbitrary(direction, command, payloadBytes);
            */

            byte device = this.GetDevice();
            byte command = this.GetCommand();
            bool pad = this.GetPad();
            byte[] payloadBytes = this.GetPayload();
            
            StringBuilder builder = new StringBuilder();

            try
            {
                this.SendReceive(
                    device,
                    command,
                    pad,
                    payloadBytes,
                    builder);
            }
            catch (Exception exception)
            {
                builder.AppendLine("Something went wrong.");
                builder.AppendLine(exception.ToString());
            }

            this.summary.Text = builder.ToString();
        }

        private byte GetDevice()
        {
            string deviceString = (string) this.device.SelectedItem;
            string[] split = deviceString.Split(' ');
            byte device = Convert.ToByte(split[0], 16);
            return device;
        }

        private byte GetCommand()
        {
            string commandString = (string) this.commandByte.SelectedItem;
            string[] split = commandString.Split(' ');
            byte command = Convert.ToByte(split[0], 16);
            return command;
        }

        private bool GetPad()
        {
            return bool.Parse((string) this.extraByte.SelectedItem);
        }

        private byte[] GetPayload()
        {
            string payloadString = this.payload.Text;
            byte[] payloadBytes = StringToBytes(payloadString);
            return payloadBytes;
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(this.summary.Text);
        }

        private void SendReceive(
            byte device, 
            byte command, 
            bool pad, 
            byte[] payloadBytes, 
            StringBuilder builder)
        {
            SsmPacket probe = SsmPacket.CreateArbitrary(device, command, pad, payloadBytes);

            builder.AppendFormat(
                "Sending {0:X2} to {1:X2} with payload of {2} bytes:",
                command,
                device,
                payloadBytes.Length);
            builder.AppendLine();
            builder.AppendLine(MainForm.BytesToString(payloadBytes));
            builder.AppendLine();

            byte[] requestBuffer = probe.Data;
            this.stream.Write(requestBuffer, 0, requestBuffer.Length);
            System.Threading.Thread.Sleep(250);

            byte[] responseBuffer = new byte[2000];
            int bytesRead = this.stream.Read(responseBuffer, 0, responseBuffer.Length);
            builder.AppendFormat(
                "Received {0} bytes total, {1} after subtracting echo, {2} payload bytes.",
                bytesRead,
                bytesRead - probe.Data.Length,
                bytesRead - (probe.Data.Length + 6));
            builder.AppendLine();

            SsmPacket response = SsmPacket.ParseResponse(responseBuffer, 0, bytesRead);
            builder.AppendLine("Response packet:");
            builder.AppendLine(BytesToString(response.Data));
            builder.AppendLine();
            builder.AppendLine("Response payload:");
            builder.AppendFormat("Header: {0}", response.Data[0].ToHex());
            builder.AppendLine();
            builder.AppendFormat("Dest: {0}", response.Data[1].ToHex());
            builder.AppendLine();
            builder.AppendFormat("Source: {0}", response.Data[2].ToHex());
            builder.AppendLine();
            builder.AppendFormat("DataSize: {0}", response.Data[3].ToHex());
            builder.AppendLine();
            builder.AppendFormat("Command: {0}", response.Data[4].ToHex());
            builder.AppendLine();
            builder.AppendFormat("Checksum: {0}", response.Data[response.Data.Length - 1].ToHex());
            builder.AppendLine();

            byte[] payload = new byte[response.Data.Length - 6];
            for (int i = 5; i < response.Data.Length - 1; i++)
            {
                payload[i - 5] = response.Data[i];
            }

            builder.AppendFormat("Payload: {0}", BytesToString(payload));
        }

        private static byte[] StringToBytes(string value)
        {
            value = value.Replace("-", string.Empty);
            value = value.Replace(" ", string.Empty);
            
            int NumberChars = value.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);
            return bytes;

            //SoapHexBinary shb = SoapHexBinary.Parse(value);
            //return shb.Value;
        }

        private static string BytesToString(byte[] value)
        {
            string hex = BitConverter.ToString(value);
            return hex.Replace("-", " ");

            //SoapHexBinary shb = new SoapHexBinary(value);
            //return shb.ToString();
        }

        private void scanButton_Click(object sender, EventArgs e)
        {
            if (this.scanButton.Text == "Stop")
            {
                this.timer.Stop();

                foreach (Control control in this.Controls)
                {
                    control.Enabled = true;
                }

                this.scanButton.Text = "Scan &All Devices";
                this.summary.Text = this.scanReportBuilder.ToString();
                return;
            }

            if (this.stream == null)
            {
                this.summary.Text = "Choose a port first.";
                return;
            }

            foreach (Control control in this.Controls)
            {
                control.Enabled = false;
            }

            this.scanButton.Enabled = true;
            this.scanButton.Text = "Stop";

            this.deviceId = 0;
            this.scanReportBuilder = new StringBuilder();

            this.scanReportBuilder.AppendLine("Scanning all SSM devices.");
            
            this.timer.Start();            
        }

        private int deviceId;
        private StringBuilder scanReportBuilder;

        private void TimerTick(object sender, EventArgs args)
        {
            byte command = this.GetCommand();
            bool pad = this.GetPad();
            byte[] payloadBytes = this.GetPayload();

            //byte command = (byte)SsmCommand.EcuInitRequest;
            //bool pad = false;
            //byte[] payloadBytes = new byte[0];

            if (deviceId <= 255)
            {
                this.summary.Text = "Scanning device " + ((byte)deviceId).ToHex();

                try
                {
                    this.scanReportBuilder.AppendLine();
                    this.scanReportBuilder.AppendLine("#################################");

                    this.SendReceive(
                        (byte) deviceId, 
                        command, 
                        pad,
                        payloadBytes, 
                        this.scanReportBuilder);
                }
                catch (Exception exception)
                {
                    this.scanReportBuilder.AppendLine("Something went wrong.");
                    this.scanReportBuilder.AppendLine(exception.ToString());
                }

                this.deviceId++;
            }
            else
            {
                this.summary.Text = this.scanReportBuilder.ToString();
                this.timer.Stop();
            }
        }
    }

    public static class ExtensionMethods
    {
        public static string ToHex(this byte b)
        {
            return BitConverter.ToString(new byte[] { b });
        }
    }
}

