///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// Program.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using NateW.Ssm;

namespace SsmSystemTest
{
    class Program
    {
        const string captureFileName = "capture.txt";

        static void Main(string[] args)
        {
            bool capture = true;
            //bool mock = true;
            bool mock = false;
            SerialPort port = null;
            Stream ecuStream;
            
            if (mock)
            {
                ecuStream = MockEcuStream.GetInstance();
            }
            else
            {
                port = new SerialPort("COM3", 4800, Parity.None, 8, StopBits.One);
                port.Open();
                port.ReadTimeout = 1000;
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                ecuStream = port.BaseStream;
            }

            if (capture)
            {
                Stream captureStream = File.Open(captureFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                captureStream.SetLength(0);
                ecuStream = new CaptureStream(ecuStream, new StreamWriter(captureStream));
            }

            ManualLoggingTest(ecuStream, port);
            //ReadMultipleTest(ecuStream, port);
        }

        static void ManualLoggingTest(Stream ecuStream, SerialPort port)
        {
            SsmLogger logger = SsmLogger.GetInstance(Environment.CurrentDirectory, ecuStream);
            IAsyncResult asyncResult = logger.BeginConnect(ConnectCallback, logger);
            asyncResult.AsyncWaitHandle.WaitOne();

            LogProfile profile = LogProfile.GetInstance();
            foreach (SsmParameter parameter in logger.Database.Parameters)
            {
                switch (parameter.Name)
                {
                    case "Engine Load":
                    case "Manifold Relative Pressure":
                    case "Engine Speed":
                        profile.Add(parameter, parameter.Conversions[0]);
                        break;
                }
            }
                         
            logger.SetProfile(profile);
            logger.LogStart += LogStart;
            logger.LogEntry += LogEntry;
            logger.LogStop += LogEnd;
            logger.StartLogging();
            for (int i = 0; i < 100000 && !Console.KeyAvailable; i++)
            {
                Thread.Sleep(1);
            }
            logger.StopLogging();
            Thread.Sleep(500);
        }

        static void ReadMultipleTest(Stream ecuStream, SerialPort port)
        {
            SsmPacket ecuInitRequest = SsmPacket.CreateEcuIdentifierRequest();
            byte[] buffer = ecuInitRequest.Data;
            ecuStream.Write(buffer, 0, buffer.Length);

            Thread.Sleep(100);
                
            byte[] receiveBuffer = new byte[1000];
            int expectedLength = 68;
            int receiveLength = ecuStream.Read(receiveBuffer, 0, receiveBuffer.Length);
            SsmPacket ecuInitResponse = SsmPacket.ParseResponse(receiveBuffer, 0, receiveLength);

            // TPS and IPW
            UInt32[] addresses = new UInt32[] { 0x29, 0x20 };

            for (int i = 0; i < 1000 && !Console.KeyAvailable; i++)
            {
                SsmPacket readRequest = SsmPacket.CreateMultipleReadRequest(addresses);
                buffer = readRequest.Data;
                ecuStream.Write(buffer, 0, buffer.Length);

                Thread.Sleep(100);
                expectedLength = 21;
                receiveLength = ecuStream.Read(receiveBuffer, 0, expectedLength);
                if (!Check("ReceiveLength", receiveLength, expectedLength))
                {
                    if (port != null)
                    {
                        port.DiscardInBuffer();
                        port.DiscardOutBuffer();
                    }
                    continue;
                }

                SsmPacket readResponse = SsmPacket.ParseResponse(receiveBuffer, 0, receiveLength);
                if (!Check("CommandID", readResponse.Command, SsmCommand.ReadAddressesResponse))
                {
                    if (port != null)
                    {
                        port.DiscardInBuffer();
                        port.DiscardOutBuffer();
                    }
                    continue;
                }
                Console.WriteLine(readResponse.Values[0] + " " + readResponse.Values[1]);
            }
        }

        private static bool Check<T>(string message, T actual, T expected)
        {
            if (!actual.Equals(expected))
            {
                Console.WriteLine(message + ": expected " + expected + " actual " + actual);
                return false;
            }

            return true;
        }
        
        private static void ConnectCallback(IAsyncResult asyncResult)
        {
            SsmLogger logger = (SsmLogger)asyncResult.AsyncState;
            logger.EndConnect(asyncResult);
        }

        private static void LogStart(object sender, LogEventArgs args)
        {
            Console.Write("Staring log: ");
            foreach (LogColumn column in args.Row.Columns)
            {
                Console.Write(column.Parameter.Name);
                Console.Write("   ");
            }
            Console.WriteLine();
        }

        private static void LogEntry(object sender, LogEventArgs args)
        {
            foreach (LogColumn column in args.Row.Columns)
            {
                Console.Write(column.Value);
                Console.Write("   ");
            }
            Console.WriteLine();
        }

        private static void LogEnd(object sender, EventArgs args)
        {
            Console.WriteLine("Log End.");
        }

        private static void LogError(object sender, LogErrorEventArgs args)
        {
            Console.WriteLine("LogError: " + args.Exception.ToString());
        }
    }
}
