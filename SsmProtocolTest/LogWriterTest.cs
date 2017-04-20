///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// LogWriterTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    public class LogWriterFilterTestBase
    {
        /// <summary>
        /// Log columns used by each of the test methods
        /// </summary>
        protected ReadOnlyCollection<LogColumn> readOnlyColumns;

        /// <summary>
        /// Create the test log columns
        /// </summary>
        public virtual void Initialize()
        {
            Conversion units = Conversion.GetInstance("units", "x", "0");
            Conversion cubits = Conversion.GetInstance("cubits", "x/2", "0.0");
            Conversion torks = Conversion.GetInstance("torks", "x * 2", "0.00");
            Conversion[] conversions = new Conversion[] { units, cubits, torks };
            SsmParameter p1 = new SsmParameter(null, "P1", "Parameter1", 1, 1, conversions);
            SsmParameter p2 = new SsmParameter(null, "P2", "Parameter2", 2, 2, conversions);
            SsmParameter p3 = new SsmParameter(null, "P3", "Parameter3", 3, 3, conversions);

            List<LogColumn> columns = new List<LogColumn>();
            columns.Add(LogColumn.GetInstance(p1, units, null, false));
            columns.Add(LogColumn.GetInstance(p2, cubits, null, false));
            columns.Add(LogColumn.GetInstance(p3, torks, null, false));
            this.readOnlyColumns = new ReadOnlyCollection<LogColumn>(columns);
            foreach (LogColumn column in columns)
            {
                double unused;
                string value;
                column.Conversion.Convert(0, out value, out unused);
                column.ValueAsString = value;
            }
        }
    }

    [TestClass()]
    public class LogWriterTest : LogWriterFilterTestBase
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

        public LogWriterTest()
        {
            Utility.CopyConfigurationFiles();
        }

        [TestInitialize()]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod()]
        public void LogWriterStart()
        {
            LogRow row = LogRow.GetInstance(this.readOnlyColumns);

            MemoryStream memoryStream = new MemoryStream();
            using (LogWriter writer = LogWriter.GetInstance(memoryStream, false))
            {
                writer.LogStart(row);
            }

            string actual = Encoding.ASCII.GetString(memoryStream.ToArray());
            string expected = 
                "Parameter1, Parameter2, Parameter3" + Environment.NewLine;
            Assert.AreEqual(expected, actual, "Log Header");
        }

        [TestMethod()]
        public void LogWriterOneEntry()
        {
            LogRow row = LogRow.GetInstance(this.readOnlyColumns);

            MemoryStream memoryStream = new MemoryStream();
            using (LogWriter writer = LogWriter.GetInstance(memoryStream, false))
            {
                writer.LogStart(row);
                writer.LogEntry(row);
            }

            string actual = Encoding.ASCII.GetString(memoryStream.ToArray());
            string expected = 
                "Parameter1, Parameter2, Parameter3"  + Environment.NewLine +
                "0, 0.0, 0.00" + Environment.NewLine;
            Assert.AreEqual(expected, actual, "Log with one entry");
        }

        [TestMethod()]
        public void LogWriterTwoEntries()
        {
            LogRow row = LogRow.GetInstance(this.readOnlyColumns);

            MemoryStream memoryStream = new MemoryStream();
            using (LogWriter writer = LogWriter.GetInstance(memoryStream, false))
            {
                writer.LogStart(row);
                writer.LogEntry(row);
                writer.LogEntry(row);
            }

            string actual = Encoding.ASCII.GetString(memoryStream.ToArray());
            string expected =
                "Parameter1, Parameter2, Parameter3" + Environment.NewLine +
                "0, 0.0, 0.00" + Environment.NewLine +
                "0, 0.0, 0.00" + Environment.NewLine;
            Assert.AreEqual(expected, actual, "Log with two entries");
        }

        [TestMethod()]
        public void LogWriterStop()
        {
            LogRow row = LogRow.GetInstance(this.readOnlyColumns);

            MemoryStream memoryStream = new MemoryStream();
            LogWriter writer = LogWriter.GetInstance(memoryStream, false);
            writer.LogStart(row);
            writer.LogEntry(row);
            writer.LogEntry(row);
            writer.LogStop();
        
            string actual = Encoding.ASCII.GetString(memoryStream.ToArray());
            string expected =
                "Parameter1, Parameter2, Parameter3" + Environment.NewLine +
                "0, 0.0, 0.00" + Environment.NewLine +
                "0, 0.0, 0.00" + Environment.NewLine;
            Assert.AreEqual(expected, actual, "Log with two entries");
        }

        [TestMethod()]
        public void LogWriterTimeColumns()
        {
            LogRow row = LogRow.GetInstance(this.readOnlyColumns);

            MemoryStream memoryStream = new MemoryStream();
            LogWriter writer = LogWriter.GetInstance(memoryStream, true);
            writer.LogStart(row);
            System.Threading.Thread.Sleep(10);
            writer.LogEntry(row);
            writer.LogEntry(row);
            writer.LogStop();

            // "yyyy-MM-dd T hh:mm:ss:fff"
            string actual = Encoding.ASCII.GetString(memoryStream.ToArray());
            string expected =
                "Time, Clock, Parameter1, Parameter2, Parameter3" + Environment.NewLine +
                @"\d\d, \d\d\d\d-\d\d-\d\d T \d\d:\d\d:\d\d:\d\d\d, 0, 0.0, 0.00" + Environment.NewLine +
                @"\d\d, \d\d\d\d-\d\d-\d\d T \d\d:\d\d:\d\d:\d\d\d, 0, 0.0, 0.00" + Environment.NewLine;

            string[] actualRows = actual.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            string[] expectedRows = expected.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            Assert.AreEqual(expectedRows.Length, actualRows.Length, "line counts");
            for (int i = 0; i < actualRows.Length; i++)
            {
                Regex regex = new Regex(expectedRows[i]);
                Match match = regex.Match(actualRows[i]);
                Assert.IsTrue(
                    match.Success,
                    "Row " + i + Environment.NewLine +
                    "Actual: " + actualRows[i] + Environment.NewLine +
                    "Expected: " + expectedRows[i] + Environment.NewLine);
            }
        }
    }
}
