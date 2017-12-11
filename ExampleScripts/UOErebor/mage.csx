#load "Specs.csx"
#load "TwoStateAbility.csx"

using System;
using System.Linq;
using Infusion.Gumps;

public static class Mage
{
    public static ScriptTrace Trace = UO.Trace.Create();

    public static TwoStateAbility Concentration { get; } = new TwoStateAbility(".concentration",
        "Nyni jsi schopen kouzlit za mene many.", "Jsi zpatky v normalnim stavu.");
    public static TwoStateAbility StandCast { get; } = new TwoStateAbility(".standcast",
        "Nyni budes kouzlit s vetsim usilim.", "Tve usili polevilo.");
    
    private static EventJournal journal = UO.CreateEventJournal();    
    
    public static int FireChargerLevel
    {
        get
        {
            var lastCharger = journal
                .Reverse()
                .OfType<GumpReceivedEvent>()
                .Where(ev =>
                {
                    if (Trace.Enabled) Trace.Log($"Id: {ev.Gump.Id}, GumpId: {ev.Gump.GumpId}");
                    
                    //return ev.Gump.Id == (GumpTypeId)0x40083F8E;
                    //return ev.Gump.Id == (GumpTypeId)0x9600057B;
                    return ev.Gump.GumpId == (GumpInstanceId)0x9600057B;
                    //return ev.Gump.GumpId == (GumpInstanceId)0x40083F8E;
                 })
                .Select(ev => {
                    var processor = new ChargerGumpProcessor(2257);
                    var parser = new GumpParser(processor);
                    
                    parser.Parse(ev.Gump);
                    
                    if (Trace.Enabled) Trace.Log($"IsCharger: {processor.IsChargerGump}, Level: {processor.ChargerLevel}");                     
                    
                    return processor;
                })
                .Where(p => p.IsChargerGump)
                .FirstOrDefault();
                
             journal.Delete();

             return lastCharger != null ? lastCharger.ChargerLevel : 0;
        }
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
