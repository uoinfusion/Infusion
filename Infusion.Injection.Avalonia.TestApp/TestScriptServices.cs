using Infusion.Injection.Avalonia.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia.TestApp
{
    public class TestScriptServices : IScriptServices
    {
        private List<string> runningScripts = new List<string>() { "Test", "Qwer" };
        public IEnumerable<string> RunningScripts => runningScripts;

        private List<string> availableScripts = new List<string>() { "Test", "Qwer", "not_running", "and", "something", "else" };
        public IEnumerable<string> AvailableScripts => availableScripts;

        public event Action RunningScriptsChanged;
        public event Action AvailableScriptsChanged;

        public void Load(string scriptFileName) { }
        public void Run(string name) { }
        public void Terminate(string name) { }

        internal void AddRunning(string text)
        {
            runningScripts.Add(text);
            AvailableScriptsChanged?.Invoke();
        }

        internal void AddAvailable(string text)
        {
            availableScripts.Add(text);
            RunningScriptsChanged?.Invoke();
        }

        internal void RemoveRunning(string text)
        {
            runningScripts.Remove(text);
            RunningScriptsChanged?.Invoke();
        }

        internal void RemoveAvailable(string text)
        {
            availableScripts.Remove(text);
            AvailableScriptsChanged?.Invoke();
        }
    }
}
