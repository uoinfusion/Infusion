using Infusion.LegacyApi.Console;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Infusion.Desktop.Console
{
    public sealed class ConsoleContent : INotifyPropertyChanged
    {
        private ObservableCollection<ConsoleLine> consoleOutput = new ObservableCollection<ConsoleLine>();
        private readonly List<ConsoleLine> unfilteredConsoleOutput = new List<ConsoleLine>();

        private Func<ConsoleLine, bool> filter = x => true;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ConsoleLine> ConsoleOutput
        {
            get => consoleOutput;
            set
            {
                consoleOutput = value;
                OnPropertyChanged("ConsoleOutput");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool GameFilter(ConsoleLine line)
        {
            return line is ConsoleSpeechLine;
        }

        private bool SpeechOnlyFilter(ConsoleLine line) 
            => line is ConsoleSpeechLine speech && speech.IsSpeech;

        private bool NoDebugFilter(ConsoleLine line)
        {
            if (line is ConsoleInfusionLine infusionLine)
                return infusionLine.Type != ConsoleLineType.Debug;
            else
                return true;
        }

        private bool NoFilter(ConsoleLine line) => true;

        private void SetFilter(Func<ConsoleLine, bool> filter)
        {
            if (this.filter != filter)
            {
                this.filter = filter;

                ConsoleOutput.Clear();
                foreach (var line in unfilteredConsoleOutput)
                {
                    if (filter(line))
                        ConsoleOutput.Add(line);
                }
            }
        }

        public void Add(ConsoleLine line)
        {
            if (filter(line))
            {
                if (ConsoleOutput.Count > 1024)
                {
                    var removedItem = ConsoleOutput[0];
                    ConsoleOutput.RemoveAt(0);
                    unfilteredConsoleOutput.Remove(removedItem);
                }

                ConsoleOutput.Add(line);
            }
            else
            {
                if (unfilteredConsoleOutput.Count > 1024)
                {
                    unfilteredConsoleOutput.RemoveAt(0);
                }
            }

            unfilteredConsoleOutput.Add(line);
        }

        public void Clear()
        {
            consoleOutput.Clear();
            unfilteredConsoleOutput.Clear();
        }

        internal void ShowToggle()
        {
            if (filter == NoFilter)
                ShowNoDebug();
            else if (filter == NoDebugFilter)
                ShowGame();
            else if (filter == GameFilter)
                ShowSpeechOnly();
            else if (filter == SpeechOnlyFilter)
                ShowAll();
        }

        internal void ShowSpeechOnly() => SetFilter(SpeechOnlyFilter);
        internal void ShowGame() => SetFilter(GameFilter);
        internal void ShowNoDebug() => SetFilter(NoDebugFilter);

        internal void ShowAll() => SetFilter(NoFilter);
    }
}