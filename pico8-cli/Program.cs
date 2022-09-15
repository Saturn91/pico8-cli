using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace pico8_cli
{ 
    class Setup
    {

        public static string GetCurrentWorkingSpace()
        {
            return Environment.CurrentDirectory;
        }

        public static Dictionary<GlobalSettings.Values, string> GetGlobalSettings()
        {
            return File.Exists(Program.GLOBAL_CONFIG_FILE_PATH) ? GlobalSettings.LoadFromFile() : GlobalSettings.Init();
        }
    }

    class Program
    {
        public const string PRG_NAME = "pico8-cli";
        public readonly static string INSTALLATION_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string current_path = Setup.GetCurrentWorkingSpace();
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

        private static void RunHelp(string[] parameters)
        {
            Util.Error("unknown command, please use one of the bellow cmds");
            Command.COMMANDS["help"].Run(parameters);
        }

        static int Main(string[] args)
        {
            Command.InitCommands();
            CommandState result;

            string[] parameters = args.Length > 1 ? args.SubArray(1, args.Length) : new string[0];
            if (parameters == null) parameters = new string[0];

            if (args.Length == 0)
            {
                RunHelp(parameters);
                result = CommandState.FAILED;
            } else if (Command.COMMANDS.ContainsKey(args[0]))
            {
                

                if (args[0] != "init" && args[0] != "help" && args[0] != "status" &! File.Exists(PROJECT_CONFIG_FILE_PATH))
                {
                    Util.Error("not within a pico8-cli project, please run pioc8-cli init");
                    Command.COMMANDS["help"].Run(new string[0]);
                    return -1;
                }

                

                result = Command.COMMANDS[args[0]].Run(parameters);
                if (result == CommandState.SUCCESS) Util.Info("cmd: " + args[0] + " succeded!");
                
            } else
            {
                RunHelp(parameters);
                result = CommandState.FAILED;
            }

            LogInstallAndLocationInfos();

            return result == CommandState.SUCCESS ? 0 : 1;
        }
    }
}
