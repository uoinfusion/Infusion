using System;
using System.Linq;
using System.Collections.Generic;

public struct AreaRect : IEquatable<AreaRect>
{
    public Location2D TopLeft { get; }
    public Location2D BottomRight { get; }

    public bool InAreaRect(Location2D location) =>
        location.X >= TopLeft.X
        && location.Y >= TopLeft.Y
        && location.X <= BottomRight.X
        && location.Y <= BottomRight.Y;

    public AreaRect(int x1, int y1, int x2, int y2)
    {
        TopLeft = new Location2D(x1, y1);
        BottomRight = new Location2D(x2, y2);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return Equals((AreaRect)obj);
    }

    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + TopLeft.GetHashCode();
        hash = (hash * 7) + BottomRight.GetHashCode();
        return hash;
    }

    public bool Equals(AreaRect other)
        => TopLeft == other.TopLeft && BottomRight == other.BottomRight;
}

public class Area
{
    public static Area Pole30 = new Area("pole 30", 2695, 3235, 2712, 3253);
    public static Area Dum62 = new Area("dum 62", new AreaRect(851, 1428, 861, 1450), new AreaRect(859, 1428, 866, 1435));
    public static Area Dum84 = new Area("dum 84", 1682, 2258, 1701, 2276);
    public static Area Dum188 = new Area("dum 188", 2715, 3240, 2725, 3255);

    public static Area DulOsamely = new Area("Osamely dul", 1189, 877, 1248, 933);
    public static Area DulZeleznehoPasu = new Area("Dul u Zelezneho pasu",
        new AreaRect(2007, 2259, 2059, 2287),
        new AreaRect(2011, 2283, 2039, 2294),
        new AreaRect(2036, 2286, 2042, 2293));
    public static Area DolyKrajske = new Area("Krajske doly", 730, 1509, 781, 1541);
    public static Area DulSamota = new Area("Dul Samota", 4919, 2307, 4958, 2341);

    public static Area BeridorLevel1 = new Area("Beridor Level 1", 540, 2618, 5890, 2996);
    public static Area BeridorLevel2 = new Area("Beridor Level 2", 1824, 5, 2084, 218);
    public static Area BeridorLevel3 = new Area("Beridor Level 3", 5194, 2628, 5514, 2910);
    public static Area BeridorBoss = new Area("Beridor Boss", 5194, 133, 5224, 95);
    public static Area Beridor = new Area("Beridor", BeridorLevel1, BeridorLevel2, BeridorLevel3, BeridorBoss);

    public static Area EtheriaLevel1 = new Area("Etheria Level 1", 5194, 1603, 5292, 1713);
    public static Area EtheriaLevel2 = new Area("Etheria Level 2", 5204, 706, 5317, 767);
    public static Area EtheriaLevel3 = new Area("Etheria Level 3", 5200, 1512, 5297, 1600);
    public static Area EtheriaLevel4 = new Area("Etheria Level 4", 5203, 1721, 5293, 1824);
    public static Area Etheria = new Area("Etheria", EtheriaLevel1, EtheriaLevel2, EtheriaLevel3, EtheriaLevel4);  

    public static Area MountDorlasLevel1 = new Area("Mount Dorlas Level 1", 5587, 148, 5800, 353);
    public static Area MountDorlasLevel2 = new Area("Mount Dorlas Level 2", 5187, 149, 5403, 374);
    public static Area MountDorlasLevel3 = new Area("Mount Dorlas Level 3", 5415, 152, 5575, 394);
    public static Area MountDorlasUtociste = new Area("Mount Dorlas Utociste", 5249, 163, 5235, 153);
    public static Area MountDorlas = new Area("Mount Dorlas", MountDorlasLevel1, MountDorlasLevel2, MountDorlasLevel3, MountDorlasUtociste);  
    
    public static Area MahatskePekloLevel1 = new Area("Mahatske peklo Level 1", 5196, 958, 5702, 1230);
    public static Area MahatskePekloLevel2 = new Area("Mahatske peklo Level 2", 5188, 1234, 5646, 1490);
    public static Area MahatskePeklo = new Area("Mahatske peklo", MahatskePekloLevel1, MahatskePekloLevel2);

    public static Area DoupeKyklopu = new Area("Doupe Kyklopu", 4843, 802, 4952, 1011);
    public static Area MoinaFilya = new Area("Moina Filya", 5370, 2368, 5538, 2566);
    public static Area OstrovPiratu = new Area("Ostrov Piratu", 228, 354, 381, 446);
    
    public static Area ArachnonLevel1 = new Area("Arachnon Level 1", 380, 1500, 550, 1598);
    public static Area ArachnonLevel2 = new Area("Arachnon Level 2", 202, 1334, 250, 1386);
    public static Area Arachnon = new Area("Arachnon", ArachnonLevel1, ArachnonLevel2);

    public static Area SnagumLevel1 = new Area("Snagum Level 1", 4922, 393, 5087, 596);
    public static Area SnagumLevel2 = new Area("Snagum Level 2", 4923, 250, 5078, 389);
    public static Area SnagumLevel3 = new Area("Snagum Level 3", 4727, 162, 4878, 367);
    public static Area Snagum = new Area("Snagum", SnagumLevel1, SnagumLevel2, SnagumLevel3);

    public static Area LedoveJeskyneLevel1 = new Area("Ledove jeskyne level 1",
        new AreaRect(5816, 3257, 6136, 3672),
        new AreaRect(5968, 3184, 6073, 3264)
    );
    public static Area LedoveJeskyneUtociste = new Area("Ledove jeskyne Utociste", 6095, 3597, 6109, 3577);
    public static Area LedoveJeskyne = new Area("Ledove jeskyne", LedoveJeskyneLevel1, LedoveJeskyneUtociste);

