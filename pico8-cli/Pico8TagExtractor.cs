using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace pico8_cli
{
    public class Pico8DataTagExtractor
    {
        public static UnpackInfo GetFileLinesOfTag(string tag, string[] fileLines)
        {
            bool withinTag = false;
            List<string> contentLines = new List<string>();
            int firstLine = -1;
            int lastLine = -1;

            for (int i = 0; i < fileLines.Length; i++)
            {
                string line = fileLines[i];

                if (line == tag)
                {
                    firstLine = i;
                    withinTag = true;
                }
                else if (withinTag)
                {
                    if (Pico8.P8_TAGS.Contains(line))
                    {
                        lastLine = i;
                        break;
                    }
                    contentLines.Add(line);
                }

                lastLine = fileLines.Length - 1;
            }

            if (contentLines.Count <= 0) return null;

            return new UnpackInfo(firstLine, lastLine, contentLines.ToArray());
        }

        public static string[] ExtractTagResourceFile(string tag)
        {
            string filePath = Program.RESOURCE_FOLDER + "/" + tag + ".txt";

            List<string> tagContentLines = new List<string>();
            if (!File.Exists(filePath)) return new string[0];

            tagContentLines.Add(tag);
            tagContentLines.AddRange(File.ReadAllLines(filePath));

            return tagContentLines.ToArray();
        }

        public static UnpackInfo UnPack(string tag, string[] fileLines)
        {
            UnpackInfo unpackInfo = GetFileLinesOfTag(tag, fileLines);
            if (unpackInfo != null)
            {
                UnpackTagContentIntoResourceFile(tag, unpackInfo.lines);
            }
            return unpackInfo;
        }

        public static void UnpackTagContentIntoResourceFile(string tag, string[] content)
        {
            Directory.CreateDirectory(Program.RESOURCE_FOLDER);
            File.WriteAllLines(Program.RESOURCE_FOLDER + "/" + tag + ".txt", content);
        }
    }
}
