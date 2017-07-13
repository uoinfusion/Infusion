using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infusion
{
    public sealed class Configuration : INotifyPropertyChanged
    {
        private string[] ignoredWords = Array.Empty<string>();
        private bool logToFileEnabled;
        private bool logPacketsToFileEnabled;
        private string logPath;
        private bool showImportantToastNotification;
        private bool toastNotificationEnabled;
        private bool hideWhenMinimized;

        public string[] IgnoredWords
        {
            get => ignoredWords;
            private set
            {
                ignoredWords = value; 
                OnPropertyChanged();
            }
        }

        public void SetIgnoredWords(IEnumerable<string> ignoredWords)
        {
            IgnoredWords = ignoredWords.ToArray();
        }

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

        public bool ShowImportantToastNotification
        {
            get => showImportantToastNotification;
            set
            {
                showImportantToastNotification = value;
                OnPropertyChanged();
            }
        }

        public bool ToastNotificationEnabled
        {
            get => toastNotificationEnabled;
            set
            {
                toastNotificationEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool HideWhenMinimized
        {
            get => hideWhenMinimized;
            set
            {
                hideWhenMinimized = value;
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
