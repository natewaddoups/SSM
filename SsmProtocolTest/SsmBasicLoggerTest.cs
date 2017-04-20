///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmBasicLoggerTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    [TestClass()]
    public class SsmBasicLoggerTest
    {
        private TestContext testContextInstance;
        private SsmBasicLogger logger;
        private int logStartCalls;
        private int logEntryCalls;
        private int logEntryCallsAfterChange;
        private int logEndCalls;
        private int logErrorCalls;

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

        public SsmBasicLoggerTest()
        {
            Utility.CopyConfigurationFiles();
        }

        [TestMethod()]
        public void BasicLoggerConnect()
        {
            MockEcuStream mock = MockEcuStream.CreateInstance();
            FragmentedStream stream = FragmentedStream.GetInstance(mock);
            this.logger = SsmBasicLogger.GetInstance(Environment.CurrentDirectory, stream);
            IAsyncResult result = logger.BeginConnect(null, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);
            Assert.AreEqual("2F12785206", this.logger.EcuIdentifier, "EcuIdentifier");
            Assert.IsNotNull(source);
            Assert.IsNotNull(source.Parameters);
        }

        [TestMethod()]
        public void BasicLoggerManualLogging()
        {
            ManualLoggingTest(delegate (ParameterDatabase unused) { });

            Console.WriteLine("LogEntry calls: " + this.logEntryCalls);
            Assert.AreEqual(1, this.logStartCalls, "LogStart calls");
            Assert.IsTrue(this.logEntryCalls > 1, "LogEntry calls > 1");
            Assert.AreEqual(1, this.logEndCalls, "LogEnd calls");
            Assert.AreEqual(0, this.logErrorCalls, "LogError calls");
        }

        [TestMethod()]
        public void BasicLoggerManualLoggingChangeProfile()
        {
            ManualLoggingTest(delegate(ParameterDatabase database)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.1));
                int entriesBefore = this.logEntryCalls;

                LogProfile newProfile = LogProfile.CreateInstance();
                foreach (SsmParameter parameter in database.Parameters)
                {
                    switch (parameter.Name)
                    {
                        case "Injector Duty Cycle":
                        case "Manifold Rel. Pressure (Corrected)":
                            newProfile.Add(parameter, parameter.Conversions[0]);
                            break;
                    }
                }

                this.logger.SetProfile(newProfile, database);

                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5));
                int entriesAfter = this.logEntryCalls;
                this.logEntryCallsAfterChange = entriesAfter - entriesBefore;
            });

            Console.WriteLine("LogEntry calls: " + this.logEntryCalls);
            Assert.AreEqual(2, this.logStartCalls, "LogStart calls");
            Assert.IsTrue(this.logEntryCalls > 1, "LogEntry calls > 1");
            Assert.IsTrue(this.logEntryCallsAfterChange > 3, "LogEntry calls after change");
            Assert.AreEqual(2, this.logEndCalls, "LogEnd calls");
            Assert.AreEqual(0, this.logErrorCalls, "LogError calls");
        }

        public void ManualLoggingTest(Action<ParameterDatabase> callback)
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            FragmentedStream fragStream = FragmentedStream.GetInstance(stream);
            this.logger = SsmBasicLogger.GetInstance(Environment.CurrentDirectory, fragStream);
            IAsyncResult result = logger.BeginConnect(null, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);

            ParameterDatabase database = ParameterDatabase.GetInstance();
            database.Add(source);

            LogProfile Profile = LogProfile.CreateInstance();
            foreach (SsmParameter parameter in database.Parameters)
            {
                Profile.Add(parameter, parameter.Conversions[0]);
                if (Profile.Columns.Count > 3)
                {
                    break;
                }
            }

            this.logStartCalls = 0;
            this.logEntryCalls = 0;
            this.logEndCalls = 0;
            this.logErrorCalls = 0;

            this.logger.SetProfile(Profile, database);
            this.logger.LogStart += this.LogStart;
            this.logger.LogEntry += this.LogEntry;
            this.logger.LogStop += this.LogStop;
            this.logger.LogError += this.LogError;

            this.logger.StartLogging();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.25));
            callback(database);
            this.logger.BeginStopLogging(NoOp, null);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.1));
        }

        private void NoOp(object state)
        {
        }

        private void LogStart(object sender, LogEventArgs args)
        {
            this.logStartCalls++;
        }

        private void LogEntry(object sender, LogEventArgs args)
        {
            this.logEntryCalls++;
        }

        private void LogStop(object sender, EventArgs args)
        {
            this.logEndCalls++;
        }

        private void LogError(object sender, LogErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            this.logErrorCalls++;
        }

        [TestMethod()]
        public void BasicLoggerProfile()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            this.logger = SsmBasicLogger.GetInstance(Environment.CurrentDirectory, stream);
            IAsyncResult result = logger.BeginConnect(null, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);

            ParameterDatabase database = ParameterDatabase.GetInstance();
            database.Add(source);

            LogProfile expectedProfile = LogProfile.CreateInstance();
            foreach (SsmParameter parameter in database.Parameters)
            {
                expectedProfile.Add(parameter, parameter.Conversions[0]);
                if (expectedProfile.Columns.Count > 3)
                {
                    break;
                }
            }
            logger.SetProfile(expectedProfile, database);
            LogProfile actualProfile = logger.CurrentProfile;

            Assert.AreEqual(expectedProfile.Columns.Count, actualProfile.Columns.Count, "Actual count and expected count");
            foreach (LogColumn expectedColumn in expectedProfile.Columns)
            {
                Assert.IsTrue(actualProfile.Contains(expectedColumn.Parameter), "Actual expected parameter set is missing something");
            }
            foreach (LogColumn actualColumn in actualProfile.Columns)
            {
                Assert.IsTrue(expectedProfile.Contains(actualColumn.Parameter), "Actual expected parameter set contains something extra");
            }
        }

        [TestMethod()]
        public void BasicLoggerAddresses()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            SsmBasicLogger logger = SsmBasicLogger.GetInstance(Environment.CurrentDirectory, stream);
            IAsyncResult result = logger.BeginConnect(null, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);

            ParameterDatabase database = ParameterDatabase.GetInstance();
            database.Add(source);

            LogProfile profile = LogProfile.CreateInstance();            
            foreach (SsmParameter parameter in database.Parameters)
            {
                profile.Add(parameter, parameter.Conversions[0]);
                if (profile.Columns.Count == 8)
                {
                    break;
                }
            }
            logger.SetProfile(profile, database);
            IList<int> actual = logger.Addresses;

            // Note that parameters get re-ordered alphabetically
            IList<int> expected = new int[]
            {
                9, 10, 8, 14, 15, 17, 18, 13, 16,
            };

            Assert.AreEqual(expected.Count, actual.Count, "Addresses.Length");
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i], "Addresses[" + i + "]");
            }
        }

        [TestMethod()]
        public void BasicLoggerAddressesDuplicates()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            this.logger = SsmBasicLogger.GetInstance(Environment.CurrentDirectory, stream);
            IAsyncResult result = logger.BeginConnect(null, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);

            ParameterDatabase database = ParameterDatabase.GetInstance();
            database.Add(source);
            LogProfile profile = LogProfile.CreateInstance();
            Parameter parameter;            

            database.TryGetParameterById("P8", out parameter); // engine speed (14)
            profile.Add(parameter, parameter.Conversions[0]);
            database.TryGetParameterById("P201", out parameter); // IDC, requires engine speed (14), IPW (32)
            profile.Add(parameter, parameter.Conversions[0]);
            database.TryGetParameterById("P202", out parameter); // MRP(corrected), requires MAP (13), Atmo (35)
            profile.Add(parameter, parameter.Conversions[0]);
            database.TryGetParameterById("P7", out parameter); // MAP (13)
            profile.Add(parameter, parameter.Conversions[0]);

            // IPW is not in the resulting set of addresses, why?
            this.logger.SetProfile(profile, database);

            List<int> addresses = this.logger.Addresses;
            Assert.AreEqual(5, addresses.Count);
            int[] expected = new int[] { 14, 15, 13, 32, 35 };
            Utility.CompareArrays("Address arrays", expected, addresses.ToArray());
        }

        [TestMethod()]
        public void BasicLoggerDependencyConversions()
        {
            MockEcuStream stream = MockEcuStream.CreateInstance();
            this.logger = SsmBasicLogger.GetInstance(Environment.CurrentDirectory, stream);
            IAsyncResult result = logger.BeginConnect(null, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);

            ParameterDatabase database = ParameterDatabase.GetInstance();
            database.Add(source);

            LogProfile baseParameters = LogProfile.CreateInstance();
            foreach (SsmParameter parameter in database.Parameters)
            {
                if (parameter.Id == "P201")
                {
                    baseParameters.Add(parameter, parameter.Conversions[0]);
                }

                if (parameter.Id == "P202")
                {
                    baseParameters.Add(parameter, parameter.Conversions[0]);
                    break;
                }
            }

            this.logger.SetProfile(baseParameters, database);

            LogEventArgs args = this.logger.GetOneRow();

            Utility.AssertColumnParameterId(args, 0, "P201");
            Utility.AssertColumnParameterId(args, 1, "P202");
            Utility.AssertColumnParameterId(args, 2, "P8");
            Utility.AssertColumnParameterId(args, 3, "P21");
            Utility.AssertColumnParameterId(args, 4, "P7");
            Utility.AssertColumnParameterId(args, 5, "P24");
            
            Assert.AreEqual(6, args.Row.Columns.Count);

            List<int> addresses = this.logger.Addresses;
            Assert.AreEqual(5, addresses.Count);
            int[] expected = new int[] { 14, 15, 32, 13, 35 };
            Utility.CompareArrays("Address arrays", expected, addresses.ToArray());

            // Values from observation...  
            Assert.AreEqual("2.08", args.Row.Columns[0].ValueAsString);
            Assert.AreEqual("1.02", args.Row.Columns[1].ValueAsString);
        }
    }
}
