///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// FragmentedStream.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NateW.Ssm;

namespace NateW.Ssm
{
    public class FragmentedStream : MemoryStream
    {
        private const int fragmentSize = 3;
        private Stream source;

        public override bool CanRead
        {
            get { return this.source.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this.source.CanSeek; }
        }

        public override bool CanTimeout
        {
            get
            {
                return this.source.CanTimeout;
            }
        }

        public override bool CanWrite
        {
            get { return this.source.CanWrite; }
        }

        public override long Length
        {
            get { return this.source.Length; }
        }

        public override long Position
        {
            get
            {
                return this.source.Position;
            }
            set
            {
                this.source.Position = value;
            }
        }

        private FragmentedStream(Stream source)
        {
            this.source = source;
        }

        public static FragmentedStream GetInstance(Stream source)
        {
            return new FragmentedStream(source);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.source.Seek(offset, origin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.source.Read(buffer, offset, fragmentSize);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.source.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            this.source.Flush();
        }

        public override void SetLength(long value)
        {
            this.source.SetLength(value);
        }
    }
}
