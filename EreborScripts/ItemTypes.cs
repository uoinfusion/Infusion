using UltimaRX.Packets;

public static class ItemTypes
{
    public static readonly ModelId Hatchet = (ModelId) 0x0F44;
    public static readonly ModelId Hatchet2 = (ModelId) 0x0F43;
    public static readonly ModelId CopperExecutionersAxe = (ModelId) 0x0F45;
    public static readonly ModelId CopperVikingSword = (ModelId) 0x13B9;

    public static readonly ModelId PickAxe = (ModelId) 0xE86;

    public static readonly ModelId FishingPole = (ModelId) 0x0DBF;

    public static readonly ModelId[] Hatchets = {Hatchet, Hatchet2, CopperExecutionersAxe};
    public static readonly ModelId[] Knives = {CopperVikingSword};

    public static readonly ModelId Fish1 = (ModelId) 0x09CF;
    public static readonly ModelId Fish2 = (ModelId) 0x09CD;
    public static readonly ModelId Fish3 = (ModelId) 0x09CC;
    public static readonly ModelId Fish4 = (ModelId) 0x09CE;

    public static readonly ModelId[] Fishes = {Fish1, Fish2, Fish3, Fish4};

    public static readonly ModelId RawFishSteak = (ModelId) 0x097A;

    public static readonly ModelId Feathers = (ModelId) 0x1BD1;
    public static readonly ModelId Bird = (ModelId) 0x0006;
    public static readonly ModelId RippadbleBody = (ModelId) 0x2006;
    public static readonly ModelId RawBird = (ModelId) 0x09B9;
    public static readonly ModelId RawRibs = (ModelId) 0x09F1;

    public static readonly ModelId Rabbit = (ModelId) 0x00CD;

    public static readonly ModelId Sheep = (ModelId) 0x00CF;

    public static readonly ModelId Cow = (ModelId) 0x00E7;

    public static readonly ModelId Rat = (ModelId) 0x00EE;

    public static readonly ModelId[] MassKillSubjects = {Bird, Rabbit, Sheep, Rat, Cow};
    public static readonly ModelId[] RippableBodies = {RippadbleBody};

    public static readonly ModelId[] RawFood = {RawBird, RawFishSteak, RawRibs};

    public static readonly ModelId Campfire = (ModelId) 0x0DE3;
    public static readonly ModelId Forge = (ModelId) 0x0FB1;
    public static readonly ModelId[] CookingPlaces = {Campfire, Forge};

    public static readonly ModelId BackPack = (ModelId) 0xE75;
}

