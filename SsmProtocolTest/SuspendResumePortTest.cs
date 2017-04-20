using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    /// <summary>
    ///This is a test class for SuspendResumePortTest and is intended
    ///to contain all SuspendResumePortTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SuspendResumePortTest
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///</summary>
        [TestMethod()]
        public void SuspendResumPortOpenClose()
        {
            SuspendResumePort port = new SuspendResumePort(
                delegate { return new SerialPort(); },
                delegate(Stream stream) { });
            port.Open();
            port.Close();
            Assert.AreEqual(1, port.Opened, "port.Open");
        }

        [TestMethod()]
        public void SuspendResumPortOpenRetryClose()
        {
            int invocations = 0;

            SuspendResumePort port = new SuspendResumePort(
                delegate
                {
                    invocations++;
                    if (invocations == 2)
                    {
                        return new SerialPort();
                    }
                    return null;
                },
                delegate(Stream stream) { });

            port.Open();

            Thread.Sleep(3000);

            port.Close();
            Assert.AreEqual(1, port.Opened, "port.Open");
        }

        [TestMethod()]
        public void SuspendResumPortOpenSuspendResumeClose()
        {
            SuspendResumePort port = new SuspendResumePort(
                delegate { return new SerialPort(); },
                delegate(Stream stream) { });
            port.Open();
            port.Suspend();
            port.Resume(TimeSpan.FromSeconds(0.01));
            Thread.Sleep(500);
            port.Close();
            Assert.AreEqual(2, port.Opened, "port.Open");
        }

    }
}
