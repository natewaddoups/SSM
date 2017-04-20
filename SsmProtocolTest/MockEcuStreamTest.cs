///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// MockEcuStreamTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    /// <summary>
    ///This is a test class for NateW.Ssm.SsmPacket and is intended
    ///to contain all NateW.Ssm.SsmPacket Unit Tests
    ///</summary>
    [TestClass()]
    public class MockEcuStreamTest
    {
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

        /// <summary>
        ///A test for CreateEcuIdentifierRequest ()
        ///</summary>
        [TestMethod()]
        public void MockEcuStreamEcuIdentifier()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            SsmPacket send = SsmPacket.CreateEcuIdentifierRequest();
            SsmPacket receive = SsmPacket.CreateEcuIdentifierResponse();
            MockEcuStreamTest.SendReceive("EcuId", stream, send, receive);
        }

        [TestMethod]
        public void MockEcuStreamBlockRead()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            SsmPacket send = SsmPacket.CreateBlockReadRequest(0x200000, 128);
            byte[] payload = new byte[128];
            SsmPacket receive = SsmPacket.CreateBlockReadResponse(payload);
            MockEcuStreamTest.SendReceive("Read", stream, send, receive);            
        }

        [TestMethod()]
        public void MockEcuStreamMultipleAddressRead()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            List<int> addresses = new List<int>();
            addresses.Add(0x000029);
            addresses.Add(0x000020);
            SsmPacket send = SsmPacket.CreateMultipleReadRequest(addresses);
            
            List<byte> payload = new List<byte>();
            payload.Add(127);
            payload.Add(39);
            SsmPacket receive = SsmPacket.CreateMultipleReadResponse(payload);
            MockEcuStreamTest.SendReceive("Read", stream, send, receive);
        }

        [TestMethod()]
        public void MockEcuStreamDefoggerTest()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();

            stream.DefoggerSwitch = false;
            SsmPacket send = SsmPacket.CreateBlockReadRequest(0x64, 1);
            byte[] payload = new byte[1];
            SsmPacket receive = SsmPacket.CreateBlockReadResponse(payload);
            MockEcuStreamTest.SendReceive("Defogger off", stream, send, receive);

            stream.DefoggerSwitch = true;
            send = SsmPacket.CreateBlockReadRequest(0x64, 1);
            payload = new byte[1];
            payload[0] = 1 << 5;
            receive = SsmPacket.CreateBlockReadResponse(payload);
            MockEcuStreamTest.SendReceive("Defogger on", stream, send, receive);

            stream.DefoggerSwitch = false;
            send = SsmPacket.CreateBlockReadRequest(0x64, 1);
            payload = new byte[1];
            receive = SsmPacket.CreateBlockReadResponse(payload);
            MockEcuStreamTest.SendReceive("Defogger off again", stream, send, receive);
        }

        private static void SendReceive(string message, MockEcuStream stream, SsmPacket send, SsmPacket receive)
        {
            byte[] buffer = send.Data;
            stream.Write(buffer, 0, buffer.Length);

            List<byte> expectedList = new List<byte>(send.Data);
            expectedList.AddRange(receive.Data);
            byte[] expected = expectedList.ToArray();
            buffer = new byte[expected.Length];
            stream.Read(buffer, 0, buffer.Length);

            Utility.CompareArrays(message, expected, buffer);
        }
    }
}
