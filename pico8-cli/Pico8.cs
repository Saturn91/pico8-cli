using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace pico8_cli
{
    public class Pico8
    {
        private const string INITAL_UNPACK_FILE = @"pico8-cli.p8.config for #GAME_NAME:

last unpacked: #UNPACKED_DATE
last packed: never
last run: never
last build: never
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

        public static bool IsPico8CliProject()
        {
            return Directory.Exists(".pico8-cli");
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
                return UnPack(true);
            }

            Util.Error(Util.GetGameName() + " is already initialized");
            return false;
        }

        private static void AddReadme()
        {
            string readme = @"# __GAMENAME

This is a Pico8 Project created with the [pico8-cli](https://github.com/Saturn91/pico8-cli) by Saturn91, using the `pico8-cli init` command.

The internal structure of the native .p8 file got splitted in the lua/* and resources/* files. In Order to use this Project as intended you need to install pico8-cli on your system as described [here](https://github.com/Saturn91/pico8-cli#installation)

## Build the Project and run in Pico8
`pico8-cli run`

## Get more Information about pico8-cli
- [installation](https://github.com/Saturn91/pico8-cli#installation)
- [usage](https://github.com/Saturn91/pico8-cli#using-the-cli)
";

            File.WriteAllText("README.md", readme.Replace("__GAMENAME", Util.GetGameName()));
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
            }
            catch { }

            if (!File.Exists(".gitignore"))
            {
                File.WriteAllLines(".gitignore", gitignore);
            }
            else
            {
                List<string> gitignoreLinesAlreadyLoaded = new List<string>(File.ReadAllLines(".gitignore"));

                foreach (string lineToAdd in gitignore)
                {
                    if (!gitignoreLinesAlreadyLoaded.Contains(lineToAdd)) gitignoreLinesAlreadyLoaded.Add(lineToAdd);
                }

                File.WriteAllLines(".gitignore", gitignoreLinesAlreadyLoaded.ToArray());
            }
        }

        public static bool Init()
        {
            InitializeGitRepository();
            AddReadme();
            return InitializePico8CliProject();
        }

        public static bool Pack()
        {
            // 1. create backup of the original file (with before_pack)
            CreateBackupOfPico8File("before_pack");

            //2. extract lua and other resource files
            List<string> projectContent = new List<string>();
            foreach (string pico8Tag in P8_TAGS)
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

        public static void UpdateProjectConfigFile(string cmdName)
        {

            string configFile = Program.PROJECT_CONFIG_FILE_PATH;
            string[] configFileLines;

            if (!File.Exists(configFile))
            {
                string content = INITAL_UNPACK_FILE
                    .Replace("#GAME_NAME", Util.GetGameName())
                    .Replace("#UNPACKED_DATE", DateTime.Now.ToString());

                configFileLines = content.Split(System.Environment.NewLine);
            }
            else
            {
                configFileLines = Util.GetFileLines(configFile);
            }

            switch (cmdName)
            {
                case "unpack":
                    for (int i = 0; i < configFileLines.Length; i++)
                    {
                        if (configFileLines[i].StartsWith("last unpacked:")) configFileLines[i] = "last unpacked: " + DateTime.Now.ToString();
                    }
                    break;
                case "pack":
                    for (int i = 0; i < configFileLines.Length; i++)
                    {
                        if (configFileLines[i].StartsWith("last packed:")) configFileLines[i] = "last packed: " + DateTime.Now.ToString();
                    }
                    break;
                case "run":
                    for (int i = 0; i < configFileLines.Length; i++)
                    {
                        if (configFileLines[i].StartsWith("last run:")) configFileLines[i] = "last run: " + DateTime.Now.ToString();
                    }
                    break;
                case "build":
                    for (int i = 0; i < configFileLines.Length; i++)
                    {
                        if (configFileLines[i].StartsWith("last build:")) configFileLines[i] = "last build: " + DateTime.Now.ToString();
                    }
                    break;
                    
            }

            File.WriteAllLines(configFile, configFileLines);
        }

        private const string backupPath = ".pico8-cli/backups/";
        private const int maxBackupFileNum = 10;

        public static void CreateBackupOfPico8File(string prefix)
        {
            // if there is no already packed file skip this part
            try
            {
                string[] lines = File.ReadAllLines(Util.GetGameName() + ".p8");

                //create backup
                Directory.CreateDirectory(".pico8-cli/backups");
                string datePrefix = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
                File.WriteAllLines(backupPath + datePrefix + "_" + Util.GetGameName() + "." + prefix + ".p8", lines);

                // delete oldest
                string[] oldFiles = Directory.GetFiles(backupPath);

                int localMaxBackupFiles = maxBackupFileNum;

                try
                {
                    localMaxBackupFiles = int.Parse(Program.GLOBAL_SETTINGS[GlobalSettings.Values.max_backup_file_cnt]);
                    Util.Debug("max files:" + localMaxBackupFiles);
                }
                catch { }

                if (oldFiles.Length > localMaxBackupFiles)
                {
                    for(int i = 0; i < oldFiles.Length - localMaxBackupFiles; i++)
                    {
                        File.Delete(oldFiles[i]);
                    }
                }
            }
            catch { }
        }

        private static void CreateRestFileContent(string[] linesBefore, string[] linesAfter)
        {
            List<string> lines = new List<string>(linesBefore);
            lines.Add("__UNPACKED");
            lines.AddRange(linesAfter);
            foreach (string line in linesAfter)
            {
                Util.Debug(line);
            }
            Directory.CreateDirectory("meta");
            File.WriteAllLines(Program.REST_OF_FILE_PATH, lines);
        }

        public static bool UnPack(bool doOverride, bool noBackup = false)
        {
            if (Directory.Exists("lua") || Directory.Exists(Program.RESOURCE_FOLDER))
            {
                if (!doOverride)
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
                        if(file.Name.EndsWith(".lua") &! file.Name.EndsWith(".test.lua")) file.Delete();
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

            if (!noBackup) CreateBackupOfPico8File("before_unpack");

            int lineBeginning = Int32.MaxValue;
            int lineEnd = -1;

            //actual unpack
            foreach (string tag in P8_TAGS)
            {
                UnpackInfo info;
                if (tag == "__lua__")
                {
                    info = Lua.Unpack(lines);
                }
                else
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

        public static CommandState Rollback(int steps = -1)
        {
            string[] backupFilePaths = Directory.GetFiles(backupPath);
            int maxNumberOfBackups = backupFilePaths.Length;

            int selectedFile = steps;

            if (selectedFile == -1)
            {
                bool invalidEntry = true;

                if (maxNumberOfBackups == 0)
                {
                    Util.Error("There are not yet Backup files to restore from...");
                    return CommandState.WRONG_PARAMS;
                }

                while (invalidEntry)
                {
                    Console.WriteLine();
                    Console.WriteLine("     " + backupPath);
                    for (int i = 0; i < backupFilePaths.Length; i++)
                    {
                        string fileContentSymbol = i == backupFilePaths.Length - 1 ? " \\-" : " |-";
                        Console.WriteLine("  " + (i + 1) + ":   " + fileContentSymbol + backupFilePaths[i].Substring(backupPath.Length));
                    }
                    Console.WriteLine();
                    Console.WriteLine();

                    Console.WriteLine("please type the number of the file you whish to restore from");
                    Console.WriteLine("enter a number / any key + [ENTER] to exit");
                    string userInput = Console.ReadLine();
                    try
                    {
                        selectedFile = int.Parse(userInput);
                        invalidEntry = false;
                        if (selectedFile < 1 || selectedFile > maxNumberOfBackups) invalidEntry = true;
                    }
                    catch
                    {
                        return CommandState.CANCEL;
                    }
                }
            }                      

            if (selectedFile == -1)
            {
                Util.Error("Please provide a value for steps=number which is > 0 and <= " + maxNumberOfBackups);
                return CommandState.FAILED;
            }

            try
            {
                string restorationFile = backupFilePaths[maxNumberOfBackups - selectedFile];
                File.Copy(restorationFile, Util.GetGameName() + ".p8", true);
                Util.Info("restored from file: " + restorationFile);
                UnPack(true, true);
                return CommandState.SUCCESS;
            } catch(Exception e)
            {
                Util.Error("Something went wrong while coping the backup file... \n" + e.ToString());
                return CommandState.FAILED;
            }           
        }
    }
}
