using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace pico8_cli
{
    class Build
    {
        public static readonly string pico8CardFolder = Program.GLOBAL_SETTINGS[GlobalSettings.Values.localPico8_cart_folder_location].Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        public static readonly string internalBuildFile = "./.pico8-cli/build.p8";
        public static readonly string buildIconConfig = "./buildIconConfig.txt";

        private static string SetupBuildP8File()
        {
            string output = File.ReadAllText(internalBuildFile);
            string[] iconSettings = File.ReadAllLines(buildIconConfig);

            Dictionary<string, int> iconSettingsLookup = new Dictionary<string, int>();
            foreach(string line in iconSettings)
            {
                if (line.Contains(":"))
                {
                    string[] substrings = line.Split(":");
                    string property = substrings[0];
                    try
                    {
                        int value = int.Parse(substrings[1]);
                        iconSettingsLookup.Add(property, value);
                    } catch { }                    
                }
            }

            output = output.Replace("_GAME_", Util.GetGameName());

            string[] possibleParams = { "i", "s", "c" };

            for (int i = 0; i < possibleParams.Length; i++)
            {
                int replacement = iconSettingsLookup.ContainsKey(possibleParams[i]) ? iconSettingsLookup[possibleParams[i]] : 0;
                output = output.Replace("__" + possibleParams[i] + "__", "-" + possibleParams[i] + " " + replacement);
            }
            return output;
        }

        private static bool Init()
        {
            // 1. get build path from global config
            string cartFolder = pico8CardFolder;
            if (!Directory.Exists(cartFolder))
            {
                Util.Error("can not find folder: " + pico8CardFolder + " you either not have installed pico8 or set up the wrong value for " + GlobalSettings.Values.localPico8_cart_folder_location + " in the global settings at " + Program.GLOBAL_CONFIG_FILE_PATH);
                return false;
            }

            if (!File.Exists(Program.RESOURCE_FOLDER + "/" + "__label__.txt"))
            {
                Util.Error("you didn't capture a cratridge label yet, please do so by pressing F7 in the running pico8 card and then save (ctrl+S)");
                return false;
            }
           
            File.Copy(Program.INSTALLATION_PATH + "/build.p8", internalBuildFile, true);
            if(!File.Exists(buildIconConfig))
            {
                string initialIconBuildConfig = @"this params are used to describe the icon for binary builds
i: 1
s: 1
c: 0";

                File.WriteAllText(buildIconConfig, initialIconBuildConfig);
            }
            string localBuildFile = SetupBuildP8File();
            File.WriteAllText(internalBuildFile, localBuildFile);       

            return true;
        }

        public static bool Do()
        {
            if (!Init())
            {
                Util.Error("not able to build...");
                return false;
            }
            string buildFolder = pico8CardFolder + "/" + Util.GetGameName();
            string gameName = Util.GetGameName();
            Lua.Pack();

            //clear build folder
            if (Directory.Exists(buildFolder))
            {
                DirectoryInfo di = new DirectoryInfo(buildFolder);
                di.Delete(true);
            }
            
            Directory.CreateDirectory(buildFolder);
            
            File.Copy(gameName + ".p8", buildFolder + "/" + gameName + ".p8");
            if (!File.Exists(buildFolder + "/" + gameName + ".p8"))
            {
                Util.Error("not able to write: " + buildFolder + "/" + gameName + ".p8");
                return false;
            }

            Directory.CreateDirectory(buildFolder + "/" + Util.GetGameName() + "_html");
            Util.ExecuteCommandSync("\"C:\\Program Files (x86)\\PICO-8\\pico8.exe\" -x " + internalBuildFile);
            Util.Info("Build succeded, find your files here: " + buildFolder);

            return true;
        }
    }
}
