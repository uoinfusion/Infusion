#load "warehouse.csx"
#load "light.csx"
#load "afk.csx"
#load "eating.csx"

using System;
using System.Linq;
using System.Collections.Generic;
using Infusion.Gumps;

public static class CraftMenu
{
    public static ScriptTrace Trace = UO.Trace.Create();

    public static void SelectItem(string gumpItemName, int amount)
    {
        var processor = new GumpProcessor();
        processor.GumpItemName = gumpItemName;
    
        var parser = new GumpParser(processor);
        parser.Parse(UO.CurrentGump);
        
        if (processor.GumpItemCheckBoxId.HasValue)
        {
            Trace.Log($"Selecting {processor.GumpItemCheckBoxId.Value}, amount {amount}");
            UO.GumpResponse()
                .SelectCheckBox(processor.GumpItemCheckBoxId.Value)
                .SetTextEntry("Mnozstvi k vyrobeni", amount.ToString(), Infusion.Gumps.GumpLabelPosition.After)
                .Trigger((GumpControlId)2);
        }
        else
            throw new InvalidOperationException($"Cannot find {gumpItemName} in gump"); 
    }

    internal static void SelectSection(string menuItemName)
    {
        UO.GumpResponse()
            .PushButton(menuItemName, Infusion.Gumps.GumpLabelPosition.Before);
    }

    private class GumpProcessor : IProcessText, IProcessCheckBox
    {
        private bool skipText = true;
        private bool found = false;
        public string GumpItemName { get; set; }
        public GumpControlId? GumpItemCheckBoxId { get; private set; }
    
        void IProcessText.OnText(int x, int y, uint hue, string text)
        {
            Trace.Log($"Text: {text}; skipText: {skipText}; gumpItemName: {GumpItemName}");
        
            if (skipText)
            {
                if (text.Equals("info", StringComparison.OrdinalIgnoreCase))
                    skipText = false;
                return;
            }
                
            if (!GumpItemCheckBoxId.HasValue && text.Equals(GumpItemName, StringComparison.OrdinalIgnoreCase))
                found = true;
        }
    
        void IProcessCheckBox.OnCheckBox(int x, int y, GumpControlId id, int uncheckId, int checkId, bool initialState)
        {
            skipText = false;
            
            if (found && !GumpItemCheckBoxId.HasValue)
                GumpItemCheckBoxId = id;
            
            Trace.Log($"CheckBox {id}"); 
        }
    }
}

public sealed class CraftProducer
{
    private readonly SpeechJournal journal = UO.CreateSpeechJournal();
    private readonly CraftProduct product;
    Dictionary<ItemSpec, Item> containersBySpec = new Dictionary<ItemSpec, Item>();
    public int BatchSize { get; set; } = 75;
    public Action StartCycle { get; set; } = () => { throw new NotImplementedException(); };
    public string[] AdditionalCycleEndPhrases { get; set; } = Array.Empty<string>();

    public CraftProducer(CraftProduct product)
    {
        this.product = product;
    }

    public void AskForResourceContainers()
    {
        foreach (var resource in product.Resources)
        {
            containersBySpec[resource.Spec] = AskForContainer(resource.Spec,
                $"Select container to reload {Specs.TranslateToName(resource.Spec)}.");
        }
    }

    public Item AskForContainer(ItemSpec spec, string prompt)
    {
        Item containerItem;
        if (Warehouse.Global.TryGetContainer(spec, out IContainer container))
        {
            container.Open();
            containerItem = container.Item;
        }
        else
            containerItem = Common.AskForContainer(prompt);
            
        return containerItem;
    }
    
    public void Produce()
    {
        AskForResourceContainers();
        var productContainerItem = AskForContainer(product.Spec, $"Select container to unload {Specs.TranslateToName(product.Spec)}.");
        var foodContainerItem = AskForContainer(Specs.Food, "Select container with food");

        while (true)
        {
            UO.ClientPrint("reloading");
            Items.MoveItems(UO.Items.Matching(product.Spec).InContainer(UO.Me.BackPack),
                productContainerItem);
            Items.Reload(foodContainerItem, 5, Specs.Food);

            foreach (var resource in product.Resources)
            {
                var resourceContainer = containersBySpec[resource.Spec];
                Items.Reload(resourceContainer, (ushort)(BatchSize * resource.Amount),
                    resource.Spec);
            }
    
            UO.Wait(1000);

            journal.Delete();
    
            try
            {
                Light.Check();
                Eating.EatFull();
                StartCycle();
                
                var lastItemName = product.Path.Last();
    
                foreach (var menuItemName in product.Path)
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
                while (!journal.Contains("S tim co mas nevyrobis nic.","Vyroba zrusena", "Vyroba ukoncena.", "Nebyl zadan zadny predmet k vyrobe")
                    && !journal.Contains(AdditionalCycleEndPhrases))
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

public sealed class CraftResource
{
    public ItemSpec Spec { get; }
    public int Amount { get; }
    
    public CraftResource(ItemSpec resourceSpec, int amount)
    {
        Spec = resourceSpec;
        Amount = amount;
    }
}

public class CraftProduct
{
    public CraftProduct(ItemSpec spec, CraftResource resource, params string[] name)
    {
        Path = name;
        Spec = spec;
        Resources = new[] { resource };
    }
    
    public CraftProduct(ItemSpec spec, CraftResource[] resources, params string[] name)
    {
        Path = name;
        Spec = spec;
        Resources = resources;
    }    

    public string[] Path { get; }
    public ItemSpec Spec { get; }
    public CraftResource[] Resources { get; }
}