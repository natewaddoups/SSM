///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// CaptureStreamTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using NateW.Ssm;

namespace SsmDisplayTest
{
    /// <summary>
    ///This is a test class for NateW.Ssm.CaptureStream and is intended
    ///to contain all NateW.Ssm.CaptureStream Unit Tests
    ///</summary>
    [TestClass()]
    public class CaptureStreamTest
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
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Read (byte[], int, int)
        ///</summary>
        [TestMethod()]
        public void CaptureStreamRead()
        {
            Stream memoryStream = new MemoryStream();
            TextWriter log = new StreamWriter(memoryStream);
            Stream dataStream = new MemoryStream();
            dataStream.Write(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
            dataStream.Position = 0;
            CaptureStream target = new CaptureStream(dataStream, log);

            byte[] buffer = new byte[5];
            int offset = 0;
            int count = buffer.Length;
            target.Read(buffer, offset, count);

            memoryStream.Position = 0;
            TextReader reader = new StreamReader(memoryStream);
            string actual = reader.ReadToEnd();
            string expected = "Read  0x01, 0x02, 0x03, 0x04, 0x05, " + Environment.NewLine;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Write (byte[], int, int)
        ///</summary>
        [TestMethod()]
        public void CaptureStreamWrite()
        {
            Stream memoryStream = new MemoryStream();
            TextWriter log = new StreamWriter(memoryStream);
            Stream dataStream = new MemoryStream();
            CaptureStream target = new CaptureStream(dataStream, log);
            byte[] buffer = new byte[5] { 0x01, 0x02, 0x03, 0x04, 0x05 };
            
            int offset = 0;
            int count = buffer.Length;
            target.Write(buffer, offset, count);
            
            memoryStream.Position = 0;
            TextReader reader = new StreamReader(memoryStream);
            string actual = reader.ReadToEnd();
            string expected = "Write 0x01, 0x02, 0x03, 0x04, 0x05, " + Environment.NewLine;
            Assert.AreEqual(expected, actual);
        }

    }


}
