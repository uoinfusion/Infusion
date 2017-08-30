#load "items.csx"
#load "Specs.csx"

using System;
using System.Linq;

// TODO: implement afk checking
public static class Tailoring
{
    public static void ProcessYarn(Item reloadContainer, uint unloadContainerId, ushort batchAmount)
    {
    
        var unloadContainer = UO.Items[unloadContainerId];
        if (unloadContainer == null)
        {
            UO.ClientPrint("Cannot find unload container");
            return;
        }
        
        while (true)
        {
            UO.ClientPrint("Unloading");
            Items.MoveItems(UO.Items.Matching(Specs.FoldedCloth).InContainer(UO.Me.BackPack), unloadContainer);
    
            var yarn = UO.Items.Matching(Specs.BallsOfYarn).InContainer(UO.Me.BackPack);
            
            if (!yarn.Any())
            {
                UO.ClientPrint("Reloading");
                yarn = reloadContainer != null ?
                    UO.Items.Matching(Specs.BallsOfYarn).InContainer(reloadContainer) :
                    UO.Items.Matching(Specs.BallsOfYarn).OnGround().MaxDistance(3);
                if (yarn.Any())
                    Items.MoveItems(yarn, batchAmount, UO.Me.BackPack);
                else
                {
                    UO.ClientPrint("No more yarn in reload conainer");
                    break;
                }
            }
    
            ProcessYarn();
        }
    }
    
    public static void ProcessYarn()
    {
        var loom = UO.Items.Matching(Specs.Loom).OnGround().OrderByDistance().FirstOrDefault();
        if (loom == null)
        {
            UO.ClientPrint("Cannot find any loom");
            return;
        }
    
        Item yarn;
        while ((yarn = UO.Items.Matching(Specs.BallsOfYarn).InContainer(UO.Me.BackPack).FirstOrDefault()) != null)
        {
            UO.Use(yarn);
            UO.WaitForTarget();
            UO.Target(loom);
            UO.Journal.WaitAny("You put the folded cloth in your pack.", "You put the folded cloth at your feet. It is too heavy..");
            UO.Wait(100);
        }
    }
    
    public static uint? ReloadContainerId;
    
    public static void YarnCommand()
    {
        UO.ClientPrint("Openning main container");
        var mainContainer = UO.Items[ReloadContainerId.Value];
        if (mainContainer == null)
        {
            UO.ClientPrint("Cannot find main container");
            return;
        }
        UO.Use(mainContainer);
        UO.Wait(1000);
        
        var reloadContainer = UO.Items[ReloadContainerId.Value];
        
        UO.ClientPrint("Openning reload container");
        if (reloadContainer != null)
        {
            UO.Use(reloadContainer);
            UO.Wait(1000);
        }
        else
        {
            UO.ClientPrint("Cannot find reload container");
            return;
        }
        
        int yarnAmount = UO.Items.Matching(Specs.BallsOfYarn).InContainer(reloadContainer).Sum(i => i.Amount);
        if (yarnAmount < 100)
        {
            UO.ClientPrint($"Found yarn amount to low: {yarnAmount}.");
            return;
        }
        else
            UO.ClientPrint($"Amount of unprocessed yarn: {yarnAmount}.");
        
        Tailoring.ProcessYarn(reloadContainer, reloadContainer.Id, 25);
    }
}