///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmParameterSourceTest.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NateW.Ssm;

namespace NateW.Ssm.Protocol.Test
{
    /// <summary>
    /// Summary description for LogProfileDatabaseTests
    /// </summary>
    [TestClass]
    public class SsmParameterSourceTest
    {
        public const string EcuIdentifier = "29044A4105";
        public static readonly byte[] CompatibilityMap = new byte[] { 
                0x73, 0xFA, 0xCB, 0xA6, 0x2B, 0x81, 0xFE, 0xA8, 
                0x00, 0x00, 0x00, 0x60, 0xCE, 0x54, 0xF9, 0xB1, 
                0xE4, 0x00, 0x0C, 0x20, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0xDC, 0x00, 0x00, 0x5D, 0x1F, 0x30, 0xC0, 
                0xF2, 0x26, 0x00, 0x00, 0x43, 0xFB, 0x00, 0xE1, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC1, 0xF0, 
                0x28,  
            };

        public SsmParameterSourceTest()
        {
            Utility.CopyConfigurationFiles();
        }

        [TestMethod]
        public void DatabaseGetStandardParameters_XPath()
        {
            DatabaseGetStandardParameters(false);
        }

        [TestMethod]
        public void DatabaseGetStandardParameters_XmlReader()
        {
            Assert.Inconclusive("Not implemented.");
            DatabaseGetStandardParameters(true);
        }

        [TestMethod]
        public void DatabaseGetExtendedParameters_XPath()
        {
            DatabaseGetExtendedParameters(false);
        }

        [TestMethod]
        public void DatabaseGetExtendedParameters_XmlReader()
        {
            Assert.Inconclusive("Not implemented.");
            DatabaseGetExtendedParameters(true);
        }

        [TestMethod]
        public void DatabaseGetSwitches_XPath()
        {
            DatabaseGetSwitches(false);
        }

        [TestMethod]
        public void DatabaseGetSwitches_XmlReader()
        {
            Assert.Inconclusive("Not implemented.");
            DatabaseGetSwitches(true);
        }

        private void DatabaseGetStandardParameters(bool useXmlReader)
        {
            ReadOnlyCollection<Parameter> noDependencies = null;
            List<Parameter> expectedList = new List<Parameter>();
            expectedList.Add(new SsmParameter(
                null,
                "P9",
                "Vehicle Speed",
                0x000010,
                1,
                new Conversion[] { 
                    Conversion.GetInstance("mph", "x*0.621371192", "0"), 
                    Conversion.GetInstance("kph", "x", "0") 
                },
                9,
                7,
                noDependencies));
            expectedList.Add(new SsmParameter(
                null,
                "P2",
                "Coolant Temperature",
                0x000008,
                1,
                new Conversion[] { 
                    Conversion.GetInstance("F", "32+9*(x-40)/5", "0"),
                    Conversion.GetInstance("C", "x-40", "0")
                },
                8,
                6,
                noDependencies));
            expectedList.Add(new SsmParameter(
                null,
                "P8",
                "Engine Speed",
                0x00000E,
                2,
                new Conversion[] { Conversion.GetInstance("rpm", "x/4", "0") },
                8,
                0,
                noDependencies));

            SsmParameterSource database = new SsmParameterSource(EcuIdentifier, CompatibilityMap);
            using (Stream inputStream = File.OpenRead("logger.xml")) 
            {
                if (useXmlReader)
                {
                    XmlReader reader = XmlReader.Create(inputStream);
                    database.LoadStandardParameters(reader);
                }
                else
                {
                    XPathDocument document = SsmParameterSource.CreateDocument(inputStream);
                    database.LoadStandardParameters(document);
                }
            }

            int expectedCount = 53; // was 92 prior to compatibility filtering
            Assert.AreEqual(expectedCount, database.Parameters.Count, "Parameters.Count");
            SsmParameterSourceTest.CompareLists(expectedList, database.Parameters);
        }

