using System;
using System.Linq;
using Infusion.Packets;
using System.Collections.Generic;
using System.Reflection;

public static class Specs
{
    public static readonly MobileSpec Player = new[] { 0x0190, 0x0191, 0x0192 };
    public static readonly ItemSpec RippadbleBody = 0x2006;
    public static readonly ItemSpec Corpse = 0x2006;
    public static readonly ItemSpec HorseShoes = 0x0FB6;

    public static readonly ItemSpec TravelStone = 0x1174;
    public static readonly ItemSpec HouseMenu = 0x0BD1;
    public static readonly ItemSpec Gold = 0x0eed;

    public static readonly ItemSpec Hatchet = new[] { 0x0F44, 0x0F43 };
    public static readonly ItemSpec CopperExecutionersAxe = new[] { 0x0F45, 0x0F46 };

    public static readonly ItemSpec CopperVikingSword = 0x13B9;
    public static readonly ItemSpec VeriteVikingSword = 0x13BA;
    public static readonly ItemSpec Dagger = 0x0F51;
    public static readonly ItemSpec WarMace = 0x1406;
    public static readonly ItemSpec Club = 0x13B3;

    public static readonly ItemSpec Crossbow = 0x0F4F;

    // Amunition
    public static readonly ItemSpec Arrow = 0x0F3F;
    public static readonly ItemSpec Bolt = 0x1BFB;
    public static readonly ItemSpec JaggedBolt = 0x38D8;

    public static readonly ItemSpec TightBoots = 0x1711;

    // Containers
    public static readonly ItemSpec BackPack = 0xE75;

    // Tools
    public static readonly ItemSpec Knives = new[] { CopperVikingSword, VeriteVikingSword, Dagger };
    public static readonly ItemSpec PickAxe = 0x0E86;
    public static readonly ItemSpec FishingPole = 0x0DBF;
    public static readonly ItemSpec Hatchets = new[] { Hatchet, CopperExecutionersAxe };
    public static readonly ItemSpec Campfire = 0x0DE3;
    public static readonly ItemSpec Forge = new[] { 0x0FB1, 0x198E };
    public static readonly ItemSpec CookingPlaces = new[] { Campfire, Forge, };
    public static readonly ItemSpec Loom = 0x1062;
    public static readonly ItemSpec TinkeringTools = 0x1EBC;
    public static readonly ItemSpec SmithsHammer = 0x13E3;
    public static readonly ItemSpec Saw = 0x1034;
    public static readonly ItemSpec Torch = 0x0F64;
    public static readonly ItemSpec BurningTorch = 0x0A12;

    // Armor
    public static readonly ItemSpec BoneHelmet = 0x1451;

    // Resources
    public static readonly ItemSpec Shaft = 0x1BD4;
    public static readonly ItemSpec Wire = 0x1879;
    public static readonly ItemSpec Feathers = 0x1BD1;
    public static readonly ItemSpec PileOfHides = 0x1078;
    public static readonly ItemSpec Furs = new[] { 0x11F6, 0x11F4 };
    public static readonly ItemSpec CutUpLeather = 0x1067;
    public static readonly ItemSpec BallsOfYarn = 0x0E1D;
    public static readonly ItemSpec PilesOfWool = 0x0DF8;
    public static readonly ItemSpec Cotton = 0x0C4F;
    public static readonly ItemSpec BaleOfCotton = 0x0DF9;
    public static readonly ItemSpec Wheat = 0x0C5A;
    public static readonly ItemSpec Bandage = 0x0E21;
    public static readonly ItemSpec BloodyBandage = 0x0E20;

    public static readonly ItemSpec FoldedCloth = 0x175D;

    public static readonly ItemSpec BlankScroll = 0x0E34;
    public static readonly ItemSpec Paper = 0x14ED;
    public static readonly ItemSpec BlankMap = 0x14EB;

    public static readonly ItemSpec PlayingCards = 0x0FA2;

    // Metals
    public static readonly ItemSpec Ore = 0x19B7;
    public static readonly ItemSpec Ingot = new[] { 0x1BE6, 0x1BF2 };
    public static readonly ItemSpec CopperOre = new ItemSpec(0x19B7, (Color)0x099A);
    public static readonly ItemSpec CopperIngot = new ItemSpec(0x1BE6, (Color)0x0000);
    public static readonly ItemSpec IronOre = new ItemSpec(0x19B7, (Color)0x0763);
    public static readonly ItemSpec IronIngot = new ItemSpec(0x1BF2, (Color)0x0000);
    public static readonly ItemSpec KremicityPisek = new ItemSpec(0x19B7, (Color)0x0481);
    public static readonly ItemSpec Sklovina = new ItemSpec(0x0EED, (Color)0x0481);
    public static readonly ItemSpec VeriteOre = new ItemSpec(0x19B7, (Color)0x097F);
    public static readonly ItemSpec VeriteIngot = new ItemSpec(0x1BF2, (Color)0x097F);
    public static readonly ItemSpec ValoriteOre = new ItemSpec(0x19B7, (Color)0x0985);
    public static readonly ItemSpec ValoriteIngot = new ItemSpec(0x1BF2, (Color)0x0985);
    public static readonly ItemSpec ObsidianOre = new ItemSpec(0x19B7, (Color)0x09BD);
    public static readonly ItemSpec ObsidianIngot = new ItemSpec(0x1BF2, (Color)0x0989);

