#load "warehouse.csx"
#load "light.csx"
#load "afk.csx"
#load "eating.csx"

using System;
using System.Linq;
using System.Collections.Generic;
using Infusion.Gumps;
using Infusion;

public static class CraftMenu
{
    public static ScriptTrace Trace = UO.Trace.Create();

    public static void Select(int amount, params string[] gumpItemPath)
    {
        foreach (var pathSegment in gumpItemPath.Take(gumpItemPath.Length - 1))
        {
            var sectionProcessor = new GumpSplitProcessor();
            var sectionParser = new GumpParser(sectionProcessor);
            sectionParser.Parse(UO.CurrentGump);

            if (sectionProcessor.Buttons.TryGetValue(pathSegment, out GumpControlId buttonId))
            {
                Trace.Log($"Triggering {buttonId} for {pathSegment}");
                UO.GumpResponse().Trigger(buttonId);
                UO.WaitForGump();
                UO.Wait(500);
            }
        }
        
        var itemProcessor = new GumpSplitProcessor();
        var itemParser = new GumpParser(itemProcessor);
        itemParser.Parse(UO.CurrentGump);
        
        var lastSegment = gumpItemPath.Last();
        if (itemProcessor.CheckBoxes.TryGetValue(lastSegment, out GumpControlId checkBoxId))
        {
            Trace.Log($"Selecting {checkBoxId}, amount {amount}");
            UO.GumpResponse()
                .SelectCheckBox(checkBoxId)
                .SetTextEntry("Mnozstvi k vyrobeni", amount.ToString(), Infusion.Gumps.GumpLabelPosition.After)
                .Trigger((GumpControlId)2);
            return;
        }

        
        throw new InvalidOperationException($"Cannot select item {gumpItemPath.Last()}");                
    }

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

    private class GumpSplitProcessor : IProcessText, IProcessCheckBox, IProcessButton
    {
        private bool skipText = true;
        private string lastLabel;
        public string[] CurrentPath { get; private set; }
        public Dictionary<string, GumpControlId> CheckBoxes { get; private set; } =
            new Dictionary<string, GumpControlId>();
        public Dictionary<string, GumpControlId> Buttons { get; private set; } =
            new Dictionary<string, GumpControlId>();
    
        void IProcessCheckBox.OnCheckBox(int x, int y, GumpControlId id, int uncheckId, int checkId, bool initialState)
        {
            CheckBoxes.Add(lastLabel, id);
        
            Trace.Log($"CheckBox {id}"); 
        }

        void IProcessText.OnText(int x, int y, uint hue, string text)
        {
            Trace.Log($"Text: {text}; skipText: {skipText};");

            if (CurrentPath == null)
            {
                CurrentPath = text.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1)
                    .ToArray();
                
                if (Trace.Enabled)
                {
                    if (CurrentPath.Any())
                    {
                        var description = CurrentPath.Aggregate(string.Empty, (l, r) => l + "," + r);
                        Trace.Log($"CurrentPath: {description}");
                    }
                    else
                        Trace.Log("Root gump");
                }
            }
            
            if (skipText)
            {
                if (text.Equals("info", StringComparison.OrdinalIgnoreCase))
                    skipText = false;
                return;
            }

            lastLabel = text;
        }

        public void OnButton(int x, int y, int down, int up, bool isTrigger, uint pageId, GumpControlId triggerId)
        {
            if (!string.IsNullOrEmpty(lastLabel))
            {
                Buttons[lastLabel] = triggerId;
                lastLabel = null;
            }
        
            Trace.Log($"Button {triggerId}"); 
        }
    }

    private class GumpProcessor : IProcessText, IProcessCheckBox
    {
        private bool skipText = true;
        private bool found = false;
        private string title;
        public string GumpItemName { get; set; }
        public GumpControlId? GumpItemCheckBoxId { get; private set; }
    
        void IProcessText.OnText(int x, int y, uint hue, string text)
        {
            Trace.Log($"Text: {text}; skipText: {skipText}; gumpItemName: {GumpItemName}");
        
            if (string.IsNullOrEmpty(title))
            {
                title = text;
                UO.Log(title);
            }
        
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
    private readonly Dictionary<ItemSpec, Item> containersBySpec = new Dictionary<ItemSpec, Item>();
    public int BatchSize { get; set; } = 75;
    public Action StartCycle { get; set; } = () => { throw new NotImplementedException(); };
    public Action OnStart { get; set; }
    public string[] AdditionalCycleEndPhrases { get; set; } = Array.Empty<string>();

    public static Item AskForItem(ItemSpec spec, string prompt = null, bool ignoreBackpack = false)
    {
        Item item = null;
        if (!ignoreBackpack)
            item = UO.Items.Matching(spec).InBackPack().FirstOrDefault()
                    ?? UO.Items.Matching(spec).OnLayer(Layer.OneHandedWeapon).FirstOrDefault()
                    ?? UO.Items.Matching(spec).OnLayer(Layer.TwoHandedWeapon).FirstOrDefault();
        
        if (item == null)
        {
            UO.ClientPrint(prompt ?? $"Select a {Specs.TranslateToName(spec)} to start crafting");
            item = UO.AskForItem();
            if (item == null)
                throw new InvalidOperationException("Crafting canceled");
            
            if (!spec.Matches(item))
                throw new InvalidOperationException($"Selected item ({Specs.TranslateToName(item)}) is not a {Specs.TranslateToName(spec)}. Crafting canceled.");
            
            if (item.ContainerId != UO.Me.BackPack.Id)
                Items.Pickup(item);
        }
        
        return item;
    }

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
        OnStart?.Invoke();
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
                UO.WaitForGump();
                UO.Wait(500);
                
                var lastItemName = product.Path.Last();
    
                CraftMenu.Select(BatchSize, product.Path);
                var productionStart = DateTime.UtcNow;
                
                while (!journal.Contains("S tim co mas nevyrobis nic.","Vyroba zrusena", "Vyroba ukoncena.", "Nebyl zadan zadny predmet k vyrobe")
                    && !journal.Contains(AdditionalCycleEndPhrases))
                {
                    if (journal.Contains("Nebyl zadan zadny predmet k vyrobe"))
                    {
                        UO.ClientPrint("Cannot find product in crafting menu", UO.Me);
                        return;
                    }
                    
                    if (journal.Contains("S tim co mas nevyrobis nic."))
                    {
                        UO.ClientPrint("Cannot find material or tools", UO.Me);
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
                    var lastProductionMessage = journal.Last("System: Vyrabim")?.Created;
                    if (lastProductionMessage.HasValue)
                    {
                        var sinceLastProductionMessage = DateTime.UtcNow - lastProductionMessage.Value;
                        if (sinceLastProductionMessage > TimeSpan.FromSeconds(30))
                        {
                            UO.Say(".abortmaking");
                            UO.ClientPrint("Craft menu stuck, restarting production");
                            break;
                        }
                    }
                }
            }
            catch (GumpException ex)
            {
                UO.Say(".abortmaking");
                UO.ClientPrint(ex.ToString(), UO.Me);
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