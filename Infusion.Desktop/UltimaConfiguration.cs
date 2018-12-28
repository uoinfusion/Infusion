using System;
using System.Collections.Generic;
using System.IO;
using Ultima;

namespace Infusion.Desktop
{
    public class UltimaConfiguration
    {
        private readonly string rootDir;

        public string ConfigFile { get; }

        public UltimaConfiguration(string rootDir)
        {
            this.rootDir = rootDir;
            ConfigFile = Path.Combine(rootDir, "uo.cfg");
        }

        public UltimaConfiguration()
        {
            this.rootDir = Files.RootDir;
        }

        public void SetUserName(string userName)
        {
            SetProperty("AcctID", userName);
        }

        private void SetProperty(string property, string value)
        {
            var updatedContent = SetProperty(File.ReadAllText(ConfigFile), property, value);
            File.WriteAllText(ConfigFile, updatedContent);
        }

        public string SetProperty(string configuration, string property, string value)
        {
            var lines = configuration.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            var outputLines = new List<string>(lines.Length + 1);

            var propertyAssignment = $"{property}={value}";
            var propertyFound = false;
            foreach (var line in lines)
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

        public void SetPassword(string password)
        {
            SetProperty("AcctPassword", password);
            SetProperty("RememberAcctPW", "yes");
        }
    }
}