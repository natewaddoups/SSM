using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NateW.Ssm.ApplicationLogic
{
    class SawMillWriter
    {
        private Stream stream;
        private StreamWriter writer;

        private SawMillWriter(Stream stream)
        {
            this.stream = stream;
            this.writer = new StreamWriter(this.stream);
        }

        public static SawMillWriter GetInstance(Stream stream)
        {
            SawMillWriter instance = new SawMillWriter(stream);
            return instance;
        }

        public void Write(IList<SawMillScreen> screens)
        {
            foreach (SawMillScreen screen in screens)
            {
                if (screen is SixPackScreen)
                {
                    continue;
                }

                this.WriteScreenHeader(screen);
                screen.WriteTo(this.writer);
            }
            this.writer.Flush();
        }

        private void WriteScreenHeader(SawMillScreen screen)
        {
            this.writer.WriteLine(screen.GetType().FullName);
        }
    }

    class SawMillReader
    {
        private Stream stream;
        private StreamReader reader;

        private SawMillReader(Stream stream)
        {
            this.stream = stream;
            this.reader = new StreamReader(stream);
        }

        public static SawMillReader GetInstance(Stream stream)
        {
            SawMillReader instance = new SawMillReader(stream);
            return instance;
        }

        public void Read(IList<SawMillScreen> screens)
        {
            string screenType;
            while (this.TryReadScreenHeader(out screenType))
            {
                SawMillScreen screen = GetScreen(screens, screenType);
                if (screen == null)
                {
                    continue;
                }
                screen.ReadFrom(this.reader);
            }
        }

        private bool TryReadScreenHeader(out string screenType)
        {
            screenType = reader.ReadLine();
            return screenType != null;
        }

        private SawMillScreen GetScreen(IList<SawMillScreen> screens, string screenType)
        {
            foreach (SawMillScreen screen in screens)
            {
                if (screen.GetType().FullName == screenType)
                {
                    return screen;
                }
            }
            return null;
        }
    }
}
