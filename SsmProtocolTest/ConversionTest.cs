///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// ConversionTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using NateW.Ssm;

namespace SsmLoggerTest
{
    [TestClass()]
    public class ConversionTest
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

        [TestMethod()]
        public void ConvertX()
        {
            Conversion conversion = Conversion.GetInstance("units", "x", "0.00");
            string actualString;
            double actualDouble;
            conversion.Convert(1.23, out actualString, out actualDouble);
            Assert.AreEqual("1.23", actualString);
            Assert.AreEqual(1.23D, actualDouble);

            conversion = Conversion.GetInstance("units", "x", "0");
            conversion.Convert(1, out actualString, out actualDouble);
            Assert.AreEqual("1", actualString);
            Assert.AreEqual(1D, actualDouble);
        }

        [TestMethod()]
        public void ConvertXOver4()
        {
            Conversion conversion = Conversion.GetInstance("units", "x/4", "0.00");
            string actualString;
            double actualDouble;
            conversion.Convert(17, out actualString, out actualDouble);
            Assert.AreEqual("4.25", actualString);
            Assert.AreEqual(4.25D, actualDouble);
        }

        [TestMethod()]
        public void ConvertBoolean()
        {
            Conversion conversion = Conversion.GetInstance(Conversion.Boolean, "x&4", "");
            string actualString;
            double actualDouble;
            conversion.Convert(0, out actualString, out actualDouble);
            Assert.AreEqual(Conversion.BooleanFalse, actualString);
            Assert.AreEqual(0F, actualDouble);

            conversion = Conversion.GetInstance(Conversion.Boolean, "x&4", "");
            conversion.Convert(4, out actualString, out actualDouble);
            Assert.AreEqual(Conversion.BooleanTrue, actualString);
            Assert.AreEqual(1F, actualDouble);

            conversion = Conversion.GetInstance(Conversion.Boolean, "x&4", "");
            conversion.Convert(5, out actualString, out actualDouble);
            Assert.AreEqual(Conversion.BooleanTrue, actualString);
            Assert.AreEqual(1F, actualDouble);

            conversion = Conversion.GetInstance(Conversion.Boolean, "x&4", "");
            conversion.Convert(1, out actualString, out actualDouble);
            Assert.AreEqual(Conversion.BooleanFalse, actualString);
            Assert.AreEqual(0F, actualDouble);
        }        
    }
}
