using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultima;

namespace Infusion.Desktop
{
    public static class LoginConfiguration
    {
        public static void SetServerAddress(string address, int port)
        {
            string configFile = Path.Combine(Files.RootDir, "login.cfg");
            string content = File.ReadAllText(configFile);
            string patchedContent = SetServerAddress(content, $"{address},{port}");
            File.WriteAllText(configFile, patchedContent);
        }

        public static string SetServerAddress(string fileContent, string loginServer)
        {
            if (string.IsNullOrEmpty(fileContent))
                return $";Inserted by Infusion{Environment.NewLine}LoginServer={loginServer}";

            var inputLines = fileContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var outputLines = new List<string>(inputLines.Length + 2);

            bool containsInfusion = false;
            for (int i = 0; i < inputLines.Length; i++)
            {
                string line = inputLines[i];

                if (line == null)
                    throw new NullReferenceException();
                if (line.Equals(";Inserted by Infusion"))
                {
                    outputLines.Add(line);
                    outputLines.Add($"LoginServer={loginServer}");
                    i++;
                    containsInfusion = true;
                }
                else if (line.StartsWith("LoginServer="))
                {
                    outputLines.Add(";" + line);
                }
                else
                {
                    outputLines.Add(line);
                }
            }

            if (!containsInfusion)
            {
                outputLines.Add(";Inserted by Infusion");
                outputLines.Add($"LoginServer={loginServer}");
            }

            return string.Join(Environment.NewLine, outputLines);
        }
    }
}
