///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// LogProfileTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    [TestClass()]
    public class LogProfileTest
    {
        private SsmBasicLogger logger;
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

        public LogProfileTest()
        {
            Utility.CopyConfigurationFiles();
        }

        [TestMethod()]
        public void LogProfileSaveAndReload()
        {
            ParameterDatabase database = this.InitializeLogger();

            LogProfile expectedProfile = LogProfile.CreateInstance();
            foreach (SsmParameter parameter in database.Parameters)
            {
                expectedProfile.Add(parameter, parameter.Conversions[0]);
                if (expectedProfile.Columns.Count > 3)
                {
                    break;
                }
            }

            this.logger.SetProfile(expectedProfile, database);
            expectedProfile.Save("profile.xml");

            LogProfile emptyProfile = LogProfile.CreateInstance();
            this.logger.SetProfile(emptyProfile, database);

            LogProfile loadedProfile = LogProfile.Load("profile.xml", database);
            this.logger.SetProfile(loadedProfile, database);

            LogProfile actualProfile = this.logger.CurrentProfile;

            foreach (LogColumn column in actualProfile.Columns)
            {
                Assert.IsTrue(expectedProfile.Contains(column.Parameter));
            }

            foreach (LogColumn column in expectedProfile.Columns)
            {
                Assert.IsTrue(actualProfile.Contains(column.Parameter));
            }
        }

        private ParameterDatabase InitializeLogger()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            this.logger = SsmBasicLogger.GetInstance(Environment.CurrentDirectory, stream);
            IAsyncResult result = logger.BeginConnect(null, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);
            Assert.AreEqual("2F12785206", this.logger.EcuIdentifier, "EcuIdentifier");
            Assert.IsNotNull(source);
            Assert.AreEqual(178, source.Parameters.Count);

            ParameterDatabase database = ParameterDatabase.GetInstance();
            database.Add(source);
            Assert.AreEqual(178, database.Parameters.Count);

            return database;
        }
    }
}
