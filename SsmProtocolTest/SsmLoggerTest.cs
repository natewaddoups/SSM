///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmLoggerTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    [TestClass()]
    public class SsmLoggerTest
    {
        public delegate void VoidVoid();
        private TestContext testContextInstance;
        private SsmLogger logger;
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

        public SsmLoggerTest()
        {
            Utility.CopyConfigurationFiles();
        }

        [TestMethod()]
        public void LoggerConnect()
        {
            MockEcuStream mock = MockEcuStream.CreateInstance();
            this.logger = SsmLogger.GetInstance(Environment.CurrentDirectory, MockEcuStream.PortName);
            IAsyncResult result = logger.BeginConnect(ConnectCallback, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);
            ParameterDatabase database = ParameterDatabase.GetInstance();
            database.Add(source);

            Assert.AreEqual("2F12785206", this.logger.EcuIdentifier, "EcuIdentifier");
            Assert.IsNotNull(database);
            Assert.IsNotNull(database.Parameters);
            Assert.AreEqual(database.Parameters.Count, 178);
        }

        private void ConnectCallback(IAsyncResult asyncResult)
        {
            this.logger.EndConnect(asyncResult);
        }

        [TestMethod()]
        public void LoggerManualLogging()
        {
            ManualLoggingTest(delegate(ParameterDatabase unused) { });

            Console.WriteLine("LogEntry calls: " + this.logEntryCalls);
            Assert.AreEqual(1, this.logStartCalls, "LogStart calls");
            Assert.IsTrue(this.logEntryCalls > 1, "LogEntry calls > 1");
            Assert.AreEqual(1, this.logEndCalls, "LogEnd calls");
            Assert.AreEqual(0, this.logErrorCalls, "LogError calls");
        }

        [TestMethod()]
        public void LoggerManualLoggingChangeProfile()
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
            this.logger = SsmLogger.GetInstance(Environment.CurrentDirectory, MockEcuStream.PortName);
            IAsyncResult result = logger.BeginConnect(null, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);

            ParameterDatabase database = ParameterDatabase.GetInstance();
            database.Add(source);

            LogProfile profile = LogProfile.CreateInstance();
            foreach (SsmParameter parameter in database.Parameters)
            {
                profile.Add(parameter, parameter.Conversions[0]);
                if (profile.Columns.Count > 3)
                {
                    break;
                }
            }

            this.logStartCalls = 0;
            this.logEntryCalls = 0;
            this.logEndCalls = 0;
            this.logErrorCalls = 0;

            this.logger.SetProfile(profile, database);
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
        public void LoggerProfile()
        {
            this.logger = SsmLogger.GetInstance(Environment.CurrentDirectory, MockEcuStream.PortName);
            IAsyncResult result = logger.BeginConnect(ConnectCallback, null);
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
        public void LoggerAddresses()
        {
            SsmLogger logger = SsmLogger.GetInstance(Environment.CurrentDirectory, MockEcuStream.PortName);
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
            IList<int> expected = new int[]
            {
                // 9, 10, 8, 14, 15, 17, 18, 13, 16
                8, 9, 10, 13, 14, 15, 16, 17, 18,
            };

            Assert.AreEqual(expected.Count, actual.Count, "Addresses.Length");
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i], "Addresses[" + i + "]");
            }
        }

        [TestMethod()]
        public void LoggerUserData()
        {
            SsmLogger logger = SsmLogger.GetInstance(Environment.CurrentDirectory, MockEcuStream.PortName);
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
            string userData = "UserData";
            profile.UserData = userData;
            logger.SetProfile(profile, database);
            logger.LogStart += this.LoggerUserDataLogStart;
            logger.LogEntry += this.LoggerUserDataLogEntry;
            logger.LogStop += this.LoggerUserDataLogStop;
            logger.StartLogging();
            System.Threading.Thread.Sleep(500);
            IAsyncResult stopResult = logger.BeginStopLogging(null, null);
            stopResult.AsyncWaitHandle.WaitOne();
            logger.EndStopLogging(stopResult);

            Assert.AreSame(userData, userDataFromLogStart);
            Assert.AreSame(userData, userDataFromLogEntry);
            Assert.AreSame(userData, userDataFromLogStop);
        }

        private object userDataFromLogStart;
        private object userDataFromLogEntry;
        private object userDataFromLogStop;

        private void LoggerUserDataLogStart(object sender, LogEventArgs args)
        {
            userDataFromLogStart = args.UserData;
        }
        
        private void LoggerUserDataLogEntry(object sender, LogEventArgs args)
        {
            userDataFromLogEntry = args.UserData;
        }

        private void LoggerUserDataLogStop(object sender, LogStopEventArgs args)
        {
            userDataFromLogStop = args.UserData;
        }
        
        [TestMethod()]
        public void LoggerAddressesDuplicates()
        {
            this.logger = SsmLogger.GetInstance(Environment.CurrentDirectory, MockEcuStream.PortName);
            IAsyncResult result = logger.BeginConnect(null, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);

            ParameterDatabase database = ParameterDatabase.GetInstance();
            database.Add(source);

            LogProfile profile = LogProfile.CreateInstance();
            Parameter parameter;            

            database.TryGetParameterById("P8", out parameter);
            profile.Add(parameter, parameter.Conversions[0]);
            database.TryGetParameterById("P201", out parameter);
            profile.Add(parameter, parameter.Conversions[0]);
            database.TryGetParameterById("P202", out parameter);
            profile.Add(parameter, parameter.Conversions[0]);
            database.TryGetParameterById("P7", out parameter);
            profile.Add(parameter, parameter.Conversions[0]);

            this.logger.SetProfile(profile, database);

            List<int> addresses = this.logger.Addresses;
            Assert.AreEqual(5, addresses.Count);
            int[] expected = new int[] { 14, 15, 13, 32, 35 };
            Utility.CompareArrays("Address arrays", expected, addresses.ToArray());
        }

        [TestMethod()]
        public void LoggerCorruptPacket()
        {
            this.logger = SsmLogger.GetInstance(Environment.CurrentDirectory, MockEcuStream.PortName);
            IAsyncResult result = logger.BeginConnect(null, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);

            ParameterDatabase database = ParameterDatabase.GetInstance();
            database.Add(source);

            LogProfile profile = LogProfile.CreateInstance();
            foreach (SsmParameter parameter in database.Parameters)
            {
                profile.Add(parameter, parameter.Conversions[0]);
                if (profile.Columns.Count > 3)
                {
                    break;
                }
            }

            this.logStartCalls = 0;
            this.logEntryCalls = 0;
            this.logEndCalls = 0;
            this.logErrorCalls = 0;

            this.logger.SetProfile(profile, database);
            this.logger.LogStart += this.LogStart;
            this.logger.LogEntry += this.LogEntry;
            this.logger.LogStop += this.LogStop;
            this.logger.LogError += this.LogError;

            this.logger.StartLogging();            

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.25));
            MockEcuStream.CorruptResponse();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.25));
            int entriesBeforeCorruption = this.logEntryCalls;
            Debug.WriteLine("Entries before corrupted packet: " + entriesBeforeCorruption.ToString());

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1.25));
            int entriesAfterCorruption = this.logEntryCalls;

            Debug.WriteLine("Entries after corrupted packet: " + entriesAfterCorruption.ToString());

            this.logger.BeginStopLogging(NoOp, null);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.1));

            Assert.IsTrue(entriesAfterCorruption > entriesBeforeCorruption, "Resumed logging after corrupted response.");
        }

        [TestMethod()]
        public void LoggerSuspendResume()
        {
            this.logger = SsmLogger.GetInstance(Environment.CurrentDirectory, MockEcuStream.PortName);
            IAsyncResult result = logger.BeginConnect(null, null);
            result.AsyncWaitHandle.WaitOne();
            ParameterSource source = logger.EndConnect(result);

            ParameterDatabase database = ParameterDatabase.GetInstance();
            database.Add(source);

            LogProfile profile = LogProfile.CreateInstance();
            foreach (SsmParameter parameter in database.Parameters)
            {
                profile.Add(parameter, parameter.Conversions[0]);
                if (profile.Columns.Count > 3)
                {
                    break;
                }
            }

            this.logStartCalls = 0;
            this.logEntryCalls = 0;
            this.logEndCalls = 0;
            this.logErrorCalls = 0;

            this.logger.SetProfile(profile, database);
            this.logger.LogStart += this.LogStart;
            this.logger.LogEntry += this.LogEntry;
            this.logger.LogStop += this.LogStop;
            this.logger.LogError += this.LogError;

            this.logger.StartLogging();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.25));

            this.logger.Suspend();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.25));
            int entriesBeforeSuspend = this.logEntryCalls;
            Debug.WriteLine("Entries before suspend: " + entriesBeforeSuspend.ToString());

            this.logger.Resume(TimeSpan.FromMilliseconds(250));
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5));
            int entriesAfterSuspend = this.logEntryCalls;
            Debug.WriteLine("Entries after suspend: " + entriesAfterSuspend.ToString());

            this.logger.BeginStopLogging(NoOp, null);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.1));

            Assert.IsTrue(entriesAfterSuspend > entriesBeforeSuspend, "Resumed logging after suspend/resume.");
        }
    }
}
