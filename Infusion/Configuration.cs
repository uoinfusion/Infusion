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
        private ImmutableArray<string> ignoredWords = ImmutableArray<string>.Empty;

        public ImmutableArray<string> IgnoredWords
        {
            get => ignoredWords;
            set
            {
                ignoredWords = value; 
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
