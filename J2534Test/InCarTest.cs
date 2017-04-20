using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NateW.J2534;

namespace J2534Test
{
    /// <summary>
    /// Test OpenPort20 API in the car
    /// </summary>
    [TestClass]
    public class InCarTest
    {
        public InCarTest()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void OpenCloseDevice()
        {
            UInt32 deviceID;
            Int32 status = NativeOpenPort20.PassThruOpen(null, out deviceID);
            TraceStatus("PassThruOpen", status);

            Trace.WriteLine("Device ID: " + deviceID);

            status = NativeOpenPort20.PassThruClose(deviceID);
            TraceStatus("PassThruClose", status);
        }

        [TestMethod]
        public void OpenCloseChannel()
        {
            UInt32 deviceID;
            Int32 status = NativeOpenPort20.PassThruOpen(null, out deviceID);
            TraceStatus("PassThruOpen", status);

            Trace.WriteLine("Device ID: " + deviceID);

            UInt32 channelID;
            status = NativeOpenPort20.PassThruConnect(
                deviceID, 
                (UInt32) PassThruProtocol.Iso9141, 
                (UInt32) PassThruConnectFlags.Iso9141NoChecksum, 
                (UInt32) PassThruBaudRate.Rate4800, 
                out channelID);
            TraceStatus("PassThruConnect", status);

            Trace.WriteLine("Channel ID: " + channelID);

            status = NativeOpenPort20.PassThruDisconnect(channelID);
            TraceStatus("PassThruDisconnect", status);

            status = NativeOpenPort20.PassThruClose(deviceID);
            TraceStatus("PassThruClose", status);
        }


        [TestMethod]
        public void SendReceiveLowLevelApi()
        {
            UInt32 deviceID;
            Int32 status = NativeOpenPort20.PassThruOpen(null, out deviceID);
            TraceStatus("PassThruOpen", status);
            Trace.WriteLine("Device ID: " + deviceID);

            UInt32 channelID;
            status = NativeOpenPort20.PassThruConnect(
                deviceID, 
                (UInt32) PassThruProtocol.Iso9141, 
                (UInt32) PassThruConnectFlags.Iso9141NoChecksum,
                (UInt32) PassThruBaudRate.Rate4800, 
                out channelID);
            TraceStatus("PassThruConnect", status);
            Trace.WriteLine("Channel ID: " + channelID);
              
            SetConfiguration P1Max = new SetConfiguration(SetConfigurationParameter.P1Max, 2);
            SetConfiguration P3Min = new SetConfiguration(SetConfigurationParameter.P3Min, 0);
            SetConfiguration P4Min = new SetConfiguration(SetConfigurationParameter.P4Min, 0);
            SetConfiguration Terminator = new SetConfiguration(SetConfigurationParameter.FinalParam, 0);
            // TODO: add loopback=1
            SetConfiguration[] setConfigurationArray = new SetConfiguration[] { P1Max, P3Min, P4Min };
            using (SetConfigurationList setConfigurationList = new SetConfigurationList(setConfigurationArray))
            //IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(SetConfiguration) * setConfigurationArray.Length);
            //IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(setConfigurationList));
            //{
                status = NativeOpenPort20.PassThruIoctl(
                    channelID,
                    PassThruIOControl.SetConfig,
                    ref setConfigurationArray,//setConfigurationList.Pointer,
                    IntPtr.Zero);
            //}
            //Marshal.FreeCoTaskMem(buffer);

            TraceStatus("PassThruIoCtl SetConfig", status);

            UInt32 filterID;
            PassThruMsg maskMsg = new PassThruMsg(PassThruProtocol.Iso9141);
            //maskMsg.ExtraDataIndex = 1;
            maskMsg.DataSize = 1;

            status = NativeOpenPort20.PassThruStartMsgFilter(
                channelID,
                (UInt32) PassThruFilterType.Pass,
                maskMsg,
                maskMsg,
                null,
                out filterID);
            TraceStatus("PassThruStartMsgFilter", status);
            Trace.WriteLine("Filter ID: " + filterID);

            byte[] messageBytes = new byte[] { 0x80, 0x10, 0xF0, 0x01, 0xBF, 0x40 };
            PassThruMsg initRequestMsg = new PassThruMsg(PassThruProtocol.Iso9141);
            initRequestMsg.ProtocolID = PassThruProtocol.Iso9141;
            initRequestMsg.DataSize = (UInt32) messageBytes.Length;
            for (int i = 0; i < messageBytes.Length; i ++)
            {
                initRequestMsg.Data[i] = messageBytes[i];
            }

            PassThruMsg[] initRequestMsgs = new PassThruMsg[] { initRequestMsg };
            UInt32 numMsgs = 1;

            status = NativeOpenPort20.PassThruWriteMsgs(
                channelID,
                initRequestMsgs,
                ref numMsgs,
                500);
            TraceStatus("PassThruWriteMsgs", status);

            PassThruMsg received = new PassThruMsg(PassThruProtocol.Iso9141);
            PassThruMsg[] receivedMessages = new PassThruMsg[] { received };
            numMsgs = 1;
            status = NativeOpenPort20.PassThruReadMsgs(
                channelID,
                receivedMessages,
                ref numMsgs,
                500);
            TraceStatus("PassThruReadMsgs", status);

            if (status == (UInt32)PassThruStatus.NoError)
            {
                for (int i = 0; i < received.DataSize; i++)
                {
                    Trace.Write(received.Data[i].ToString());
                    Trace.Write(' ');
                }
            }

            status = NativeOpenPort20.PassThruDisconnect(channelID);
            TraceStatus("PassThruDisconnect", status);

            status = NativeOpenPort20.PassThruClose(deviceID);
            TraceStatus("PassThruClose", status);
        }

