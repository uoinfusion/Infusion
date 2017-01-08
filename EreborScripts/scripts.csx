using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using UltimaRX.Proxy.InjectionApi;
using static UltimaRX.Proxy.InjectionApi.Injection;
using UltimaRX.Proxy;
using UltimaRX.Packets;

void HarvestTrees(string mapFileInfoFile)
{
    Harvest(mapFileInfoFile , HarvestTree);
}

void HarvestOre(string mapFileInfoFile)
{
    Harvest(mapFileInfoFile , Mine);
}

void Harvest(string mapFileInfoFile, Action<string> harvestAction)
{
    var mapLines = File.ReadAllLines(mapFileInfoFile);
    foreach (var mapLine in mapLines)
    {
        ProcessHarvestMapLine(mapLine, harvestAction);
    }
}

void ProcessHarvestMapLine(string mapLine, Action<string> harvestAction)
{
    if (mapLine.StartsWith("harvest: "))
    {
        var parameters = mapLine.Substring("harvest: ".Length);
        Print($"Harvesting tree: {parameters}");
        harvestAction(parameters);
    }
    else if (mapLine.StartsWith("walk: "))
    {
        var parameters = mapLine.Substring("walk: ".Length).Split(',').Select(x => ushort.Parse(x.Trim())).ToArray();
        Print($"Walking to: {parameters[0]}, {parameters[1]}");
        WalkTo(parameters[0], parameters[1]);
    }
}

string[] skipOre = { "Copper Ore" };

void Mine(string tileInfo)
{
    bool canMine = true;

    while (canMine)
    {
        Wait(1000);
        DeleteJournal();
        UseType(ItemTypes.PickAxe);
        TargetTile(tileInfo);

        WaitForJournal("You put", "Nevykopal jsi nic zajimaveho", "Try mining in rock", "There is no ore here to mine");

        if (InJournal("Jeste nemuzes pouzit skill."))
        {
            Wait(5000);
            Print("waiting for skill");
        }

        canMine = !InJournal("Try mining in rock", "There is no ore here to mine");
        canMine &= !skipOre.Any(s => InJournal(s));
    }
}

void Fish(string tileInfo)
{
    bool canFish = true;

    while (canFish)
    {
        DeleteJournal();
        UseType(ItemTypes.FishingPole);
        TargetTile(tileInfo);

        WaitForJournal("You pull out a fish", "There are no fish here", "Dneska nejak neberou");

        if (InJournal("Jeste nemuzes pouzit skill."))
        {
            Wait(5000);
            Print("waiting for skill");
        }
        else
        {
            Wait(1000);
        }

        var fish = Items.FindTypeOnGround(ItemTypes.Fishes);
        while (fish != null)
        {
            Pickup(fish);
            fish = Items.FindTypeOnGround(ItemTypes.Fishes);
        }

        canFish = !InJournal("There are no fish here");
    }
}

void HarvestTree(string tileInfo)
{
    bool treeHarvestable = true;

    while (treeHarvestable)
    {
        Wait(1000);
        DeleteJournal();
        UseType(ItemTypes.Hatchets);
        TargetTile(tileInfo);

        WaitForJournal("You put", "Drevo se nepodarilo", "of a way to use", "immune", "There are no logs");

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

void Cook(ushort rawFoodType)
{
    var campfire = Items.FindType(ItemTypes.Campfire);
    var rawFood = Items.FindType(rawFoodType);

    while (campfire != null && rawFood != null)
    {
        Use(rawFood);
        Target(campfire);

        WaitForJournal("Jidlo neni pozivatelne", "Mmm, smells good");
        Wait(500);

        campfire = Items.FindType(ItemTypes.Campfire);
        rawFood = Items.FindType(rawFoodType);
    }
}

void Cook(ushort rawFoodType, string campfireTile)
{
    var rawFood = Items.FindType(rawFoodType);

    while (rawFood != null)
    {
        Use(rawFood);
        TargetTile(campfireTile);

        WaitForJournal("Jidlo neni pozivatelne", "Mmm, smells good");
        Wait(500);

        rawFood = Items.FindType(rawFoodType);
    }
}