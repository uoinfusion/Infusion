// Mentioned on wiki\Commands.md

using System;
using Infusion.Commands;
using Infusion.LegacyApi;

public static class QuestArrow
{
    private static Location2D? currentArrowLocation;
    private static EventJournal journal = UO.CreateEventJournal();
    
    public static void Run() =>
        journal
            .When<QuestArrowEvent>(HandleQuestChange)
            .Incomming();

    private static void HandleQuestChange(QuestArrowEvent e)
    {
        string message;
    
        if (e.Active)
        {
            currentArrowLocation = e.Location;            
            message = CurrentQuestDescription;
        }
        else
        {
            message = $"Quest deactivated (target location: {e.Location}";
            currentArrowLocation = null;
        }
        
        UO.ClientPrint(message);
    }

    private static string CurrentQuestDescription =>
        currentArrowLocation.HasValue ?
            $"New quest active, target location: {currentArrowLocation.Value}" :
            "no quest active";

    
    public static void Last()
    {
        UO.ClientPrint(CurrentQuestDescription);
    }
    
    public static void Disable()
    {
        UO.CommandHandler.Terminate("questarrow");
    }
    
    public static void Enable()
    {
        UO.CommandHandler.Invoke("questarrow");
    }
}

UO.RegisterBackgroundCommand("questarrow", QuestArrow.Run);
UO.RegisterCommand("questarrow-enable", QuestArrow.Enable);
UO.RegisterCommand("questarrow-disable", QuestArrow.Disable);
UO.RegisterCommand("questarrow-last", QuestArrow.Last);