///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmParameterDatabaseTest.cs
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
    /// Tests for ParameterDatabase class.
    /// </summary>
    [TestClass]
    public class ParameterDatabaseTest
    {
        public ParameterDatabaseTest()
        {
            Utility.CopyConfigurationFiles();
        }

        [TestMethod]
        public void ParameterDatabaseLoadRemove()
        {
            ParameterDatabase db = ParameterDatabase.GetInstance();
            ParameterSource source = new MockParameterSource();
            db.Add(source);
            Assert.AreEqual(source.Parameters.Count, db.Parameters.Count);
            db.Remove(source);
            Assert.AreEqual(0, db.Parameters.Count);
        }
    }
}