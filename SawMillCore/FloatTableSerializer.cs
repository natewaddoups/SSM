using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NateW.Ssm.ApplicationLogic
{
    public delegate float GetRowHeader(int row);
    public delegate float GetColumnHeader(int column);

    class FloatTableSerializer
    {
        private const string dataPointFormat = "0.000";
        private const string dataPointSeparator = ", ";

        private TextWriter writer;
        private TextReader reader;

        private FloatTableSerializer(TextReader reader)
        {
            this.reader = reader;
        }

        private FloatTableSerializer(TextWriter writer)
        {
            this.writer = writer;
        }

        public static FloatTableSerializer GetInstance(TextReader reader)
        {
            FloatTableSerializer instance = new FloatTableSerializer(reader);
            return instance;
        }

        public static FloatTableSerializer GetInstance(TextWriter writer)
        {
            FloatTableSerializer instance = new FloatTableSerializer(writer);
            return instance;
        }

        /// <summary>
        /// Write data to text stream
        /// </summary>
        public void Write(
            float[][] dataPoints, 
            GetRowHeader getRowHeader, 
            GetColumnHeader getColumnHeader)
        {
            if (this.writer == null)
            {
                throw new InvalidOperationException("This FloatTableSerializer was created with the wrong factory method");
            }

            int rows = dataPoints[0].Length;
            int columns = dataPoints.Length;

            for (int y = rows; y > -2; y--)
            {
                for (int x = -1; x < columns; x++)
                {
                    float value = 0;

                    if ((y == rows) || (y == -1))
                    {
                        if (x >= -0)
                        {
                            value = getColumnHeader(x);
                        }
                    }
                    else if (x == -1)
                    {
                        value = getRowHeader(y);
                    }
                    else
                    {
                        value = dataPoints[x][y];
                    }

                    writer.Write(value.ToString(dataPointFormat));
                    if (x != columns - 1)
                    {
                        writer.Write(dataPointSeparator);
                    }
                }
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Read data from text stream
        /// </summary>
        public void Read(float[][] dataPoints)
        {
            if (this.reader == null)
            {
                throw new InvalidOperationException("This FloatTableSerializer was not created with the wrong factory method");
            }

            int rows = dataPoints[0].Length;
            int columns = dataPoints.Length;

            for (int y = rows; y > -2; y--)
            {
                string line = this.reader.ReadLine();
                if (line == null)
                {
                    return;
                }

                if ((y == rows) || (y == -1))
                {
                    continue;
                }
                    
                string[] values = line.Split(',');
                if (values.Length != columns + 1)
                {
                    continue;
                }

                for (int x = 1; x < columns+1; x++)
                {
                    string valueString = values[x];
                    float value;
                    if (float.TryParse(valueString, out value))
                    {
                        dataPoints[x-1][y] = value;
                    }
                }
            }
        }

        private bool TryReadDataPoint(out float value)
        {
            value = 0f;

            char[] buffer = new char[dataPointFormat.Length];
            int chars = reader.Read(buffer, 0, buffer.Length);
            if (chars != buffer.Length)
            {
                return false;
            }

            string valueString = new string(buffer);
            if (!float.TryParse(valueString, out value))
            {
                return false;
            }

            return true;
        }

        private bool TryReadSeparator()
        {
            char[] buffer = new char[dataPointSeparator.Length];
            int chars = reader.Read(buffer, 0, buffer.Length);
            if (chars != buffer.Length)
            {
                return false;
            }

            string separator = new string(buffer);
            return (separator == dataPointSeparator);
        }
    }
}
