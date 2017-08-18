using System;
using System.Linq;

public static class Hidding
{
    // Create a "private" instance of journal for cooking. If you delete this journal it
    // doesn't affect either UO.Journal or other instances of GameJournal so
    // you can try hidding and healing in parallel (use bandage, target self, start hidding)
    // and you can still use journal.Delete method in both scripts at the same time.
    // It means, that you don't need tricks like UO.SetJournalLine(number,text) in
    // Injection.
    private static GameJournal hiddingJournal = UO.CreateJournal();

    public static void Hide()
    {
        var originalLocation = UO.Me.Location;
    
        bool hidden = false;
        do
        {
            UO.ClientPrint("Trying to hide");
            
            // Don't worry, it will not affect any other script.
            hiddingJournal.Delete();
            UO.UseSkill(Skill.Hiding);

            // This waits until "Skryti se povedlo." or "Nepovedlo se ti schovat" arrives to journal.
            hiddingJournal
                .When("Skryti se povedlo.", () =>
                    // what happens when "Skryti se povdelo." arrives first to the journal
                    hidden = true)
                .When("Nepovedlo se ti schovat", () =>
                    // what happens when "Nepovedlo se ti schovat" arrives first to the journal
                    hidden = false)
                // if server sends neither "Skryti se povedlo." nor "Nepovedlo se ti schovat"
                // in one minute, then the script terminates with an TimoutException.
                .WaitAny(TimeSpan.FromMinutes(1));
            
            // If player moves then terminate the attempt. There is actually
            // no specific reason for this check.
            // Just a matter of taste - by moving player states that he doesn't
            // want to try hide anymore (and this is may not be always true).
            if (UO.Me.Location != originalLocation)
            {
                UO.ClientPrint("Player has moved, stopping hidding");
                break;
            }
        } while (!hidden);
    }
}

UO.CommandHandler.RegisterCommand("hide", Hidding.Hide);
