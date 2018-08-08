#load "Specs.csx"
#load "common.csx"
#load "container.csx"
#load "area.csx"

using System;
using System.Collections.Generic;
using Infusion;
using Infusion.Commands;

public static class Banking
{
    private static Dictionary<Area, string> commandByArea = new Dictionary<Area, string>();

    public static void OpenBank()
    {
        var command = GetHouseMenuCommand();
        if (command != null)
        {
            if (OpenBankViaHouseMenu(command))
                return;
        }
        
        OpenBankViaBanker();
    }

    public static void OpenBankViaBanker(string bankerName = null)
    {
        Gump gump;
    
        int failedCount = 0;
    
        do
        {
            if (!string.IsNullOrEmpty(bankerName))
                UO.Say(bankerName + " hi");
            else
                UO.Say("banker hi");
                
            gump = UO.WaitForGump(true, TimeSpan.FromSeconds(10));
            if (gump == null)
            {
                failedCount++;
                if (failedCount > 5)
                    UO.Alert("Cannot open bank");
            }
        } while (gump == null);
        
        UO.Wait(1000);
    
        UO.SelectGumpButton("Bankovni sluzby", GumpLabelPosition.After);
        UO.WaitForGump();
    
        UO.Wait(1000);
        UO.SelectGumpButton("Otevrit banku.", GumpLabelPosition.After);
        Common.WaitForContainer();        
    }
    
    public static bool OpenBankViaHouseMenu(string houseMenuEquip = null)
    {
        if (houseMenuEquip == null)
            houseMenuEquip = GetHouseMenuCommand();
    
        if (string.IsNullOrEmpty(houseMenuEquip))
        {
            var menu = UO.Items.Matching(Specs.HouseMenu).OrderByDistance().FirstOrDefault();

            if (menu != null)
                UO.Use(menu);
            else
                return false;
        }
        else
            UO.Say(houseMenuEquip);
        
        UO.WaitForGump();
        UO.SelectGumpButton("Otevrit banku", GumpLabelPosition.Before);
        Common.WaitForContainer();        
        
        return true;
    }
    
    public static void SetHouseMenuCommand(Area area, string command)
    {
        commandByArea[area] = command;
    }
    
    public static string GetHouseMenuCommand()
    {
        foreach (var pair in commandByArea)
        {
            var area = pair.Key;
            
            if (area.InArea())
                return pair.Value;
        }
        
        return null;
    }
}

public class BankContainer : IContainer
{
    public ObjectId Id => UO.Me.BankBox.Id;

    public Item Item => UO.Me.BankBox;

    public bool Contains(Item item)
        => item.ContainerId.HasValue && item.ContainerId.Value == Id;

    public void Open()
    {
        if (UO.Me.BankBox == null || !OpenContainerTracker.IsOpen(UO.Me.BankBox.Id))
        {
            Banking.OpenBank();
            OpenContainerTracker.SetOpen(UO.Me.BankBox.Id);
        }
    }
}

UO.RegisterCommand("bank-open", Banking.OpenBank);