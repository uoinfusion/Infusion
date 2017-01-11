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
        WaitForTarget();
        TargetTile(tileInfo);

        WaitForJournal("You put", "Nevykopal jsi nic zajimaveho", "Try mining in rock", "There is no ore here to mine");

        canMine = !InJournal("Try mining in rock", "There is no ore here to mine");
        canMine &= !skipOre.Any(s => InJournal(s));

        if (canMine && InJournal("Jeste nemuzes pouzit skill."))
        {
            Print("waiting for skill");
            Wait(5000);
        }
    }
}

void Fish(string tileInfo)
{
    bool canFish = true;

    while (canFish)
    {
        DeleteJournal();
        UseType(ItemTypes.FishingPole);
        WaitForTarget();
        TargetTile(tileInfo);

        WaitForJournal("You pull out a fish", "There are no fish here", "Dneska nejak neberou");

        Wait(1000);

        var fish = Items.FindTypeOnGround(ItemTypes.Fishes);
        while (fish != null)
        {
            Pickup(fish);
            fish = Items.FindTypeOnGround(ItemTypes.Fishes);
        }

        canFish = !InJournal("There are no fish here");
        if (canFish && InJournal("Jeste nemuzes pouzit skill."))
        {
            Print("waiting for skill");
            Wait(5000);
        }
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
        WaitForTarget();
        TargetTile(tileInfo);

        WaitForJournal("You put", "Drevo se nepodarilo", "of a way to use", "immune", "There are no logs");

        treeHarvestable = !InJournal("of a way to use", "immune", "There are no logs here to chop.");

        if (treeHarvestable && InJournal("Jeste nemuzes pouzit skill."))
        {
            Print("waiting for skill");
            Wait(5000);
        }
    }
}

void StepToward(Location2D currentLocation, Location2D targetLocation)
{
    var walkVector = (targetLocation - currentLocation).Normalize();
    if (walkVector != Vector.NullVector)
    {
        WaitWalk();
        Walk(walkVector.ToDirection());
    }
}

void StepToward(Item item)
{
    StepToward((Location2D)item.Location);
}

void StepToward(Location2D targetLocation)
{
    StepToward((Location2D)Me.PredictedLocation, targetLocation);
}

void WalkTo(Location2D targetLocation)
{
    while ((Location2D)Me.PredictedLocation != targetLocation)
    {
        StepToward(targetLocation);
    }
}

void WalkTo(ushort xloc, ushort yloc)
{
    WalkTo(new Location2D(xloc, yloc));
}

void Cook(ModelId rawFoodType)
{
    var campfire = Items.FindType(ItemTypes.Campfire);
    var rawFood = Items.FindType(rawFoodType);

    while (campfire != null && rawFood != null)
    {
        Use(rawFood);
        WaitForTarget();
        Target(campfire);

        WaitForJournal("Jidlo neni pozivatelne", "Mmm, smells good");
        Wait(500);

        campfire = Items.FindType(ItemTypes.Campfire);
        rawFood = Items.FindType(rawFoodType);
    }
}

void Cook(ModelId rawFoodType, string campfireTile)
{
    var rawFood = Items.FindType(rawFoodType);

    while (rawFood != null)
    {
        Use(rawFood);
        WaitForTarget();
        TargetTile(campfireTile);

        WaitForJournal("Jidlo neni pozivatelne", "Mmm, smells good");
        Wait(500);

        rawFood = Items.FindType(rawFoodType);
    }
}

void Cook()
{
    var campfireTile = Info();

    var rawFood = Items.FindType(ItemTypes.RawFood);
    while (rawFood != null)
    {
        Cook(rawFood.Type, campfireTile);
        rawFood = Items.FindType(ItemTypes.RawFood);
    }
}

void Loot()
{
    var container = ItemInfo();
    if (container != null)
    {
        Loot(container);
    }
    else 
        Print("no container for loot");
}

void Loot(Item container)
{
    var item = Items.InContainer(container);
    while (item != null)
    {
        Print("looting item");
        Pickup(item);
        item = Items.InContainer(container);
    }

    Print("nothing to loot");
}

void Kill(Item subjectToKill)
{
    WarModeOn();
    Attack(subjectToKill);

    while (subjectToKill != null)
    {
        if (subjectToKill.GetDistance(Me.Location) > 1)
            StepToward(subjectToKill);
        subjectToKill = Items.RefreshItem(subjectToKill);
    }

    Item bodyOfSubject;
    int retryCount = 0;

    do
    {
        Print("waiting for body");
        Wait(1000);
        bodyOfSubject = Items.FindTypeAll(ItemTypes.RippableBodies).OrderBy(i => i.GetDistance(Me.Location)).First();
        retryCount++;
    } while (bodyOfSubject == null && retryCount < 20);

    if (bodyOfSubject != null)
    {
        DeleteJournal();
        UseType(ItemTypes.Knives);
        WaitForTarget();
        Target(bodyOfSubject);
        WaitForJournal("Rozrezal jsi mrtvolu.");
        Wait(1000);
        Loot(bodyOfSubject);
    }
    else
        Print("no body found");

    WarModeOff();
}

void Kill()
{
    var subjectToKill = ItemInfo();
    if (subjectToKill != null)
        Kill(subjectToKill);
}

void MassKill()
{
    do
    {
        var subject = Items.FindTypeAll(ItemTypes.MassKillSubjects).OrderBy(i => i.GetDistance(Me.Location)).FirstOrDefault();
        if (subject == null)
        {
            Print("nothing to kill");
            break;
        }

        Kill(subject);
    } while (true);
}