    public static Area CarnDumLevel1 = new Area("Carn Dum Level 1", 5305, 1863, 5404, 1995);
    public static Area CarnDumLevel2 = new Area("Carn Dum Level 2", 5199, 2281, 5362, 2459);
    public static Area CarnDumLevel3 = new Area("Carn Dum Level 3", 5190, 1857, 5301, 2005);
    public static Area CarnDumLevel4 = new Area("Carn Dum Level 4", 5195, 2470, 5366, 2618);
    public static Area CarnDumLevel5 = new Area("Carn Dum Level 5", 5191, 2023, 5364, 2270);
    public static Area CarnDumUtociste = new Area("Carn Dum Utociste", 5259, 1860, 5301, 1901);
    public static Area CarnDum = new Area("Carn Dum", CarnDumLevel1, CarnDumLevel2, CarnDumLevel3, CarnDumLevel4, CarnDumLevel5, CarnDumUtociste);

    public static Area Nexmirias = new Area("Nexmirias", 5758, 2098, 5960, 2306);

    public static Area DolGuldurLevel1 = new Area("Dol Guldur Level 1", 5204, 646, 5250, 701);
    public static Area DolGuldurLevel2 = new Area("Dol Guldur Level 2", 5200, 820, 5300, 932);
    public static Area DolGuldurLevel3 = new Area("Dol Guldur Level 3", 5258, 644, 5308, 697);
    public static Area DolGuldurBonus = new Area("Dol Guldur bonus", 5196, 3364, 5672, 3458);
    public static Area DolGuldur = new Area("Dol Guldur", DolGuldurLevel1, DolGuldurLevel2, DolGuldurLevel3, DolGuldurBonus);

    public static Area TummaOstoLevel1 = new Area("Tumma Osto level 1", 36, 2, 188, 140);
    public static Area TummaOstoLevel2 = new Area("Tumma Osto level 2", 365, 2, 483, 124);
    public static Area TummaOstoLevel3 = new Area("Tumma Osto level 3", 203, 2, 364, 110);
    public static Area TummaOstoBonus = new Area("Tumma Osto bonus", 210, 111, 250, 146);
    public static Area TummaOsto = new Area("Tumma Osto", TummaOstoLevel1, TummaOstoLevel2, TummaOstoLevel3, TummaOstoBonus);

    public static Area ZtraceneMesto = new Area("Ztracene Mesto", 712, 349, 1322, 830);

    public static Area StaraHrobka = new Area("Stara hrobka",
        new AreaRect(5622, 3475, 5794, 3597),
        new AreaRect(5809, 3608, 5846, 3669)
    );

    public static Area BrouciDira = new Area("Brouci dira",
        new AreaRect(5520, 648, 5581, 588),
        new AreaRect(5393, 720, 5478, 612)
    );

    public static Area Dormil = new Area("Dormil", 5730, 2466, 5864, 2590);

    public string Name { get; }

    private readonly AreaRect[] rects;

    public Area(string name, int x1, int y1, int x2, int y2)
        : this(name, new AreaRect(x1, y1, x2, y2))
    {
    }

    public Area(string name, params Area[] subAreas)
        : this(name, subAreas.SelectMany(a => a.rects).ToArray())
    {
    }

    public Area(string name, params AreaRect[] rects)
    {
        Name = name;
        this.rects = rects;
    }

    public bool InArea()
    {
        Location2D location = UO.Me.Location;

        foreach (var rect in rects)
        {
            if (rect.InAreaRect(location))
                return true;
        }

        return false;
    }

    private static EventJournal journal = UO.CreateEventJournal();

    public static void Run()
    {
        journal.When<PlayerLocationChangedEvent>(HandlePlayerMove)
            .Incomming();
    }

    public static void Enable()
    {
        if (!UO.CommandHandler.IsCommandRunning("areas"))
        {
            UO.CommandHandler.Invoke("areas");
        }
    }

    public static void Disable()
    {
        UO.CommandHandler.Terminate("areas");
    }

    public static void Toggle()
    {
        if (UO.CommandHandler.IsCommandRunning("areas"))
        {
            UO.Log("Disabling areas");
            Disable();
        }
        else
        {
            UO.Log("Enabling areas");
            Enable();
        }
    }

    private static void HandlePlayerMove(PlayerLocationChangedEvent ev)
    {
        var now = DateTime.UtcNow;

        if (now - lastEnterCheck > EnterCheckFrequency)
        {
            if (currentArea != null && !currentArea.InArea())
            {
                UO.Log($"Leaving {currentArea.Name}.");
                currentArea = null;
            }

            foreach (var enterPair in EnterActions)
            {
                var area = enterPair.Key;
                if (area.InArea() && area != currentArea)
                {
                    UO.Log($"Entering {area.Name}.");
                    currentArea = area;
                    var enterAction = enterPair.Value;
                    enterAction?.Invoke();                    
                }
            }

            lastEnterCheck = now;
        }
    }
    
    public static void RegisterEnterAction(Area area, Action enterAction)
    {
        EnterActions[area] = enterAction;
    }

    private static Dictionary<Area, Action> EnterActions { get; set; } = new Dictionary<Area, Action>();

    public static TimeSpan EnterCheckFrequency { get; } = TimeSpan.FromSeconds(3);

    private static DateTime lastEnterCheck = DateTime.MinValue;

    private static Area currentArea = null;
}

UO.RegisterBackgroundCommand("areas", Area.Run);
UO.RegisterCommand("areas-enable", Area.Enable);
UO.RegisterCommand("areas-disable", Area.Disable);
UO.RegisterCommand("areas-toggle", Area.Toggle);