    // Logs
    public static readonly ItemSpec Log = 0x1BDD;
    public static readonly ItemSpec SpruceLog = new ItemSpec(0x1BDD, (Color)0x0000);
    public static readonly ItemSpec ChestnutLog = new ItemSpec(0x1BDD, (Color)0x05E5);
    public static readonly ItemSpec OakLog = new ItemSpec(0x1BDD, (Color)0x05F2);
    public static readonly ItemSpec TeakLog = new ItemSpec(0x1BDD, (Color)0x01C4);
    public static readonly ItemSpec MahagonLog = new ItemSpec(0x1BDD, (Color)0x00ED);
    public static readonly ItemSpec EbenLog = new ItemSpec(0x1BDD, (Color)0x0774);

    // Potions
    public static readonly ItemSpec PotionKeg = 0x1AD6;
    public static readonly ItemSpec Bottle = 0x0F0E;
    public static readonly ItemSpec EmptyBottle = new ItemSpec(0x0F0E, (Color)0x0000);

    public static readonly ItemSpec NightsightKeg = new ItemSpec(0x1AD6, (Color)0x0980);
    public static readonly ItemSpec GreaterHealKeg = new ItemSpec(0x1AD6, (Color)0x0160);
    
    public static readonly ItemSpec NightsighPoition = new ItemSpec(0x0F0E, (Color)0x0980);
    public static readonly ItemSpec DispellExplosionPotion = new ItemSpec(0x0F0E, (Color)0x0993);
    public static readonly ItemSpec ExplosionLesserPotion = new ItemSpec(0x0F0E, (Color)0x00E2);
    public static readonly ItemSpec ExplosionPotion = new ItemSpec(0x0F0E, (Color)0x001E);
    public static readonly ItemSpec ExplosionGreaterPotion = new ItemSpec(0x0F0E, (Color)0x0017);
    public static readonly ItemSpec LavaPotion = new ItemSpec(0x0F0E, (Color)0x00DE);
    public static readonly ItemSpec AgilityLesserPotion = new ItemSpec(0x0F0E, (Color)0x0006);
    public static readonly ItemSpec AgilityPotion = new ItemSpec(0x0F0E, (Color)0x0005);
    public static readonly ItemSpec AgilityGreaterPotion = new ItemSpec(0x0F0E, (Color)0x00CF);
    public static readonly ItemSpec CureLesserPotion = new ItemSpec(0x0F0E, (Color)0x002D);
    public static readonly ItemSpec CurePotion = new ItemSpec(0x0F0E, (Color)0x002B);
    public static readonly ItemSpec CureGreaterPotion = new ItemSpec(0x0F0E, (Color)0x008E);
    public static readonly ItemSpec HealLesserPotion = new ItemSpec(0x0F0E, (Color)0x0100);
    public static readonly ItemSpec HealPotion = new ItemSpec(0x0F0E, (Color)0x0099);
    public static readonly ItemSpec HealGreaterPotion = new ItemSpec(0x0F0E, (Color)0x0160);
    public static readonly ItemSpec HealPotions = new[] { HealLesserPotion, HealPotion, HealGreaterPotion };
    public static readonly ItemSpec RefreshLesserPotion = new ItemSpec(0x0F0E, (Color)0x0029);
    public static readonly ItemSpec RefreshPotion = new ItemSpec(0x0F0E, (Color)0x0027);
    public static readonly ItemSpec RefreshGreaterPotion = new ItemSpec(0x0F0E, (Color)0x00ED);
    public static readonly ItemSpec StrengthLesserPotion = new ItemSpec(0x0F0E, (Color)0x0835);
    public static readonly ItemSpec StrengthPotion = new ItemSpec(0x0F0E, (Color)0x0388);
    public static readonly ItemSpec StrengthGreaterPotion = new ItemSpec(0x0F0E, (Color)0x076B);
    public static readonly ItemSpec InvisibilityPotion = new ItemSpec(0x0F0E, (Color)0x0447);
    public static readonly ItemSpec ClevernessLesserPotion = new ItemSpec(0x0F0E, (Color)0x06C2);
    public static readonly ItemSpec ClevernessPotion = new ItemSpec(0x0F0E, (Color)0x073E);
    public static readonly ItemSpec ClevernessGreaterPotion = new ItemSpec(0x0F0E, (Color)0x047D);
    public static readonly ItemSpec StoneskinPotion = new ItemSpec(0x0F0E, (Color)0x0999);
    public static readonly ItemSpec MobilityPotion = new ItemSpec(0x0F0E, (Color)0x000F);
    public static readonly ItemSpec EnergyLesserPotion = new ItemSpec(0x0F0E, (Color)0x0060);
    public static readonly ItemSpec EnergyPotion = new ItemSpec(0x0F0E, (Color)0x005D);
    public static readonly ItemSpec EnergyGreaterPotion = new ItemSpec(0x0F0E, (Color)0x00C5);
    public static readonly ItemSpec ReflectionPotion = new ItemSpec(0x0F0E, (Color)0x0985);
    public static readonly ItemSpec ReactivePotion = new ItemSpec(0x0F0E, (Color)0x0A82);
    public static readonly ItemSpec ShieldPotion = new ItemSpec(0x0F0E, (Color)0x0059);
    public static readonly ItemSpec ParalyzePoison = new ItemSpec(0x0F0E, (Color)0x0314);
    public static readonly ItemSpec FizzlePoison = new ItemSpec(0x0F0E, (Color)0x022d);
    public static readonly ItemSpec StaminaPoison = new ItemSpec(0x0F0E, (Color)0x002f);
    public static readonly ItemSpec WeakenPoison = new ItemSpec(0x0F0E, (Color)0x02e6);
    public static readonly ItemSpec ClumsyPoison = new ItemSpec(0x0F0E, (Color)0x02ed);
    public static readonly ItemSpec FeeblemindPoison = new ItemSpec(0x0F0E, (Color)0x02c9);
    public static readonly ItemSpec PoisonLesser = new ItemSpec(0x0F0E, (Color)0x00b0);
    public static readonly ItemSpec Poison = new ItemSpec(0x0F0E, (Color)0x00b2);
    public static readonly ItemSpec PoisonGreater = new ItemSpec(0x0F0E, (Color)0x0179);
    public static readonly ItemSpec PoisonDeadly = new ItemSpec(0x0F0E, (Color)0x0593);
    public static readonly ItemSpec Shrink = new ItemSpec(0x0F0E, (Color)0x0995);
    public static readonly ItemSpec ElixirPoznani = new ItemSpec(0x0F0E, (Color)0x0088);
    public static readonly ItemSpec AgilityElixir = new ItemSpec(0x182A, (Color)0x00CF);
    public static readonly ItemSpec HealElixir = new ItemSpec(0x182A, (Color)0x0160);
    public static readonly ItemSpec RefreshElixir = new ItemSpec(0x182A, (Color)0x00ed);
    public static readonly ItemSpec StrengthElixir = new ItemSpec(0x182A, (Color)0x076b);
    public static readonly ItemSpec ClevernessElixir = new ItemSpec(0x182A, (Color)0x047d);
    public static readonly ItemSpec StoneskinElixir = new ItemSpec(0x182A, (Color)0x0999);
    public static readonly ItemSpec EnergyElixir = new ItemSpec(0x182A, (Color)0x00c5);
    public static readonly ItemSpec SpellShieldElixir = new ItemSpec(0x182A, (Color)0x0059);

