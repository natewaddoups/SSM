///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// LogFilter.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NateW.Ssm
{
    /// <summary>
    /// Creates LogWriters on demand
    /// </summary>
    /// <returns>an instance of LogWriter</returns>
    public delegate LogWriter LogWriterFactory();

    public class HistoryRow : ILogRow
    {
        private string[] data;

        /// <summary>
        /// Gets the number of columms.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return this.data.Length;
            }
        }

        public HistoryRow(LogRow row)
        {
            this.data = new string[row.Columns.Count];
            for (int index = 0; index < row.Columns.Count; index++)
            {
                this.data[index] = row.Columns[index].ValueAsString;
            }
        }
        
        /// <summary>
        /// Gets the value of a column in string form.
        /// </summary>
        /// <param name="index">Index of the desired column.</param>
        /// <returns>Value in string form.</returns>
        public string GetColumnValueAsString(int index)
        {
            return this.data[index];
        }
    }

    /// <summary>
    /// Writes logs only when specified conditions are met
    /// </summary>
    /// <remarks>This is the key thing for defogger-controlled logging.</remarks>
    public class LogFilter
    {
        /// <summary>
        /// Number of rows to log before and after filter criteria are met.
        /// </summary>
        private const int extraRows = 10;

        /// <summary>
        /// For testability
        /// </summary>
        private static bool defaultBehavior = true;

        /// <summary>
        /// Creates LogWriters on demand
        /// </summary>
        private LogWriterFactory factory;

        /// <summary>
        /// Writes logs
        /// </summary>
        private LogWriter writer;

        /// <summary>
        /// The parameter required by the filter
        /// </summary>
        private Parameter parameter;

        /// <summary>
        /// The conversion required by the filter
        /// </summary>
        private Conversion conversion;

        /// <summary>
        /// Queue of recent log entries, to be written at the start of each segment.
        /// </summary>
        private Queue<LogRow> queue;

        /// <summary>
        /// 
        /// </summary>
        internal Parameter Parameter { get { return this.parameter; } }

        internal Conversion Conversion { get { return this.conversion; } }

        /// <summary>
        /// Protected constructor.  Use the factory method instead
        /// </summary>
        internal LogFilter(
            LogWriterFactory factory, 
            Parameter parameter,
            Conversion conversion)
        {
            this.factory = factory;
            this.parameter = parameter;
            this.conversion = conversion;
            this.queue = new Queue<LogRow>(extraRows);
        }

        /// <summary>
        /// Create an instance for test use
        /// </summary>
        internal static LogFilter GetTestInstance(
            LogWriterFactory factory,
            Parameter parameter,
            Conversion conversion)
        {
            return new LogFilter(factory, parameter, conversion);
        }

        /// <summary>
        /// Write the names of the colums
        /// </summary>
        /// <param name="row">collection of columns, values, and conversions</param>
        public void LogStart(LogRow row)
        {
            this.ManageWriterState(row);
        }

        /// <summary>
        /// Write values from the given log entry to the stream
        /// </summary>
        /// <param name="row">collection of columns, values, and conversions</param>
        public void LogEntry(LogRow row)
        {
            if (this.ManageWriterState(row))
            {
                this.writer.LogEntry(row);
            }
        }

        /// <summary>
        /// Flush the writer
        /// </summary>
        public void LogStop()
        {
            this.SafelyDisposeWriter();
        }

        /// <summary>
        /// For testability
        /// </summary>
        internal static void SetDefaultBehavior(bool value)
        {
            LogFilter.defaultBehavior = value;
        }

        /// <summary>
        /// Base class will always log (unless testing is underway)
        /// </summary>
        protected virtual bool ShouldLog(LogColumn column)
        {
            return LogFilter.defaultBehavior;
        }

        /// <summary>
        /// Create and dispose log writer as necessary
        /// </summary>
        /// <param name="row">row of data from the SSM interface</param>
        /// <returns>true if </returns>
        private bool ManageWriterState(LogRow row)
        {
            if (this.ShouldLog(row))
            {
                if (this.writer == null)
                {
                    this.writer = this.factory();

                    if (this.queue.Count == 0)
                    {
                        this.writer.LogStart(row);
                    }
                    else
                    {
                        this.writer.LogStart(this.queue.Dequeue());

                        while (this.queue.Count > 0)
                        {
                            this.writer.LogEntry(this.queue.Dequeue());
                        }
                        
                        this.writer.LogEntry(row);
                    }
                }
                return true;
            }
            else
            {
                // Keep the values from the most recent N rows in the queue for later replay.
                this.queue.Enqueue(new HistoryRow(row));
                if (this.queue.Count > extraRows)
                {
                    this.queue.Dequeue();
                }

                this.SafelyDisposeWriter();
                return false;
            }
        }

        /// <summary>
        /// Indicate whether this row should be logged 
        /// </summary>
        private bool ShouldLog(LogRow row)
        {
            // For the immutable filter types
            if (this.parameter == null)
            {
                return this.ShouldLog((LogColumn) null);
            }

            // The real logic
            foreach (LogColumn column in row.Columns)
            {
                if (column.Parameter.Id == this.parameter.Id)
                {
                    return this.ShouldLog(column);
                }
            }

            // This row doesn't contain the parameter that the log filter expects.
            // This is not unusual; the next one probably will.  
            return false;
        }

        /// <summary>
        /// Stop and dispose the writer, if there is one
        /// </summary>
        private void SafelyDisposeWriter()
        {
            if (this.writer != null)
            {
                this.writer.LogStop();
                this.writer.Dispose();
                this.writer = null;
            }
        }
    }
}
