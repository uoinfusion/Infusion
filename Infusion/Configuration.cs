using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Infusion
{
    public sealed class Configuration : INotifyPropertyChanged
    {
        internal HashSet<SoundId> FilteredSoundSet = new HashSet<SoundId>();

        private bool filterLightEnabled = true;

        private bool filterWeatherEnabled = true;
        private bool hideWhenMinimized;
        private string[] ignoredWords = Array.Empty<string>();
        private bool logPacketsToFileEnabled;
        private string logPath;
        private bool logToFileEnabled;
        private bool showImportantToastNotification;
        private bool toastNotificationEnabled;

        public string[] IgnoredWords
        {
            get => ignoredWords;
            private set
            {
                ignoredWords = value;
                OnPropertyChanged();
            }
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

        public bool FilterWeatherEnabled
        {
            get => filterWeatherEnabled;
            set
            {
                filterWeatherEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool FilterLightEnabled
        {
            get => filterLightEnabled;
            set
            {
                filterLightEnabled = value;
                OnPropertyChanged();
            }
        }

        public SoundId[] FilteredSounds
        {
            get => FilteredSoundSet.ToArray();
            set
            {
                FilteredSoundSet = value == null ? new HashSet<SoundId>() : new HashSet<SoundId>(value);
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetIgnoredWords(IEnumerable<string> ignoredWords)
        {
            IgnoredWords = ignoredWords.ToArray();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}