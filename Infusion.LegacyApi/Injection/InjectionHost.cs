using Infusion.Commands;
using Infusion.LegacyApi.Cliloc;
using Infusion.LegacyApi.Console;
using Infusion.Packets;
using InjectionScript;
using InjectionScript.Debugging;
using InjectionScript.Runtime;
using InjectionScript.Runtime.State;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infusion.LegacyApi.Injection
{
    internal class InjectionStateWrapper<T>
    {
        private readonly RuntimeDictionary<T> dictionary;

        public InjectionStateWrapper(RuntimeDictionary<T> dictionary)
        {
            this.dictionary = dictionary;
        }

        public Dictionary<string, T> Get()
        {
            var result = new Dictionary<string, T>();
            foreach (var pair in dictionary)
            {
                result.Add(pair.Key, pair.Value);
            }

            return result;
        }

        public void Set(Dictionary<string, T> value)
        {
            dictionary.Clear();
            foreach (var pair in value)
            {
                dictionary.Set(pair.Key, pair.Value);
            }
        }
    }

    public sealed class InjectionHost
    {
        private const string injectionCommandPrefix = "inj-";

        private readonly InjectionRuntime runtime;
        private readonly Legacy api;
        private readonly IConsole console;
        private readonly IInjectionWindow injectionWindow;
        private readonly DebuggerBridge debuggerBridge;

        public IDebuggerServer Debugger { get; }
        public ITracer Tracer { get; }

        private InjectionStateWrapper<int> objectsWrapper;
        private Dictionary<string, int> injectionObjects
        {
            get => objectsWrapper.Get();
            set => objectsWrapper.Set(value);
        }

        private InjectionStateWrapper<EquipSet> armSetsWrapper;
        private Dictionary<string, EquipSet> injectionArmSets
        {
            get => armSetsWrapper.Get();
            set => armSetsWrapper.Set(value);
        }

        private InjectionStateWrapper<EquipSet> dressSetsWrapper;
        private Dictionary<string, EquipSet> injectionDressSets
        {
            get => dressSetsWrapper.Get();
            set => dressSetsWrapper.Set(value);
        }


        public InjectionOptions InjectionOptions
        {
            get => runtime.Options;
            set
            {
                value.CopyTo(runtime.Options);
            }
        }

        public bool AutoOpenGui { get; set; }
        public int DressSpeed { get => runtime.State.DressSpeed; set => runtime.State.DressSpeed = value; }

        public InjectionApi InjectionApi => runtime.Api;

        public bool IsInjectionCommandName(string name) 
            => name.IndexOf(injectionCommandPrefix, StringComparison.Ordinal) == 0;
        private string ConvertInfusionCommandNameToInjection(string name) 
            => name.Substring(injectionCommandPrefix.Length);

        public IEnumerable<string> RunningCommands
            => api.CommandHandler.RunningCommands
                .Where(x => IsInjectionCommandName(x.Name))
                .Select(x => ConvertInfusionCommandNameToInjection(x.Name));

        public event Action RunningCommandsChanged;
        public event Action ScriptLoaded;

        internal InjectionHost(Legacy api, IInjectionWindow injectionWindow, IConsole console, PacketDefinitionRegistry packetRegistry,
            ITimeSource timeSource, IClilocSource clilocSource, ISoundPlayer soundPlayer)
        {
            this.api = api;
            this.injectionWindow = injectionWindow;
            this.console = console;

            var bridge = new InjectionApiBridge(api, this, console, packetRegistry, clilocSource, soundPlayer);
            var debuggerServer = new DebuggerServer(() => api.CancellationToken);
            debuggerServer.DebuggerBreakHit += (sender, e) => debuggerBridge.HandleBreakHit(e);
            runtime = new InjectionRuntime(bridge, debuggerServer, timeSource, () => api.CancellationToken.Value);

            debuggerBridge = new DebuggerBridge(api.CommandHandler, debuggerServer, console, runtime);
            debuggerBridge.RegisterCommands();

            Debugger = debuggerServer;
            Tracer = debuggerServer;

            api.CommandHandler.RegisterCommand(new Command("exec", ExecCommand, false, true, executionMode: CommandExecutionMode.AlwaysParallel));

            objectsWrapper = new InjectionStateWrapper<int>(runtime.Objects);
            api.Config.Register("injection.objects", () => injectionObjects);

            armSetsWrapper = new InjectionStateWrapper<EquipSet>(runtime.ArmSets);
            api.Config.Register("injection.arms", () => injectionArmSets);

            dressSetsWrapper = new InjectionStateWrapper<EquipSet>(runtime.DressSets);
            api.Config.Register("injection.dresses", () => injectionDressSets);

            api.Config.Register("injection.options", () => InjectionOptions);
            api.Config.Register("injection.autoOpen", () => AutoOpenGui, () => true);
            api.Config.Register("injection.dressSpeed", () => DressSpeed, () => 0);

            api.CommandHandler.RunningCommandAdded += (sender, e) => NotifyRunningCommandsChange(e.CommandName);
            api.CommandHandler.RunningCommandRemoved+= (sender, e) => NotifyRunningCommandsChange(e.CommandName);
        }

        public void NotifyRunningCommandsChange(string name)
        {
            if (IsInjectionCommandName(name))
                RunningCommandsChanged?.Invoke();
        }

        public void OpenGui() => injectionWindow.Open(runtime, InjectionApi.UO, api, this);

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

            ActivateInjectionOptions();

            ScriptLoaded?.Invoke();
        }

        private void ActivateInjectionOptions()
        {
            if (AutoOpenGui)
                OpenGui();

            if (InjectionOptions.Light)
                api.ClientFilters.Light.Enable();
            else
                api.ClientFilters.Light.Disable();
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

        private string GetCommandName(SubrutineDefinition subrutine) => injectionCommandPrefix + subrutine.Name;

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