    // Reagents
    public static readonly ItemSpec GraveDust = 0x0f8f;
    public static readonly ItemSpec Pumice = 0x0f8b;
    public static readonly ItemSpec Ruby = 0x0f13;
    public static readonly ItemSpec Diamond = 0x0f26;
    public static readonly ItemSpec BatWings = 0x0f78;
    public static readonly ItemSpec SulfurousAsh = 0x0f8c;
    public static readonly ItemSpec BloodMoss = 0x0f7b;
    public static readonly ItemSpec BlackPearl = 0x0f7a;
    public static readonly ItemSpec Garlic = 0x0f84;
    public static readonly ItemSpec Mandrake = 0x0f86;
    public static readonly ItemSpec Nightshade = 0x0f88;
    public static readonly ItemSpec Ginseng = 0x0f85;
    public static readonly ItemSpec Obsidian = 0x0f89;
    public static readonly ItemSpec FertileDirt = 0x0f81;
    public static readonly ItemSpec WyrmsHeart = 0x0f91;
    public static readonly ItemSpec EyeOfNewt = 0x0f87;
    public static readonly ItemSpec SpidersSilk = 0x0f8d;
    public static readonly ItemSpec DragonsBlood = 0x0F82;
    public static readonly ItemSpec DaemonBlood = 0x0F7D;
    public static readonly ItemSpec DaemonBones = 0x0F80;
    public static readonly ItemSpec Bone = 0x0F7E;
    public static readonly ItemSpec NoxCrystal = 0x0F8E;
    public static readonly ItemSpec ExecutionersCap = 0x0F83;
    public static readonly ItemSpec PieceOfAmber = 0x0F25;
    public static readonly ItemSpec PigIron = 0x0F8A;
    public static readonly ItemSpec Tourmaline = 0x0F18;
    public static readonly ItemSpec Citrine = 0x0F15;
    public static readonly ItemSpec Amethyst = 0x0F16;
    public static readonly ItemSpec Emerald = 0x0F10;
    public static readonly ItemSpec Sapphire = new[] { 0x0F11, 0x0f19 };
    public static readonly ItemSpec StarSapphire = 0x0F0F;

