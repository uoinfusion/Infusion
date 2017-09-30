using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Infusion.Scripts.UOErebor.Extensions.StatusBars
{
    public sealed class Statuses
    {
        private StatusesWindow statusesWindow;

        internal string Title { get; private set; }

        public event EventHandler<ObjectId> MobileTargeted;

        public Statuses(string title)
        {
            Title = title;
        }

        public int Count => StatusBars.Count;

        internal ObservableCollection<StatusBar> StatusBars { get; } = new ObservableCollection<StatusBar>();

        private bool NeedsReopen =>
            statusesWindow == null || PresentationSource.FromVisual(statusesWindow) == null ||
            !statusesWindow.IsVisible;

        internal void OnMobileTargeted(ObjectId id)
        {
            MobileTargeted?.Invoke(this, id);
        }

        public void Open()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (statusesWindow != null && PresentationSource.FromVisual(statusesWindow) == null)
                    statusesWindow = new StatusesWindow(this);

                if (statusesWindow == null)
                    statusesWindow = new StatusesWindow(this);

                statusesWindow.Show();
            }));
        }

        private StatusBar Get(uint id) => StatusBars.FirstOrDefault(x => x.Id == id);

        internal void Add(StatusBar statusBar)
        {
            if (NeedsReopen)
                Open();
            StatusBars.Add(statusBar);
        }

        public void Add(Mobile mobile, StatusBarType type)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                var statusBar = Get(mobile.Id);
                if (statusBar != null)
                {
                    StatusBars.Remove(statusBar);
                }
                else
                {
                    statusBar = new StatusBar(mobile.Id)
                    {
                        Name = mobile.Name,
                        CurrentHealth = mobile.CurrentHealth,
                        MaxHealth = mobile.MaxHealth,
                        Type = type
                    };
                }

                Add(statusBar);
            }));
        }

        public void Remove(Mobile mobile)
        {
            var statusBar = Get(mobile.Id);
            if (statusBar != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() => { StatusBars.Remove(statusBar); }));
            }
        }

        public void Update(Mobile mobile)
        {
            var statusBar = Get(mobile.Id);
            if (statusBar != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    statusBar.CurrentHealth = mobile.CurrentHealth;
                    statusBar.MaxHealth = mobile.MaxHealth;
                    if (string.IsNullOrEmpty(statusBar.Name) && !string.IsNullOrEmpty(mobile.Name))
                        statusBar.Name = mobile.Name;
                }));
            }
        }

        public bool Contains(Mobile mobile) => StatusBars.Any(x => x.Id == mobile.Id);

        internal void Remove(StatusBar statusBar)
        {
            if (NeedsReopen)
                Open();

            StatusBars.Remove(statusBar);
        }
    }
}