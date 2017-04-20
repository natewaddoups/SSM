using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NateW.J2534;

namespace J2534Exe
{
    class Program
    {
        private static void Copy(string fileName)
        {
            string sourceDirectory = @"C:\Documents\Development\DotNet\Projects\SSM\J2534Mock\Debug\";
            string sourcePath = Path.Combine(sourceDirectory, fileName);
            string destinationPath = Path.Combine(Environment.CurrentDirectory, fileName);
            File.Copy(sourcePath, destinationPath, true);
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return;
            }

            string mode = args[0];
            IPassThru implementation = null;
            if (mode.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                implementation = DynamicPassThru.GetInstance(mode);
            }
            else if (mode == "mock")
            {
                //Copy("J2534Mock.dll");
                //Copy("J2534Mock.pdb");
                implementation = new MockPassThru();
            }
            else if (mode == "op")
            {
                implementation = new OpenPort20PassThru();
            }
            else
            {
                PrintUsage();
                return;
            }

            Console.WriteLine("Opening device");
            PassThruDevice device = PassThruDevice.GetInstance(implementation);
            device.Open();

            Console.WriteLine("Opening channel");
            PassThruChannel channel = device.OpenChannel(
                PassThruProtocol.Iso9141,
                PassThruConnectFlags.Iso9141NoChecksum,
                PassThruBaudRate.Rate4800);

            Console.WriteLine("Initializing channel for SSM");
            channel.InitializeSsm();

            byte[] messageBytes = new byte[] { 0x80, 0x10, 0xF0, 0x01, 0xBF, 0x40 };
            PassThruMsg initRequestMsg = new PassThruMsg(PassThruProtocol.Iso9141);
            initRequestMsg.ProtocolID = PassThruProtocol.Iso9141;
            initRequestMsg.DataSize = (UInt32) messageBytes.Length;
            for (int i = 0; i < messageBytes.Length; i ++)
            {
                initRequestMsg.Data[i] = messageBytes[i];
            }

            //PassThruMsg[] initRequestMsgs = new PassThruMsg[] { initRequestMsg };
            //UInt32 numMsgs = 1;

            Console.WriteLine("Sending SSM init message");
            channel.WriteMessage(initRequestMsg, TimeSpan.FromMilliseconds(1000));

            PassThruMsg received = new PassThruMsg(PassThruProtocol.Iso9141);
            //PassThruMsg[] receivedMessages = new PassThruMsg[] { received };
            //UInt32 numMsgs = 1;

            Console.WriteLine("Waiting for SSM init response");
            bool success = channel.ReadMessage(
                received,
                TimeSpan.FromMilliseconds(1000));

            Console.WriteLine("ReadMessage success: " + success);

            Console.WriteLine("SSM init response:");
            string response = BitConverter.ToString(received.Data, 0, (int) received.DataSize);
            Console.WriteLine(response);

            Console.WriteLine("Closing channel");
            channel.Close();

            Console.WriteLine("Closing device");
            device.Close();
            implementation.Dispose();
        }

        private static void PrintUsage()
        {
            Console.WriteLine("J2534Exe.exe <mock|op>");
        }
    }
}
