///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// Utility.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    public static class Utility
    {
        /// <summary>
        /// Copy configuration files from the official location to the current directory.
        /// </summary>
        public static void CopyConfigurationFiles()
        {
            try
            {
                File.Copy(@"..\..\..\Configuration\logger.xml", "logger.xml");
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            try
            {
                File.Copy(@"..\..\..\Configuration\logger.dtd", "logger.dtd");
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public static void CopyJ2534Mock()
        {
            try
            {
                File.Copy(@"..\..\..\J2534Mock\Debug\J2534Mock.dll", "J2534Mock.dll");
            }
            catch(IOException ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            try
            {
                File.Copy(@"..\..\..\J2534Mock\Debug\J2534Mock.pdb", "J2534Mock.pdb");
            }
            catch(IOException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Dump the given IList.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="buffer"></param>
        public static void DumpArray<T>(string type, IList<T> buffer)
        {
            Debug.WriteLine(type);
            for (int i = 0; i < buffer.Count; i++)
            {
                string note = "payload";
                if (i < 4)
                {
                    note = ((SsmPacketIndex)(byte)i).ToString();
                }

                if (i == buffer.Count - 1)
                {
                    note = "Checksum";
                }

                Debug.WriteLine(string.Format("{0:X02} - {1:X02} - {2}", i, buffer[i], note));
            }
        }

        /// <summary>
        /// Compare two lists, throw if they are not identical.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public static void CompareArrays<T>(string message, IList<T> expected, IList<T> actual)
        {
            Utility.DumpArray("Actual", actual);
            Utility.DumpArray("Expected", expected);

            Assert.AreEqual(expected.Count, actual.Count, message + ": packet length");
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i], message + ": offset[" + i.ToString() + "]");
            }
        }

/*        internal static void CompareArrays<T>(T[] expected, T[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length, "Packet length");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], "Offset[" + i.ToString() + "]");
            }
        }*/


        internal static void AssertContains(LogProfile profile, string parameterId, string conversionUnits)
        {
            foreach (LogColumn column in profile.Columns)
            {
                if (column.Parameter.Id == parameterId)
                {
                    IList<Conversion> conversions = profile.GetConversions(column.Parameter);
                    foreach (Conversion conversion in conversions)
                    {
                        if (conversion.Units == conversionUnits)
                        {
                            return;
                        }
                    }
                }
            }
            Assert.Fail("Selected parameters does not contain " + parameterId + " / " + conversionUnits);
        }

        internal static void AssertColumnParameterId(LogEventArgs args, int column, string expected)
        {
            string actual = args.Row.Columns[column].Parameter.Id;
            Assert.AreEqual(expected, actual,
                string.Format(
                    "Column {0} parameter ID", column));
        }
    }
}
