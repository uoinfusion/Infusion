using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;

public class Countdown : IDisposable
{
    public static ScriptTrace Trace = UO.Trace.Create();

    private static CountdownStage[] defaultStages = new[] { new CountdownStage(TimeSpan.MaxValue, TimeSpan.FromSeconds(1)) };
    private Timer timer;
    private object countdownLock = new object();
    private DateTime timerStarted;
    private DateTime lastNotificationTime;
    private int currentStageIndex;
    private static readonly TimeSpan countdownPrecission = TimeSpan.FromMilliseconds(50);

    public TimeSpan Timeout { get; }
    public string Name { get; }
    public Color Color { get; }
    public ObjectId TargetObjectId { get; }
    public ModelId TargetObjectType { get; }

    public CountdownStage[] Stages { get; }

    public bool Expired { get; private set; }

    public Countdown(TimeSpan timeout, string name, Color color, IEnumerable<CountdownStage> stages)
    {
        this.Timeout = timeout;
        this.Name = name;
        this.Color = color;

        this.Stages = stages.OrderByDescending(x => x.StartBeforeTimeout).ToArray();

        this.TargetObjectId = UO.Me.PlayerId;
        this.TargetObjectType = UO.Me.BodyType;
    }

    public Countdown(TimeSpan timeout, string name, Color color, ObjectId targetObjectId, ModelId targetObjectType,
        IEnumerable<CountdownStage> stages) : this(timeout, name, color, stages)
    {
        this.TargetObjectId = targetObjectId;
        this.TargetObjectType = targetObjectType;
    }

    public Countdown(TimeSpan timeout, string name, Color color,
        ObjectId targetObjectId, ModelId targetObjectType)
        : this(timeout, name, color, defaultStages)
    {
        this.TargetObjectId = targetObjectId;
        this.TargetObjectType = targetObjectType;
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

            string text = string.IsNullOrEmpty(this.Name) ?
                $"{Timeout.TotalSeconds:F0}" :
                $"{this.Name}: {Timeout.TotalSeconds:F0}";
            UO.ClientPrint(text, this.Name, TargetObjectId, TargetObjectType,
                SpeechType.Speech, this.Color, false);
            lastNotificationTime = timerStarted;
        }
    }

    private void CountdownTick()
    {
        var targetObject = (GameObject)UO.Mobiles[TargetObjectId] ?? UO.Items[TargetObjectId];
        if (targetObject == null)
        {
            Trace.Log($"Cannot see {TargetObjectId}.");
            return;
        }

        var now = DateTime.UtcNow;
        var elapsed = now - this.timerStarted;
        var timeLeft = this.Timeout - elapsed;

        if (timeLeft < countdownPrecission)
        {
            UO.ClientPrint($"{this.Name} !!!", this.Name, targetObject.Id, targetObject.Type,
                SpeechType.Speech, this.Color, false);
            this.Dispose();
            return;
        }

        if (Stages != null && Stages.Any())
        {
            if (currentStageIndex + 1 < Stages.Length && timeLeft < Stages[currentStageIndex + 1].StartBeforeTimeout)
            {
                currentStageIndex++;
                Trace.Log($"currentStageIndex: {currentStageIndex}");
            }

            if (Trace.Enabled)
            {
                Trace.Log($"{timeLeft} < {Stages[currentStageIndex].StartBeforeTimeout}");
                Trace.Log($"currentIndex: {currentStageIndex}, {now - lastNotificationTime} > {Stages[currentStageIndex].NotificationPeriod}");
            }

            if (timeLeft < Stages[currentStageIndex].StartBeforeTimeout
                && now - lastNotificationTime > Stages[currentStageIndex].NotificationPeriod)
            {
                string text = string.IsNullOrEmpty(this.Name) ?
                    $"{Math.Round(timeLeft.TotalSeconds):F0}" :
                    $"{this.Name}: {Math.Round(timeLeft.TotalSeconds):F0}";

                UO.ClientPrint(text, this.Name, TargetObjectId, TargetObjectType,
                    SpeechType.Speech, this.Color, false);
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
            Expired = true;
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
