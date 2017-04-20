///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmInterfaceTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using NateW.Ssm;

namespace SsmDisplayTest
{
    /// <summary>
    /// Tests for the SsmInterface class.
    ///</summary>
    [TestClass()]
    public class SsmInterfaceTest
    {
        private TestContext testContextInstance;
        private string ecuIdentifier;
        private byte[] values;
        
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
        ///A test for CreateEcuInitRequest ()
        ///</summary>
        [TestMethod()]
        public void SsmInterfaceGetEcuIdentifier()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            SsmInterface ssm = SsmInterface.GetInstance(stream);
            this.ecuIdentifier = null;
            IAsyncResult asyncResult = ssm.BeginGetEcuIdentifier(GetEcuIdentifierCallack, ssm);
            asyncResult.AsyncWaitHandle.WaitOne();
            Assert.AreEqual("2F12785206", this.ecuIdentifier, "EcuIdentifier");
        }

        private void GetEcuIdentifierCallack(IAsyncResult asyncResult)
        {
            SsmInterface ssm = (SsmInterface) asyncResult.AsyncState;
            ssm.EndGetEcuIdentifier(asyncResult);
            this.ecuIdentifier = ssm.EcuIdentifier;
        }

        [TestMethod()]
        public void SsmInterfaceMultipleRead()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            SsmInterface ssm = SsmInterface.GetInstance(stream);
            IList<int> addresses = new List<int>();
            addresses.Add(0x29);
            addresses.Add(0x20);
            this.values = null;
            IAsyncResult asyncResult = ssm.BeginMultipleRead(addresses, MultipleReadCallack, ssm);
            asyncResult.AsyncWaitHandle.WaitOne();
            Assert.AreEqual(2, this.values.Length, "Values.Count");
            Assert.AreEqual(127, values[0], "Values[0]");
            Assert.AreEqual(39, values[1], "Values[1]");
        }

        private void MultipleReadCallack(IAsyncResult asyncResult)
        {
            SsmInterface ssm = (SsmInterface)asyncResult.AsyncState;
            this.values = ssm.EndMultipleRead(asyncResult);
        }

        [TestMethod]
        public void SsmInterfaceSyncReadMultiple()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            SsmInterface ssm = SsmInterface.GetInstance(stream);
            List<int> addresses = new List<int>();
            addresses.Add(0x000029);
            addresses.Add(0x000020);
            byte[] values = ssm.SyncReadMultiple(addresses);
            Assert.AreEqual(values.Length, addresses.Count, "Values.Count");
            Assert.AreEqual(127, values[0], "Values[0]");
            Assert.AreEqual(39,  values[1], "Values[1]");
        }

        [TestMethod()]
        public void SsmInterfaceBlockRead()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            SsmInterface ssm = SsmInterface.GetInstance(stream);
            IAsyncResult asyncResult = ssm.BeginBlockRead(0, 200, BlockReadCallack, ssm);
            asyncResult.AsyncWaitHandle.WaitOne();
            Assert.AreEqual(200, this.values.Length, "Values.Count");
            Assert.AreEqual(0,   values[0],   "Values[0]");
            Assert.AreEqual(1,   values[1],   "Values[1]");
            Assert.AreEqual(198, values[198], "Values[198]");
            Assert.AreEqual(199, values[199], "Values[199]");
        }

        private void BlockReadCallack(IAsyncResult asyncResult)
        {
            SsmInterface ssm = (SsmInterface)asyncResult.AsyncState;
            this.values = ssm.EndBlockRead(asyncResult);
        }

        [TestMethod]
        public void SsmInterfaceSyncReadBlock()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            SsmInterface ssm = SsmInterface.GetInstance(stream);
            List<int> addresses = new List<int>();
            byte[] values = ssm.SyncReadBlock(0, 16);
            Assert.AreEqual(values.Length, 16, "Values.Count");
            Assert.AreEqual(0x00, values[0], "Values[0]");
            Assert.AreEqual(0x01, values[1], "Values[1]");
            Assert.AreEqual(0x02, values[2], "Values[2]");
            Assert.AreEqual(0x03, values[3], "Values[3]");
            Assert.AreEqual(0x0c, values[12], "Values[12]");
            Assert.AreEqual(0x6C, values[13], "Values[13]");
            Assert.AreEqual(0x03, values[14], "Values[14]");
            Assert.AreEqual(0xE8, values[15], "Values[15]");
        }
    }
}
