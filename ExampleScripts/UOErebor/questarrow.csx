using System;
using Infusion.LegacyApi;

public static class QuestArrow
{
    private static Location2D? currentArrowLocation;
    
    public static EventJournalAwaiter CreateHandler(EventJournal eventJournal) =>
        eventJournal.When<QuestArrowEvent>(HandleQuestChange);

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
}

UO.RegisterCommand("questarrow-last", QuestArrow.Last);
UO.RegisterHandler("questarrow", QuestArrow.CreateHandler);