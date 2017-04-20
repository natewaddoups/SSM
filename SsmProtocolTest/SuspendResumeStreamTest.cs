using NateW.Ssm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace NateW.Ssm.Protocol.Test
{
    /// <summary>
    ///This is a test class for SuspendResumeStreamTest and is intended
    ///to contain all SuspendResumeStreamTest Unit Tests
    ///</summary>
    //[TestClass()]
    public class SuspendResumeStreamTest
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

        private Stream CreateStream()
        {
            byte[] buffer = new byte[100];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte) i;
            }

            return new MemoryStream(buffer);
        }

        public void SRStreamCreate()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream);
            byte[] buffer = new byte[10];

        }


        /*
        /// <summary>
        ///A test for WriteTimeout
        ///</summary>
        [TestMethod()]
        public void WriteTimeoutTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.WriteTimeout = expected;
            actual = target.WriteTimeout;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ReadTimeout
        ///</summary>
        [TestMethod()]
        public void ReadTimeoutTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.ReadTimeout = expected;
            actual = target.ReadTimeout;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Position
        ///</summary>
        [TestMethod()]
        public void PositionTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            long expected = 0; // TODO: Initialize to an appropriate value
            long actual;
            target.Position = expected;
            actual = target.Position;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Length
        ///</summary>
        [TestMethod()]
        public void LengthTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            long actual;
            actual = target.Length;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CanWrite
        ///</summary>
        [TestMethod()]
        public void CanWriteTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CanWrite;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CanTimeout
        ///</summary>
        [TestMethod()]
        public void CanTimeoutTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CanTimeout;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CanSeek
        ///</summary>
        [TestMethod()]
        public void CanSeekTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CanSeek;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CanRead
        ///</summary>
        [TestMethod()]
        public void CanReadTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CanRead;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Write
        ///</summary>
        [TestMethod()]
        public void WriteTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            byte[] buffer = null; // TODO: Initialize to an appropriate value
            int offset = 0; // TODO: Initialize to an appropriate value
            int count = 0; // TODO: Initialize to an appropriate value
            target.Write(buffer, offset, count);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetLength
        ///</summary>
        [TestMethod()]
        public void SetLengthTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            long value = 0; // TODO: Initialize to an appropriate value
            target.SetLength(value);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Seek
        ///</summary>
        [TestMethod()]
        public void SeekTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            long offset = 0; // TODO: Initialize to an appropriate value
            SeekOrigin origin = new SeekOrigin(); // TODO: Initialize to an appropriate value
            long expected = 0; // TODO: Initialize to an appropriate value
            long actual;
            actual = target.Seek(offset, origin);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Read
        ///</summary>
        [TestMethod()]
        public void ReadTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            byte[] buffer = null; // TODO: Initialize to an appropriate value
            int offset = 0; // TODO: Initialize to an appropriate value
            int count = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.Read(buffer, offset, count);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Flush
        ///</summary>
        [TestMethod()]
        public void FlushTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            target.Flush();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for EndWrite
        ///</summary>
        [TestMethod()]
        public void EndWriteTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            IAsyncResult asyncResult = null; // TODO: Initialize to an appropriate value
            target.EndWrite(asyncResult);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for EndRead
        ///</summary>
        [TestMethod()]
        public void EndReadTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            IAsyncResult asyncResult = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.EndRead(asyncResult);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NateW.Ssm.Protocol.dll")]
        public void DisposeTest()
        {
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
            //PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            //SuspendResumeStream_Accessor target = new SuspendResumeStream_Accessor(param0); // TODO: Initialize to an appropriate value
            //bool disposing = false; // TODO: Initialize to an appropriate value
            //target.Dispose();            
        }

        /// <summary>
        ///A test for Close
        ///</summary>
        [TestMethod()]
        public void CloseTest()
        {
            SuspendResumeStream target = new SuspendResumeStream(this.CreateStream); // TODO: Initialize to an appropriate value
            target.Close();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
         */
    }
}