    public static readonly ItemSpec Gem = new []
    { 
        Sapphire, Ruby, Diamond, Tourmaline, Citrine, Amethyst,
        Emerald, Sapphire, StarSapphire
     };

    public static readonly ItemSpec Regs = new[]
    {
        BatWings, SulfurousAsh, BloodMoss, BlackPearl, Garlic, Mandrake,
        Nightshade, Ginseng, Obsidian, FertileDirt, WyrmsHeart, EyeOfNewt, SpidersSilk, DragonsBlood,
        Bone, NoxCrystal, ExecutionersCap, DaemonBlood, GraveDust, Pumice, PieceOfAmber, DaemonBones, PigIron
    };

    // Spell Scrolls
    public static readonly ItemSpec ScrollReactiveArmor = 0x1f2d;
    public static readonly ItemSpec ScrollClumsy = 0x1f2e;
    public static readonly ItemSpec ScrollCreateFood = 0x1f2f;
    public static readonly ItemSpec ScrollFeeblemind = 0x1f30;
    public static readonly ItemSpec ScrollHeal = 0x1f31;
    public static readonly ItemSpec ScrollMagicArrow = 0x1f32;
    public static readonly ItemSpec ScrollNightSight = 0x1f33;
    public static readonly ItemSpec ScrollWeaken = 0x1f34;
    public static readonly ItemSpec ScrollAgility = 0x1f35;
    public static readonly ItemSpec ScrollCunning = 0x1f36;
    public static readonly ItemSpec ScrollCure = 0x1f37;
    public static readonly ItemSpec ScrollHarm = 0x1f38;
    public static readonly ItemSpec ScrollMagicTrap = 0x1f39;
    public static readonly ItemSpec ScrollMagicUntrap = 0x1f3a;
    public static readonly ItemSpec ScrollProtection = 0x1f3b;
    public static readonly ItemSpec ScrollStrength = 0x1f3c;
    public static readonly ItemSpec ScrollBless = 0x1f3d;
    public static readonly ItemSpec ScrollFireball = 0x1f3e;
    public static readonly ItemSpec ScrollMagicLock = 0x1f3f;
    public static readonly ItemSpec ScrollPoison = 0x1f40;
    public static readonly ItemSpec ScrollTelekinesis = 0x1f41;
    public static readonly ItemSpec ScrollTeleport = 0x1f42;
    public static readonly ItemSpec ScrollUnlock = 0x1f43;
    public static readonly ItemSpec ScrollWallOfStone = 0x1f44;
    public static readonly ItemSpec ScrollArchcure = 0x1f45;
    public static readonly ItemSpec ScrollArchprotection = 0x1f46;
    public static readonly ItemSpec ScrollCurse = 0x1f47;
    public static readonly ItemSpec ScrollFireField = 0x1f48;
    public static readonly ItemSpec ScrollGreaterHeal = 0x1f49;
    public static readonly ItemSpec ScrollLightning = 0x1f4a;
    public static readonly ItemSpec ScrollManaDrain = 0x1f4b;
    public static readonly ItemSpec ScrollRecall = 0x1f4c;
    public static readonly ItemSpec ScrollBladeSpirits = 0x1f4d;
    public static readonly ItemSpec ScrollDispelField = 0x1f4e;
    public static readonly ItemSpec ScrollIncognito = 0x1f4f;
    public static readonly ItemSpec ScrollMagicReflection = 0x1f50;
    public static readonly ItemSpec ScrollMindBlast = 0x1f51;
    public static readonly ItemSpec ScrollParalyze = 0x1f52;
    public static readonly ItemSpec ScrollPoisonField = 0x1f53;
    public static readonly ItemSpec ScrollSummonCreature = 0x1f54;
    public static readonly ItemSpec ScrollDispel = 0x1f55;
    public static readonly ItemSpec ScrollEnergyBolt = 0x1f56;
    public static readonly ItemSpec ScrollExplosion = 0x1f57;
    public static readonly ItemSpec ScrollInvisibility = 0x1f58;
    public static readonly ItemSpec ScrollMark = 0x1f59;
    public static readonly ItemSpec ScrollMassCurse = 0x1f5a;
    public static readonly ItemSpec ScrollParalyzeField = 0x1f5b;
    public static readonly ItemSpec ScrollReveal = 0x1f5c;
    public static readonly ItemSpec ScrollChainLightning = 0x1f5d;
    public static readonly ItemSpec ScrollEnergyField = 0x1f5e;
    public static readonly ItemSpec ScrollFlamestrike = 0x1f5f;
    public static readonly ItemSpec ScrollGateTravel = 0x1f60;
    public static readonly ItemSpec ScrollManaVampire = 0x1f61;
    public static readonly ItemSpec ScrollMassDispel = 0x1f62;
    public static readonly ItemSpec ScrollMeteorSwarm = 0x1f63;
    public static readonly ItemSpec ScrollPolymorph = 0x1f64;
    public static readonly ItemSpec ScrollEarthquake = 0x1f65;
    public static readonly ItemSpec ScrollEnergyVortex = 0x1f66;
    public static readonly ItemSpec ScrollResurrection = 0x1f67;
    public static readonly ItemSpec ScrollSummonElemAir = 0x1f68;
    public static readonly ItemSpec ScrollSummonDaemon = 0x1f69;
    public static readonly ItemSpec ScrollSummonElemEarth = 0x1f6a;
    public static readonly ItemSpec ScrollSummonElemFire = 0x1f6b;
    public static readonly ItemSpec ScrollSummonElemWater = 0x1f6c;
    public static readonly ItemSpec ScrollXGreen2 = 0x1f6d;
    public static readonly ItemSpec ScrollXTeal2 = 0x1f6f;
    public static readonly ItemSpec ScrollXBrown2 = 0x1f71;

