// Optional status lines written by scripts (active ability, potion timers...) and script heartbeats.
// Scripts don't need to reference this library: they write to a shared dictionary stored in the AppDomain,
// so the same scripts keep working on Windows, where this library is not used.
//
// Contract: AppDomain.CurrentDomain.GetData("UOErebor.StatusLines") is a
// ConcurrentDictionary<string, Tuple<string, DateTime>> (key -> text, end of validity in UTC).
//   - DateTime.MaxValue = line without a countdown, otherwise the remaining time is shown,
//   - keys starting with "_hb:" are heartbeats: (description, last beat). When a heartbeat is older
//     than 30 s, the status gump turns red and shows "<description> is not running!",
//   - other keys starting with "_" are ignored (internal data of scripts).
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Infusion.Scripts.UOErebor.Extensions.StatusBars
{
    public static class ScriptStatus
    {
        public const string AppDomainKey = "UOErebor.StatusLines";
        public static readonly TimeSpan HeartbeatLimit = TimeSpan.FromSeconds(30);

        public struct Line
        {
            public string Text;
            public string Remaining;   // "" = no countdown
            public int Seconds;        // -1 = no countdown
        }

        /// <summary>The shared dictionary (created on first use), so scripts can also get it from here.</summary>
        public static ConcurrentDictionary<string, Tuple<string, DateTime>> Data
        {
            get
            {
                var d = AppDomain.CurrentDomain.GetData(AppDomainKey) as ConcurrentDictionary<string, Tuple<string, DateTime>>;
                if (d != null)
                    return d;
                lock (typeof(ScriptStatus))
                {
                    d = AppDomain.CurrentDomain.GetData(AppDomainKey) as ConcurrentDictionary<string, Tuple<string, DateTime>>;
                    if (d == null)
                    {
                        d = new ConcurrentDictionary<string, Tuple<string, DateTime>>();
                        AppDomain.CurrentDomain.SetData(AppDomainKey, d);
                    }
                    return d;
                }
            }
        }

        private static ConcurrentDictionary<string, Tuple<string, DateTime>> Existing()
            => AppDomain.CurrentDomain.GetData(AppDomainKey) as ConcurrentDictionary<string, Tuple<string, DateTime>>;

        /// <summary>Texts of warnings about scripts that stopped sending heartbeats.</summary>
        public static string[] Warnings()
        {
            var d = Existing();
            if (d == null) return new string[0];
            var now = DateTime.UtcNow;
            return d.Where(kv => kv.Key.StartsWith("_hb:") && now - kv.Value.Item2 > HeartbeatLimit)
                .Select(kv => kv.Value.Item1 + " is not running!")
                .OrderBy(x => x).ToArray();
        }

        public static Line[] Current()
        {
            var d = Existing();
            if (d == null) return new Line[0];

            var now = DateTime.UtcNow;
            var list = new List<Line>();
            foreach (var kv in d.ToArray())
            {
                if (kv.Key.StartsWith("_")) continue;
                var end = kv.Value.Item2;
                if (end == DateTime.MaxValue)
                {
                    list.Add(new Line { Text = kv.Value.Item1, Remaining = "", Seconds = -1 });
                    continue;
                }
                var remaining = end - now;
                if (remaining <= TimeSpan.Zero)
                {
                    if (remaining < TimeSpan.FromSeconds(-5))
                        ((ICollection<KeyValuePair<string, Tuple<string, DateTime>>>)d).Remove(kv);
                    continue;
                }
                int s = (int)Math.Ceiling(remaining.TotalSeconds);
                list.Add(new Line { Text = kv.Value.Item1, Seconds = s, Remaining = s >= 60 ? $"{s / 60}:{s % 60:00}" : $"{s} s" });
            }
            // lines without a countdown first, then by remaining time
            return list.OrderBy(p => p.Seconds < 0 ? 0 : 1).ThenBy(p => p.Seconds).ThenBy(p => p.Text).ToArray();
        }
    }
}
