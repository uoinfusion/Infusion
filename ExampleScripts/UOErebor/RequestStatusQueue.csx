using System;
using System.Timers;
using System.Linq;
using System.Collections.Generic;

public class RequestStatusQueue
{
    public static ScriptTrace Trace { get; } = UO.Trace.Create();

    private object requestQueueLock = new object();
    private LinkedList<ObjectId> requestQueue = new LinkedList<ObjectId>();
    private DateTime lastProcessing;
    private Timer timer = new Timer();

    private TimeSpan oneRequestInterval = TimeSpan.FromMilliseconds(500);

    public TimeSpan OneRequestInterval
    {
        get => oneRequestInterval;
        set
        {
            oneRequestInterval = value;
            timer.Interval = value.TotalMilliseconds;
        }
    }

    public void StartProcessing()
    {
        timer.Interval = OneRequestInterval.TotalMilliseconds;
        timer.AutoReset = true;
        timer.Elapsed += (sender, args) => ProcessRequests();
        timer.Start();
    }
    
    public void StopProcessing()
    {
        timer.Stop();
    }

    private void ProcessRequests()
    {
        var now = DateTime.UtcNow;
        if (now - lastProcessing < OneRequestInterval)
            return;

        lock (requestQueueLock)
        {
            var requestedMobile = DequeueRequest();
            if (requestedMobile != null)
            {
                UO.RequestStatus(requestedMobile);
                requestQueue.Remove(requestedMobile.Id);
                Trace.Log($"Requested status of {requestedMobile.Id}, queue lenght {requestQueue.Count}");
            }
        }

        lastProcessing = now;
    }
    
    private Mobile DequeueRequest()
    {
        while (requestQueue.Any())
        {
            var requestedMobile = UO.Mobiles[requestQueue.Last.Value];
            requestQueue.RemoveLast();
            
            if (requestedMobile != null)
                return requestedMobile;
        }
        
        return null;
    }
 
    public void RequestStatus(ObjectId mobileId)
    {
        lock (requestQueueLock)
        {
            requestQueue.AddFirst(mobileId);
        }
    }
}