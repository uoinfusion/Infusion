using System;
using System.Linq;

public static class Hidding
{
    private static GameJournal hiddingJournal = UO.CreateJournal();

    public static void Hide()
    {
        var originalLocation = UO.Me.Location;
    
        bool hidden = false;
        do
        {
            UO.ClientPrint("Trying to hide");
            hiddingJournal.Delete();
            UO.UseSkill(Skill.Hiding);
            
            hiddingJournal
                .When("Skryti se povedlo.", () => hidden = true)
                .When("Nepovedlo se ti schovat", () => hidden = false)
                .WaitAny();
            
            if (UO.Me.Location != originalLocation)
            {
                UO.ClientPrint("Player has moved, stopping hidding");
                break;
            }
        } while (!hidden);
    }
}

UO.CommandHandler.RegisterCommand("hide", Hidding.Hide);
