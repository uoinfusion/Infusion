using Infusion.Diagnostic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Infusion.LegacyApi
{
    public sealed class LogConfiguration : INotifyPropertyChanged, IDiagnosticConfiguration
    {
        private bool hideWhenMinimized;
        private bool logPacketsToFileEnabled;
        private string logPath;
        private bool logToFileEnabled;

        public bool LogToFileEnabled
        {
            get => logToFileEnabled;
            set
            {
                logToFileEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool LogPacketsToFileEnabled
        {
            get => logPacketsToFileEnabled;
            set
            {
                logPacketsToFileEnabled = value;
                OnPropertyChanged();
            }
        }

        public string LogPath
        {
            get => logPath;
            set
            {
                logPath = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}