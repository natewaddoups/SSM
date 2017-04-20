using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NateW.Ssm;
using NateW.Ssm.ApplicationLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SawMillCoreTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class SawMillTest
    {
        public SawMillTest()
        {
            SsmDisplayTest.Utility.CopyConfigurationFiles();
        }

        private TestContext testContextInstance;

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

        private void Update(LogEventArgs args)
        {
        }

        [TestMethod]
        public void SawMillScatterPlotPersistance()
        {
            List<SawMillScreen> screens = new List<SawMillScreen>();
            screens.Add(new RpmMafScreen(7100f, 250f));
            screens.Add(new RpmLoadScreen(7100f, 2.5f));
            SawMill sawMill1 = SawMill.GetInstance(Environment.CurrentDirectory, "Mock ECU", screens, this.Update);

            MemoryStream stream = new MemoryStream();
            sawMill1.WriteTo(stream);

            stream.Position = 0;

            SawMill sawMill2 = SawMill.GetInstance(Environment.CurrentDirectory, "Mock ECU", screens, this.Update);
            sawMill2.ReadFrom(stream);            

            // TODO: verify consistency of data
        }

        [TestMethod]
        public void SawMillHeatMapPersistance()
        {
            List<SawMillScreen> screens = new List<SawMillScreen>();
            screens.Add(new VEScreen(7100, 2.5f));
            SawMill sawMill1 = SawMill.GetInstance(Environment.CurrentDirectory, "Mock ECU", screens, this.Update);

            MemoryStream stream = new MemoryStream();
            sawMill1.WriteTo(stream);

            stream.Position = 0;

            SawMill sawMill2 = SawMill.GetInstance(Environment.CurrentDirectory, "Mock ECU", screens, this.Update);
            sawMill2.ReadFrom(stream);

            // TODO: verify consistency of data
        }
    }
}
