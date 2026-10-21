// Status window inside the game (a gump shown in ClassicUO) - replacement of the WPF statuses window.
// Infusion sends the gump to the client (0xB0); client responses (0xB1) to this gump are caught
// by a client packet filter and NEVER reach the server. Clicking a party member targets it (same as on Windows).
// Command: ,status-gump (show / hide). It is shown automatically after login.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infusion;
using Infusion.LegacyApi;
using Infusion.Packets;

namespace Infusion.Scripts.UOErebor.Extensions.StatusBars
{
    public static class StatusGump
    {
        private const uint GumpType = 0x7E5B0A11;   // custom, doesn't collide with server gumps
        private const uint GumpSerial = 0x7E5B0A12;
        private const int Width = 260;

        private static readonly object sync = new object();
        private static bool registered;
        private static bool filterRegistered;
        private static volatile bool visible;
        private static string lastLayout;
        private static DateTime ignoreCloseUntil = DateTime.MinValue;
        private static Dictionary<int, uint> buttonTargets = new Dictionary<int, uint>();
        private static Thread loop;

        public static bool ShowAfterLogin { get; set; } = true;

        public static void Register()
        {
            lock (sync)
            {
                if (registered) return;
                registered = true;
            }
            try { EnsureFilter(); } catch (Exception ex) { try { UO.Log("Status gump: " + ex.Message); } catch { } }
            try { UO.RegisterCommand("status-gump", Toggle); } catch { }

            if (ShowAfterLogin)
                Task.Run(() =>
                {
                    try
                    {
                        for (int i = 0; i < 600; i++) // wait for login up to 10 minutes
                        {
                            Thread.Sleep(1000);
                            var me = UO.Me;
                            if (me != null && !string.IsNullOrEmpty(me.Name) && me.MaxHealth > 0)
                            {
                                Thread.Sleep(3000);
                                if (!visible) Show();
                                return;
                            }
                        }
                    }
                    catch { }
                });
        }

        public static void Toggle()
        {
            if (visible) Hide(); else Show();
        }

        public static void Show()
        {
            EnsureFilter();
            visible = true;
            lastLayout = null;
            lock (sync)
            {
                if (loop == null || !loop.IsAlive)
                {
                    loop = new Thread(Loop) { IsBackground = true, Name = "StatusGump" };
                    loop.Start();
                }
            }
            UO.ClientPrint("Status gump: on (,status-gump = off)");
        }

        public static void Hide()
        {
            visible = false;
            CloseInClient();
            UO.ClientPrint("Status gump: off");
        }

        private static void EnsureFilter()
        {
            lock (sync)
            {
                if (filterRegistered) return;
                filterRegistered = true;
            }
            // IClientPacketSubject is internal in Infusion.dll -> reflection
            var iface = typeof(UltimaClient).Assembly.GetType("Infusion.IClientPacketSubject", true);
            var register = iface.GetMethod("RegisterFilter");
            register.Invoke(UO.Client, new object[] { new Func<Packet, Packet?>(FilterClientPacket) });
        }

        // Packets from the client to the server. A response to our gump is dropped (null).
        private static Packet? FilterClientPacket(Packet packet)
        {
            try
            {
                if (packet.Id != 0xB1) return packet;
                var p = packet.Payload;
                if (p == null || p.Length < 15) return packet;
                uint serial = ReadUInt(p, 3);
                uint type = ReadUInt(p, 7);
                if (serial != GumpSerial || type != GumpType) return packet;

                int button = (int)ReadUInt(p, 11);
                HandleResponse(button);
                return null;
            }
            catch
            {
                return packet;
            }
        }

