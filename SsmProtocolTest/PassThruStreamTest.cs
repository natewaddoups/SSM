///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// PassThruStreamTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    /// <summary>
    /// Tests for the PassThruStream class.
    /// </summary>
    [TestClass()]
    public class PassThruStreamTest
    {
        private string ecuIdentifier;

        public PassThruStreamTest()
        {
            Utility.CopyJ2534Mock();
        }

        [TestMethod()]
        public void PassThruStreamEcuInit()
        {
            Assert.Inconclusive("Not implemented.");
            /*
            PassThruStream stream = PassThruStream.GetInstance("Mock");
            stream.OpenSsmChannel();
            SsmInterface ssm = SsmInterface.GetInstance(stream);
            this.ecuIdentifier = null;
            IAsyncResult asyncResult = ssm.BeginGetEcuIdentifier(GetEcuIdentifierCallack, ssm);
            asyncResult.AsyncWaitHandle.WaitOne();
            Assert.AreEqual("2F12785206", this.ecuIdentifier, "EcuIdentifier");*/
        }

        private void GetEcuIdentifierCallack(IAsyncResult asyncResult)
        {
            SsmInterface ssm = (SsmInterface)asyncResult.AsyncState;
            ssm.EndGetEcuIdentifier(asyncResult);
            this.ecuIdentifier = ssm.EcuIdentifier;
        }
    }
}
