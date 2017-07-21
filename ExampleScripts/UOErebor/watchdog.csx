#load "ItemSpecs.csx"

using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Infusion.Packets.Server;

public static class Watchdog
{
    public static ItemSpec[] WatchedItemSpecs =
    {
        ItemSpecs.Wheat, ItemSpecs.Cotton, ItemSpecs.DoorSecretStone2, ItemSpecs.DoorSecretStone3, ItemSpecs.DoorSecretStone4,
        ItemSpecs.DoorSecretWood1, ItemSpecs.DoorSecretWood2, ItemSpecs.Door
    };

    public static void HandleObjectInfo(ObjectInfoPacket packet)
    {
        if (WatchedItemSpecs.Any(x => x.Matches(packet.Type)))
        {
            EnqueueNotification(packet.Id);
        }
    }
    
    public static void HandleDrawObject(DrawObjectPacket packet)
    {
        if (WatchedItemSpecs.Any(x => x.Matches(packet.Type)))
        {
            EnqueueNotification(packet.Id);
        }
    }
    
    private static object queueLock = new object();
    private static List<uint> notificationQueue = new List<uint>();
    
    private static void EnqueueNotification(uint itemId)
    {
        lock (queueLock)
        {
            notificationQueue.Add(itemId);
        }
    }
    
    private static uint[] DequeueNotifications()
    {
        lock (queueLock)
        {
            uint[] result;
        
            if (notificationQueue.Count > 0)
            {
                result = notificationQueue.ToArray();
                notificationQueue.Clear();
            }
            else
                result = Array.Empty<uint>();   
            
            return result;
        }
    }

    private static bool stopRequested;
    
    public static void StopWatchdog()
    {
        UO.ClientPrint("Stopping watchdog");
        Cleanup();
        stopRequested = true;
    }
    
    private static void Cleanup()
    {
        UO.Server.Unsubscribe(PacketDefinitions.ObjectInfo, HandleObjectInfo);
        UO.Server.Unsubscribe(PacketDefinitions.DrawObject, HandleDrawObject);

        DequeueNotifications();
    }
    
    public static void ShowWatchdog()
    {
        var items = WatchedItemSpecs.SelectMany(spec => UO.Items.Matching(spec)).ToArray();
        
        foreach (var item in items)
        {
            ShowNotification(item.Id);
        }
    }
    
    private static void ShowNotification(uint itemId)
    {
        var item = UO.Items[itemId];
        if (item == null)
        {
            UO.ClientPrint($"!!!!! Found some secret !!!!!");
        }
        else
        {
            UO.ClientPrint($"!!!!!!!!!!!!!!!", "Watchdog", item);
            UO.ClientPrint($"!!!!! Found {ItemSpecs.TranslateToName(item)} !!!!!");
        }
    }
    
    public static void StartWatchdog()
    {
        UO.ClientPrint("Starting watchdog");
    
        Cleanup();
        stopRequested = false;
        UO.Server.Subscribe(PacketDefinitions.ObjectInfo, HandleObjectInfo);
        UO.Server.Subscribe(PacketDefinitions.DrawObject, HandleDrawObject);
    
        while (!stopRequested)
        {
            var notifications = DequeueNotifications();
            foreach (var itemId in notifications)
            {
                ShowNotification(itemId);
            }
            
            Thread.Sleep(1000);
        }
        
        Cleanup();
        stopRequested = false;
        UO.ClientPrint("Watchdog stopped");
    }
}

UO.RegisterCommand("watchdog-start", Watchdog.StartWatchdog);
UO.RegisterCommand("watchdog-stop", Watchdog.StopWatchdog);
UO.RegisterCommand("watchdog-show", Watchdog.ShowWatchdog);
