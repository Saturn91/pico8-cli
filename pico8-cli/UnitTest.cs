using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
        public static readonly string ORIGINAL_TEST_FOLDER_PATH = Program.INSTALLATION_PATH + "/test";
        public static readonly string LOCAL_TEST_PATH = "test";
        private static Dictionary<string, UnitTestFile> testFiles;

        private static string SanitizeShortPico8Ifs(string line)
        {
            string lineWithoutSpaces = line.Replace(" ", "");
            if (lineWithoutSpaces.Contains("if("))
            {
                int startIndex = line.IndexOf("(");
                int endIndex = -1;

                char[] lineAsChars = line.ToCharArray();
                int openBrackets = 0;
                for (int i = startIndex; i < line.Length; i++)
                {
                    if (lineAsChars[i] == '(') openBrackets += 1;
                    if (lineAsChars[i] == ')') openBrackets -= 1;
                    if (openBrackets == 0)
                    {
                        endIndex = i;
                        break;
                    }
                }

                if (endIndex != -1)
                {
                    lineAsChars[startIndex] = ' ';
                    lineAsChars[endIndex] = ' ';
                    line = new string(lineAsChars);
                    string newLine = line.Substring(0, endIndex) + " then " + line.Substring(endIndex + 1) + " end";
                    line = newLine;
                }
            }

            return line;
        }

        private static string SanitizeShortPlusEquals(string line)
        {
            if (line.Contains("+="))
            {
                // get variable to Assign
                string lineWithoutSpaces = line.Replace(" ", "");
                int endOfVariableToAssign = lineWithoutSpaces.IndexOf("+=");
                string variableToAssign = lineWithoutSpaces.Substring(0, endOfVariableToAssign);

                int startIndex = line.IndexOf("+=");                

                string newLine = variableToAssign + " = " + variableToAssign + " + " + line.Substring(startIndex + 2);
                line = newLine;
            }

            return line;
        }

        // sanitize with "!=" -> "~=" and x+=1 -> x = x+1, if(x) y -> if x then y end
        private static string SanitizePico8CodeLine(string line)
        {
            line = SanitizeShortPlusEquals(line);
            line = SanitizeShortPico8Ifs(line);
            line = line.Replace("!=", "~=");
            return line;
        }

        
        private static string[] ConvertPico8LuaLinesToLua(string[] luaLines)
        {
            for(int i=0; i < luaLines.Length; i++) luaLines[i] = SanitizePico8CodeLine(luaLines[i]);
           
            return luaLines;
        }

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
            RunTest(true);
        }

        public static void RunTest(bool showWarning)
        {
            // 1. search for test.lua files in lua folder and break if none are found
            testFiles = new Dictionary<string, UnitTestFile>();

            if(!Directory.Exists("lua"))
            {
                if(showWarning) Util.Error("Please verify that pico8-cli was started within a pico8-cli project, you can run pico8-cli init");
                return;
            }

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
                    testFiles[fileName].sourceFileLines = ConvertPico8LuaLinesToLua(testFiles[fileName].sourceFileLines);
                    testFiles[fileName].testFileLines = ConvertPico8LuaLinesToLua(testFiles[fileName].testFileLines);
                    testFileCounter++;
                } else
                {
                    testFiles.Remove(fileName);
                }
            }

            // 2. check if initial setup of the test framework has to be executed and if the files are at the installation folder
            if (!Directory.Exists(ORIGINAL_TEST_FOLDER_PATH))
            {
                Util.Info("your installation is not set up for testing, directory '.test' does not exist on the instalation path");
                return;
            }


            if (!Directory.Exists(LOCAL_TEST_PATH))
            {
                Directory.CreateDirectory(LOCAL_TEST_PATH);
                Directory.CreateDirectory(LOCAL_TEST_PATH + "/test_framework");
                Util.Debug("created " + LOCAL_TEST_PATH + " Folder");
                Util.CopyPasteFromDirectory(ORIGINAL_TEST_FOLDER_PATH, LOCAL_TEST_PATH);
            }
            

            // 3. initialize empty index.html file
            string[] initial_index_html = File.ReadAllLines(LOCAL_TEST_PATH + "/index.html");
            List<string> index_html = new List<string>();          

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
