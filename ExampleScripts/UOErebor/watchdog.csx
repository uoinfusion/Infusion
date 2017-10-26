#load "Specs.csx"

using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Infusion.Packets.Server;

public static class Watchdog
{
    public static ItemSpec WatchedItemSpec = new[]
    {
        Specs.Wheat, Specs.Cotton, Specs.DoorSecretStone2, Specs.DoorSecretStone3, Specs.DoorSecretStone4,
        Specs.DoorSecretWood1, Specs.DoorSecretWood2, Specs.Door, Specs.Packa
    };
    
    private static EventJournal eventJournal = UO.CreateEventJournal();
    
    public static void ShowWatchdog()
    {
        foreach (var item in UO.Items.Matching(WatchedItemSpec))
        {
            ShowNotification(item);
        }
    }
    
    private static void ShowNotification(Item item)
    {
        UO.ClientPrint($"!!!!!!!!!!!!!!!", "Watchdog", item);
        UO.ClientPrint($"!!!!! Found {Specs.TranslateToName(item)} !!!!!");
    }
    
    public static void Run()
    {
        UO.ClientPrint("Starting watchdog");
    
        while (true)
        {
            eventJournal
                .When<ItemEnteredViewEvent>(x =>
                {
                    if (WatchedItemSpec.Matches(x.NewItem))
                        ShowNotification(x.NewItem);
                })
                .WaitAny();
        }
    }
}

UO.RegisterCommand("watchdog", Watchdog.Run);
UO.RegisterCommand("watchdog-show", Watchdog.ShowWatchdog);
