using Infusion.Commands;
using Infusion.LegacyApi.Cliloc;
using Infusion.LegacyApi.Console;
using Infusion.Packets;
using InjectionScript;
using InjectionScript.Debugging;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;

namespace Infusion.LegacyApi.Injection
{
    public sealed class InjectionHost
    {
        private readonly InjectionRuntime runtime;
        private readonly Legacy api;
        private readonly IConsole console;
        private readonly IInjectionWindow injectionWindow;
        private readonly DebuggerBridge debuggerBridge;

        public IDebuggerServer Debugger { get; }
        public ITracer Tracer { get; }

        private Dictionary<string, int> injectionObjects
        {
            get
            {
                var objects = new Dictionary<string, int>();
                foreach (var pair in runtime.Objects)
                {
                    objects.Add(pair.Key, pair.Value);
                }

                return objects;
            }

            set
            {
                runtime.Objects.Clear();
                foreach (var pair in value)
                {
                    runtime.Objects.Set(pair.Key, pair.Value);
                }
            }
        }

        public InjectionApi InjectionApi => runtime.Api;

        internal InjectionHost(Legacy api, IInjectionWindow injectionWindow, IConsole console, PacketDefinitionRegistry packetRegistry, ITimeSource timeSource, IClilocSource clilocSource)
        {
            this.api = api;
            this.injectionWindow = injectionWindow;
            this.console = console;

            var bridge = new InjectionApiBridge(api, this, console, packetRegistry, clilocSource);
            var debuggerServer = new DebuggerServer(() => api.CancellationToken);
            debuggerServer.DebuggerBreakHit += (sender, e) => debuggerBridge.HandleBreakHit(e);
            runtime = new InjectionRuntime(bridge, debuggerServer, timeSource, () => api.CancellationToken.Value);

            debuggerBridge = new DebuggerBridge(api.CommandHandler, debuggerServer, console, runtime);
            debuggerBridge.RegisterCommands();

            Debugger = debuggerServer;
            Tracer = debuggerServer;

            api.CommandHandler.RegisterCommand(new Command("exec", ExecCommand, false, true, executionMode: CommandExecutionMode.AlwaysParallel));

            api.Config.Register("injection.Objects", () => injectionObjects);
        }

        public void OpenGui() => injectionWindow.Open(api, InjectionApi);

        public void LoadScript(string fileName)
        {
            console.Info($"Loading {fileName}");

            UnregisterCommands();
            var messages = runtime.Load(fileName);
            var error = false;
            foreach (var message in messages)
            {
                switch (message.Severity)
                {
                    case MessageSeverity.Warning:
                        console.Important($"Warning: {message.StartLine}, {message.StartColumn} {message.Text}");
                        break;
                    case MessageSeverity.Error:
                        console.Error($"Error: {message.StartLine}, {message.StartColumn} {message.Text}");
                        error = true;
                        break;
                }
            }

            if (!error)
                RegisterCommands();

        }

        public void ExecSubrutine(string subrutineName) => ExecCommand(subrutineName);

        private void ExecCommand(string parameters)
        {
            var subrutine = runtime.Metadata.GetSubrutine(parameters, 0);
            if (subrutine == null)
                throw new NotImplementedException();

            var commandName = GetCommandName(subrutine);

            this.api.CommandHandler.InvokeSyntax("," + commandName, CommandExecutionMode.AlwaysParallel);
        }

        public void Terminate(string subrutineName)
        {
            if (runtime.Metadata.TryGetSubrutine(subrutineName, 0, out var subrutine))
            {
                var commandName = GetCommandName(subrutine);
                this.api.CommandHandler.Terminate(commandName);
            }
        }

        internal bool FunRunning(string subrutineName)
        {
            if (runtime.Metadata.TryGetSubrutine(subrutineName, 0, out var subrutine))
            {
                var commandName = GetCommandName(subrutine);
                return this.api.CommandHandler.IsCommandRunning(commandName);
            }

            return false;
        }

        private void RegisterCommands()
        {
            foreach (var subrutine in runtime.Metadata.Subrutines)
            {
                var name = GetCommandName(subrutine);
                this.api.CommandHandler.RegisterCommand(name, () => CallSubrutine(subrutine.Name));
            }
        }

        private void UnregisterCommands()
        {
            foreach (var subrutine in runtime.Metadata.Subrutines)
            {
                var name = GetCommandName(subrutine);
                this.api.CommandHandler.Unregister(name);
            }
        }

        private string GetCommandName(SubrutineDefinition subrutine) => "inj-" + subrutine.Name;

        public void CallSubrutine(string subrutineName)
        {
            try
            {
                runtime.CallSubrutine(subrutineName);
            }
            catch (ScriptFailedException ex)
            {
                console.Error($"Line {ex.Line}, {runtime.CurrentScript.FileName} - {ex.Message}");
            }
            catch (InternalInterpretationException ex)
            {
                console.Error($"Line {ex.Line}, {runtime.CurrentScript.FileName} - {ex.Message}");
                console.Debug(ex.InnerException.ToString());
            }
        }

        internal int GetObject(string id) => runtime.GetObject(id);

        internal void AddObject(string currentObjectName, int value) => runtime.Api.UO.AddObject(currentObjectName, value);
    }
}
