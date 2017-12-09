using System;
using System.Linq;

public static class Hiding
{
    // Create a "private" instance of journal for cooking. If you delete this journal it
    // doesn't affect either UO.Journal or other instances of SpeechJournal so
    // you can try hiding and healing in parallel (use bandage, target self, start hiding)
    // and you can still use journal.Delete method in both scripts at the same time.
    // It means, that you don't need tricks like UO.SetJournalLine(number,text) in
    // Injection.
    private static SpeechJournal hidingJournal = UO.CreateSpeechJournal();
    
    public static bool AlwaysWalkEnabled { get; set; } = true;
    public static TimeSpan AlwaysWalkDelayTime { get; set; } = TimeSpan.FromMilliseconds(1800);
    public static ScriptTrace Trace = UO.Trace.Create();

    public static void Hide()
    {
        UO.ClientFilters.Stamina.Disable();

        if (UO.CommandHandler.IsCommandRunning("hide-run"))
        {
            UO.ClientPrint("Hiding stopped", "hiding", UO.Me);
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
            alwaysWalkJournal.WaitAny(TimeSpan.MaxValue,
                "Byla jsi objevena",
                "Byl jsi objeven",
                "You have been revealed!",
                "You're now visible.");
            UO.ClientFilters.Stamina.Disable();
        }
    }

    public static void RunHiding()
    {
        UO.WarModeOff();
    
        var originalLocation = UO.Me.Location;
    
        bool hidden = false;
        do
        {
            UO.ClientPrint("Trying to hide", "hiding", UO.Me);
            
            // Don't worry, it will not affect any other scripts.
            hidingJournal.Delete();
            UO.UseSkill(Skill.Hiding);
            
            bool unfinishedAttempt;
            do 
            {
                unfinishedAttempt = false;
                // This waits until "Skryti se povedlo." or "Nepovedlo se ti schovat" arrives to journal.
                hidingJournal
                    .When("You must wait a few moments to use another skill", () =>
                        // wait when you have to wait before using another script
                        UO.Wait(5000))
                    .When("Skryti se povedlo.", () =>
                        // when hiding is successful terminate the do while loop
                        // (you cannot use break statement directly in an annonynous method)
                        hidden = true)
                    .When("Nepovedlo se ti schovat", () =>
                    {
                        // when hiding fails, do while loop continues
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
                        Trace.Log($"hide timeout for always walk after {AlwaysWalkDelayTime}");
                        if (AlwaysWalkEnabled)
                        {
                            Trace.Log("enabling fake stamina");
                            UO.ClientFilters.Stamina.SetFakeStamina(1);
                        }
                        
                        unfinishedAttempt = true;
                    })
                    // if server sends neither "Skryti se povedlo." nor "Nepovedlo se ti schovat"
                    // in one minute, then the script terminates with an TimoutException.
                    .WaitAny(AlwaysWalkDelayTime);
            } while (unfinishedAttempt);
        } while (!hidden);
    }
}

UO.RegisterCommand("hide", Hiding.Hide);
UO.RegisterBackgroundCommand("hide-run", Hiding.RunHiding);
UO.RegisterBackgroundCommand("hide-watchalwayswalk", Hiding.RunWatchAlwaysWalk);
