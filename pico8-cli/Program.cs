using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;


namespace pico8_cli
{
    class Setup
    {
        public static Dictionary<string, bool> properties = new Dictionary<string, bool>()
        {
            { "debug", false }
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

        public static string ArrayToString<T>(T[] array)
        {
            string result = "";
            foreach (T entry in array) result += entry + ", ";
            return result;
        }
    }

    class Pico8
    {
        private static void StartLog()
        {
            Util.Debug("starting [" + Program.current_mode + "] process at: " + Program.current_path);
        }
        public static void Init()
        {
            StartLog();
        }

        public static void Pack()
        {
            StartLog();
        }

        public static void UnPack()
        {
            StartLog();
        }
    }

    class Program
    {
        public const string PRG_NAME = "pico8-cli";
        public readonly static string INSTALLATION_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string current_path = Setup.GetCurrentWorkingSpace();
        public enum RUN_OPTIONS { init, unpack, pack };
        public static readonly string[] RUN_OPTIONS_STRINGS = Enum.GetNames(typeof(RUN_OPTIONS));
        public static RUN_OPTIONS current_mode = RUN_OPTIONS.init;

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

            switch (current_mode)
            {
                case RUN_OPTIONS.init:
                    Pico8.Init();
                    break;
                case RUN_OPTIONS.unpack:
                    Pico8.UnPack();
                    break;
                case RUN_OPTIONS.pack:
                    Pico8.Pack();
                    break;
            }

            return 1;
        }
    }
}
