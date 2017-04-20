using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NateW.Ssm.ApplicationLogic
{
    public class PathDisplayAdaptor : IEquatable<string>
    {
        private string path;

        public string Path
        {
            [DebuggerStepThrough()]
            get { return this.path; }
        }

        public PathDisplayAdaptor(string path)
        {
            this.path = path;
        }

        public override string ToString()
        {
            return System.IO.Path.GetFileNameWithoutExtension(this.path);
        }

        public bool Equals(string other)
        {
            return this.path == other;
        }
    }
}
