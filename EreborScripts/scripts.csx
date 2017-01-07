using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

void Harvest(string mapFileInfoFile)
{
    var mapLines = File.ReadAllLines(mapFileInfoFile);
    foreach (var mapLine in mapLines)
    {
        ProcessHarvestMapLine(mapLine);
    }
}

void ProcessHarvestMapLine(string mapLine)
{
    if (mapLine.StartsWith("tree: "))
    {
        var parameters = mapLine.Substring("tree: ".Length);
        Print($"Harvesting: {parameters}");
        HarvestTree(parameters);
    }
    else if (mapLine.StartsWith("walk: "))
    {
        var parameters = mapLine.Substring("walk: ".Length).Split(',').Select(x => ushort.Parse(x.Trim())).ToArray();
        Print($"Walking to: {parameters[0]}, {parameters[1]}");
        WalkTo(parameters[0], parameters[1]);
    }
}

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
        { 
            Wait(5000);
            Print("waiting for skill");
        }

        treeHarvestable = !InJournal("of a way to use", "immune", "There are no logs here to chop.");
    }
}

void StepToward(Location2D currentLocation, Location2D targetLocation)
{
    var walkVector = (targetLocation - currentLocation).Normalize();
    if (walkVector != Vector.NullVector)
    {
        Walk(walkVector.ToDirection());
    }
}

void StepToward(Location2D targetLocation)
{
    StepToward((Location2D)Me.Location, targetLocation);
}

void WalkTo(Location2D targetLocation)
{
    while ((Location2D)Me.Location != targetLocation)
    {
        StepToward(targetLocation);
        Wait(1000);
    }
}

void WalkTo(ushort xloc, ushort yloc)
{
    WalkTo(new Location2D(xloc, yloc));
}