    public static readonly ItemSpec Scrolls = new[] { ScrollReactiveArmor, ScrollClumsy, ScrollCreateFood, ScrollFeeblemind, ScrollHeal, ScrollMagicArrow, ScrollNightSight, ScrollWeaken, ScrollAgility, ScrollCunning, ScrollCure, ScrollHarm, ScrollMagicTrap, ScrollMagicUntrap, ScrollProtection, ScrollStrength, ScrollBless, ScrollFireball, ScrollMagicLock, ScrollPoison, ScrollTelekinesis, ScrollTeleport, ScrollUnlock, ScrollWallOfStone, ScrollArchcure, ScrollArchprotection, ScrollCurse, ScrollFireField, ScrollGreaterHeal, ScrollLightning, ScrollManaDrain, ScrollRecall, ScrollBladeSpirits, ScrollDispelField, ScrollIncognito, ScrollMagicReflection, ScrollMindBlast, ScrollParalyze, ScrollPoisonField, ScrollSummonCreature, ScrollDispel, ScrollEnergyBolt, ScrollExplosion, ScrollInvisibility, ScrollMark, ScrollMassCurse, ScrollParalyzeField, ScrollReveal, ScrollChainLightning, ScrollEnergyField, ScrollFlamestrike, ScrollGateTravel, ScrollManaVampire, ScrollMassDispel, ScrollMeteorSwarm, ScrollPolymorph, ScrollEarthquake, ScrollEnergyVortex, ScrollResurrection, ScrollSummonElemAir, ScrollSummonDaemon, ScrollSummonElemEarth, ScrollSummonElemFire, ScrollSummonElemWater, ScrollXGreen2, ScrollXTeal2, ScrollXBrown2 };

    // Food
    public static readonly ItemSpec Fishes = new[] { 0x09CF, 0x09CD, 0x09CC, 0x09CE };
    public static readonly ItemSpec RawFishSteak = 0x097A;
    public static readonly ItemSpec RawBird = 0x09B9;
    public static readonly ItemSpec CookedBird = 0x09B7;
    public static readonly ItemSpec RawRibs = 0x09F1;
    public static readonly ItemSpec Ribs = 0x09F2;
    public static readonly ItemSpec BunchOfDates = 0x1727;
    public static readonly ItemSpec Coconut = 0x1726;
    public static readonly ItemSpec Muffin = 0x09EA;
    public static readonly ItemSpec GrapeBunch = 0x09D1;
    public static readonly ItemSpec Gourd = new[] { 0x0C66, 0x0C64 };
    public static readonly ItemSpec Watermelon = 0x0C5C;
    public static readonly ItemSpec HoneydewMelon = 0x0C74;
    public static readonly ItemSpec JarOfHoney = 0x09EC;
    public static readonly ItemSpec FrenchBread = 0x098C;
    public static readonly ItemSpec Cake = 0x09E9;
    public static readonly ItemSpec BakedPie = 0x1041;
    public static readonly ItemSpec Pizza = 0x1040;
    public static readonly ItemSpec Turnip = 0x0D39;
    public static readonly ItemSpec Apple = 0x09D0;
    public static readonly ItemSpec Peache = 0x09D2;
    public static readonly ItemSpec Onion = 0x0C6D;
    public static readonly ItemSpec Pear = 0x0994;
    public static readonly ItemSpec LegOfLamb = 0x160A;
    public static readonly ItemSpec Sausage = 0x09C0;
    public static readonly ItemSpec EarOfCorn = 0x0C7F;
    public static readonly ItemSpec Pumpkin = 0x0C6A;
    public static readonly ItemSpec WheelOfCheese = 0x097E;
    public static readonly ItemSpec Squash = 0x0C72;
    public static readonly ItemSpec Canteloup = 0x0C79;
    public static readonly ItemSpec BreadLoave = 0x103B;
    public static readonly ItemSpec HeadOfCabbage = 0x0C7B;
    public static readonly ItemSpec HeadOfLettuce = 0x0C70;
    public static readonly ItemSpec Carrot = 0x0C77;
    public static readonly ItemSpec Banana = 0x171F;
    public static readonly ItemSpec SliceOfBacon = 0x0978;
    public static readonly ItemSpec Lemon = 0x1728;
    public static readonly ItemSpec Ham = 0x09C9;
    public static readonly ItemSpec ChickenLeg = 0x1608;
    public static readonly ItemSpec Limes = 0x172A;

