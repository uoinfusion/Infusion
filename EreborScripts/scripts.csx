void HarvestTree(string tileInfo)
{
    bool treeHarvestable = true;

    while (treeHarvestable)
    { 
        DeleteJournal();
        UseType(ItemTypes.Hatchets);
        TargetTile(tileInfo);

        WaitForJournal("You put", "Drevo se nepodarilo", "of a way to use", "immune", "There are no logs");
        Wait(250);

        treeHarvestable = !InJournal("of a way to use", "immune", "There are no logs here to chop.");
    }
}