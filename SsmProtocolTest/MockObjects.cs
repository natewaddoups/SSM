using System;
using System.Collections.Generic;
using System.Text;
using NateW.Ssm.Protocol;

namespace NateW.Ssm.Protocol.Test
{
    internal class MockParameter : Parameter
    {
        private static Conversion mockConversion = Conversion.GetInstance("cubits", "x", "0.00");

        public MockParameter(
            ParameterSource source,
            string id,
            string name)
            : base(source, id, name, new Conversion[] { mockConversion }, null)
        {
        }

        public MockParameter(
            ParameterSource source,
            string id,
            string name,
            IEnumerable<Parameter> dependencies)
            : base(
            source,
            id,
            name, 
            new Conversion[] { mockConversion }, 
            dependencies)
        {
        }
    }

    internal class MockParameterSource : ParameterSource
    {
        public MockParameterSource()
            : base("Mock")
        {
            Parameter m1 = new MockParameter(this, "M1", "Mock 1");
            this.AddParameter(m1);
            Parameter m2 = new MockParameter(this, "M2", "Mock 2");
            this.AddParameter(m2);
            Parameter mc1 = new MockParameter(this, "MC1", "Mock Calculated", new Parameter[] { m1, m2 });
            this.AddParameter(mc1);
        }
    }
}
