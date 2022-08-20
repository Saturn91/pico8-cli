using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace pico8_cli
{
    public enum RUN_OPTIONS { init, unpack, pack, run, build, test };

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

    class Program
    {
        public const string PRG_NAME = "pico8-cli";
        public readonly static string INSTALLATION_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string current_path = Setup.GetCurrentWorkingSpace();
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
