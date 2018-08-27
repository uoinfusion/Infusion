#load "banking.csx"
#load "common.csx"
#load "light.csx"
#load "afk.csx"
#load "meditation.csx"
#load "eating.csx"
#load "items.csx"
#load "warehouse.csx"
#load "CraftMenu.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Infusion;
using Infusion.LegacyApi.Events;

public static class Inscription
{
    public static ushort BatchSize { get; set; } = 75;
    
    private static SpeechJournal journal = UO.CreateSpeechJournal();

    public static class Circle1
    {
        public static InscriptionScroll Weaken = new InscriptionScroll(Specs.ScrollWeaken, "Kruh 1", "Weaken Scroll");
        public static InscriptionScroll NightSight = new InscriptionScroll(Specs.ScrollNightSight, "Kruh 1", "Night Sight Scroll");
        public static InscriptionScroll MagicArrow = new InscriptionScroll(Specs.ScrollMagicArrow, "Kruh 1", "Magic Arrow Scroll");
        public static InscriptionScroll Feeblemind = new InscriptionScroll(Specs.ScrollFeeblemind, "Kruh 1", "Feeblemind Scroll");
        public static InscriptionScroll Clumsy = new InscriptionScroll(Specs.ScrollClumsy, "Kruh 1", "Clumsy Scroll");
        public static InscriptionScroll ReactiveArmor = new InscriptionScroll(Specs.ScrollReactiveArmor, "Kruh 1", "Reactive Armor Scr");
        public static InscriptionScroll Heal = new InscriptionScroll(Specs.ScrollHeal, "Kruh 1", "Heal Scroll");
    }

    public static class Circle2
    {
        public static InscriptionScroll Strength = new InscriptionScroll(Specs.ScrollStrength, "Kruh 2", "Strength Scroll");
        public static InscriptionScroll Harm = new InscriptionScroll(Specs.ScrollHarm, "Kruh 2", "Harm Scroll");
        public static InscriptionScroll Cure = new InscriptionScroll(Specs.ScrollCure, "Kruh 2", "Cure Scroll");
        public static InscriptionScroll Cunnig = new InscriptionScroll(Specs.ScrollCunning, "Kruh 2", "Cunning Scroll");
        public static InscriptionScroll Agility = new InscriptionScroll(Specs.ScrollAgility, "Kruh 2", "Agility Scroll");
        public static InscriptionScroll Protection = new InscriptionScroll(Specs.ScrollProtection, "Kruh 2", "Protection Scroll");
    }
    
    public static class Circle3
    {
        public static InscriptionScroll WallOfStone = new InscriptionScroll(Specs.ScrollWallOfStone, "Kruh 3", "Wall of Stone Scro");
        public static InscriptionScroll Poison = new InscriptionScroll(Specs.Poison, "Kruh 3", "Poison Scroll");
        public static InscriptionScroll Fireball = new InscriptionScroll(Specs.ScrollFireball, "Kruh 3", "Fireball Scroll");
        public static InscriptionScroll Teleport = new InscriptionScroll(Specs.ScrollTeleport, "Kruh 3", "Teleport Scroll");
        public static InscriptionScroll Bless = new InscriptionScroll(Specs.ScrollBless, "Kruh 3", "Bless Scroll");
    }

    public static class Circle4
    {
        public static InscriptionScroll Recall = new InscriptionScroll(Specs.ScrollRecall, "Kruh 4", "Recall");
        public static InscriptionScroll ManaDrain = new InscriptionScroll(Specs.ScrollManaDrain, "Kruh 4", "ManaDrain Scroll");
        public static InscriptionScroll GreaterHeal = new InscriptionScroll(Specs.ScrollGreaterHeal, "Kruh 4", "Greater Heal Scrol");
        public static InscriptionScroll ArchProtection = new InscriptionScroll(Specs.ScrollArchprotection, "Kruh 4", "Arch Protection Sc");
        public static InscriptionScroll ArchCure = new InscriptionScroll(Specs.ScrollArchcure, "Kruh 4", "Archcure Scroll");
    }

    public static class Circle5
    {
        public static InscriptionScroll SummonCreature = new InscriptionScroll(Specs.ScrollSummonCreature, "Kruh 5", "Summon Creature Sc");
        public static InscriptionScroll Paralyze = new InscriptionScroll(Specs.ScrollParalyze, "Kruh 5", "Paralyze Scroll");
        public static InscriptionScroll MagicReflection = new InscriptionScroll(Specs.ScrollMagicReflection, "Kruh 5", "Magic Reflection");
        public static InscriptionScroll DispelField = new InscriptionScroll(Specs.ScrollDispelField, "Kruh 5", "Dispel Field Scrol");
    }

    public static class Circle6
    {
        public static InscriptionScroll Reveal = new InscriptionScroll(Specs.ScrollReveal, "Kruh 6", "Reveal Scroll");
        public static InscriptionScroll Dispel = new InscriptionScroll(Specs.ScrollDispel, "Kruh 6", "Dispel Scroll");
        public static InscriptionScroll Mark = new InscriptionScroll(Specs.ScrollMark, "Kruh 6", "Mark Scroll");
        public static InscriptionScroll ParalyzeField = new InscriptionScroll(Specs.ScrollParalyzeField, "Kruh 6", "Paralyze Field Scr");
    }

