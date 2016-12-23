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

        if (InJournal("Jeste nemuzes pouzit skill."))
            Wait(1000);

        treeHarvestable = !InJournal("of a way to use", "immune", "There are no logs here to chop.");
    }
}

void StepToward(Location3D currentLocation, Location3D targetLocation)
{
    var walkVector = (targetLocation - currentLocation).Normalize();
    if (walkVector != Vector.NullVector)
    {
        Walk(walkVector.ToDirection());
    }
}

void StepToward(Location3D targetLocation)
{
    StepToward(Me.Location, targetLocation);
}

void WalkTo(Location3D targetLocation)
{
    while (Me.Location != targetLocation)
    {
        StepToward(targetLocation);
        Wait(1000);
    }
}

void WalkTo(ushort xloc, ushort yloc)
{
    WalkTo(new Location3D(xloc, yloc, 0));
}
