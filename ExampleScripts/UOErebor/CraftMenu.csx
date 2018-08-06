using System;
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