    public static class Circle7
    {
        public static InscriptionScroll Flamestrike = new InscriptionScroll(Specs.ScrollFlamestrike, "Kruh 7", "Flamestrike Scroll");
        public static InscriptionScroll MassDispel = new InscriptionScroll(Specs.ScrollMassDispel, "Kruh 7", "Mass Dispel Scrol");
        public static InscriptionScroll EnergyFied = new InscriptionScroll(Specs.EnergyField, "Kruh 7", "Energy Fied Scroll");
        public static InscriptionScroll GateTravel = new InscriptionScroll(Specs.ScrollGateTravel, "Kruh 7", "Gate Travel Scroll");
    }
    
    public static class Circle8
    {
        public static InscriptionScroll Resurrection = new InscriptionScroll(Specs.ScrollResurrection, "Kruh 8", "Resurrection Scrol");
    }

    public static class Necromancery
    {
        public static InscriptionScroll Light = new InscriptionScroll(Specs.NecroScrollOfLight, "Necromancery", "Scroll of Light");
        public static InscriptionScroll BoneArmor = new InscriptionScroll(Specs.NecroScrollOfBoneArmor, "Necromancery", "Scroll of Bone Armor");
        public static InscriptionScroll FireBolt = new InscriptionScroll(Specs.NecroScrollOfFireBolt, "Necromancery", "Scroll of Fire Bolt");
    }
    
    public static InscriptionScroll ManaShield = new InscriptionScroll(Specs.ScrollManaShield, "Mana Shield Scroll");
    public static InscriptionScroll FrostBolt = new InscriptionScroll(Specs.ScrollFrostBolt, "Frost Bolt Scroll");

    public static void Inscribe(InscriptionScroll scroll)
    {
        string productDescription = string.Join("/", scroll.Path);
        UO.ClientPrint($"Inscribing {productDescription}");

        Item scrollContainerItem;
        if (Warehouse.Global.TryGetContainer(scroll.Spec, out IContainer scrollContainer))
        {
            scrollContainer.Open();
            scrollContainerItem = scrollContainer.Item;
        }
        else
            scrollContainerItem = Common.AskForContainer("Select container to unload scrolls.");
        
        Item blankScrollContainerItem;
        if (Warehouse.Global.TryGetContainer(Specs.BlankScroll, out IContainer blankScrollContainer))
        {
            blankScrollContainer.Open();
            blankScrollContainerItem = blankScrollContainer.Item;
        }
        else
            blankScrollContainerItem = Common.AskForContainer("Select container with blank scrolls");
        
        Item foodContainerItem;
        if (Warehouse.Global.TryGetContainer(Specs.Food, out IContainer foodContainer))
        {
            foodContainer.Open();
            foodContainerItem = foodContainer.Item;
        }
        else
            blankScrollContainerItem = Common.AskForContainer("Select container with food");
    
        while (true)
        {
            UO.ClientPrint("reloading");
            Items.MoveItems(UO.Items.Matching(scroll.Spec).InContainer(UO.Me.BackPack), scrollContainerItem);
            Items.Reload(blankScrollContainerItem, BatchSize, Specs.BlankScroll);
            Items.Reload(foodContainer.Item, 5, Specs.Food);
    
            UO.Wait(1000);

            Meditation.Meditate();
            Eating.EatFull();
            journal.Delete();
            
            Light.Check();
    
            try
            {
                UO.Use(Specs.BlankScroll);
                
                var lastItemName = scroll.Path.Last();
    
                foreach (var menuItemName in scroll.Path)
                {
                    UO.ClientPrint("waiting for crafting menu");
                    UO.WaitForGump();
                    UO.Wait(500);
        
                    UO.ClientPrint($"selecting {menuItemName} from the menu");
                    if (menuItemName == lastItemName)
                    {
                        CraftMenu.SelectItem(menuItemName, BatchSize);
                    }
                    else
                    {
                        CraftMenu.SelectSection(menuItemName);
                    }
                }
                
                UO.ClientPrint("crafting...");
                while (!journal.Contains("S tim co mas nevyrobis nic.","Vyroba zrusena", "Vyroba ukoncena.", "Nebyl zadan zadny predmet k vyrobe","Nemas dost many.","Nemas dost svitku."))
                {
                    if (journal.Contains("Nebyl zadan zadny predmet k vyrobe"))
                    {
                        UO.Alert("Cannot find product in crafting menu");
                        return;
                    }
                    
                    if (journal.Contains("S tim co mas nevyrobis nic."))
                    {
                        UO.Alert("Cannot find material or tools");
                        return;
                    }
                
                    UO.Wait(1000);
                    Afk.Check();
                    if (journal.Contains("Je spatne videt."))
                    {
                        UO.Say(".abortmaking");
                        break;
                    }
                    
                    UO.Wait(1000);
                }
            }
            catch (GumpException ex)
            {
                UO.Say(".abortmaking");
                UO.Alert(ex.ToString());
                throw;
            }
            catch
            {
                UO.Say(".abortmaking");
                throw;
            }
        }
    }
}

public class InscriptionScroll
{
    public InscriptionScroll(ItemSpec spec, params string[] name)
    {
        Path = name;
        Spec = spec;
    }

    public string[] Path { get; }
    public ItemSpec Spec { get; }
}
