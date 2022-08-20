﻿using System;
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

        private static bool Init()
        {
            // 1. get build path from global config
            string cartFolder = pico8CardFolder;
            if (!Directory.Exists(cartFolder))
            {
                Util.Error("can not find folder: " + pico8CardFolder + " you either not have installed pico8 or set up the wrong value for " + GlobalSettings.Values.localPico8_cart_folder_location + " in the global settings at " + Program.GLOBAL_CONFIG_FILE_PATH);
                return false;
            }

            if(!File.Exists(internalBuildFile))
            {
                File.Copy(Program.INSTALLATION_PATH + "/build.p8", internalBuildFile);
                string localBuildFile = File.ReadAllText(internalBuildFile).Replace("_GAME_", Util.GetGameName());
                File.WriteAllText(internalBuildFile, localBuildFile);
            }

            return true;
        }

        public static void Do()
        {
            if (!Init())
            {
                Util.Error("not able to build...");
                return;
            }
            string buildFolder = pico8CardFolder + "/" + Util.GetGameName();
            string gameName = Util.GetGameName();
            Lua.Pack();
            
            Directory.CreateDirectory(buildFolder);
            
            //clear build folder
            DirectoryInfo di = new DirectoryInfo(buildFolder);
            foreach (FileInfo file in di.GetFiles()) file.Delete();

            File.Copy(gameName + ".p8", buildFolder + "/" + gameName + ".p8");
            if (!File.Exists(buildFolder + "/" + gameName + ".p8")) Util.Error("not able to write: " + buildFolder + "/" + gameName + ".p8");

            Util.ExecuteCommandSync("\"C:\\Program Files (x86)\\PICO-8\\pico8.exe\" -x " + internalBuildFile);
            Util.Info("Build succeded, find your files here: " + buildFolder);
        }
    }
}
