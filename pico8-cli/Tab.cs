using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace pico8_cli
{
    public class Tab
    {
        private string name;
        private string[] content;
        private int number;

        public Tab(string name, int number)
        {
            this.name = name;
            this.number = number;
        }

        public void SetContent(string[] codeLines)
        {
            content = codeLines;
            Unpack();
        }

        private void Unpack()
        {
            string numberFiller = "";
            if (number < 10) numberFiller = "0" + numberFiller;
            string tabNumberString = numberFiller + number;
            Directory.CreateDirectory("lua");
            string numberedTabName = tabNumberString + "_" + name;
            File.WriteAllLines("lua/" + numberedTabName + ".lua", content);
        }
    }
}
