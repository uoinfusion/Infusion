using Avalonia.Controls;
using Avalonia.Diagnostics.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Injection.Avalonia.Scripts
{
    public class ScriptItem
    {
        public ScriptItem(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class ScriptsViewModel : ViewModelBase
    {
        private IScriptServices scriptServices;

        public ScriptsViewModel(Func<Task<string[]>> openFile)
        {
            var canActOnRunningScript = this.WhenAnyValue(x => x.CanActOnRunningScript);
            var canActOnAvailableScript = this.WhenAnyValue(x => x.CanActOnAvailableScript);

            TerminateCommand = ReactiveCommand.Create(Terminate, canActOnRunningScript);
            RunCommand = ReactiveCommand.Create(Run, canActOnAvailableScript);
            LoadCommand = ReactiveCommand.CreateFromTask(Load);
            this.openFile = openFile;
        }

        private bool CanActOnRunningScript => SelectedRunningScript != null;

        private ObservableCollection<ScriptItem> runningScripts;
        public ObservableCollection<ScriptItem> RunningScripts
        {
            get => runningScripts;
            private set
            {
                RaiseAndSetIfChanged(ref runningScripts, value);
                RaisePropertyChanged(nameof(CanActOnRunningScript));
            }
        }

        private bool CanActOnAvailableScript => SelectedAvailableScript != null;

        private ObservableCollection<ScriptItem> availableScripts;
        public ObservableCollection<ScriptItem> AvailableScripts
        {
            get => availableScripts;
            private set
            {
                RaiseAndSetIfChanged(ref availableScripts, value);
                RaisePropertyChanged(nameof(CanActOnAvailableScript));
            }
        }

        public bool IsRunningScriptSelected => selectedRunningScript != null;

        private ScriptItem selectedRunningScript;
        public ScriptItem SelectedRunningScript
        {
            get => selectedRunningScript;
            set
            {
                RaiseAndSetIfChanged(ref selectedRunningScript, value);
                RaisePropertyChanged(nameof(CanActOnRunningScript));
            }
        }

        public bool IsAvailableScriptSelected => selectedAvailableScript != null;

        private ScriptItem selectedAvailableScript;
        private readonly Func<Task<string[]>> openFile;

        public ScriptItem SelectedAvailableScript
        {
            get => selectedAvailableScript;
            set
            {
                RaiseAndSetIfChanged(ref selectedAvailableScript, value);
                RaisePropertyChanged(nameof(CanActOnAvailableScript));
            }
        }

        private ObservableCollection<ScriptItem> GetObservableCommands(IEnumerable<string> commands)
            => new ObservableCollection<ScriptItem>(commands.Select(x => new ScriptItem(x)));

        public void SetServices(IScriptServices scriptServices)
        {
            this.scriptServices = scriptServices;

            RunningScripts = GetObservableCommands(scriptServices.RunningScripts);
            AvailableScripts = GetObservableCommands(scriptServices.AvailableScripts);

            this.scriptServices.RunningScriptsChanged += 
                () => RunningScripts = GetObservableCommands(scriptServices.RunningScripts);
        }

        public ReactiveCommand<Unit, Unit> TerminateCommand { get; }
        private void Terminate()
        {
            if (SelectedRunningScript?.Name != null)
                scriptServices.Terminate(SelectedRunningScript.Name);
        }

        public ReactiveCommand<Unit, Unit> RunCommand { get; }
        private void Run()
        {
            if (SelectedAvailableScript?.Name != null)
                scriptServices.Run(SelectedAvailableScript.Name);
        }

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        private async Task Load()
        {
            var files = await openFile();
            if (files.Any())
                scriptServices.Load(files.First());
        }
    }
}
