using System;
using System.Timers;

public class Counter : IDisposable
{
    private Timer timer;
    private object countdownLock = new object();
    private DateTime timerStarted;
    private DateTime lastNotificationTime;
    private int finalCount;

    public static ScriptTrace Trace = UO.Trace.Create();

    public string Name { get; }
    public Color Color { get; }
    public ObjectId TargetObjectId { get; }
    public ModelId TargetObjectType { get; }
    public Location3D TargetObjectLocation { get; }

    public void Dispose()
    {
        timer.Dispose();
        timer = null;
    }
    
    public Counter(string name, Color color, Item targetItem, int finalCount)
    {
        this.Name = name;
        this.Color = color;
        
        this.TargetObjectId = targetItem.Id;
        this.TargetObjectType = targetItem.Type;
        this.TargetObjectLocation = targetItem.Location;
        this.finalCount = finalCount;
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
            
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += (sender, args) => Tick();
            timer.Enabled = true;
            timerStarted = DateTime.UtcNow;
            
            Trace.Log($"Countdown {TargetObjectId}, {TargetObjectType}, {TargetObjectLocation} started");
        }
    }
    
    private void Tick()
    {
        var targetObject = (GameObject)UO.Mobiles[TargetObjectId] ?? UO.Items[TargetObjectId];
        var distance = UO.Me.Location.WithZ(0).GetDistance(TargetObjectLocation.WithZ(0));
        
        if (targetObject == null)
        {
            if (distance < 20)
            {
                Trace.Log($"Target item {TargetObjectId} gone.");
                Dispose();
                return;
            }
            else
            {
                Trace.Log($"Cannot see {TargetObjectId}.");
                return;
            }
        }

        var now = DateTime.UtcNow;
        var elapsed = now - this.timerStarted;
        if (elapsed.TotalSeconds > finalCount)
        {
            Trace.Log($"Final count {finalCount} reached after {elapsed.TotalSeconds} seconds");
            Dispose();
            return;
        }
                
        if (now - lastNotificationTime > TimeSpan.FromSeconds(1))
        {
            Trace.Log($"Counter tick, distance {distance}");
            UO.ClientPrint($"{Math.Round(elapsed.TotalSeconds):F0}", this.Name, TargetObjectId, TargetObjectType,
                SpeechType.Speech, this.Color, false);
            lastNotificationTime = now;
        }
    }
}