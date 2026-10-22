// Cross-platform version with the same API as the WPF original. The state is kept in memory and shown
// in StatusGump (in game), StatusWindow (browser) and in the Infusion console (,party-show).
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infusion.LegacyApi;

namespace Infusion.Scripts.UOErebor.Extensions.StatusBars
{
    public sealed class Statuses
    {
        private static readonly Dictionary<string, Statuses> statusesInstances = new Dictionary<string, Statuses>();
        private readonly Func<IUltimaClientWindow> ultimaClientWindow;
        private readonly object sync = new object();
        private bool opened;

        public static Statuses Create(string title, Func<IUltimaClientWindow> ultimaClientWindow)
        {
            lock (statusesInstances)
            {
                if (statusesInstances.TryGetValue(title, out Statuses instance))
                {
                    instance.MobileTargeted = delegate { };
                    instance.Clear();
                    return instance;
                }

                instance = new Statuses(title, ultimaClientWindow);
                statusesInstances[title] = instance;
                StatusWindow.Start();
                return instance;
            }
        }

        internal Statuses(string title, Func<IUltimaClientWindow> ultimaClientWindow)
        {
            Title = title;
            this.ultimaClientWindow = ultimaClientWindow;
            Configuration = new StatusesConfiguration(400, 400, WindowDock.None);
        }

        public StatusesConfiguration Configuration { get; }
        public string Title { get; private set; }
        public event EventHandler<ObjectId> MobileTargeted;
        public int Count { get { lock (sync) return StatusBars.Count; } }
        public ObservableCollection<StatusBar> StatusBars { get; } = new ObservableCollection<StatusBar>();

        internal void OnMobileTargeted(ObjectId id) => MobileTargeted?.Invoke(this, id);

        public void Clear() { lock (sync) StatusBars.Clear(); }

        // for StatusGump / StatusWindow: snapshot of all groups and their bars
        internal static Statuses[] AllInstances()
        {
            lock (statusesInstances) return statusesInstances.Values.ToArray();
        }

        internal StatusBar[] Snapshot()
        {
            lock (sync) return StatusBars.ToArray();
        }

        public void Open()
        {
            StatusWindow.OpenWindow();
            opened = true;
            StatusBar[] bars;
            lock (sync) bars = StatusBars.ToArray();
            UO.Log($"== {Title} ({bars.Length}) ==");
            foreach (var b in bars)
            {
                var flags = (b.IsDead ? " [dead]" : "") + (b.IsPoisoned ? " [poisoned]" : "") + (b.IsOutOfSight ? " [out of sight]" : "");
                UO.Log($"  {b.NamePrefix}{b.Name}: {b.CurrentHealth}/{b.MaxHealth} ({b.Type}){flags}");
            }
        }

        public string WindowInfo => opened ? $"window {StatusWindow.Url} (+ console output)" : "closed";

        public void Close() => opened = false;

        private StatusBar Get(uint id) { lock (sync) return StatusBars.FirstOrDefault(x => x.Id == id); }

        public void Add(Mobile mobile, StatusBarType type, string namePrefix = null)
        {
            lock (sync)
            {
                var statusBar = StatusBars.FirstOrDefault(x => x.Id == mobile.Id);
                if (statusBar != null)
                    StatusBars.Remove(statusBar);
                else
                    statusBar = new StatusBar(mobile.Id, namePrefix)
                    {
                        Name = mobile.Name,
                        CurrentHealth = mobile.CurrentHealth,
                        MaxHealth = mobile.MaxHealth,
                        Type = type,
                        IsDead = mobile.IsDead,
                        IsPoisoned = mobile.IsPoisoned,
                    };
                StatusBars.Add(statusBar);
            }
        }

        public void Add(ObjectId id, string name, int hp, int maxHp, StatusBarType type, string namePrefix = null)
        {
            lock (sync)
            {
                var statusBar = StatusBars.FirstOrDefault(x => x.Id == id);
                if (statusBar != null)
                    StatusBars.Remove(statusBar);
                else
                    statusBar = new StatusBar(id, namePrefix)
                    {
                        Name = name, CurrentHealth = hp, MaxHealth = maxHp, Type = type,
                    };
                StatusBars.Add(statusBar);
            }
        }

        public void Remove(ObjectId id)
        {
            lock (sync)
            {
                var bar = StatusBars.FirstOrDefault(x => x.Id == id);
                if (bar != null) StatusBars.Remove(bar);
            }
        }

        public void Remove(Mobile mobile) => Remove(mobile.Id);

        public void SetOutOfSight(ObjectId id, bool isOutOfSight)
        {
            var bar = Get(id);
            if (bar != null) bar.IsOutOfSight = isOutOfSight;
        }

        public void Update(Mobile mobile)
        {
            var bar = Get(mobile.Id);
            if (bar == null) return;
            bar.IsOutOfSight = false;
            bar.CurrentHealth = mobile.CurrentHealth;
            bar.MaxHealth = mobile.MaxHealth;
            bar.IsDead = mobile.IsDead;
            bar.IsPoisoned = mobile.IsPoisoned;
            if (string.IsNullOrEmpty(bar.Name) && !string.IsNullOrEmpty(mobile.Name))
                bar.Name = mobile.Name;
        }

        public void UpdateHealth(ObjectId id, int currentHealt, int maxHealth)
        {
            var bar = Get(id);
            if (bar == null) return;
            bar.CurrentHealth = currentHealt;
            bar.MaxHealth = maxHealth;
        }

        public bool Contains(Mobile mobile) => Get(mobile.Id) != null;
        public bool Contains(ObjectId id) => Get(id) != null;
    }
}
