///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// AsyncResultTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    [TestClass()]
    public class AsyncResultTest : LogWriterFilterTestBase
    {
        private TestContext testContextInstance;

        private const string asyncState = "asyncState";
        private int completionCallbackInvocations;
        private object completionCallbackState;

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

        public AsyncResultTest()
        {
        }

        [TestInitialize()]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod()]
        public void Complete()
        {
            this.completionCallbackInvocations = 0;
            this.completionCallbackState = null;

            AsyncResult ar = new AsyncResult(CompletionCallback, asyncState);
            ar.Completed();
            Assert.AreEqual(1, this.completionCallbackInvocations, "completionCallbackInvocations");
            bool waitResult = ar.AsyncWaitHandle.WaitOne(500, false);
            Assert.IsTrue(waitResult, "WaitOne result");
            Assert.AreSame(asyncState, this.completionCallbackState, "completionCallbackState");
        }

        [TestMethod()]
        public void CompleteTwice()
        {
            this.completionCallbackInvocations = 0;
            this.completionCallbackState = null;

            AsyncResult ar = new AsyncResult(CompletionCallback, asyncState);
            ar.Completed();
            ar.Completed();
            Assert.AreEqual(1, this.completionCallbackInvocations, "completionCallbackInvocations");
            bool waitResult = ar.AsyncWaitHandle.WaitOne(500, false);
            Assert.IsTrue(waitResult, "WaitOne result");
            Assert.AreSame(asyncState, this.completionCallbackState, "completionCallbackState");
        }


        [TestMethod()]
        public void CompletePriorToTimeout()
        {
            this.completionCallbackInvocations = 0;
            this.completionCallbackState = null;

            AsyncResult ar = new AsyncResultWithTimeout(CompletionCallback, asyncState);
            ar.Completed();
            System.Threading.Thread.Sleep(1500);
            Assert.AreEqual(1, this.completionCallbackInvocations, "completionCallbackInvocations");
            bool waitResult = ar.AsyncWaitHandle.WaitOne(500, false);
            Assert.IsTrue(waitResult, "WaitOne result");
            Assert.AreSame(asyncState, this.completionCallbackState, "completionCallbackState");
        }

        [TestMethod()]
        public void CompleteAfterTimeout()
        {
            this.completionCallbackInvocations = 0;
            this.completionCallbackState = null;

            AsyncResult ar = new AsyncResultWithTimeout(CompletionCallback, asyncState);
            System.Threading.Thread.Sleep(1500);
            ar.Completed();
            Assert.AreEqual(1, this.completionCallbackInvocations, "completionCallbackInvocations");
            bool waitResult = ar.AsyncWaitHandle.WaitOne(500, false);
            Assert.IsTrue(waitResult, "WaitOne result");
            Assert.AreSame(asyncState, this.completionCallbackState, "completionCallbackState");
        }

        private void CompletionCallback(IAsyncResult result)
        {
            this.completionCallbackInvocations++;
            this.completionCallbackState = result.AsyncState;
        }
    }
}
