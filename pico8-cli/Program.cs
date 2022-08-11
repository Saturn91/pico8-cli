using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace pico8_cli
{
    class GlobalSettings
    {
        public enum Values
        {
            localRunCommand,
            pico8Version
        }

        public string localRunCommand { get; private set; }
        public string pico8Version { get; private set; }

        public static Dictionary<Values, string> EmptySettings()
        {
            Dictionary<Values, string> values = new Dictionary<Values, string>();
            foreach (Values value in Enum.GetValues(typeof(Values)))
            {
                values.Add(value, "");
            }

            return values;
        }

        public static Dictionary<GlobalSettings.Values, string> LoadFromFile()
        {
            
            string[] loadedLines = File.ReadAllLines(Program.GLOBAL_CONFIG_FILE_PATH);
            Dictionary<Values, string> loadedValues = EmptySettings();

            foreach (string line in loadedLines)
            {
                foreach(Values value in Enum.GetValues(typeof(Values)))
                {
                    
                    string loadedValue = Util.GetPropertyFromLine(line, value.ToString() + ":");
                    if (loadedValue != null) loadedValues[value] = loadedValue;
                }
            }

            Util.Debug("global settings loaded");
            return loadedValues;
        }

        private static void SaveToFile(Dictionary<Values, string> values)
        {
            List<string> globalSettingsAsStringList = new List<string>();

            foreach (Values value in Enum.GetValues(typeof(Values)))
            {
                globalSettingsAsStringList.Add(value.ToString() + ": " + values[value]);
            }


            try
            {
                File.WriteAllLines(Program.GLOBAL_CONFIG_FILE_PATH, globalSettingsAsStringList.ToArray());
                Util.Debug("create/override global config");
            } catch(Exception e)
            {
                Util.Debug("not able to create config file: " + Program.GLOBAL_CONFIG_FILE_PATH + " please retry in a terminal with administrator right, or create the file manually");
            }
            
        }

        public static Dictionary<Values, string> Init()
        {
            Dictionary<Values, string> initialValues = EmptySettings();
            initialValues[Values.pico8Version] = "version 36";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) initialValues[Values.localRunCommand] = "\"C:\\Program Files (x86)\\PICO-8\\pico8.exe\" -run ";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) initialValues[Values.localRunCommand] = "/opt/pico-8 - run ";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) initialValues[Values.localRunCommand] = "opt/pico-8 - run ";

            Util.Debug("initialy creating global config file");
            SaveToFile(initialValues);
            

            return initialValues;
        }
    }

    class Setup
    {
        public static Dictionary<string, bool> properties = new Dictionary<string, bool>()
        {
            { "debug", false },
            { "override", false }
        };

        public static void HandleArgs(string[] args)
        {
            if (args == null || args.Length <= 1) return;

            for (int i = 1; i < args.Length; i++)
            {
                string key = args[i];
                if (properties.ContainsKey(key))
                {
                    Console.WriteLine("[Prop]: enabled: " + key);
                    properties[key] = true;
                } else
                {
                    Console.WriteLine("[Prop]: invalid property: " + key);
                }              
            }                
        }

        public static string GetCurrentWorkingSpace()
        {
            return Environment.CurrentDirectory;
        }

        public static Dictionary<GlobalSettings.Values, string> GetGlobalSettings()
        {
            return File.Exists(Program.GLOBAL_CONFIG_FILE_PATH) ? GlobalSettings.LoadFromFile() : GlobalSettings.Init();
        }
    }

    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int end)
        {
            T[] result = new T[end - offset];
            Array.Copy(array, offset, result, 0, end - offset);
            return result;
        }

        public static bool Contains<T>(this T[] array, T value)
        {
            foreach(T entry in array)
            {
                if (entry.Equals(value)) return true;
            }

            return false;
        }
    }

    class Util
    {
        public static void Debug(string msg)
        {
            if (!Setup.properties["debug"]) return;
            Console.WriteLine("[debg]: " + msg);
        }

        public static void Info(string msg)
        {
            Console.WriteLine("[info]: " + msg);
        }

        public static void Error(string err)
        {
            Console.WriteLine("[err.]: " + err);
        }

        public static string ArrayToString<T>(T[] array)
        {
            string result = "";
            foreach (T entry in array) result += entry + ", ";
            return result;
        }

        public static string GetGameName()
        {
            string[] folders = Program.current_path.Split("\\");
            return folders[folders.Length - 1];
        }

        public static string[] GetFileLines(string file)
        {
            List<string> lines = new List<string>();
            foreach (string line in File.ReadLines(@file))
            {
                lines.Add(line);
            }

            return lines.ToArray();
        }

        public static string GetPropertyFromLine(string line, string property)
        {
            if (line.StartsWith(property))
            {
                return line.Substring(line.IndexOf(":") + 2);
            }

            return null;
        }

        public static void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
            }
            catch (Exception objException)
            {
                Error(command + " failed.: " + objException.ToString());
            }
        }
    }

    class Pico8
    {
        private const string INITAL_UNPACK_FILE = @"pico8-cli.p8.config for #GAME_NAME:

last unpacked: #UNPACKED_DATE
last packed: never
last run: never
";

        public static readonly string[] P8_TAGS =
        {
            "__lua__",
            "__gfx__",
            "__gff__",
            "__label__",
            "__map__",
            "__sfx__",
            "__music__"
        };

        private static void StartLog()
        {
            Util.Debug("starting [" + Program.current_mode + "] process at: " + Program.current_path);
        }

        private static void EndLog()
        {
            Util.Debug("[" + Program.current_mode + "] has succeded");
        }

        public static void Run(Program.RUN_OPTIONS mode) {
            StartLog();
            bool succeded = false;
            switch (mode)
            {
                case Program.RUN_OPTIONS.init:
                    succeded = Init();
                    break;
                case Program.RUN_OPTIONS.unpack:
                    succeded = UnPack();
                    break;
                case Program.RUN_OPTIONS.pack:
                    succeded = Pack();
                    break;
                case Program.RUN_OPTIONS.run:
                    Pack();
                    Util.ExecuteCommandSync(Program.GLOBAL_SETTINGS[GlobalSettings.Values.localRunCommand] + Util.GetGameName() + ".p8");
                    succeded = true;
                    break;
            }

            if (succeded)
            {
                UpdateProjectConfigFile(mode);
                EndLog();
            }
        }

        private static bool InitializePico8CliProject()
        {
            if (!File.Exists(Program.PROJECT_CONFIG_FILE_PATH))
            {
                Directory.CreateDirectory(".pico8-cli");
                string empty_pico8_project = @"pico-8 cartridge // http://www.pico-8.com
#VERSION
__lua__
--main

function _init()
	
end

function _update()
	
end

function _draw()
	cls()
	print('hello world')
end
__gfx__
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00700700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00077000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00077000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00700700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__sfx__
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
";


                if (!File.Exists(Util.GetGameName() + ".p8"))
                {
                    File.WriteAllText(Util.GetGameName() + ".p8", empty_pico8_project.Replace("#VERSION", Program.GLOBAL_SETTINGS[GlobalSettings.Values.pico8Version]).ToString());
                }
                else
                {
                    Util.Info("File: " + Util.GetGameName() + ".p8" + " already exists, initialized project with existing file");
                }
                UpdateProjectConfigFile(Program.RUN_OPTIONS.init);
                return UnPack();
            }

            Util.Error(Util.GetGameName() + " is already initialized");
            return false;
        }

        private static void InitializeGitRepository()
        {
            string[] gitignore =
            {
                ".pico8-cli",
                Util.GetGameName() + ".p8"
            };

            try
            {
                Util.ExecuteCommandSync("git init");
            } catch { }

            if (!File.Exists(".gitignore"))
            {
                File.WriteAllLines(".gitignore", gitignore);
            } else
            {
                List<string> gitignoreLinesAlreadyLoaded = new List<string>(File.ReadAllLines(".gitignore"));

                foreach(string lineToAdd in gitignore)
                {
                    if (!gitignoreLinesAlreadyLoaded.Contains(lineToAdd)) gitignoreLinesAlreadyLoaded.Add(lineToAdd);
                }

                File.WriteAllLines(".gitignore", gitignoreLinesAlreadyLoaded.ToArray());
            }
        }

        private static bool Init()
        {
            InitializeGitRepository();
            return InitializePico8CliProject();
        }

        private static bool Pack() {
            // 1. create backup of the original file (with before_pack)
            CreateBackupOfPico8File("before_pack");

            //2. extract lua and other resource files
            List<string> projectContent = new List<string>();
            foreach(string pico8Tag in P8_TAGS)
            {
                string[] linesToAdd = pico8Tag == "__lua__" ? Lua.Pack() : Pico8DataTagExtractor.ExtractTagResourceFile(pico8Tag);
                if (linesToAdd.Length > 0)
                {
                    projectContent.AddRange(linesToAdd);
                }
                
            }

            // 3. override file
            List<string> packedPico8Lines = new List<string>();
            List<string> restOfFileLines = new List<string>(File.ReadAllLines(Program.REST_OF_FILE_PATH));

            foreach (string line in restOfFileLines)
            {
                if (line == "__UNPACKED")
                {
                    packedPico8Lines.AddRange(projectContent);
                }
                else
                {
                    packedPico8Lines.Add(line);
                }
            }

            File.WriteAllLines(Util.GetGameName() + ".p8", packedPico8Lines);

            return true;
        }

        private static void UpdateProjectConfigFile(Program.RUN_OPTIONS mode)
        {

            string configFile = Program.PROJECT_CONFIG_FILE_PATH;
            string[] configFileLines;

            if (!File.Exists(configFile))
            {
                string content = INITAL_UNPACK_FILE
                    .Replace("#GAME_NAME", Util.GetGameName())
                    .Replace("#UNPACKED_DATE", DateTime.Now.ToString());

                configFileLines = content.Split(System.Environment.NewLine);
            } else
            {
                configFileLines = Util.GetFileLines(configFile);
            }

            switch (mode)
            {
                case Program.RUN_OPTIONS.unpack:
                    for(int i = 0; i < configFileLines.Length; i++)
                    {
                        if (configFileLines[i].StartsWith("last unpacked:")) configFileLines[i] = "last unpacked: " + DateTime.Now.ToString();
                    }
                    break;
                case Program.RUN_OPTIONS.pack:
                    for (int i = 0; i < configFileLines.Length; i++)
                    {
                        if (configFileLines[i].StartsWith("last packed:")) configFileLines[i] = "last packed: " + DateTime.Now.ToString();
                    }
                    break;
                case Program.RUN_OPTIONS.run:
                    for (int i = 0; i < configFileLines.Length; i++)
                    {
                        if (configFileLines[i].StartsWith("last run:")) configFileLines[i] = "last run: " + DateTime.Now.ToString();
                    }
                    break;
            }

            File.WriteAllLines(configFile, configFileLines);
        }

        public static void CreateBackupOfPico8File(string prefix)
        {
            // if there is no already packed file skip this part
            try
            {
                string[] lines = File.ReadAllLines(Util.GetGameName() + ".p8");

                //create backup
                Directory.CreateDirectory(".pico8-cli/backups");
                string datePrefix = DateTime.Now.ToString()
                    .Replace(".", "")
                    .Replace(" ", "")
                    .Replace(":", "") + "_";
                File.WriteAllLines(".pico8-cli/backups/" + datePrefix + Util.GetGameName() + "." + prefix + ".p8", lines);
            } catch(Exception e) {}           
        }

        private static void CreateRestFileContent(string[] linesBefore, string[] linesAfter)
        {
            List<string> lines = new List<string>(linesBefore);
            lines.Add("__UNPACKED");
            lines.AddRange(linesAfter);
            foreach(string line in linesAfter)
            {
                Util.Debug(line);
            }
            Directory.CreateDirectory("meta");
            File.WriteAllLines(Program.REST_OF_FILE_PATH, lines);
        }

        private static bool UnPack() {
            if (Directory.Exists("lua") || Directory.Exists(Program.RESOURCE_FOLDER))
            {
                if (!Setup.properties["override"])
                {
                    Util.Info("The directory 'lua' already exists, by unpacking it will get overriden! if you are sure, run 'unpack override'");
                    return false;
                }                

                if (Directory.Exists("lua"))
                {
                    //clear lua folder
                    DirectoryInfo di = new DirectoryInfo("lua");

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }
                
                if (Directory.Exists(Program.RESOURCE_FOLDER))
                {
                    //clear lua folder
                    DirectoryInfo di = new DirectoryInfo(Program.RESOURCE_FOLDER);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }
            }

            string fileToUnpack = Util.GetGameName() + ".p8";
            if (!File.Exists(fileToUnpack))
            {
                Util.Error(fileToUnpack + " does not exist...");
                return false;
            }

            string[] lines = File.ReadAllLines(Util.GetGameName() + ".p8");

            CreateBackupOfPico8File("before_unpack");

            int lineBeginning = Int32.MaxValue;
            int lineEnd = -1;

            //actual unpack
            foreach(string tag in P8_TAGS)
            {
                UnpackInfo info;
                if (tag == "__lua__")
                {
                    info = Lua.Unpack(lines);
                } else
                {
                    info = Pico8DataTagExtractor.UnPack(tag, lines);
                }

                if (info != null)
                {
                    if (info.firstLine < lineBeginning) lineBeginning = info.firstLine;
                    if (info.lastLine >= lineEnd) lineEnd = info.lastLine;
                }           
            }

            string[] before = lines.SubArray(0, lineBeginning);
            string[] after = lines.SubArray(lineEnd + 1, lines.Length);
            CreateRestFileContent(before, after);

            return true;
        }
    }

    class Tab
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

    class Lua
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

                string[] lines = File.ReadAllLines(tab);
                luaLines.AddRange(lines);
                if (i < tabFiles.Length - 1)
                {
                    luaLines.Add("-->8");
                }
            }

            return luaLines.ToArray();
        }
    }

    class Pico8DataTagExtractor
    {
        public static UnpackInfo GetFileLinesOfTag(string tag, string[] fileLines)
        {
            bool withinTag = false;
            List<string> contentLines = new List<string>();
            int firstLine = -1;
            int lastLine = -1;

            for(int i = 0; i < fileLines.Length; i++)
            {
                string line = fileLines[i];

                if (line == tag)
                {
                    firstLine = i;
                    withinTag = true;
                } else if (withinTag)
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

    class Program
    {
        public const string PRG_NAME = "pico8-cli";
        public readonly static string INSTALLATION_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string current_path = Setup.GetCurrentWorkingSpace();
        public enum RUN_OPTIONS { init, unpack, pack, run };
        public static readonly string[] RUN_OPTIONS_STRINGS = Enum.GetNames(typeof(RUN_OPTIONS));
        public static RUN_OPTIONS current_mode = RUN_OPTIONS.init;
        public static readonly string PROJECT_CONFIG_FILE_PATH = ".pico8-cli/" + Util.GetGameName() + ".p8.config";
        public static readonly string REST_OF_FILE_PATH = "meta/" + "restOfFile.p8";
        public static readonly string GLOBAL_CONFIG_FILE_PATH = INSTALLATION_PATH + "/pico8-cli.config";
        public const string RESOURCE_FOLDER = "resources";

        public static Dictionary<GlobalSettings.Values, string> GLOBAL_SETTINGS = Setup.GetGlobalSettings();

        static void LogInstallAndLocationInfos()
        {
            Util.Debug(PRG_NAME + " installed at: " + INSTALLATION_PATH);
            Util.Debug(PRG_NAME + " opened at: " + current_path);
        }

        static bool SetCurrentMode(string[] args)
        {
            string errorMsgNoRunOption = "please provide one of the options: " + Util.ArrayToString<string>(RUN_OPTIONS_STRINGS);
            if (args == null || args.Length <= 0)
            {
                Util.Info(errorMsgNoRunOption);
                return false;
            }
            if (Array.IndexOf(RUN_OPTIONS_STRINGS, args[0]) == -1)
            {
                Util.Info(errorMsgNoRunOption);
                return false;
            }

            current_mode = (RUN_OPTIONS) Array.IndexOf(RUN_OPTIONS_STRINGS, args[0]);

            return true;
        }

        static int Main(string[] args)
        {
            Setup.HandleArgs(args);
            LogInstallAndLocationInfos();

            if (!SetCurrentMode(args)) return 1;

            Util.Info(current_mode.ToString());

            Pico8.Run(current_mode);

            return 1;
        }
    }
}
