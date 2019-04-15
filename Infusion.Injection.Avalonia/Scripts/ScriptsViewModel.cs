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
            TerminateCommand = ReactiveCommand.Create(Terminate);
            RunCommand = ReactiveCommand.Create(Run);
            LoadCommand = ReactiveCommand.CreateFromTask(Load);
            this.openFile = openFile;
        }

        private ObservableCollection<ScriptItem> runningScripts;
        public ObservableCollection<ScriptItem> RunningScripts
        {
            get => runningScripts;
            private set
            {
                RaiseAndSetIfChanged(ref runningScripts, value);
            }
        }

        private ObservableCollection<ScriptItem> availableScripts;
        public ObservableCollection<ScriptItem> AvailableScripts
        {
            get => availableScripts;
            private set
            {
                RaiseAndSetIfChanged(ref availableScripts, value);
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
            }
        }

        public void SetServices(IScriptServices scriptServices)
        {
            this.scriptServices = scriptServices;

            RunningScripts = new ObservableCollection<ScriptItem>(scriptServices.RunningScripts.Select(x => new ScriptItem(x)));
            AvailableScripts = new ObservableCollection<ScriptItem>(scriptServices.AvailableScripts.Select(x => new ScriptItem(x)));
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
