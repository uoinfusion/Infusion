using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Infusion.LegacyApi;

namespace Infusion.Scripts.UOErebor.Extensions.StatusBars
{
    public sealed class Statuses
    {
        public static Statuses Create(string title, Func<IUltimaClientWindow> ultimaClientWindow)
        {
            if (statusesInstances.TryGetValue(title, out Statuses instance))
            {
                instance.MobileTargeted = delegate { };
                return instance;
            }

            instance = new Statuses(title, ultimaClientWindow);
            statusesInstances[title] = instance;

            return instance;
        }

        private readonly static Dictionary<string, Statuses> statusesInstances = new Dictionary<string, Statuses>();

        private readonly Func<IUltimaClientWindow> ultimaClientWindow;
        public StatusesConfiguration Configuration { get; }
        public string Title { get; private set; }

        private StatusesWindow statusesWindow;
        

        public event EventHandler<ObjectId> MobileTargeted;

        internal Statuses(string title, Func<IUltimaClientWindow> ultimaClientWindow)
        {
            Title = title;
            this.ultimaClientWindow = ultimaClientWindow;
            Configuration = new StatusesConfiguration(400, 400, WindowDock.None);
        }

        public int Count => StatusBars.Count;

        internal ObservableCollection<StatusBar> StatusBars { get; } = new ObservableCollection<StatusBar>();

        private bool NeedsReopen =>
            statusesWindow == null || PresentationSource.FromVisual(statusesWindow) == null ||
            !statusesWindow.IsVisible;

        internal void OnMobileTargeted(ObjectId id)
        {
            ultimaClientWindow().Focus();
            MobileTargeted?.Invoke(this, id);
        }

        public void Clear()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                StatusBars.Clear();
            }));
        }

        public void Open()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (statusesWindow != null && PresentationSource.FromVisual(statusesWindow) == null)
                    statusesWindow = new StatusesWindow(this);

                if (statusesWindow == null)
                    statusesWindow = new StatusesWindow(this);

                statusesWindow.Title = Title;
                statusesWindow.Width = Configuration.Width;
                statusesWindow.Height = Configuration.Height;
                var bounds = ultimaClientWindow().GetBounds();
                if (bounds.HasValue)
                {
                    switch (Configuration.GameClientWindowDock)
                    {
                        case WindowDock.BottomLeft:
                            statusesWindow.Left = bounds.Value.Left;
                            statusesWindow.Top = bounds.Value.Bottom - Configuration.Height;
                            break;
                        case WindowDock.BottomRight:
                            statusesWindow.Left = bounds.Value.Right - Configuration.Width;
                            statusesWindow.Top = bounds.Value.Bottom - Configuration.Height;
                            break;
                        case WindowDock.TopLeft:
                            statusesWindow.Top = bounds.Value.Top;
                            statusesWindow.Left = bounds.Value.Left;
                            break;
                        case WindowDock.TopRight:
                            statusesWindow.Top = bounds.Value.Top;
                            statusesWindow.Left = bounds.Value.Right - Configuration.Width;
                            break;
                    }
                }

                statusesWindow.Show();
            }));
        }

        public string WindowInfo
        {
            get
            {
                string result = "invalid";

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    result = statusesWindow != null
                        ? $"X: {statusesWindow.Left}, Y: {statusesWindow.Top}, Width: {statusesWindow.Width}, Height: {statusesWindow.Height}"
                        : "closed";
                }));

                return result;
            }
        }

        public void Close()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                statusesWindow?.Close();
            }));
        }

        private StatusBar Get(uint id) => StatusBars.FirstOrDefault(x => x.Id == id);

        internal void Add(StatusBar statusBar)
        {
            if (NeedsReopen)
                Open();
            StatusBars.Add(statusBar);
        }

        public void Add(Mobile mobile, StatusBarType type, string namePrefix = null)
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
                    statusBar = new StatusBar(mobile.Id, namePrefix)
                    {
                        Name = mobile.Name,
                        CurrentHealth = mobile.CurrentHealth,
                        MaxHealth = mobile.MaxHealth,
                        Type = type,
                        IsDead = mobile.IsDead,
                        IsPoisoned = mobile.IsPoisoned,
                    };
                }

                Add(statusBar);
            }));
        }

        public void Remove(ObjectId id)
        {
            var statusBar = Get(id);
            if (statusBar != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() => { StatusBars.Remove(statusBar); }));
            }
        }

        public void SetOutOfSight(ObjectId id, bool isOutOfSight)
        {
            var statusBar = Get(id);
            if (statusBar != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() => { statusBar.IsOutOfSight = isOutOfSight; }));
            }
        }

        public void Remove(Mobile mobile)
        {
            Remove(mobile.Id);
        }

        public void Update(Mobile mobile)
        {
            var statusBar = Get(mobile.Id);
            if (statusBar != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    statusBar.IsOutOfSight = false;
                    statusBar.CurrentHealth = mobile.CurrentHealth;
                    statusBar.MaxHealth = mobile.MaxHealth;
                    statusBar.IsDead = mobile.IsDead;
                    statusBar.IsPoisoned = mobile.IsPoisoned;
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