        [TestMethod]
        public void SendReceiveHighLevelApi()
        {
            try
            {
                System.IO.File.Copy(@"..\..\..\J2534Mock\Debug\J2534Mock.dll", "J2534Mock.dll");
            }
            catch (System.IO.IOException ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            try
            {
                System.IO.File.Copy(@"..\..\..\J2534Mock\Debug\J2534Mock.pdb", "J2534Mock.pdb");
            }
            catch (System.IO.IOException ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            Debug.WriteLine("Opening device");
            //PassThruDevice device = PassThruDevice.GetInstance(new OpenPort20());
            PassThruDevice device = PassThruDevice.GetInstance(new MockPassThru());
            device.Open();

            Debug.WriteLine("Opening channel");
            PassThruChannel channel = device.OpenChannel(
                PassThruProtocol.Iso9141,
                PassThruConnectFlags.Iso9141NoChecksum,
                PassThruBaudRate.Rate4800);

            Debug.WriteLine("Initializing channel for SSM");
            channel.InitializeSsm();

            byte[] messageBytes = new byte[] { 0x80, 0x10, 0xF0, 0x01, 0xBF, 0x40 };
            PassThruMsg initRequestMsg = new PassThruMsg(PassThruProtocol.Iso9141);
            initRequestMsg.ProtocolID = PassThruProtocol.Iso9141;
            initRequestMsg.DataSize = (UInt32) messageBytes.Length;
            for (int i = 0; i < messageBytes.Length; i ++)
            {
                initRequestMsg.Data[i] = messageBytes[i];
            }

            PassThruMsg[] initRequestMsgs = new PassThruMsg[] { initRequestMsg };
            UInt32 numMsgs = 1;

            Debug.WriteLine("Sending SSM init message");
            channel.WriteMessages(initRequestMsgs, ref numMsgs, TimeSpan.FromMilliseconds(1000));

            PassThruMsg received = new PassThruMsg(PassThruProtocol.Iso9141);
            
            Debug.WriteLine("Waiting for SSM init response");
            channel.ReadMessage(
                received,
                TimeSpan.FromMilliseconds(1000));

            Debug.WriteLine("Closing channel");
            channel.Close();

            Debug.WriteLine("Closing device");
            device.Close();
        }

        private void TraceStatus(string message, Int32 status)
        {
            if (status == (Int32) PassThruStatus.NoError)
            {
                Trace.WriteLine(message + ": no error.");
                return;
            }

            Trace.WriteLine(message);
            Trace.WriteLine("        " + ((PassThruStatus)status).ToString());

            byte[] messageBytes = new byte[80];
            PassThruStatus returnCode = (PassThruStatus)NativeMock.PassThruGetLastError(messageBytes);
            string errorMessage = System.Text.Encoding.ASCII.GetString(messageBytes);

            //byte[] errorMessageBytes = new byte[100];
            //IntPtr pointer = 
            //string errorM
            //NativeOpenPort20.PassThruGetLastError(errorMessageBytes);
            //int length = 0;
            //while (errorMessageBytes[length] != 0) length++;
            //string errorMessage = System.Text.Encoding.ASCII.GetString(errorMessageBytes, 0, length);
            //string errorMessage;
            //NativeOpenPort20.PassThruGetLastError(out errorMessage);
            Trace.WriteLine("        " + errorMessage);
        }
    }
}
