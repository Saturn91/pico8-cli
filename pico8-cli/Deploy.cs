using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace pico8_cli
{

    class DeployToItch
    {
        private static readonly string butlerExePath = Program.INSTALLATION_PATH + "/butler/butler.exe";
        private static readonly string itchIoConfig = Program.current_path + "/deploy.config";
        private static string initialItchIoConfig = @"itch.io-project: youruser/yourgame";
        private static string[] pico8CliBuilds = { "X.bin/X_linux.zip", "X.bin/X_osx.zip", "X.bin/X_raspi.zip", "X.bin/X_windows.zip", "X_html.zip", };
        private static string[] itchIoChannels = { "linux", "osx", "raspbi", "windows", "web" };

        public enum DeployPlatform
        {
            itch
        }

        public static CommandState Do(bool build, DeployPlatform platform)
        {
            // 1. check setup for deployment platform
            if (!PreInstall(platform)) return CommandState.FAILED;

            // 2. check if config file for platform exists else fire warning and create default file
            if (!Init(platform)) return CommandState.FAILED;

            // 3. build project if hasParameter("-b") build
            if (build) Command.COMMANDS["build"].Run(new string[0]);

            // 4. check if build specific file(s) exists and deploy them -> log id successfull or not
            if (!Deploy(platform)) return CommandState.FAILED;

            return CommandState.SUCCESS;
        }

        private static bool PreInstall(DeployPlatform platform)
        {
            switch (platform)
            {
                case DeployPlatform.itch:
                    bool fileExists = File.Exists(butlerExePath);
                    if (!fileExists) Util.Error("the file: " + butlerExePath + " does not exist please download butler from itch.io and past all contained files there");
                    return fileExists;
            }

            return false;
        }

        private static bool Init(DeployPlatform platform)
        {
            switch (platform)
            {
                case DeployPlatform.itch:
                    if (File.Exists(itchIoConfig)) return true;
                    File.WriteAllText(itchIoConfig, initialItchIoConfig);
                    Util.Info("created File: " + itchIoConfig + ", please define your username and game as shown in the example");
                    break;
            }

            return false;
        }

        private static bool Deploy(DeployPlatform platform)
        {
            bool succeded = false;
            for(int i = 0; i < pico8CliBuilds.Length; i++)
            {
                string path = Build.buildFolder + "/" + pico8CliBuilds[i].Replace("X", Util.GetGameName());

                //2. check if build Directory exist
                if (File.Exists(path)) {

                    // 3. run deploy
                    switch (platform)
                    {
                        case DeployPlatform.itch:
                            string itchIoGameId = File.ReadAllLines(itchIoConfig)[0].Split(":")[1].Substring(1);
                            Util.ExecuteCommandSync(butlerExePath +  " push " + path + " " + itchIoGameId + ":" + itchIoChannels[i] );
                            succeded = true;

                            if (itchIoChannels[i] == "web") Util.Info("Deployed web version to https://" + itchIoGameId.Replace("/", ".itch.io/") + " please make sure to set this project up as playable in the browser");

                            break;
                    }
                } else
                {
                    Util.Info("Did not find File: " + path + ", was not able to deploy");
                }                
            }            

            return succeded;
        }
    }
}