        private void DatabaseGetExtendedParameters(bool useXmlReader)
        {
            List<Parameter> expectedList = new List<Parameter>();
            expectedList.Add(new SsmParameter(
                null,
                "E1",
                "IAM*",
                0x20124, //0x20118,
                4,
                new Conversion[] { 
                    Conversion.GetInstance("raw ecu value", "x", "0"),
                    Conversion.GetInstance("multiplier", "x/16", "0.0000")
                }));

            expectedList.Add(new SsmParameter(
                null,
                "E2",
                "Engine Load*",
                0x2009A, //0x21847,
                4,
                new Conversion[] { 
                    Conversion.GetInstance("g/rev", "x*.00006103515625", "0.00"),
                }));

            SsmParameterSource database = new SsmParameterSource(EcuIdentifier, CompatibilityMap);
            using (Stream inputStream = File.OpenRead("logger.xml"))
            {
                if (useXmlReader)
                {
                    XmlReader reader = XmlReader.Create(inputStream);
                    database.LoadExtendedParameters(reader);
                }
                else
                {
                    XPathDocument document = SsmParameterSource.CreateDocument(inputStream);
                    database.LoadExtendedParameters(document);
                }
            }

            Assert.AreEqual(34, database.Parameters.Count, "Parameters.Count");
            Console.WriteLine("Count: " + database.Parameters.Count);

            foreach (SsmParameter parameter in database.Parameters)
            {
                Console.WriteLine(
                    string.Format(
                        "Parameter: {0,50} ID {1,5} Address {2,10}",
                            parameter.Name,
                            parameter.Id,
                            parameter.Address));
            }

            SsmParameterSourceTest.CompareLists(expectedList, database.Parameters);
        }

        private void DatabaseGetSwitches(bool useXmlReader)
        {
            List<Parameter> expectedList = new List<Parameter>();
            expectedList.Add(new SsmParameter(null, "S20", "Defogger Switch", 0x64, 1, new Conversion[] { Conversion.GetInstance(Conversion.Boolean, "x&(2^5)", "") }));
            expectedList.Add(new SsmParameter(null, "S65", "Set/Coast Switch", 0x121, 1, new Conversion[] { Conversion.GetInstance(Conversion.Boolean, "x&(2^5)", "") }));
            expectedList.Add(new SsmParameter(null, "S66", "Resume/Accelerate Switch", 0x121, 1, new Conversion[] { Conversion.GetInstance(Conversion.Boolean, "x&(2^4)", "") }));
            SsmParameterSource database = new SsmParameterSource(EcuIdentifier, CompatibilityMap);

            using (Stream inputStream = File.OpenRead("logger.xml"))
            {
                if (useXmlReader)
                {
                    XmlReader reader = XmlReader.Create(inputStream);
                    database.LoadSwitches(reader);
                }
                else
                {
                    XPathDocument document = SsmParameterSource.CreateDocument(inputStream);
                    database.LoadSwitches(document);
                }
            }

            Assert.AreEqual(68, database.Parameters.Count, "Switches.Count");
            Console.WriteLine("Count: " + database.Parameters.Count);

            foreach (SsmParameter parameter in database.Parameters)
            {
                Console.WriteLine(
                    string.Format(
                        "Parameter: {0,50} ID {1,5} Address {2,10} Conversion {3,10}",
                            parameter.Name,
                            parameter.Id,
                            parameter.Address,
                            parameter.Conversions[0]));
            }

            SsmParameterSourceTest.CompareLists(expectedList, database.Parameters);
        }
        
        [TestMethod]
        public void DatabaseGetCalculatedParameters_XmlReader()
        {
            Assert.Inconclusive("Not implemented.");
            SsmParameterSource source = SsmParameterSource.GetInstance(Environment.CurrentDirectory, EcuIdentifier, CompatibilityMap);
            using (Stream inputStream = File.OpenRead("logger.xml"))
            {
#if XPath
               PathDocument document = SsmParameterSource.CreateDocument(inputStream);
                database.LoadExtendedParameters(document);
#else
                XmlReader reader = XmlReader.Create(inputStream);
                source.LoadExtendedParameters(reader);
#endif
            }

            SsmParameter engineSpeed = null;
            SsmParameter maf = null;
            SsmParameter load = null;
            foreach (SsmParameter parameter in source.Parameters)
            {
                if (parameter.Id == "P8")
                {
                    engineSpeed = parameter;
                }

                if (parameter.Id == "P12")
                {
                    maf = parameter;
                }

                if (parameter.Id == "P200")
                {
                    load = parameter;
                }
            }

            Assert.AreSame(engineSpeed, load.Dependencies[0], "First dependency");
            Assert.AreSame(maf, load.Dependencies[1], "Second dependency");
        }

