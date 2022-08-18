using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace pico8_cli
{
    class UnitTestFile
    {
        public string[] testFileLines;
        public string[] sourceFileLines;

        public bool IsComplete => testFileLines != null && sourceFileLines != null;
    }

    class UnitTest
    {
        private static Dictionary<string, UnitTestFile> testFiles;

        private static void AddTestFile(string name, UnitTestFile testFile)
        {
            if(testFiles.ContainsKey(name))
            {

                if (testFile.testFileLines != null) testFiles[name].testFileLines = testFile.testFileLines;
                if (testFile.sourceFileLines != null) testFiles[name].sourceFileLines = testFile.sourceFileLines;
            } else
            {
                testFiles.Add(name, testFile);
            }
        }

        public static void RunTest()
        {
            // 1. search for test.lua files in lua folder and break if none are found
            testFiles = new Dictionary<string, UnitTestFile>();

            string[] luaFiles = Directory.GetFiles("lua");
            foreach(string luaFile in luaFiles)
            {
                string[] fileNameParts = luaFile.Split(".");
                if (luaFile.EndsWith(".test.lua"))
                {
                    string name = fileNameParts.SubArray(0, fileNameParts.Length - 2).Join(".");
                    UnitTestFile testFile = new UnitTestFile();
                    testFile.testFileLines = File.ReadAllLines(luaFile);
                    AddTestFile(name, testFile);
                }

                if (luaFile.EndsWith(".lua"))
                {
                    string name = fileNameParts.SubArray(0, fileNameParts.Length - 1).Join(".");
                    UnitTestFile testFile = new UnitTestFile();
                    testFile.sourceFileLines = File.ReadAllLines(luaFile);
                    AddTestFile(name, testFile);
                }
            }

            int testFileCounter = 0;

            foreach(string fileName in testFiles.Keys)
            {
                if (testFiles[fileName].IsComplete)
                {
                    testFileCounter++;
                } else
                {
                    testFiles.Remove(fileName);
                }
            }

            // 2. initialize empty index.html file
            string[] initial_index_html = File.ReadAllLines(".test/index.html");
            List<string> index_html = new List<string>();


            // 3. paste code to test into index.html file (and sanitize with "!=" -> "~=" and x=+1 -> x = x+1, if(x) y -> if x then y end
            // TODO

            // 4. past test code into index.html


            for (int i = 0; i < initial_index_html.Length; i++)
            {
                string nextLine = initial_index_html[i];
                
                if (initial_index_html[i] == "__pico8-cli_code_to_test__")
                {
                    foreach (string fileName in testFiles.Keys)
                    {
                        index_html.AddRange(testFiles[fileName].sourceFileLines);
                    }
                } else if (initial_index_html[i] == "__pico8-cli_tests__")
                {
                    foreach (string fileName in testFiles.Keys)
                    {
                        index_html.Add("log('[RUN ]: " + fileName + ".test.lua')");
                        index_html.AddRange(testFiles[fileName].testFileLines);
                    }
                } else
                {
                    index_html.Add(nextLine);
                }                
            }

            File.WriteAllLines("index.html", index_html);

            // 5. open index.html in browser
            Util.ExecuteCommandSync("cmd /c start index.html");
        }
    }
}
