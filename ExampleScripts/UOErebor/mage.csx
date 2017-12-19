#load "Specs.csx"
#load "TwoStateAbility.csx"

using System;
using System.Linq;
using Infusion.Gumps;
using Infusion.LegacyApi.Events;

public static class Mage
{
    public static ScriptTrace Trace = UO.Trace.Create();

    public static TwoStateAbility Concentration { get; } = new TwoStateAbility(".concentration",
        "Nyni jsi schopen kouzlit za mene many.", "Jsi zpatky v normalnim stavu.");
    public static TwoStateAbility StandCast { get; } = new TwoStateAbility(".standcast",
        "Nyni budes kouzlit s vetsim usilim.", "Tve usili polevilo.");
    
    private static EventJournal journal = UO.CreateEventJournal();    

    private static readonly GumpInstanceId chargerGumpId = (GumpInstanceId)0x9600057B;

    public static int FireChargerLevel
    {
        get
        {
            int lastCharger = 0;
        
            journal
                .When<ServerRequestedGumpCloseEvent>(ev => lastCharger = ProcessCloseRequest(ev, lastCharger))
                .When<GumpReceivedEvent>(ev => lastCharger = ProcessGump(ev, lastCharger))
                .All();
                
            return lastCharger;
        }
    }

    private static int ProcessGump(GumpReceivedEvent ev, int lastCharger)
    {
        if (ev.Gump.GumpId == chargerGumpId)
            return lastCharger;
    
        var processor = new ChargerGumpProcessor(2257);
        var parser = new GumpParser(processor);
        
        parser.Parse(ev.Gump);
        
        if (Trace.Enabled) Trace.Log($"IsCharger: {processor.IsChargerGump}, Level: {processor.ChargerLevel}");                     
        
        return processor.IsChargerGump ? processor.ChargerLevel : lastCharger;
    }

    private static int ProcessCloseRequest(ServerRequestedGumpCloseEvent ev, int lastCharger)
    {
        return ev.GumpId == chargerGumpId ? 0 : lastCharger;
    }

    private class ChargerGumpProcessor : IProcessGumpPic, IProcessTilePicHue
    {
        public int ChargerLevel { get; private set; }
        public bool IsChargerGump { get; private set; }
        private readonly int chargerGumpId;
        
        public ChargerGumpProcessor(int chargerGumpId)
        {
            this.chargerGumpId = chargerGumpId;
        }
    
        public void OnGumpPic(int x, int y, int gumpId)
        { 
            if (chargerGumpId == gumpId)
                IsChargerGump = true;
        }

        public void OnTilePicHue(int x, int y, uint itemId, int hue)
        {
            if (itemId == 6254)
                ChargerLevel++;
        }
    }
}
