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

        public static bool ButlerIsInstalled()
        {
            bool fileExists = File.Exists(butlerExePath);
            if(!fileExists) Util.Error("the file: " + butlerExePath + " does not exist please download butler from itch.io and past all contained files there");
            return fileExists;
        }

        public static void Init()
        {
            // check if file already exist
            if (File.Exists(itchIoConfig)) return;
            File.WriteAllText(itchIoConfig, initialItchIoConfig);
        }

        public static void Do(bool build)
        {
            // 1. read itch.io config file else log error
            if (!File.Exists(itchIoConfig))
            {
                Init();
                Util.Info("created File: " + itchIoConfig + ", please define your username and game as shown in the example");
                return;
            }

            // 2. build project if hasParameter("-b") build
            if (build) Command.COMMANDS["build"].Run(new string[0]);

            // 3. check if build specific file(s) exists and deploy them -> log id successfull or not

        }
    }
}
