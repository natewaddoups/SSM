///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmPacketTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    [TestClass()]
    public class SsmPacketTest
    {
        private TestContext testContextInstance;
        private SsmPacket packet;

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

        [DeploymentItem("SsmDisplay.exe")]
        [TestMethod()]
        public void SsmPacketEcuIdentifierRequest()
        {
            SsmPacket packet = SsmPacket.CreateEcuIdentifierRequest();
            IList<byte> expected = SamplePacketData.EcuInitRequest;                        
            Utility.CompareArrays("EcuIdentifierRequest", expected, packet.Data);
            Assert.AreEqual(SsmDirection.ToEcu, packet.Direction, "Direction");
            Assert.AreEqual(1, packet.PayloadLength, "Payload size");
            Assert.AreEqual(SsmCommand.EcuInitRequest, packet.Command, "Command");
        }

        [DeploymentItem("SsmDisplay.exe")]
        [TestMethod()]
        public void SsmPacketEcuIdentifierResponse()
        {
            SsmPacket packet = SsmPacket.CreateEcuIdentifierResponse();

            IList<byte> expected = SamplePacketData.EcuInitResponse;
            Utility.CompareArrays("EcuIdentifierResponse", expected, packet.Data);
            Assert.AreEqual(SsmDirection.FromEcu, packet.Direction, "Direction");
            Assert.AreEqual(SsmCommand.EcuInitResponse, packet.Command, "Command");
            Assert.AreEqual("2F12785206", packet.EcuIdentifier, "EcuIdentifier");

            byte[] expectedData = new byte[] { 
                0x73, 0xFA, 0xCB, 0xA6, 0x2B, 0x81, 0xFE, 0xA8, 
                0x00, 0x00, 0x00, 0x60, 0xCE, 0x54, 0xF9, 0xB1, 
                0xE4, 0x00, 0x0C, 0x20, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0xDC, 0x00, 0x00, 0x5D, 0x1F, 0x30, 0xC0, 
                0xF2, 0x26, 0x00, 0x00, 0x43, 0xFB, 0x00, 0xE1, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC1, 0xF0, 
                0x28,  
            };
            Utility.CompareArrays("CompatibilityMap", expectedData, packet.CompatibilityMap);
        }

        [DeploymentItem("SsmDisplay.exe")]
        [TestMethod()]
        public void SsmPacketMultipleReadRequest()
        {
            List<int> addresses = new List<int>();
            addresses.Add(0x000029);
            addresses.Add(0x000020);
            SsmPacket packet = SsmPacket.CreateMultipleReadRequest(addresses);
            IList<byte> expected = SamplePacketData.MultipleReadRequest;
            Utility.CompareArrays("MultipeReadRequest", expected, packet.Data);
            Assert.AreEqual(SsmDirection.ToEcu, packet.Direction, "Direction");
            Assert.AreEqual(SsmCommand.ReadAddressesRequest, packet.Command, "Command");
            Assert.AreEqual(2, packet.Addresses.Count, "Addresses.Count");
            Assert.AreEqual(0x29, packet.Addresses[0], "Addresses[0]");
            Assert.AreEqual(0x20, packet.Addresses[1], "Addresses[1]");
        }

        [DeploymentItem("SsmDisplay.exe")]
        [TestMethod()]
        public void SsmPacketMultipleReadResponse()
        {
            List<byte> values = new List<byte>();
            values.Add(0x00);
            values.Add(0x09);
            SsmPacket packet = SsmPacket.CreateMultipleReadResponse(values);
            IList<byte> expected = SamplePacketData.MultipleReadResponse;
            Utility.CompareArrays("MultipleReadResponse", expected, packet.Data);
            Assert.AreEqual(SsmDirection.FromEcu, packet.Direction, "Direction");
            Assert.AreEqual(SsmCommand.ReadAddressesResponse, packet.Command, "Command");
            Assert.AreEqual(2, packet.Values.Count, "Values.Count");
            Assert.AreEqual((byte)0x00, packet.Values[0], "Values[0]");
            Assert.AreEqual((byte)0x09, packet.Values[1], "Values[1]");
        }

        [DeploymentItem("SsmDisplay.exe")]
        [TestMethod()]
        public void SsmPacketBlockReadRequest()
        {
            int address = 0x200000;
            byte length = 0x80;
            SsmPacket packet = SsmPacket.CreateBlockReadRequest(address, length);
            IList<byte> expected = SamplePacketData.BlockReadRequest;
            Utility.CompareArrays("BlockReadRequest", expected, packet.Data);
            Assert.AreEqual(SsmDirection.ToEcu, packet.Direction, "Direction");
            Assert.AreEqual(SsmCommand.ReadBlockRequest, packet.Command, "Command");
            Assert.AreEqual(address, packet.BlockStart, "Block start");
            Assert.AreEqual(length, packet.BlockLength, "Block length");
        }

        [TestMethod()]
        public void SsmPacketBlockReadResponse()
        {
            byte[] payload = new byte[128];
            SsmPacket packet = SsmPacket.CreateBlockReadResponse(payload);
            IList<byte> expected = SamplePacketData.BlockReadResponse;
            Utility.CompareArrays("BlockReadResponse", expected, packet.Data);
            Assert.AreEqual(SsmDirection.FromEcu, packet.Direction, "Direction");
            Assert.AreEqual(SsmCommand.ReadBlockResponse, packet.Command, "Command");
        }

        [TestMethod()]
        public void SsmPacketReadFromStream()
        {
            ReadFromStream(false);
        }

        [TestMethod()]
        public void SsmPacketReadFromFragmentedStream()
        {
            ReadFromStream(true);
        }
        
        private void ReadFromStream(bool fragmented)
        {
            IList<int> addresses = new int[] { 1, 2, 3 };
            SsmPacket request = SsmPacket.CreateMultipleReadRequest(addresses);
            IList<byte> values = new byte[] { 1, 2, 3 };
            SsmPacket response = SsmPacket.CreateMultipleReadResponse(values);
            List<byte> buffer = new List<byte>(request.Data);
            buffer.AddRange(response.Data);
            MemoryStream stream = new MemoryStream(buffer.ToArray());
            
            if (fragmented)
            {
                stream = FragmentedStream.GetInstance(stream);
            }
            
            SsmPacket actual = SsmPacketParser.ReadFromStream(stream);
            Utility.CompareArrays("ReadFromStream", response.Data, actual.Data);
        }

        [TestMethod()]
        [ExpectedException(typeof(SsmPacketFormatException))]
        public void SsmPacketReadOverlongFromStream()
        {
            IList<int> addresses = new int[] { 1, 2, 3 };
            SsmPacket request = SsmPacket.CreateMultipleReadRequest(addresses);
            IList<byte> values = new byte[] { 1, 2, 3 };
            SsmPacket response = SsmPacket.CreateMultipleReadResponse(values);
            List<byte> buffer = new List<byte>(request.Data);
            buffer.AddRange(response.Data);
            buffer.AddRange(new byte[] { 0, 0, 0, 0 });
            MemoryStream stream = new MemoryStream(buffer.ToArray());

            SsmPacket actual = SsmPacketParser.ReadFromStream(stream);
            Utility.CompareArrays("ReadFromStream", response.Data, actual.Data);
        }

        [TestMethod()]
        public void SsmPacketReadFromStreamAsync()
        {
            ReadFromStreamAsync(false);
        }

        [TestMethod()]
        public void SsmPacketReadFromFragmentedStreamAsync()
        {
            ReadFromStreamAsync(true);
        }

        [TestMethod()]
        public void SsmPacketCreateArbitrary()
        {
            SsmPacket expectedReadRequest = SsmPacket.CreateBlockReadRequest(123, 12);
            SsmPacket actualReadRequest = SsmPacket.CreateArbitrary(
                0x10,
                (byte)SsmCommand.ReadBlockRequest,
                true,
                new byte[] { 0, 0, 123, 11, });
            Utility.CompareArrays("Block read request", expectedReadRequest.Data, actualReadRequest.Data);
        }

        private void ReadFromStreamAsync(bool fragmented)
        {
            IList<int> addresses = new int[] { 1, 2, 3 };
            SsmPacket request = SsmPacket.CreateMultipleReadRequest(addresses);
            IList<byte> values = new byte[] { 1, 2, 3 };
            SsmPacket response = SsmPacket.CreateMultipleReadResponse(values);
            List<byte> buffer = new List<byte>(request.Data);
            buffer.AddRange(response.Data);
            MemoryStream stream = new MemoryStream(buffer.ToArray());

            if (fragmented)
            {
                stream = FragmentedStream.GetInstance(stream);
            }

            SsmPacketParser parser = SsmPacketParser.CreateInstance();            
            IAsyncResult asyncResult = parser.BeginReadFromStream(stream, ReadCompleted, parser);
            asyncResult.AsyncWaitHandle.WaitOne();
            Utility.CompareArrays("ReadFromStreamAsync", response.Data, this.packet.Data);
        }

        private void ReadCompleted(IAsyncResult asyncResult)
        {
            SsmPacketParser parser = (SsmPacketParser) asyncResult.AsyncState;
            this.packet = parser.EndReadFromStream(asyncResult);
        }
    }
}
