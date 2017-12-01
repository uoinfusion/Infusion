using System;
using System.Linq;

public static class Hidding
{
    // Create a "private" instance of journal for cooking. If you delete this journal it
    // doesn't affect either UO.Journal or other instances of SpeechJournal so
    // you can try hidding and healing in parallel (use bandage, target self, start hidding)
    // and you can still use journal.Delete method in both scripts at the same time.
    // It means, that you don't need tricks like UO.SetJournalLine(number,text) in
    // Injection.
    private static SpeechJournal hiddingJournal = UO.CreateSpeechJournal();

    public static void Hide()
    {
        UO.ClientFilters.Stamina.Disable();

        if (UO.CommandHandler.IsCommandRunning("hide-run"))
        {
            UO.ClientPrint("Hidding stopped", "hidding", UO.Me);
            UO.CommandHandler.Terminate("hide-run");
        }
        else
        {
            UO.CommandHandler.Invoke("hide-run");
            if (!UO.CommandHandler.IsCommandRunning("hide-watchalwayswalk"))
                UO.CommandHandler.Invoke("hide-watchalwayswalk");
        }
    }
    
    private static SpeechJournal alwaysWalkJournal = UO.CreateSpeechJournal();
    
    public static void RunWatchAlwaysWalk()
    {
        while (true)
        {
            alwaysWalkJournal.WaitAny(TimeSpan.MaxValue, "Byla jsi objevena", "Byl jsi objeven", "You have been revealed!");
            UO.ClientFilters.Stamina.Disable();
        }
    }

    public static void RunHidding()
    {
        UO.WarModeOff();
    
        var originalLocation = UO.Me.Location;
    
        bool hidden = false;
        do
        {
            UO.ClientPrint("Trying to hide", "hidding", UO.Me);
            
            // Don't worry, it will not affect any other scripts.
            hiddingJournal.Delete();
            UO.UseSkill(Skill.Hiding);
            
            bool unfinishedAttempt;
            do 
            {
                unfinishedAttempt = false;
                // This waits until "Skryti se povedlo." or "Nepovedlo se ti schovat" arrives to journal.
                hiddingJournal
                    .When("You must wait a few moments to use another skill", () =>
                        // wait when you have to wait before using another script
                        UO.Wait(5000))
                    .When("Skryti se povedlo.", () =>
                        // when hidding is successful terminate the do while loop
                        // (you cannot use break statement directly in an annonynous method)
                        hidden = true)
                    .When("Nepovedlo se ti schovat", () =>
                    {
                        // when hidding fails, do while loop continues
                        hidden = false;
                        UO.ClientFilters.Stamina.Disable();
                    })
                    .When("You are preoccupied with thoughts of battle.", () =>
                    {
                        UO.WarModeOff();
                        hidden = false;
                    })
                    .WhenTimeout(() =>
                    {
                        UO.ClientFilters.Stamina.SetFakeStamina(1);
                        unfinishedAttempt = true;
                    })
                    // if server sends neither "Skryti se povedlo." nor "Nepovedlo se ti schovat"
                    // in one minute, then the script terminates with an TimoutException.
                    .WaitAny(TimeSpan.FromMilliseconds(2000));
            } while (unfinishedAttempt);
        } while (!hidden);
    }
}

UO.RegisterCommand("hide", Hidding.Hide);
UO.RegisterBackgroundCommand("hide-run", Hidding.RunHidding);
UO.RegisterBackgroundCommand("hide-watchalwayswalk", Hidding.RunWatchAlwaysWalk);
