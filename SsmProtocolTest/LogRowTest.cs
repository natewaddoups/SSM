///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// LogRowTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    [TestClass()]
    public class LogRowTest
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

        public LogRowTest()
        {
            Utility.CopyConfigurationFiles();
        }
    }
}
