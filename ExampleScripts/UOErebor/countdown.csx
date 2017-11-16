using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;

public class Countdown : IDisposable
{
    private Timer timer;
    private object countdownLock = new object();
    private DateTime timerStarted;
    private DateTime lastNotificationTime;
    private int currentStageIndex;
    private static readonly TimeSpan countdownPrecission = TimeSpan.FromMilliseconds(50);

    public TimeSpan Timeout { get; }
    public string Name { get; }
    public Color Color { get; }
    
    public CountdownStage[] Stages { get; }
    
    public Countdown(TimeSpan timeout, string name, Color color, IEnumerable<CountdownStage> stages)
    {
        this.Timeout = timeout;
        this.Name = name;
        this.Color = color;
        
        this.Stages = stages.OrderByDescending(x => x.StartBeforeTimeout).ToArray();
    }
    
    public void Start()
    {
        lock (countdownLock)
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
            
            currentStageIndex = 0;
            timer = new Timer();
            timer.Interval = countdownPrecission.TotalMilliseconds;
            timer.Elapsed += (sender, args) => CountdownTick();
            timer.Enabled = true;
            timerStarted = DateTime.UtcNow;
            UO.ClientPrint($"{this.Name}: {Timeout.TotalSeconds:F0} s", this.Name, UO.Me.PlayerId, UO.Me.BodyType,
                SpeechType.Speech, this.Color, true);
        }
    }
    
    private void CountdownTick()
    {
        var now = DateTime.UtcNow;
        var elapsed = now - this.timerStarted;
        var timeLeft = this.Timeout - elapsed;
        
        if (timeLeft < countdownPrecission)
        {
            UO.ClientPrint($"{this.Name} !!!", this.Name, UO.Me.PlayerId, UO.Me.BodyType,
                SpeechType.Speech, this.Color, true);
            this.Dispose();
            return;
        }
        
        if (Stages.Any())
        {
            if (currentStageIndex + 1 < Stages.Length && timeLeft < Stages[currentStageIndex + 1].StartBeforeTimeout)
            {
                currentStageIndex++;
            }
        
            if (timeLeft < Stages[currentStageIndex].StartBeforeTimeout 
                && now - lastNotificationTime > Stages[currentStageIndex].NotificationPeriod)
            {
                UO.ClientPrint($"{this.Name}: {Math.Round(timeLeft.TotalSeconds):F0} s", this.Name, UO.Me.PlayerId, UO.Me.BodyType,
                    SpeechType.Speech, this.Color, true);
                lastNotificationTime = now;
            }
         }
    }
    
    public void Cancel()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        lock (countdownLock)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }
    }
}

public class CountdownStage
{
    public TimeSpan StartBeforeTimeout { get; }
    public TimeSpan NotificationPeriod { get; }
    
    public CountdownStage(TimeSpan startBeforeTimeout, TimeSpan notificationPeriod)
    {
        this.StartBeforeTimeout = startBeforeTimeout;
        this.NotificationPeriod = notificationPeriod;
    }
}
