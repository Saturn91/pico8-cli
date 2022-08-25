using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace pico8_cli
{
    public class GlobalSettings
    {
        public enum Values
        {
            localRunCommand,
            pico8Version,
            localPico8_cart_folder_location,
            max_backup_file_cnt
        }

        public string localRunCommand { get; private set; }
        public string pico8Version { get; private set; }

        public static Dictionary<Values, string> EmptySettings()
        {
            Dictionary<Values, string> values = new Dictionary<Values, string>();
            foreach (Values value in Enum.GetValues(typeof(Values)))
            {
                values.Add(value, "");
            }

            return values;
        }

        public static Dictionary<Values, string> LoadFromFile()
        {

            string[] loadedLines = File.ReadAllLines(Program.GLOBAL_CONFIG_FILE_PATH);
            Dictionary<Values, string> loadedValues = EmptySettings();

            foreach (string line in loadedLines)
            {
                foreach (Values value in Enum.GetValues(typeof(Values)))
                {

                    string loadedValue = Util.GetPropertyFromLine(line, value.ToString() + ":");
                    if (loadedValue != null) loadedValues[value] = loadedValue;
                }
            }

            Util.Debug("global settings loaded");
            return loadedValues;
        }

        private static void SaveToFile(Dictionary<Values, string> values)
        {
            List<string> globalSettingsAsStringList = new List<string>();

            foreach (Values value in Enum.GetValues(typeof(Values)))
            {
                globalSettingsAsStringList.Add(value.ToString() + ": " + values[value]);
            }


            try
            {
                File.WriteAllLines(Program.GLOBAL_CONFIG_FILE_PATH, globalSettingsAsStringList.ToArray());
                Util.Debug("create/override global config");
            }
            catch (Exception e)
            {
                Util.Debug("not able to create config file: " + Program.GLOBAL_CONFIG_FILE_PATH + " please retry in a terminal with administrator right, or create the file manually");
            }

        }

        public static Dictionary<Values, string> Init()
        {
            Dictionary<Values, string> initialValues = EmptySettings();
            initialValues[Values.pico8Version] = "version 36";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) initialValues[Values.localRunCommand] = "\"C:\\Program Files (x86)\\PICO-8\\pico8.exe\" -run ";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) initialValues[Values.localRunCommand] = "/opt/pico-8 - run ";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) initialValues[Values.localRunCommand] = "opt/pico-8 - run ";

            Util.Debug("initialy creating global config file");
            SaveToFile(initialValues);


            return initialValues;
        }
    }
}