    public static readonly ItemSpec RawFood = new[] { RawBird, RawFishSteak, RawRibs };
    public static readonly ItemSpec Food = new[] { Ribs, CookedBird,
        BunchOfDates, Muffin, GrapeBunch, Gourd, Watermelon, HoneydewMelon,
        JarOfHoney, FrenchBread, Cake, BakedPie, Pizza, Turnip, Apple, Peache,
        Onion, Pear, LegOfLamb, Sausage, EarOfCorn, Pumpkin, WheelOfCheese, Squash,
        Canteloup, BreadLoave, HeadOfCabbage, HeadOfLettuce, Carrot, Banana,
        SliceOfBacon, Lemon, Ham, ChickenLeg, Limes, Coconut
    };
    public static readonly ItemSpec QuestFood = new[] { BunchOfDates, Muffin, GrapeBunch, Gourd, Watermelon, HoneydewMelon,
        JarOfHoney, FrenchBread, Cake, BakedPie, Pizza, Turnip, Apple, Peache,
        Onion, Pear, LegOfLamb, Sausage, EarOfCorn, Pumpkin, WheelOfCheese, Squash,
        Canteloup, BreadLoave, HeadOfCabbage, HeadOfLettuce, Carrot, Banana,
        SliceOfBacon, Lemon, Ham, ChickenLeg, Limes, Coconut };

    // Doors
    public static readonly ItemSpec DoorSecretStone1 = new[] { 0x00e8, 0x00c6, 0x00c7, 0x00c8, 0x00c9, 0x00ca, 0x00cb, 0x00cc, 0x00cd, 0x00ce, 0x00cf, 0x00d0, 0x00d1, 0x00d2, 0x00d3, 0x00d4, 0x00d5, 0x00d6, 0x00d7, 0x00d8, 0x00d9, 0x00da, 0x00db, 0x00dc, 0x00dd, 0x00de, 0x00df, 0x00e0, 0x00e1, 0x00e2, 0x00e3, 0x00e4, 0x00e5, 0x00e6, 0x00e7 };
    public static readonly ItemSpec DoorSecretStone2 = new[] { 0x0314, 0x02f0, 0x02f1, 0x02f2, 0x02f3, 0x02f4, 0x0372, 0x0373, 0x0374, 0x0375, 0x0376, 0x0377 };
    public static readonly ItemSpec DoorSecretStone3 = new[] { 0x0324, 0x0317, 0x0319, 0x031b, 0x031d, 0x031f, 0x0321, 0x0323 };
    public static readonly ItemSpec DoorSecretWood1 = new[] { 0x0334, 0x0327, 0x0329, 0x032b, 0x032d, 0x032f, 0x0331, 0x0333 };
    public static readonly ItemSpec DoorSecretWood2 = new[] { 0x0344, 0x0337, 0x0339, 0x033b, 0x033d, 0x033f, 0x0341, 0x0343 };
    public static readonly ItemSpec DoorSecretStone4 = new[] { 0x0354, 0x0347, 0x0349, 0x034b, 0x034d, 0x034f, 0x0351, 0x0353 };
    public static readonly ItemSpec DoorMetal = new[] { 0x0675, 0x0600, 0x0601, 0x0602, 0x0603, 0x0604, 0x0605, 0x0606, 0x0607, 0x0608, 0x0609, 0x060a, 0x060b, 0x060c, 0x060d, 0x060e, 0x060f, 0x0610, 0x0611, 0x0612, 0x0613, 0x0614, 0x0615, 0x0616, 0x0617, 0x0618, 0x0619, 0x061a, 0x061b, 0x061c, 0x061d, 0x061e, 0x061f, 0x0620, 0x0621, 0x0622, 0x0623, 0x0624, 0x0625, 0x0626, 0x0627, 0x0628, 0x0629, 0x062a, 0x0633, 0x0634, 0x0635, 0x0636, 0x0648, 0x0649, 0x064a, 0x064b, 0x064c, 0x064d, 0x064e, 0x064f, 0x0650, 0x0651, 0x065e, 0x065f, 0x0660, 0x0661, 0x0662, 0x0663, 0x0664, 0x0665, 0x0666, 0x0667, 0x0668, 0x0669, 0x066a, 0x066b, 0x066c, 0x066d, 0x066e, 0x066f, 0x0670, 0x0671, 0x0672, 0x0673, 0x0674 };
    public static readonly ItemSpec DoorMetalBar = new[] { 0x0685, 0x0678, 0x067a, 0x067c, 0x067e, 0x0680, 0x0682, 0x0684 };
    public static readonly ItemSpec DoorRattan = new[] { 0x0695, 0x0688, 0x068a, 0x068c, 0x068e, 0x0690, 0x0692, 0x0694 };
    public static readonly ItemSpec DoorWood = new[] { 0x06a5, 0x0698, 0x069a, 0x069c, 0x069e, 0x06a0, 0x06a2, 0x06a4 };
    public static readonly ItemSpec DoorWood4 = new[] { 0x06b5, 0x06a8, 0x06aa, 0x06ac, 0x06ae, 0x06b0, 0x06b2, 0x06b4 };
    public static readonly ItemSpec DoorMetal2 = new[] { 0x06c5, 0x06b8, 0x06ba, 0x06bc, 0x06be, 0x06c0, 0x06c2, 0x06c4 };
    public static readonly ItemSpec DoorWood2 = new[] { 0x06d5, 0x06c8, 0x06ca, 0x06cc, 0x06ce, 0x06d0, 0x06d2, 0x06d4 };
    public static readonly ItemSpec DoorWood3 = new[] { 0x06e5, 0x06d8, 0x06da, 0x06dc, 0x06de, 0x06e0, 0x06e2, 0x06e4 };
    public static readonly ItemSpec ClosedDoor = new[] { DoorSecretStone1, DoorSecretStone2, DoorSecretStone3, DoorSecretWood1, DoorSecretWood2, DoorSecretStone4, DoorMetal, DoorMetalBar, DoorRattan, DoorWood, DoorWood4, DoorMetal2, DoorWood2, DoorWood3 };

