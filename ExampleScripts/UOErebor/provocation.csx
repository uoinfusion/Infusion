using System.Linq;

public static class Provocation
{
    private static SpeechJournal journal = UO.CreateSpeechJournal();
    
    public static ScriptTrace Trace = UO.Trace.Create();

    public static void ProvocateAll()
    {
        var creatures = UO.Mobiles
            .Where(x => x.Notoriety == Notoriety.Murderer)
            .ToArray();
            
        UO.ClientPrint($"{creatures.Length} creatures found");
        
        for (int i = 1; i < creatures.Length; i++)
        {
            bool done = false;
            while (!done)
            {
                var target1 = UO.Mobiles[creatures[i - 1].Id];
                var target2 = UO.Mobiles[creatures[i].Id];
                
                if (target1 == null)
                {
                    UO.ClientPrint("cannot see target1");
                    break;
                }
                
                if (target2 == null)
                {
                    UO.ClientPrint("cannot see target2");
                    break;
                }

                UO.ClientPrint("attacker", "provocation", target1);
                UO.ClientPrint("target", "provocation", target2);
                UO.ClientPrint($"Provoking {target1.Name} against {target2.Name}, remaining creatures {creatures.Length - i}");
                Trace.Log("starting provo attempt");
            
                UO.WarModeOff();
                UO.WaitTargetObject(target1, target2);
                UO.Say(".provo");
                Trace.Log(".provo");
                
                journal
                    .When(new[] { "poorly", "nepovedla" }, () =>
                    {
                        UO.Wait(2800);
                        done = false;
                    })
                    .When("Provokace se povedla.", () => 
                    {
                        UO.Wait(2800);
                        done = true;
                    })
                    .When("Nespechej s pouzivanim skillu", () =>
                    {
                        Trace.Log("Waiting for skill");  
                        UO.Wait(5000);
                        done = false;
                    })
                    .When("You can't see that", "Provokace teto prisery presahuje tve moznosti", () =>
                    {
                        UO.Wait(2800);
                        done = true;
                    })
                    .WaitAny();
                    
                Trace.Log("provo attempt finished");
            }
        }
    }
}

UO.RegisterCommand("provo-all", Provocation.ProvocateAll);