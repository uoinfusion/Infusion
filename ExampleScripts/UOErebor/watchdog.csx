#load "Specs.csx"

using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Infusion.Packets.Server;

public static class Watchdog
{
    public static ItemSpec[] WatchedItemSpecs =
    {
        Specs.Wheat, Specs.Cotton, Specs.DoorSecretStone2, Specs.DoorSecretStone3, Specs.DoorSecretStone4,
        Specs.DoorSecretWood1, Specs.DoorSecretWood2, Specs.Door, Specs.Packa
    };
    
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
        UO.Events.ItemEnteredView -= HandleNewItem;

        DequeueNotifications();
    }
    
    private static void HandleNewItem(object sender, ItemEnteredViewEvent args)
    {
        if (WatchedItemSpecs.Any(s => s.Matches(args.NewItem)))
            EnqueueNotification(args.NewItem.Id);
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
            UO.ClientPrint($"!!!!! Found {Specs.TranslateToName(item)} !!!!!");
        }
    }
    
    public static void StartWatchdog()
    {
        UO.ClientPrint("Starting watchdog");
    
        Cleanup();
        stopRequested = false;
        UO.Events.ItemEnteredView += HandleNewItem;
    
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
