using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pico8_cli
{
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
    }

    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int end)
        {
            T[] result = new T[end - offset];
            Array.Copy(array, offset, result, 0, end - offset);
            return result;
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

__Lua tabs:
__end_tabs
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
                    Util.ExecuteCommandSync("\"C:\\Program Files (x86)\\PICO-8\\pico8.exe\" -run " + Util.GetGameName() + ".p8");
                    break;
            }

            if (succeded)
            {
                UpdateConfigFile(mode);
                EndLog();
            }
        }

        private static bool Init()
        {
            if (!File.Exists(Program.CONFIG_FILE_PATH))
            {
                Directory.CreateDirectory(".pico8-cli");
                string empty_pico8_project = @"pico-8 cartridge // http://www.pico-8.com
version 36
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
                    File.WriteAllText(Util.GetGameName() + ".p8", empty_pico8_project.ToString());
                } else
                {
                    Util.Info("File: " + Util.GetGameName() + ".p8" + " already exists, initialized project with existing file");
                }
                UpdateConfigFile(Program.RUN_OPTIONS.init);
                return UnPack();
            }

            Util.Error(Util.GetGameName() + " is already initialized");
            return false;
        }

        private static bool Pack() {
            // 1. get all tabs from config file
            string[] configLines = File.ReadAllLines(Program.CONFIG_FILE_PATH);
            List<string> tabFiles = new List<string>();

            bool inTabsArea = false;
            for(int i = 0; i < configLines.Length; i++)
            {
                string line = configLines[i];
                if (!inTabsArea && line == "__Lua tabs:")
                {
                    inTabsArea = true;
                } else if (line == "__end_tabs")
                {
                    break;
                } else if (inTabsArea)
                {
                    tabFiles.Add("lua/" + line + ".lua");
                }
            }

            // 2. for each tab insert the lines into the original .p8 file
            List<string> luaLines = new List<string>();
            for (int i = 0; i < tabFiles.Count; i++)
            {
                string tab = tabFiles[i];

                string[] lines = File.ReadAllLines(tab);
                luaLines.AddRange(lines);
                if (i < tabFiles.Count -1)
                {
                    luaLines.Add("-->8");
                }
            }

            // 3. create backup of the original file (with before_pack)
            CreateBackupOfPico8File("before_pack");

            // 4. override file
            List<string> packedPico8Lines = new List<string>();
            List<string> restOfFileLines = new List<string>(File.ReadAllLines("." + Program.PRG_NAME + "/" + "restOfFile.p8"));

            foreach(string line in restOfFileLines)
            {
                if (line == "__UNPACKED")
                {
                    packedPico8Lines.Add("__lua__");
                    packedPico8Lines.AddRange(luaLines);
                } else
                {
                    packedPico8Lines.Add(line);
                }
            }

            File.WriteAllLines(Util.GetGameName() + ".p8", packedPico8Lines);

            return true;
        }

        private static void UpdateConfigFile(Program.RUN_OPTIONS mode)
        {

            string configFile = Program.CONFIG_FILE_PATH;
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
                        if (configFileLines[i].Contains("last unpacked:")) configFileLines[i] = "last unpacked: " + DateTime.Now.ToString();
                    }
                    break;
                case Program.RUN_OPTIONS.pack:
                    for (int i = 0; i < configFileLines.Length; i++)
                    {
                        if (configFileLines[i].Contains("last packed:")) configFileLines[i] = "last packed: " + DateTime.Now.ToString();
                    }
                    break;
                case Program.RUN_OPTIONS.run:
                    for (int i = 0; i < configFileLines.Length; i++)
                    {
                        if (configFileLines[i].Contains("last run:")) configFileLines[i] = "last run: " + DateTime.Now.ToString();
                    }
                    break;
            }

            File.WriteAllLines(configFile, configFileLines);
        }

        private static void CreateBackupOfPico8File(string prefix)
        {
            string[] lines = File.ReadAllLines(Util.GetGameName() + ".p8");

            //create backup
            Directory.CreateDirectory(".pico8-cli/backups");
            string datePrefix = DateTime.Now.ToString()
                .Replace(".", "")
                .Replace(" ", "")
                .Replace(":", "") + "_";
            File.WriteAllLines(".pico8-cli/backups/" + datePrefix + Util.GetGameName() + "." + prefix + ".p8", lines);
        }

        private static void CreateRestFileContent(string[] linesBefore, string[] linesAfter)
        {
            List<string> lines = new List<string>(linesBefore);
            lines.Add("__UNPACKED");
            lines.AddRange(linesAfter);
            File.WriteAllLines(".pico8-cli/restOfFile.p8", lines);
        }

        private static bool UnPack() {
            if (Directory.Exists("lua"))
            {
                if (!Setup.properties["override"])
                {
                    Util.Info("The directory 'lua' already exists, by unpacking it will get overriden! if you are sure, run 'unpack override'");
                    return false;
                }                

                //clear lua folder
                DirectoryInfo di = new DirectoryInfo("lua");

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
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

            //actual unpack
            Unpack.UnpackInfo info = Unpack.Lua(lines);

            string[] before = lines.SubArray(0, info.firstLine - 1);
            string[] after = lines.SubArray(info.lastLine + 1, lines.Length-1);
            CreateRestFileContent(before, after);

            return true;
        }
    }

    class Tab
    {
        public static List<string> currentTabs;
        public static void InitTabs()
        {
            currentTabs = new List<string>();
        }

        public static void AddTab(string tab)
        {
            currentTabs.Add(tab);
        }

        public static void UpdateConfig()
        {
            List<string> configLines = new List<string>(File.ReadAllLines(Program.CONFIG_FILE_PATH));
            List<string> newConfigLines = new List<string>();
            for(int i = 0; i < configLines.Count; i++)
            {
                string line = configLines[i];
                newConfigLines.Add(line);
                if(configLines[i] == "__Lua tabs:")
                {
                    for(int j = 0; j < currentTabs.Count; j++)
                    {
                        newConfigLines.Add(currentTabs[j]);
                    }
                    break;
                }
            }

            newConfigLines.Add("__end_tabs");

            File.WriteAllLines(Program.CONFIG_FILE_PATH, newConfigLines.ToArray());
        }

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
            AddTab(numberedTabName);
        }
    }

    class Unpack
    {
        public class UnpackInfo
        {
            public int firstLine { get; }
            public int lastLine { get; }

            public UnpackInfo(int firstLine, int lastLine)
            {
                this.firstLine = firstLine;
                this.lastLine = lastLine;
            }
        }

        public static UnpackInfo Lua(string[] fileLines)
        {
            bool inLuaSection = false;
            int firstLuaLine = -1;
            int lastluaLine = -1;

            Tab.InitTabs();
            int tabCounter = 1;
            Tab actualTab = new Tab("main", tabCounter);
            List<string> currentTabContent = new List<string>();

            for (int i = 0; i < fileLines.Length; i++)
            {
                string line = fileLines[i];
                if (! inLuaSection )
                {
                    if (line == "__lua__")
                    {
                        inLuaSection = true;
                        firstLuaLine = i + 1;
                    }
                } else if (inLuaSection)
                {
                    //create new Tab
                    if (line == "-->8")
                    {
                        actualTab.SetContent(currentTabContent.ToArray());
                        currentTabContent.Clear();

                        tabCounter += 1;

                        string tabName = "tab";

                        string nextLine = fileLines[i + 1];
                        if (nextLine.StartsWith("--") && nextLine.Length > 2) tabName = nextLine.Substring(2);

                        actualTab = new Tab(tabName, tabCounter);
                    }
                    // other tag reached no longer within lua code
                    else if (Array.IndexOf(Pico8.P8_TAGS, line) != -1) {
                        lastluaLine = i - 1;
                        break;
                    } else {
                        currentTabContent.Add(line);
                    }                
                }
            }

            actualTab.SetContent(currentTabContent.ToArray());
            Tab.UpdateConfig();

            return new UnpackInfo(firstLuaLine, lastluaLine);
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
        public static readonly string CONFIG_FILE_PATH = ".pico8-cli/" + Util.GetGameName() + ".p8.config";

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
