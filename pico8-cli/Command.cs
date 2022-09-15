﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace pico8_cli
{
    public enum CommandState
    {
        SUCCESS,
        WRONG_PARAMS,
        FAILED,
        CANCEL
    }

    public class NumberParameterValue
    {
        public int value = -1;
        public bool valid = false;
    }

    public abstract class Command
    {
        public static string[] DEFAULT_PARAMETERS;
        public static bool debugLogerActive { get; private set; }


        public static Dictionary<string, Command> COMMANDS;

        public string name { get; private set; }
        public bool updateConfigFile { get; private set; }

        protected string[] allowedParameters;

        public static void InitCommands()
        {
            DEFAULT_PARAMETERS = new string[] { "debug" };
            COMMANDS = new Dictionary<string, Command>()
            {
                { "status", new Status()},
                { "init", new Init() },
                { "unpack", new Unpack()},
                { "pack", new Pack()},
                { "run", new Run()},
                { "build", new Export()},
                { "deploy", new Deploy()},
                { "test", new Test()},
                { "--h", new Help()},
                { "-h" , new Help()},
                { "help", new Help() },
                { "?", new Help() },
                { "restore", new Restore() }
            };            
        }

        protected Command(string name, string[] allowedParameters)
        {
            this.name = name;

            List<string> allowedParamsList = new List<string>(allowedParameters);
            allowedParamsList.AddRange(DEFAULT_PARAMETERS);
            this.allowedParameters = allowedParamsList.ToArray();
            updateConfigFile = false;
        }

        protected Command(string name, string[] allowedParameters, bool updateConfigFile)
        {
            this.name = name;

            List<string> allowedParamsList = new List<string>(allowedParameters);
            allowedParamsList.AddRange(DEFAULT_PARAMETERS);
            this.allowedParameters = allowedParamsList.ToArray();
            this.updateConfigFile = updateConfigFile;
        }

        private bool IsAllowedParameter(string parameter) {
            if (allowedParameters.Contains(parameter)) return true;
            if (allowedParameters.Contains(Regex.Replace(parameter, @"[0-9]+$", "number"))) return true;
            return false;
        }

        public CommandState Run(string[] parameters)
        {
            foreach (string parameter in parameters)
            {
                if (!IsAllowedParameter(parameter))
                {
                    Console.WriteLine("Parameter: " + parameter + " is no valid parameter for the cmd: " + name);
                    Console.WriteLine("    " + Help());
                    return CommandState.WRONG_PARAMS;
                }
            }

            foreach(string parameter in parameters)
            {
                if (parameter == "debug")
                {
                    Util.Info("debug enabled");
                    debugLogerActive = true;
                }
            }

            CommandState result =  OnRun(parameters);
            if(result == CommandState.SUCCESS && updateConfigFile) Pico8.UpdateProjectConfigFile(name);
            return result;
        }

        protected abstract CommandState OnRun(string[] parameters);

        protected abstract string GetSpecificHelp();

        public string Help()
        {
            string parameterString = "params: [";

            foreach (string parameter in allowedParameters)
            {
                parameterString += parameter + ", ";
            }

            if(parameterString.Length > 2) parameterString = parameterString.Substring(0, parameterString.Length - 2);

            parameterString += "] ";

            return name + ": -> " + parameterString + " " + GetSpecificHelp();
        }

        protected bool HasParameter(string parameter, string[] passedParameters)
        {
            return passedParameters.Contains(parameter);
        }

        protected NumberParameterValue HasNumberParameter(string prefix, string[] passedParameters)
        {
            NumberParameterValue result = new NumberParameterValue();

            foreach(string passedParameter in passedParameters)
            {
                if(Regex.Match(passedParameter, @"[A-Za-z]*=\d*").Success && passedParameter.StartsWith(prefix+"="))
                {
                    try
                    {
                        result.value = int.Parse(passedParameter.Substring(prefix.Length+1));
                        result.valid = true;
                    }
                    catch { }
                }
            }

            return result;
        }
    }

    public class Init : Command
    {
        public Init() : base("init", new string[0], true) { }

        protected override CommandState OnRun(string[] parameters)
        {
            if (Pico8.Init()) return CommandState.SUCCESS;
            return CommandState.FAILED;
        }

        protected override string GetSpecificHelp()
        {
            return "initialize a project. Run this in an empty folder called like the new projects name";
        }
    }

    public class Unpack : Command
    {
        public Unpack() : base("unpack", new string[] { "override" }, true) {}
        protected override CommandState OnRun(string[] parameters)
        {
            return Pico8.UnPack(HasParameter("override", parameters)) ? CommandState.SUCCESS : CommandState.FAILED;
        }

        protected override string GetSpecificHelp()
        {
            return "unpack pico8's .p8 file into seperate files, this will blindly override any changes you might have pending in any lua or resource files!";
        }
    }

    public class Pack: Command
    {
        public Pack() : base("pack", new string[0], true) {}

        protected override CommandState OnRun(string[] parameters)
        {
            return Pico8.Pack() ? CommandState.SUCCESS : CommandState.FAILED;
        }

        protected override string GetSpecificHelp()
        {
            return "pack the project seperate file into a runnable .p8 file";
        }
    }

    public class Run: Command
    {
        public Run() : base("run", new string[] { "-t", "-u" }, true) { }

        protected override CommandState OnRun(string[] parameters)
        { 
            Pico8.Pack();
            if (Directory.Exists(UnitTest.LOCAL_TEST_PATH) &! HasParameter("-t", parameters)) UnitTest.RunTest();
            Util.ExecuteCommandSync(Program.GLOBAL_SETTINGS[GlobalSettings.Values.localRunCommand] + " " + Util.GetGameName() + ".p8");
            if (!HasParameter("-u", parameters)) Pico8.UnPack(true);
            else Util.Info("[-u]: did not unpack automatically, if you have changed the .p8 with pico8 unpack now manually!");
            return CommandState.SUCCESS;
        }

        protected override string GetSpecificHelp()
        {
            return "pack and run pico8 and unpack afterwards, if -t is not provided and tests are setup, they will get triggered, -u will not trigger an automatic unpack after the run";
        }
    }

    public class Export: Command
    {
        public Export() : base("build", new string[] { "-t" }, true) { }

        protected override CommandState OnRun(string[] parameters)
        {
            if (Directory.Exists(UnitTest.LOCAL_TEST_PATH) & !HasParameter("-t", parameters) && Build.CanBuild()) UnitTest.RunTest();
            return Build.Do() ? CommandState.SUCCESS : CommandState.FAILED;
        }

        protected override string GetSpecificHelp()
        {
            return "pack and export pico8, if -t is not provided and tests are setup, they will get triggered aswell";
        }
    }

    public class Deploy: Command{

        public Deploy() : base("deploy", new string[] { "-b", "itch" }, true) { }

        protected override CommandState OnRun(string[] parameters)
        {
            if (HasParameter("itch", parameters))
            {
                return DeployToItch.Do(HasParameter("-b", parameters), DeployToItch.DeployPlatform.itch);
            }

            Util.Error("please provide for which platform you would like to deploy i.e. itch");
            
            return CommandState.FAILED;
        }

        protected override string GetSpecificHelp()
        {
            return " deploy project to the selected platforem i.e. itch. use -b to build before deploy";
        }
    }

    public class Test : Command
    {
        public Test() : base("test", new string[0], true) { }

        protected override CommandState OnRun(string[] parameters)
        {
            return UnitTest.RunTest() ? CommandState.SUCCESS : CommandState.FAILED;
        }

        protected override string GetSpecificHelp()
        {
            return "setup (first time) and run unit-tests";
        }
    }

    public class Help : Command
    {
        public Help() : base("help", new string[0]) { }

        protected override CommandState OnRun(string[] parameters)
        {
            Console.WriteLine("pico8-cli help:");
            Console.WriteLine("run one of the bellow commands and provide their parameters");
            Console.WriteLine("-----------------------------------------------------------");
            foreach (Command cmd in COMMANDS.Values)
            {
                if (!string.Equals(cmd.name, this.name))
                {
                    Console.WriteLine();
                    Console.WriteLine("    " + cmd.Help());
                }
            }

            return CommandState.SUCCESS;
        }

        protected override string GetSpecificHelp()
        {
            return "help";
        }
    }

    public class Status : Command
    {
        public Status() : base("status", new string[0]) { }

        protected override CommandState OnRun(string[] parameters)
        {
            string status = "not within a pico8-cli project";
            if (File.Exists(Program.PROJECT_CONFIG_FILE_PATH))
            {
                status = "Project: " + Util.GetGameName() + " is setup";
            }

            Console.WriteLine(status);

            return CommandState.SUCCESS;
        }

        protected override string GetSpecificHelp()
        {
            return "see status of current project";
        }
    }

    public class Restore : Command
    {
        public Restore() : base("restore", new string[] {"steps=number"}) { }

        protected override CommandState OnRun(string[] parameters)
        {
            int steps = -1;

            NumberParameterValue passedStepsParameter = HasNumberParameter("steps", parameters);

            if (passedStepsParameter.valid) steps = passedStepsParameter.value;     

            return Pico8.Rollback(steps);
        }

        protected override string GetSpecificHelp()
        {
            return "rollback to an older state from a backup file (maybe after some lost changes due to an error)";
        }
    }
}
