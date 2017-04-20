using System.IO;
using NateW.Ssm.ApplicationLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NateW.Ssm.ApplicationLogic
{
    
    
    /// <summary>
    ///This is a test class for FloatTableSerializerTest and is intended
    ///to contain all FloatTableSerializerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FloatTableSerializerTest
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

        ///<summary>
        ///Write a small table, then read it, and verify
        ///</summary>
        [TestMethod()]
        public void TableSerializerWriteThenRead()
        {
            float[][] dataPoints = new float[2][];
            dataPoints[1] = new float[] { 1f, 2f };
            dataPoints[0] = new float[] { 0f, 1f };

            StringWriter stringWriter = new StringWriter();
            FloatTableSerializer tableWriter = FloatTableSerializer.GetInstance(stringWriter);
            tableWriter.Write(dataPoints, this.GetRowHeader, this.GetColumnHeader);

            string serialized = stringWriter.ToString();

            float[][] readDataPoints = new float[2][];
            readDataPoints[1] = new float[] { -1f, -1f };
            readDataPoints[0] = new float[] { -1f, -1f };
            
            StringReader stringReader = new StringReader(serialized);
            FloatTableSerializer tableReader = FloatTableSerializer.GetInstance(stringReader);
            tableReader.Read(readDataPoints);

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    string errorMessage = string.Format ("Mismatch at [{0}][{1}]", x, y);
                    Assert.AreEqual(dataPoints[x][y], readDataPoints[x][y], 0.01, errorMessage);
                }
            }
        }

        private float GetRowHeader(int y)
        {
            return SawMillUtility.GetHeader(y, 2, 0f, 10f);
        }

        private float GetColumnHeader(int x)
        {
            return SawMillUtility.GetHeader(x, 2, 0f, 10f);
        }
    }
}
