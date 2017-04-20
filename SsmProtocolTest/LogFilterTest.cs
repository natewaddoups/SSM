///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// LogFilterTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    [TestClass()]
    public class LogFilterTest : LogWriterFilterTestBase
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

        public LogFilterTest()
        {
            Utility.CopyConfigurationFiles();
        }

        [TestInitialize()]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod()]
        public void LogFilterPositive()
        {
            LogRow row = LogRow.GetInstance(this.readOnlyColumns);
            LogColumn column = this.readOnlyColumns[0];

            MemoryStream memoryStream = new MemoryStream();
            LogFilter filter = LogFilter.GetTestInstance(
                delegate { return LogWriter.GetInstance(memoryStream, false); },
                column.Parameter, 
                column.Conversion);
            LogFilter.SetDefaultBehavior(true);

            filter.LogStart(row);
            filter.LogEntry(row);
            filter.LogEntry(row);
            filter.LogStop();

            string actual = Encoding.ASCII.GetString(memoryStream.ToArray());
            string expected =
                "Parameter1, Parameter2, Parameter3" + Environment.NewLine +
                "0, 0.0, 0.00" + Environment.NewLine +
                "0, 0.0, 0.00" + Environment.NewLine;
            Assert.AreEqual(expected, actual, "Log with two entries");
        }


        [TestMethod()]
        public void LogFilterNegative()
        {
            LogRow row = LogRow.GetInstance(this.readOnlyColumns);
            LogColumn column = this.readOnlyColumns[0];
            MemoryStream memoryStream = new MemoryStream();
            LogFilter filter = LogFilter.GetTestInstance(
                delegate { return LogWriter.GetInstance(memoryStream, false); },
                column.Parameter,
                column.Conversion);
            LogFilter.SetDefaultBehavior(false);

            filter.LogStart(row);
            filter.LogEntry(row);
            filter.LogEntry(row);
            filter.LogStop();

            string actual = Encoding.ASCII.GetString(memoryStream.ToArray());
            string expected = string.Empty;
            Assert.AreEqual(expected, actual, "Log with two entries");
        }

        [TestMethod()]
        public void LogFilterTwoLogs()
        {
            int iteration = 1;
            MemoryStream memoryStream1 = new MemoryStream();
            MemoryStream memoryStream2 = new MemoryStream();

            LogRow row = LogRow.GetInstance(this.readOnlyColumns);
            LogColumn column = this.readOnlyColumns[0];
            LogFilter filter = LogFilter.GetTestInstance(
                delegate
                {
                    if (iteration == 1)
                    {
                        return LogWriter.GetInstance(memoryStream1, false);
                    }
                    else
                    {
                        return LogWriter.GetInstance(memoryStream2, false);
                    }
                },
                column.Parameter, 
                column.Conversion);
            LogFilter.SetDefaultBehavior(true);

            filter.LogStart(row);
            filter.LogEntry(row);
            filter.LogEntry(row);
            filter.LogStop();
            iteration = 2;
            filter.LogStart(row);
            filter.LogEntry(row);
            filter.LogEntry(row);
            filter.LogStop();

            string actual = Encoding.ASCII.GetString(memoryStream1.ToArray());
            string expected =
                "Parameter1, Parameter2, Parameter3" + Environment.NewLine +
                "0, 0.0, 0.00" + Environment.NewLine +
                "0, 0.0, 0.00" + Environment.NewLine;            
            Assert.AreEqual(expected, actual, "first log");

            actual = Encoding.ASCII.GetString(memoryStream2.ToArray());
            Assert.AreEqual(expected, actual, "second log");

        }

        [TestMethod()]
        public void LogFilterDynamic()
        {
            int iteration = 1;
            MemoryStream memoryStream1 = new MemoryStream();
            MemoryStream memoryStream2 = new MemoryStream();
            MemoryStream memoryStream3 = new MemoryStream();

            LogRow row = LogRow.GetInstance(this.readOnlyColumns);
            LogColumn column = this.readOnlyColumns[0];

            LogFilter filter = TestLogFilter.GetInstance(
                delegate
                {
                    if (iteration == 1)
                    {
                        return LogWriter.GetInstance(memoryStream1, false);
                    }
                    else if (iteration == 2)
                    {
                        return LogWriter.GetInstance(memoryStream2, false);
                    }
                    else if (iteration == 3)
                    {
                        return LogWriter.GetInstance(memoryStream3, false);
                    }
                    return null;
                },
                column.Parameter, 
                column.Conversion,
                "1");

            foreach (LogColumn tempColumn in this.readOnlyColumns)
            {
                string value;
                double unused;
                tempColumn.Conversion.Convert(1, out value, out unused);
                tempColumn.ValueAsString = value;
            }

            filter.LogStart(row);
            filter.LogEntry(row);
            filter.LogEntry(row);

            iteration = 2;
            foreach (LogColumn tempColumn in this.readOnlyColumns)
            {
                string value;
                double unused;
                tempColumn.Conversion.Convert(0, out value, out unused);
                tempColumn.ValueAsString = value;
            }

            filter.LogEntry(row);
            filter.LogEntry(row);

            iteration = 3;
            foreach (LogColumn tempColumn in this.readOnlyColumns)
            {
                string value;
                double unused;
                tempColumn.Conversion.Convert(1, out value, out unused);
                tempColumn.ValueAsString = value;
            }

            filter.LogEntry(row);
            filter.LogEntry(row);
            filter.LogStop();

            // First log
            string actual = Encoding.ASCII.GetString(memoryStream1.ToArray());
            string expected =
                "Parameter1, Parameter2, Parameter3" + Environment.NewLine +
                "1, 0.5, 2.00" + Environment.NewLine +
                "1, 0.5, 2.00" + Environment.NewLine;
            Assert.AreEqual(expected, actual, "first log");

            // Second log
            actual = Encoding.ASCII.GetString(memoryStream2.ToArray());
            Assert.AreEqual(string.Empty, actual, "second log");

            // Third log
            actual = Encoding.ASCII.GetString(memoryStream3.ToArray());
            Assert.AreEqual(expected, actual, "second log");
        }
    }
}
