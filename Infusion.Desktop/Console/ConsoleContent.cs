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

        public ObservableCollection<ConsoleLine> ConsoleOutput
        {
            get => consoleOutput;
            set
            {
                consoleOutput = value;
                OnPropertyChanged("ConsoleOutput");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Func<ConsoleLine, bool> filter = (line) => true;

        private bool GameFilter(ConsoleLine line)
        {
            return line is ConsoleSpeechLine;
        }

        private bool SpeechOnlyFilter(ConsoleLine line)
        {
            if (line is ConsoleSpeechLine speech)
            {
                if (string.IsNullOrEmpty(speech.Name) || speech.Name.Equals("system", StringComparison.OrdinalIgnoreCase))
                    return false;
                if (speech.Name.Equals(speech.Message, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            else
                return false;

            return true;
        }

        private bool NoFilter(ConsoleLine line)
        {
            return true;
        }

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
            if (filter != GameFilter && filter != SpeechOnlyFilter)
                SetFilter(GameFilter);
            else if (filter != SpeechOnlyFilter)
                SetFilter(SpeechOnlyFilter);
            else
                SetFilter(NoFilter);
        }

        internal void ShowSpeechOnly()
        {
            SetFilter(SpeechOnlyFilter);
        }

        internal void ShowGame()
        {
            SetFilter(GameFilter);
        }

        internal void ShowAll()
        {
            SetFilter(NoFilter);
        }
    }
}