        private static void HandleResponse(int button)
        {
            lastLayout = null; // the client closed the gump after the click - send it again
            if (button == 0)
            {
                // closed by right click (unless it was our own close while redrawing)
                if (DateTime.UtcNow > ignoreCloseUntil)
                    visible = false;
                return;
            }

            if (buttonTargets.TryGetValue(button, out var id))
            {
                var owner = Statuses.AllInstances().FirstOrDefault(s => s.Contains((ObjectId)id));
                if (owner != null)
                    Task.Run(() => { try { owner.OnMobileTargeted((ObjectId)id); } catch (Exception ex) { UO.Log(ex.Message); } });
            }
        }

        private static void Loop()
        {
            while (true)
            {
                try
                {
                    if (visible)
                        Refresh();
                }
                catch (Exception ex)
                {
                    try { UO.Log("Status gump: " + ex.Message); } catch { }
                }
                Thread.Sleep(700);
            }
        }

        private static void Refresh()
        {
            var texts = new List<string>();
            var targets = new Dictionary<int, uint>();
            var layout = BuildLayout(texts, targets);
            var key = layout + "|" + string.Join("|", texts);
            if (key == lastLayout) return;  // redraw only on change

            if (lastLayout != null)
            {
                ignoreCloseUntil = DateTime.UtcNow.AddMilliseconds(1500);
                CloseInClient();
            }
            buttonTargets = targets;
            UO.Client.ShowGump((GumpInstanceId)GumpSerial, (GumpTypeId)GumpType, 40, 120, layout, texts.ToArray());
            lastLayout = key;
        }

        private static void CloseInClient()
        {
            try
            {
                ignoreCloseUntil = DateTime.UtcNow.AddMilliseconds(1500);
                UO.Client.CloseGump((GumpTypeId)GumpType);
            }
            catch { }
        }

        // text hues
        private const int White = 1152, Grey = 946, Green = 68, Red = 38, Blue = 88, Yellow = 53, Poison = 63, Orange = 43;

