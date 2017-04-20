///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// InternalLogProfileTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    [TestClass()]
    public class InternalLogProfileTest
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

        public InternalLogProfileTest()
        {
            Utility.CopyConfigurationFiles();
        }

        [TestMethod()]
        public void InternalLogProfileNoParameters()
        {
            ParameterDatabase database = ParameterDatabase.GetInstance();
            MockParameterSource source = new MockParameterSource();
            database.Add(source);

            LogProfile publicProfile = LogProfile.CreateInstance();
            InternalLogProfile internalProfile = InternalLogProfile.GetInstance(publicProfile, database);

            Assert.AreEqual(0, internalProfile.Addresses.Count);
        }

        [TestMethod()]
        public void InternalLogProfileMockParameters()
        {
            ParameterDatabase database = ParameterDatabase.GetInstance();
            MockParameterSource source = new MockParameterSource();
            database.Add(source);

            LogProfile publicProfile = LogProfile.CreateInstance();
            publicProfile.Add(database.Parameters[0], database.Parameters[0].Conversions[0]);
            publicProfile.Add(database.Parameters[1], database.Parameters[1].Conversions[0]);
            InternalLogProfile internalProfile = InternalLogProfile.GetInstance(publicProfile, database);

            Assert.AreEqual(0, internalProfile.Addresses.Count);
            Assert.AreEqual(2, internalProfile.LogEventArgs.Row.Columns.Count);
        }

        [TestMethod()]
        public void InternalLogProfileMockCalculatedParameter()
        {
            ParameterDatabase database = ParameterDatabase.GetInstance();
            MockParameterSource source = new MockParameterSource();
            database.Add(source);

            LogProfile publicProfile = LogProfile.CreateInstance();
            publicProfile.Add(database.Parameters[2], database.Parameters[2].Conversions[0]);
            InternalLogProfile internalProfile = InternalLogProfile.GetInstance(publicProfile, database);

            Assert.AreEqual(0, internalProfile.Addresses.Count);
            Assert.AreEqual(3, internalProfile.LogEventArgs.Row.Columns.Count);
            Assert.AreEqual(2, internalProfile.LogEventArgs.Row.Columns[0].DependencyMap.Count);
            Assert.AreEqual("M1", internalProfile.LogEventArgs.Row.Columns[1].Parameter.Id);
            Assert.AreEqual("M2", internalProfile.LogEventArgs.Row.Columns[2].Parameter.Id);
        }

        [TestMethod()]
        public void InternalLogProfileSsmAddressesOneParameter()
        {
            ParameterDatabase database = ParameterDatabase.GetInstance();
            SsmParameterSource source = SsmParameterSource.GetInstance(
                Environment.CurrentDirectory,
                SsmParameterSourceTest.EcuIdentifier,
                SsmParameterSourceTest.CompatibilityMap); 
            database.Add(source);

            LogProfile publicProfile = LogProfile.CreateInstance();
            SsmParameter parameter = database.Parameters[0] as SsmParameter;
            publicProfile.Add(parameter, parameter.Conversions[0]);

            InternalLogProfile internalProfile = InternalLogProfile.GetInstance(publicProfile, database);
            
            Assert.AreEqual(1, internalProfile.Addresses.Count);
            Assert.AreEqual(parameter.Address, internalProfile.Addresses[0]);
            Assert.AreEqual(1, internalProfile.LogEventArgs.Row.Columns.Count);
            LogColumn column = internalProfile.LogEventArgs.Row.Columns[0];
            Assert.AreEqual(column.PropertyBag[InternalLogProfile.ColumnAddressIndex], 0);            
        }

    }
}