    public static readonly ItemSpec Packa = 0x1091;
    public static readonly ItemSpec SpiderWeb = new[] {0x0EE3, 0x0EE4, 0x0EE5, 0x0EE6};

    public static readonly ItemSpec OpenDoor = new[]
    {
        0x00e9, 0x00ea, 0x00ec, 0x00ee, 0x00f0, 0x00f2, 0x00f4, 0x00f6,
        0x0315, 0x0316, 0x0318, 0x031a, 0x031c, 0x031e, 0x0320, 0x0322,
        0x0325, 0x0326, 0x0328, 0x032a, 0x032c, 0x032e, 0x0330, 0x0332,
        0x0335, 0x0336, 0x0338, 0x033a, 0x033c, 0x033e, 0x0340, 0x0342,
        0x0345, 0x0346, 0x0348, 0x034a, 0x034c, 0x034e, 0x0350, 0x0352,
        0x0355, 0x0356, 0x0358, 0x035a, 0x035c, 0x035e, 0x0360, 0x0362,
        0x0676, 0x0677, 0x0679, 0x067b, 0x067d, 0x067f, 0x0681, 0x0683,
        0x0686, 0x0687, 0x0689, 0x068b, 0x068d, 0x068f, 0x0691, 0x0693,
        0x0696, 0x0697, 0x0699, 0x069b, 0x069d, 0x069f, 0x06a1, 0x06a3,
        0x06a6, 0x06a7, 0x06a9, 0x06ab, 0x06ad, 0x06af, 0x06b1, 0x06b3,
        0x06b6, 0x06b7, 0x06b9, 0x06bb, 0x06bd, 0x06bf, 0x06c1, 0x06c3,
        0x06c6, 0x06c7, 0x06c9, 0x06cb, 0x06cd, 0x06cf, 0x06d1, 0x06d3,
        0x06d6, 0x06d7, 0x06d9, 0x06db, 0x06dd, 0x06df, 0x06e1, 0x06e3,
        0x06e6, 0x06e7, 0x06e9, 0x06eb, 0x06ed, 0x06ef, 0x06f1, 0x06f3
    };

    public static readonly ItemSpec Door = new[] { OpenDoor, ClosedDoor };
    
    public static readonly ItemSpec StoneArch = 0x0080;
    public static readonly ItemSpec EnergyField = new[] { 0x3947, 0x3956 };

    // Animals
    public static readonly MobileSpec Bird = 0x0006;
    public static readonly MobileSpec Eagle = 0x0005;
    public static readonly MobileSpec Rabbit = 0x00CD;
    public static readonly MobileSpec Sheep = 0x00CF;
    public static readonly MobileSpec Cow = new[] { 0x00E7, 0x00D8 };
    public static readonly MobileSpec Bull = new[] { 0x00E8, 0x00E9 };
    public static readonly MobileSpec Rat = 0x00EE;
    public static readonly MobileSpec Dog = 0x00D9;
    public static readonly MobileSpec Deer = 0x00ED;
    public static readonly MobileSpec Hart = 0x00EA;
    public static readonly MobileSpec Bear = 0x00D3;
    public static readonly MobileSpec Grizzlik = 0x00D4;
    public static readonly MobileSpec Vlcek = 0x00E1;
    public static readonly MobileSpec Hrabos = 0x00D7;
    public static readonly MobileSpec Had = 0x0034;
    public static readonly MobileSpec Prase = 0x00CB;
    public static readonly MobileSpec SedyVlk = new [] {0x00E1, 0x03B3};
    public static readonly MobileSpec SneznyLeopard = new MobileSpec(0x00D6, (Color)0x0482);

