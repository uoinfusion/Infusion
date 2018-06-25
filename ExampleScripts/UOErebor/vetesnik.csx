public static class Vetesnik
{
    public static void IdentifyCommand()
    {
        UO.ClientPrint("Vyber batoh s predmety, ktere chces");
        var sourceContainer = UO.AskForItem();
        if (sourceContainer == null)
        {
            UO.ClientPrint("Targeting canelled");
            return;
        }
        
        var itemsToIdentify = UO.Items.InContainer(sourceContainer);
        foreach (var item in itemsToIdentify)
        {
            UO.WaitTargetObject(item);
            UO.Say("vetesnik identifikuj");
            
            UO.Journal.WaitAny("Na takovou vzdalenost ti to neidentifikuje.", "Moc pekna vecicka",
                "Zameruj neidentifikovane magicke predmety");
            
            UO.Wait(250);
        }
    }
}

UO.RegisterCommand("vetesnik-identifikuj", Vetesnik.IdentifyCommand);