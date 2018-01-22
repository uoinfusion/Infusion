#load "Specs.csx"
#load "afk.csx"
#load "items.csx"

static class Taming
{
    private static SpeechJournal journal = UO.CreateSpeechJournal();

    public static void Domesticate()
    {
        var animal = UO.AskForMobile();
        while (true)
        {
            UO.UseSkill(Skill.AnimalTaming);
            UO.WaitForTarget();
            UO.Target(animal);
            
            journal
                .When("Zda se, ze te zvire prijalo za sveho pana", () =>
                {
                    var onion = UO.Items
                        .Matching(Specs.Onion)
                        .InBackPack()
                        .FirstOrDefault();

                    if (onion != null)
                    {
                        Items.TryMoveItem(onion, 1, animal.Id);
                        UO.Wait(500);
                        UO.Say("all release");
                    }
                })
                .When("Nepovedlo se ti zvire ochocit", () => { UO.Wait(500); })
                .WaitAny();
                
            Afk.Check();
        }
    }
}

UO.RegisterCommand("taming", Taming.Domesticate);