    // Monsters
    public static readonly MobileSpec Troll = new[] {0x0035, 0x0036};
    public static readonly MobileSpec Ghost = 0x03CA;
    public static readonly MobileSpec ElementalLedu = new MobileSpec(0x000E, (Color)0x0480);
    public static readonly MobileSpec ElementalSnehu = new MobileSpec(0x000E, (Color)0x0481);
    public static readonly MobileSpec ElementalKamene = new MobileSpec("Elemental Kamene");
    
    public static readonly MobileSpec Brouk = new MobileSpec(0x0317, (Color)0x0000);

    // Summons
    public static readonly MobileSpec Satan = 0x0310;
    public static readonly MobileSpec Mumie = new MobileSpec(0x009A, (Color)0x0481);
    public static readonly MobileSpec KostlivyLucistnik = new MobileSpec(0x0038, (Color)0x098F);
    public static readonly MobileSpec Lich = new MobileSpec(0x0018, (Color)0x0000);
    public static readonly MobileSpec Vampir = new MobileSpec(0x0004, (Color)0x0021);
    public static readonly MobileSpec LordLich = new MobileSpec(0x0018, (Color)0x098A);
    public static readonly MobileSpec TemnyVampir = new MobileSpec(0x0004, (Color)0x0981);

    public static readonly MobileSpec NecroSummons = new[] { Satan, Mumie, KostlivyLucistnik, Lich, Vampir, LordLich, TemnyVampir };

    public static readonly ItemSpec Torsos = new[]
    {
        0x1DAD, 0x1CEB, 0x1DB2, 0x1CE3, 0x1DA2, 0x1CEE, 0x1CEF, 0x1CE4, 0x1DA3, 0x1DA1, 0x1D9F,
        0x1DA0, 0x1CE7, 0x1CE2, 0x1CEC, 0x1CE8, 0x1CE0, 0x1CDD, 0x1CE5, 0x1CE1, 0x1DA4, 0x1CED,
        0x1CDF, 0x1CDE, 0x1CE6, 0x1CF0, 0x1CEA
    };

    // Special
    // Hair items are not visible in game client but, looting script sees them and tries to loot
    // them and of course it cannot.
    public static readonly ItemSpec HairLong = 0x203C;
    public static readonly ItemSpec HairShort = 0x203B;
    public static readonly ItemSpec HairPageBoy = 0x2045;
    public static readonly ItemSpec HairBuns = 0x2046;
    public static readonly ItemSpec HairAfro = 0x2047;
    public static readonly ItemSpec HairReceding = 0x2048;
    public static readonly ItemSpec HairPigtails = 0x2049;
    public static readonly ItemSpec HairPonytail = 0x203D;
    public static readonly ItemSpec Hairs = new[]
    {
        HairLong, HairShort, HairPageBoy, HairBuns, HairAfro,
        HairReceding, HairPigtails, HairPonytail
    };

    public static readonly ItemSpec Rocks = new[] { 0x1368, 0x136D, 0x136C, 0x1364, 0x1367, 0x136B, 0x1366, 0x1363, 0x1365, 0x136A, 0x1369 };

    private static Lazy<Dictionary<string, ItemSpec>> itemSpecs = new Lazy<Dictionary<string, ItemSpec>>(GetItemSpecs, true);
    private static Lazy<Dictionary<string, MobileSpec>> mobileSpecs = new Lazy<Dictionary<string, MobileSpec>>(GetMobileSpecs, true);

    public static string TranslateToName(Item item)
    {
        foreach (var spec in itemSpecs.Value)
        {
            if (spec.Value.Matches(item))
                return spec.Key;
        }

        return item.Type.ToString();
    }

    public static string TranslateToName(Mobile item)
    {
        foreach (var spec in mobileSpecs.Value)
        {
            if (spec.Value.Matches(item))
                return spec.Key;
        }

        return item.Type.ToString();
    }
    
    public static string TranslateToName(ItemSpec targetSpec)
    {
        foreach (var spec in itemSpecs.Value)
        {
            if (spec.Value == targetSpec)
            {
                return spec.Key;
            }
        }
        
        return null;
    }

    private static Dictionary<string, ItemSpec> GetItemSpecs() =>
        typeof(Specs)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType.Equals(typeof(ItemSpec)))
            .Select(f => new KeyValuePair<string, ItemSpec>(f.Name, (ItemSpec)f.GetValue(null)))
            .OrderByDescending(x => x.Value.Specificity)
            .ToDictionary(x => x.Key, x => x.Value);

    private static Dictionary<string, MobileSpec> GetMobileSpecs() =>
        typeof(Specs)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType.Equals(typeof(MobileSpec)))
            .Select(f => new KeyValuePair<string, MobileSpec>(f.Name, (MobileSpec)f.GetValue(null)))
            .OrderByDescending(x => x.Value.Specificity)
            .ToDictionary(x => x.Key, x => x.Value);
}