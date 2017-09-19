using System;
using Infusion.LegacyApi;

public static class QuestArrow
{
    private static bool enabled = false;
    private static Location2D? currentArrowLocation;

    static QuestArrow()
    {
        UO.Events.QuestArrowChanged += HandleQuestChange;
    }

    public static void Start()
    {
        enabled = true;
    }

    private static void HandleQuestChange(object sender, QuestArrowArgs e)
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
        
        if (enabled)
            UO.ClientPrint(message);
    }

    private static string CurrentQuestDescription =>
        currentArrowLocation.HasValue ?
            $"New quest active, target location: {currentArrowLocation.Value}" :
            "no quest active";

    public static void Stop()
    {
        enabled = false;
    }
    
    public static void Info()
    {
        UO.ClientPrint(CurrentQuestDescription);
    }
}

UO.RegisterCommand("questarrow-info", QuestArrow.Info);
UO.RegisterCommand("questarrow-start", QuestArrow.Start);
UO.RegisterCommand("questarrow-stop", QuestArrow.Stop);