        private static string BuildLayout(List<string> texts, Dictionary<int, uint> targets)
        {
            var body = new StringBuilder();
            int y = 10;

            // a script stopped running -> the whole gump is red, so it is noticed immediately
            string[] warnings;
            try { warnings = ScriptStatus.Warnings(); } catch { warnings = new string[0]; }
            bool failure = warnings.Length > 0;

            int T(string s) { texts.Add(s ?? ""); return texts.Count - 1; }
            void Text(int x, int yy, int hue, string s) => body.Append($"{{ text {x} {yy} {(failure ? Red : hue)} {T(s)} }}");
            void Bar(int x, int yy, int w, int cur, int max, bool poisoned)
            {
                body.Append($"{{ gumppictiled {x} {yy} {w} 11 2053 }}"); // 0x0805 background
                int fill = max > 0 ? (int)Math.Max(0L, Math.Min((long)w, (long)cur * w / max)) : 0;
                if (fill > 0)
                    body.Append($"{{ gumppictiled {x} {yy} {fill} 11 {(poisoned ? 2056 : 2054)} }}"); // 0x0808 / 0x0806
            }

            foreach (var w in warnings) { Text(10, y, Red, Shorten("! " + w, 30)); y += 18; }
            if (failure) { Text(10, y, Orange, "try ,reload and check the log"); y += 24; }

            try
            {
                var m = UO.Me;
                if (m != null)
                {
                    string state = m.IsDead ? " (dead)" : m.IsHidden ? " (hidden)" : "";
                    Text(10, y, m.IsPoisoned ? Poison : White, (m.Name ?? "") + state); y += 20;
                    // label | bar | value on fixed positions; 109 = natural width of the bar image (wider repeats it)
                    const int barX = 40, barW = 109, numX = 160;
                    Text(10, y, Red, "HP"); Bar(barX, y + 4, barW, m.CurrentHealth, m.MaxHealth, m.IsPoisoned);
                    Text(numX, y, White, $"{m.CurrentHealth}/{m.MaxHealth}"); y += 18;
                    Text(10, y, Blue, "MA"); Bar(barX, y + 4, barW, m.CurrentMana, m.MaxMana, false);
                    Text(numX, y, White, $"{m.CurrentMana}/{m.MaxMana}"); y += 18;
                    Text(10, y, Yellow, "ST"); Bar(barX, y + 4, barW, m.CurrentStamina, m.MaxStamina, false);
                    Text(numX, y, White, $"{m.CurrentStamina}/{m.MaxStamina}"); y += 24;

                    Text(10, y, Grey, "Str"); Text(42, y, White, m.Strength.ToString());
                    Text(92, y, Grey, "Dex"); Text(124, y, White, m.Dexterity.ToString());
                    Text(174, y, Grey, "Int"); Text(204, y, White, m.Intelligence.ToString()); y += 18;
                    int maxWeight = m.Strength * 4;
                    Text(10, y, Grey, "AR"); Text(42, y, White, m.Armor.ToString());
                    Text(92, y, Grey, "Weight"); Text(140, y, m.Weight >= maxWeight * 0.9 ? Orange : White, $"{m.Weight} / {maxWeight}"); y += 26;
                }
            }
            catch { }

            try
            {
                // lines from scripts (see ScriptStatus) - text on the left, remaining time on the right
                var lines = ScriptStatus.Current();
                if (lines.Length > 0)
                {
                    Text(10, y, Grey, "ACTIVE"); y += 20;
                    foreach (var line in lines)
                    {
                        Text(10, y, Green, Shorten(line.Text, 28));
                        if (line.Seconds >= 0)
                            Text(Width - 58, y, line.Seconds <= 10 ? Orange : White, line.Remaining);
                        y += 18;
                    }
                    y += 8;
                }
            }
            catch { }

            int button = 1;
            bool any = false;
            foreach (var g in Statuses.AllInstances())
            {
                var bars = g.Snapshot();
                if (bars.Length == 0) continue;
                any = true;
                Text(10, y, Grey, (g.Title ?? "Party").ToUpperInvariant()); y += 20;
                foreach (var b in bars)
                {
                    Mobile mob = null;
                    try { mob = UO.Mobiles[(ObjectId)b.Id]; } catch { }
                    string name = (b.NamePrefix ?? "") + (string.IsNullOrEmpty(b.Name) ? mob?.Name ?? "?" : b.Name);
                    int hp = mob != null ? (int)mob.CurrentHealth : b.CurrentHealth;
                    int max = mob != null ? (int)mob.MaxHealth : b.MaxHealth;
                    bool dead = mob != null ? mob.IsDead : b.IsDead;
                    bool poisoned = mob != null ? mob.IsPoisoned : b.IsPoisoned;
                    bool away = mob == null || b.IsOutOfSight;

                    targets[button] = (uint)b.Id;
                    body.Append($"{{ button 8 {y + 1} 5601 5605 1 0 {button} }}"); // small arrow = target
                    int hue = dead || away ? Grey : poisoned ? Poison : White;
                    string tag = dead ? " (dead)" : away ? " (away)" : "";
                    Text(26, y, hue, Shorten(name, 18) + tag);
                    y += 18;
                    Bar(26, y, 109, dead ? 0 : hp, max, poisoned);
                    Text(152, y - 4, hue, max > 0 ? $"{hp}/{max}" : "?");
                    y += 16;
                    button++;
                }
                y += 6;
            }
            if (!any)
            {
                Text(10, y, Grey, "Party is empty (,party-add)"); y += 20;
            }

            int height = y + 6;
            return $"{{ page 0 }}{{ resizepic 0 0 9270 {Width} {height} }}{{ checkertrans 6 6 {Width - 12} {height - 12} }}" + body;
        }

        private static string Shorten(string s, int max) => s.Length <= max ? s : s.Substring(0, max - 1) + ".";

        private static uint ReadUInt(byte[] p, int i) => (uint)(p[i] << 24 | p[i + 1] << 16 | p[i + 2] << 8 | p[i + 3]);
    }
}
