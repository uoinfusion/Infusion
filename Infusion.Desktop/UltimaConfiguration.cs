using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultima;

namespace Infusion.Desktop
{
    public static class UltimaConfiguration
    {
        public static void SetUserName(string userName)
        {
            SetProperty("AcctID", userName);
        }

        private static void SetProperty(string property, string value)
        {
            string path = Path.Combine(Files.RootDir, "uo.cfg");
            string updatedContent = SetProperty(File.ReadAllText(path), property, value);
            File.WriteAllText(path, updatedContent);
        }

        public static string SetProperty(string configuration, string property, string value)
        {
            var lines = configuration.Split(new [] { Environment.NewLine }, StringSplitOptions.None);
            var outputLines = new List<string>(lines.Length + 1);

            string propertyAssignment = $"{property}={value}";
            bool propertyFound = false;
            foreach (string line in lines)
            {
                if (line.StartsWith($"{property}="))
                {
                    outputLines.Add(propertyAssignment);
                    propertyFound = true;
                }
                else
                    outputLines.Add(line);
            }

            if (!propertyFound)
            {
                outputLines.Add(propertyAssignment);
            }

            return string.Join(Environment.NewLine, outputLines);
        }

        public static void SetPassword(string password)
        {
            SetProperty("AcctPassword", password);
            SetProperty("RememberAcctPW", "yes");
        }
    }
}
