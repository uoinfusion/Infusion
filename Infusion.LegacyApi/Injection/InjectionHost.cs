using Infusion.LegacyApi.Console;
using InjectionScript.Interpretation;
using InjectionScript.Parsing;
using System;

namespace Infusion.LegacyApi.Injection
{
    public sealed class InjectionHost
    {
        private Runtime runtime;
        private readonly Legacy api;
        private readonly IConsole console;

        public InjectionHost(Legacy api, IConsole console)
        {
            this.api = api;
            this.console = console;
        }

        public void LoadScript(string fileName)
        {
            runtime = new Runtime();

            try
            {
                runtime.Load(fileName);

                RegisterNatives();
            }
            catch (SyntaxErrorException ex)
            {
                foreach (var error in ex.Errors)
                    console.Error($"{error.Line}, {error.CharPos} {error.Message}");
            }
        }

        public void CallSubrutine(string subrutineName)
        {
            runtime.CallSubrutine(subrutineName);
        }

        private void RegisterNatives()
        {
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "print", (Action<string>)Print));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getx", (Func<int>)GetX));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "gety", (Func<int>)GetY));
            runtime.Metadata.Add(new NativeSubrutineDefinition("UO", "getz", (Func<int>)GetZ));
        }

        private void Print(string msg) => api.ClientPrint(msg);
        private int GetX() => api.Me.Location.X;
        private int GetY() => api.Me.Location.Y;
        private int GetZ() => api.Me.Location.Z;
    }
}
