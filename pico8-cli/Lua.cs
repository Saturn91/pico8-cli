using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace pico8_cli
{
    public class Lua
    {
        public static UnpackInfo Unpack(string[] fileLines)
        {
            //get lua lines
            UnpackInfo luaUnpacked = Pico8DataTagExtractor.GetFileLinesOfTag("__lua__", fileLines);

            int tabCounter = 1;
            Tab actualTab = new Tab("main", tabCounter);
            List<string> currentTabContent = new List<string>();

            for (int i = 0; i < luaUnpacked.lines.Length; i++)
            {
                string line = luaUnpacked.lines[i];

                //create new Tab
                if (line == "-->8")
                {
                    actualTab.SetContent(currentTabContent.ToArray());
                    currentTabContent.Clear();

                    tabCounter += 1;

                    string tabName = "tab";

                    string nextLine = luaUnpacked.lines[i + 1];
                    if (nextLine.StartsWith("--") && nextLine.Length > 2) tabName = nextLine.Substring(2);

                    actualTab = new Tab(tabName, tabCounter);
                }
                else
                {
                    currentTabContent.Add(line);
                }
            }

            actualTab.SetContent(currentTabContent.ToArray());

            return luaUnpacked;
        }

        public static string[] Pack()
        {
            string[] tabFiles = Directory.GetFiles("lua");

            // 2. for each tab insert the lines into the original .p8 file
            List<string> luaLines = new List<string>();
            luaLines.Add("__lua__");
            for (int i = 0; i < tabFiles.Length; i++)
            {
                string tab = tabFiles[i];
                if (tab.EndsWith(".lua") & !tab.EndsWith(".test.lua"))
                {
                    string[] lines = File.ReadAllLines(tab);
                    luaLines.AddRange(lines);
                    luaLines.Add("-->8");
                }
            }

            // remove -->8 (tab seperator) at the end
            luaLines.RemoveAt(luaLines.Count - 1);

            return luaLines.ToArray();
        }
    }
}
