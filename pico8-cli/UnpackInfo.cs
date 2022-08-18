using System;
using System.Collections.Generic;
using System.Text;

namespace pico8_cli
{
    public class UnpackInfo
    {
        public int firstLine { get; }
        public int lastLine { get; }

        public string[] lines { get; }

        public UnpackInfo(int firstLine, int lastLine)
        {
            this.firstLine = firstLine;
            this.lastLine = lastLine;
        }

        public UnpackInfo(int firstLine, int lastLine, string[] lines)
        {
            this.firstLine = firstLine;
            this.lastLine = lastLine;
            this.lines = lines;
        }
    }
}