        [TestMethod]
        public void DatabaseCompareMethods()
        {
            Assert.Inconclusive("Not implemented.");

            // XPath method
            SsmParameterSource xpathDatabase = new SsmParameterSource(EcuIdentifier, CompatibilityMap);
            using (Stream stream = File.OpenRead("logger.xml"))
            {
                XPathDocument document = SsmParameterSource.CreateDocument(stream);
                xpathDatabase.LoadExtendedParameters(document);
                xpathDatabase.LoadStandardParameters(document);
            }

            // XmlReader method
            SsmParameterSource xmlReaderDatabase = new SsmParameterSource(EcuIdentifier, CompatibilityMap);
            using (Stream stream = File.OpenRead("logger.xml"))
            {
                XmlReader reader = XmlReader.Create(stream);
                xmlReaderDatabase.LoadStandardParameters(reader);

                stream.Position = 0;
                reader = XmlReader.Create(stream);
                xmlReaderDatabase.LoadExtendedParameters(reader);
            }

            Assert.AreEqual(xpathDatabase.Parameters.Count, xmlReaderDatabase.Parameters.Count, "Counts");
            CompareLists(
                new List<Parameter>(xpathDatabase.Parameters),
                new List<Parameter>(xmlReaderDatabase.Parameters));
        }

        private static void CompareLists(IList<Parameter> expectedList, IList<Parameter> actualList)
        {
            foreach (SsmParameter expected in expectedList)
            {
                SsmParameter actual = null;
                if (!SsmParameterSourceTest.TryGetParameter(actualList, expected.Id, out actual))
                {
                    throw new Exception("Actual list does not contain " + expected.Name + " (" + expected.Id + ")");
                }
                                
                Assert.AreEqual(expected.Conversions.Count, actual.Conversions.Count, "Conversions.Count");
                for (int i = 0; i < actual.Conversions.Count; i++)
                {
                    Assert.AreEqual(
                        expected.Conversions[i].Units, 
                        actual.Conversions[i].Units, 
                        "Conversions[i].Units");

                    Assert.AreEqual(
                        expected.Conversions[i].Expression, 
                        actual.Conversions[i].Expression, 
                        "Conversions[i].Expression");

                    Assert.AreEqual(
                        expected.Conversions[i].Format, 
                        actual.Conversions[i].Format, 
                        "Conversions[i].Format");
                }

                Assert.AreEqual(expected.Id, actual.Id, actual.Name + " Id");

                if (expected.IsCalculated)
                {
                    Assert.IsTrue(actual.IsCalculated, "IsCalculated");
                    Assert.IsTrue(actual.Dependencies != null, "Dependencies != null");
                    Assert.IsTrue(actual.Dependencies.Count > 0, "Dependencies.Count > 0");
                }
                else
                {
                    Assert.AreEqual(expected.Address, actual.Address, actual.Name + " Address");
                    Assert.IsTrue(actual.Dependencies == null, "Dependencies == null");
                }

                Assert.AreEqual(expected.EcuCapabilityByteIndex, actual.EcuCapabilityByteIndex, actual.Name + " EcuCapabilityByteIndex");
                Assert.AreEqual(expected.EcuCapabilityBitIndex, actual.EcuCapabilityBitIndex, actual.Name + " EcuCapabilityBitIndex");
            }

            // TODO: ensure no extra parameters
        }

        private static bool TryGetParameter(IList<Parameter> actualList, string id, out SsmParameter actual)
        {
            foreach (SsmParameter candidate in actualList)
            {
                if (candidate.Id == id)
                {
                    actual = candidate;
                    return true;
                }
            }
            actual = null;
            return false;
        }
    }
}