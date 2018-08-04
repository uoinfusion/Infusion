#load "banking.csx"
#load "common.csx"
#load "light.csx"
#load "afk.csx"
#load "meditation.csx"
#load "eating.csx"
#load "items.csx"
#load "warehouse.csx"

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
    }

    public static class Circle2
    {
        public static InscriptionScroll Weaken = new InscriptionScroll(Specs.ScrollStrength, "Kruh 2", "Strength Scroll");
        public static InscriptionScroll NightSight = new InscriptionScroll(Specs.ScrollHarm, "Kruh 2", "Harm Scroll");
        public static InscriptionScroll MagicArrow = new InscriptionScroll(Specs.ScrollCure, "Kruh 2", "Cure Scroll");
        public static InscriptionScroll Feeblemind = new InscriptionScroll(Specs.ScrollCunning, "Kruh 2", "Cunning Scroll");
        public static InscriptionScroll Clumsy = new InscriptionScroll(Specs.ScrollAgility, "Kruh 2", "Agility Scroll");
    }
    
    public static class Circle3
    {
        public static InscriptionScroll WallOfStone = new InscriptionScroll(Specs.ScrollWallOfStone, "Kruh 3", "Wall of Stone Scro");
        public static InscriptionScroll Poison = new InscriptionScroll(Specs.Poison, "Kruh 3", "Poison Scroll");
        public static InscriptionScroll Fireball = new InscriptionScroll(Specs.ScrollFireball, "Kruh 3", "Fireball Scroll");
    }

    public static class Circle4
    {
        public static InscriptionScroll Recall = new InscriptionScroll(Specs.ScrollRecall, "Kruh 4", "Recall Scroll");
        public static InscriptionScroll ManaDrain = new InscriptionScroll(Specs.ScrollManaDrain, "Kruh 4", "ManaDrain Scroll");
    }

    public static class Circle5
    {
        public static InscriptionScroll SummonCreature = new InscriptionScroll(Specs.ScrollSummonCreature, "Kruh 5", "Summon Creature Sc");
        public static InscriptionScroll Paralyze = new InscriptionScroll(Specs.ScrollParalyze, "Kruh 5", "Paralyze Scroll");
    }
    
    public static class Necromancery
    {
        public static InscriptionScroll Light = new InscriptionScroll(Specs.NecroScrollOfLight, "Necromancery", "Scroll of Light");
        public static InscriptionScroll BoneArmor = new InscriptionScroll(Specs.NecroScrollOfBoneArmor, "Necromancery", "Scroll of Bone Armor");
    }
    
    public static InscriptionScroll ManaShield = new InscriptionScroll(Specs.ScrollManaShield, "Mana Shield Scroll");

    public static void Inscribe(InscriptionScroll scroll)
    {
        string productDescription = string.Join("/", scroll.Path);
        UO.ClientPrint($"Inscribing {productDescription}");
    
        var scrollContainer = Warehouse.GetContainer(scroll.Spec);
        var blankScrollContainer = Warehouse.GetContainer(Specs.BlankScroll);
        var foodContainer = Warehouse.GetContainer(Specs.Food);

        scrollContainer.Open();
        blankScrollContainer.Open();
        foodContainer.Open();
    
        while (true)
        {
            UO.ClientPrint("reloading");
            Items.MoveItems(UO.Items.Matching(scroll.Spec).InContainer(UO.Me.BackPack), scrollContainer.Item);
            Items.Reload(blankScrollContainer.Item, BatchSize, Specs.BlankScroll);
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
                        UO.GumpResponse()
                            .SelectCheckBox(menuItemName, Infusion.Gumps.GumpLabelPosition.Before)
                            .SetTextEntry("Mnozstvi k vyrobeni", BatchSize.ToString(), Infusion.Gumps.GumpLabelPosition.After)
                            .Trigger((GumpControlId)2);
                    }
                    else
                    {
                        UO.GumpResponse()
                            .PushButton(menuItemName, Infusion.Gumps.GumpLabelPosition.Before);
                    }
                }
                
                UO.ClientPrint("crafting...");
                while (!journal.Contains("S tim co mas nevyrobis nic.","Vyroba zrusena", "Vyroba ukoncena.", "Nebyl zadan zadny predmet k vyrobe","Nemas dost many."))
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
