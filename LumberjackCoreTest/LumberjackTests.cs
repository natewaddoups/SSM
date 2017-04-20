using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace NateW.Ssm.ApplicationLogic
{
    /// <summary>
    /// Unit tests for the Lumberjack application logic
    /// </summary>
    /// <remarks>
    /// Lumberjack test coverage is very weak because it was initially
    /// developed without automated testing.  There is a log of work
    /// to do here.
    /// </remarks>
    [TestClass]
    public class LumberjackTests
    {
        private const int loadSleep = 500;
        private const int actionSleep = 10;

        private class OneSecondMinimum : IDisposable
        {
            private DateTime start;
            public OneSecondMinimum()
            {
                start = DateTime.Now;
            }

            public void Dispose()
            {
                TimeSpan duration = DateTime.Now.Subtract(start);
                TimeSpan remainder = TimeSpan.FromSeconds(1).Subtract(duration);
                if (remainder.Ticks > 0)
                {
                    Thread.Sleep(remainder);
                }
            }
        }

        private DateTime testStart;
        
        public LumberjackTests()
        {
        }

        /// <summary>
        /// Note the time when the current (next) test case begins
        /// </summary>
        [TestInitialize()]
        public void BeforeEachTest()
        {
            this.testStart = DateTime.Now;
        }
        
        /// <summary>
        /// Sleep to ensure that tests run at least 1 second apart, so they use different CSV files
        /// </summary>
        [TestCleanup()]
        public void AfterEachTest()
        {
            TimeSpan duration = DateTime.Now.Subtract(testStart);
            TimeSpan remainder = TimeSpan.FromSeconds(1).Subtract(duration);
            int milliseconds = remainder.Milliseconds;
            if (milliseconds < 0)
            {
                milliseconds = 100;
            }
            milliseconds = Math.Max(100, milliseconds);
            Thread.Sleep(milliseconds);
        }

        [TestMethod]
        public void LumberjackLoadClose()
        {
            MockUserInterface mui = new MockUserInterface();
            Lumberjack lumberjack = new Lumberjack(mui);
            mui.Lumberjack = lumberjack;

            lumberjack.Load();
            Thread.Sleep(loadSleep);
            
            bool cancel = false;
            lumberjack.Closing(ref cancel);
            Thread.Sleep(actionSleep);

            string record = mui.ToString();
            AssertRecordingStart();
            AssertRecorded(ref record, "SetTitle");
        }

        [TestMethod]
        public void LumberjackLoadNewClose()
        {
            MockUserInterface mui = new MockUserInterface();
            Lumberjack lumberjack = new Lumberjack(mui);
            mui.Lumberjack = lumberjack;

            lumberjack.Load();
            Thread.Sleep(loadSleep);
            lumberjack.ProfileNew();
            Thread.Sleep(actionSleep);

            mui.Profile = LogProfile.CreateInstance();
            mui.Profile.Add("P8", "rpm", lumberjack.Database);
            lumberjack.SelectedProfileSettingsChanged();
            Thread.Sleep(actionSleep);

            bool cancel = false;
            lumberjack.Closing(ref cancel);
            Thread.Sleep(actionSleep);

            string record = mui.ToString();
            AssertRecordingStart();
            AssertRecorded(ref record, "SetTitle");
            AssertRecorded(ref record, "ShowNewProfileSettings");
            AssertRecorded(ref record, "SaveButtonEnabled: False");
            AssertRecorded(ref record, "SetTitle: Lumberjack - Mock ECU - new profile");
            //AssertRecorded(ref record, "RenderLogEntry");
            AssertRecorded(ref record, "GetNewProfileSettings");
            AssertRecorded(ref record, "SaveButtonEnabled: True");
            AssertRecorded(ref record, "SetTitle: Lumberjack - Mock ECU - new profile*");
            //AssertRecorded(ref record, "New Profile.profile");
            //AssertRecorded(ref record, "Close");
            Assert.Inconclusive("This test needs more work.");
        }

        [TestMethod]
        public void LumberjackBlankSlateLoadProfile()
        {
            MockUserInterface mui = new MockUserInterface();
            Lumberjack lumberjack = new Lumberjack(mui);
            mui.Lumberjack = lumberjack;
            lumberjack.Settings.Reset();

            lumberjack.SetStartupLogFilterType(LogFilterType.NeverLog);
            lumberjack.Load();
            Thread.Sleep(loadSleep);
            
            mui.ShowOpenFileDialogPath = Path.Combine(Lumberjack.GetConfigurationDirectory(), "Turbo Dynamics.profile");
            mui.ShowOpenFileDialogResult = DialogResult.OK;
            lumberjack.ProfileOpen();
            Thread.Sleep(actionSleep);

            bool cancel = false;
            lumberjack.Closing(ref cancel);
            Thread.Sleep(actionSleep);

            Assert.IsFalse(lumberjack.CurrentProfileIsChanged);
            string record = mui.ToString();
            AssertRecordingStart();
            AssertRecorded(ref record, "SetTitle");
            AssertRecorded(ref record, "ShowNewProfileSettings");
            AssertRecorded(ref record, "RenderLogEntry");
        }

        [TestMethod]
        public void LumberjackDefoggerLogging()
        {
            MockUserInterface mui = new MockUserInterface();
            Lumberjack lumberjack = new Lumberjack(mui);
            mui.Lumberjack = lumberjack;
            lumberjack.Settings.Reset();

            lumberjack.SetStartupLogFilterType(LogFilterType.NeverLog);
            lumberjack.Load();
            Thread.Sleep(loadSleep);
            
            mui.ShowOpenFileDialogPath = Path.Combine(Lumberjack.GetConfigurationDirectory(), "Turbo Dynamics.profile");
            mui.ShowOpenFileDialogResult = DialogResult.OK;
            lumberjack.ProfileOpen();
            Thread.Sleep(actionSleep);

            bool cancel = false;
            lumberjack.Closing(ref cancel);
            Thread.Sleep(actionSleep);

            Assert.IsFalse(lumberjack.CurrentProfileIsChanged);
            string record = mui.ToString();
            AssertRecordingStart();
            AssertRecorded(ref record, "SetTitle");
            AssertRecorded(ref record, "ShowNewProfileSettings");
            AssertRecorded(ref record, "RenderLogEntry");
        }

        [TestMethod]
        public void LumberjackCondenseFileName()
        {
            TestCondenseFileName("", "New");
            TestCondenseFileName(null, "New");
            TestCondenseFileName("Test", "Test");
            TestCondenseFileName("A B.profile", "AB");
        }

        void TestCondenseFileName(string input, string expected)
        {
            string actual = Lumberjack.CondenseFileName(input);
            Assert.AreEqual(expected, actual);
        }

        private void AssertRecordingStart()
        {
        }

        private void AssertRecorded(ref string record, string message)
        {
            int start = record.IndexOf(message);
            Assert.IsTrue(start != -1, message + " was never recorded");
            record = record.Substring(start + message.Length);
        }
    }
}
