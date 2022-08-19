﻿using System;
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

        public static bool IsPico8CliProject()
        {
            return Directory.Exists(".pico8-cli");
        }

        public static void Run(RUN_OPTIONS mode)
        {
            StartLog();
            bool succeded = false;

            if (mode != RUN_OPTIONS.init && !IsPico8CliProject())
            {
                Util.Error("currently not in a pico8-cli project, please run pico8-cli init");
                return;
            }

            switch (mode)
            {
                case RUN_OPTIONS.init:
                    succeded = Init();
                    break;
                case RUN_OPTIONS.unpack:
                    succeded = UnPack(Setup.properties["override"]);
                    break;
                case RUN_OPTIONS.pack:
                    succeded = Pack();
                    break;
                case RUN_OPTIONS.run:
                    Pack();
                    if (Directory.Exists(UnitTest.LOCAL_TEST_PATH)) UnitTest.RunTest();
                    Util.ExecuteCommandSync(Program.GLOBAL_SETTINGS[GlobalSettings.Values.localRunCommand] + Util.GetGameName() + ".p8");
                    succeded = true;
                    UnPack(true);
                    break;
                case RUN_OPTIONS.test:
                    UnitTest.RunTest();
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
                UpdateProjectConfigFile(RUN_OPTIONS.init);
                return UnPack(Setup.properties["override"]);
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

        private static bool Init()
        {
            InitializeGitRepository();
            AddReadme();
            return InitializePico8CliProject();
        }

        private static bool Pack()
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

        private static void UpdateProjectConfigFile(RUN_OPTIONS mode)
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

            switch (mode)
            {
                case RUN_OPTIONS.unpack:
                    for (int i = 0; i < configFileLines.Length; i++)
                    {
                        if (configFileLines[i].StartsWith("last unpacked:")) configFileLines[i] = "last unpacked: " + DateTime.Now.ToString();
                    }
                    break;
                case RUN_OPTIONS.pack:
                    for (int i = 0; i < configFileLines.Length; i++)
                    {
                        if (configFileLines[i].StartsWith("last packed:")) configFileLines[i] = "last packed: " + DateTime.Now.ToString();
                    }
                    break;
                case RUN_OPTIONS.run:
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
            }
            catch (Exception e) { }
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

        private static bool UnPack(bool doOverride)
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

            CreateBackupOfPico8File("before_unpack");

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
    }
}
