public static class Chargers
{
    public static ScriptTrace Trace = UO.Trace.Create();

    public static int FireLevel => GetLevel(fireCharger);
    public static int EnergyLevel => GetLevel(energyCharger);

    private static EventJournal journal = UO.CreateEventJournal();    

    private static Charger fireCharger = new Charger((GumpTypeId)0x9600057B, 2257, 6254, Trace);
    private static Charger energyCharger = new Charger((GumpTypeId)0x9600057B, 2281, 6256, Trace);

    private static int GetLevel(Charger charger)
    {
        if (!UO.CommandHandler.IsCommandRunning("chargers-track"))
        {
            GetAwaiter().All();
            UO.CommandHandler.Invoke("chargers-track");
        }
        
        return charger.Level;
    }

    private static EventJournalAwaiter GetAwaiter() =>
        journal
            .When<ServerRequestedGumpCloseEvent>(ev => ProcessCloseRequest(ev))
            .When<GumpReceivedEvent>(ev => ProcessGump(ev));
            
    private static void ProcessCloseRequest(ServerRequestedGumpCloseEvent ev)
    {
        fireCharger.ProcessCloseRequest(ev);
        energyCharger.ProcessCloseRequest(ev);
    }
    
    private static void ProcessGump(GumpReceivedEvent ev)
    {
        fireCharger.ProcessGump(ev);
        energyCharger.ProcessGump(ev);
    }
    
    public static void TrackChargers() => GetAwaiter().Incomming();

    private class Charger
    {
        public int Level { get; private set; }
        
        private readonly GumpTypeId chargerGumpId = (GumpTypeId)0x9600057B;
        private readonly int tilePicHueId;
        private readonly int gumpPicId;
        private readonly ScriptTrace trace;
    
        public Charger(GumpTypeId gumpId, int gumpPicId, int tilePicHueId, ScriptTrace trace)
        {
            this.chargerGumpId = gumpId;
            this.tilePicHueId = tilePicHueId;
            this.gumpPicId = gumpPicId;
            this.trace = trace;
        }
        
        public void ProcessGump(GumpReceivedEvent ev)
        {
            if (ev.Gump.GumpTypeId != chargerGumpId)
                return;
        
            trace.Log("ProcessGump");
            var processor = new ChargerGumpProcessor(gumpPicId, tilePicHueId);
            var parser = new GumpParser(processor);
            
            parser.Parse(ev.Gump);
            
            Level = processor.IsChargerGump ? processor.ChargerLevel : Level;
    
            if (trace.Enabled) trace.Log($"IsCharger: {processor.IsChargerGump}, Level: {processor.ChargerLevel}, lastCharger: {Level}");                     
        }
    
        public void ProcessCloseRequest(ServerRequestedGumpCloseEvent ev)
        {
            Level = ev.GumpTypeId == chargerGumpId ? 0 : Level;
            trace.Log($"Processing close request {ev.GumpTypeId}, Level={Level}");
        }
    
        private class ChargerGumpProcessor : IProcessGumpPic, IProcessTilePicHue
        {
            public int ChargerLevel { get; private set; }
            public bool IsChargerGump { get; private set; }
            private readonly int chargerGumpId;
            private readonly int tilePicHueId;
            
            public ChargerGumpProcessor(int chargerGumpId, int tilePicHueId)
            {
                this.chargerGumpId = chargerGumpId;
                this.tilePicHueId = tilePicHueId;
            }
        
            public void OnGumpPic(int x, int y, int gumpId)
            { 
                if (chargerGumpId == gumpId)
                    IsChargerGump = true;
            }
    
            public void OnTilePicHue(int x, int y, uint itemId, int hue)
            {
                if (itemId == tilePicHueId)
                    ChargerLevel++;
            }
        }
    }
}

UO.RegisterBackgroundCommand("chargers-track", Chargers.TrackChargers);