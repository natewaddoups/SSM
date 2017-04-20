///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// PlxTest.cs
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
using NSFW.PlxSensors;

namespace NateW.Ssm.Protocol.Test
{
    /// <summary>
    /// Tests for Plx classes.
    /// </summary>
    [TestClass]
    public class PlxTest
    {
        public PlxTest()
        {
            Utility.CopyConfigurationFiles();
        }

        [TestMethod]
        public void PlxParameters()
        {
            ParameterDatabase db = ParameterDatabase.GetInstance();
            ParameterSource plxSource = PlxParameterSource.GetInstance();
            db.Add(plxSource);

            Parameter wb = db.Parameters[0];
            PlxSensorId id = new PlxSensorId(PlxSensorType.WidebandAfr, 0);
            Assert.AreEqual(id, ((PlxParameter)wb).SensorId);
            
            Parameter egt = db.Parameters[1];
            id = new PlxSensorId(PlxSensorType.ExhaustGasTemperature, 0);
            Assert.AreEqual(id, ((PlxParameter)egt).SensorId);
        }
    }
}