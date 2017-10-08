using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;

public static class Timers
{
    private struct TimerDefinition
    {
        public DateTime TimeOut { get; }
        public string Name { get; }
        public Color Color { get; }

        public TimerDefinition(TimeSpan timeOut, string name, Color color)
        {
            TimeOut = DateTime.UtcNow.Add(timeOut);
            Name = name;
            Color = color;
        }
    }

    private static object timersLock = new object();
    private static List<TimerDefinition> timers = new List<TimerDefinition>();
    private static Timer timer = new Timer();
    private static DateTime lastPrint;

    public static TimeSpan CountdownSpan { get; set; } = TimeSpan.FromSeconds(3);

    static Timers()
    {
        timer.AutoReset = true;
        timer.Interval = 50;
        timer.Elapsed += (sender, args) => PrintTimeouts();
        timer.Start();
    }

    public static void AddTimer(TimeSpan timeOut, string name, Color color)
    {
        lock (timersLock)
        {
            timers.Add(new TimerDefinition(timeOut, name, color));
        }
    }

    public static void CancelTimer(string name)
    {
        lock (timersLock)
        {
            var timersToRemove = timers.Where(x => x.Name.Equals(name, StringComparison.Ordinal));
            foreach (var timer in timersToRemove)
            {
                timers.Remove(timer);
            }
        }
    }

    private static void PrintTimeouts()
    {
        lock (timersLock)
        {
            DateTime now = DateTime.UtcNow;
            var expiredTimers = timers.Where(x => x.TimeOut < now).ToArray();

            foreach (var timer in expiredTimers)
            {
                timers.Remove(timer);
                UO.ClientPrint(timer.Name + "!!!", "timer", UO.Me.PlayerId, UO.Me.BodyType,
                    SpeechType.Speech, timer.Color, false);
            }
            
            if (lastPrint.AddSeconds(1) < now)
            {
                lastPrint = now;

                foreach (var timer in timers)
                {
                    var timeLeft = timer.TimeOut - now;
    
                    if (timeLeft < CountdownSpan)
                    {
                        UO.ClientPrint($"{timer.Name}: {timeLeft.TotalSeconds:N0}", "timer", UO.Me.PlayerId, UO.Me.BodyType,
                            SpeechType.Speech, timer.Color, false);
                    }
                }
            }
        }
    }
}