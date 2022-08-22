using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace pico8_cli
{
    public class Util
    {
        public static void Debug(string msg)
        {
            if (!Command.debugLogerActive) return;
            Console.WriteLine("[debg]: " + msg);
        }

        public static void Info(string msg)
        {
            Console.WriteLine("[info]: " + msg);
        }

        public static void Error(string err)
        {
            Console.WriteLine("[err.]: " + err);
        }

        public static string ArrayToString<T>(T[] array)
        {
            string result = "";
            foreach (T entry in array) result += entry + ", ";
            return result;
        }

        public static string GetGameName()
        {
            string[] folders = Program.current_path.Split("\\");
            return folders[folders.Length - 1];
        }

        public static string[] GetFileLines(string file)
        {
            List<string> lines = new List<string>();
            foreach (string line in File.ReadLines(@file))
            {
                lines.Add(line);
            }

            return lines.ToArray();
        }

        public static string GetPropertyFromLine(string line, string property)
        {
            if (line.StartsWith(property))
            {
                return line.Substring(line.IndexOf(":") + 2);
            }

            return null;
        }

        public static void ExecuteCommandSync(object command)
        {
            Debug("staring cmd: " + command);
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
                proc.WaitForExit();
            }
            catch (Exception objException)
            {
                Error(command + " failed.: " + objException.ToString());
            }
        }

        public static void CopyPasteFromDirectory(string sourceDirectory, string newDirectory)
        {
            var allFiles = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);
            foreach (string newPath in allFiles)
            {
                File.Copy(newPath, newPath.Replace(sourceDirectory, newDirectory), true);
            }
        }
    }
}
