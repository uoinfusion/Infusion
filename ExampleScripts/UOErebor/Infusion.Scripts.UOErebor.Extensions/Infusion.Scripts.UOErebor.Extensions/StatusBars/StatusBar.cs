using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Scripts.UOErebor.Extensions.StatusBars
{
    internal sealed class StatusBar : INotifyPropertyChanged
    {
        private string name;
        private int maxHealth;
        private int currentHealth;

        public StatusBarType Type { get; set; }

        public uint Id { get; }

        public StatusBar(uint id)
        {
            Id = id;
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public int MaxHealth
        {
            get => maxHealth;
            set
            {
                maxHealth = value;
                OnPropertyChanged();
            }
        }

        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                currentHealth = value;
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
