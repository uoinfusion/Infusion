#load "colors.csx"
#load "area.csx"

using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;

public static class Phantoms
{
    public static ScriptTrace Trace { get; } = UO.Trace.Create();

    private static List<Phantom> phantoms = new List<Phantom>(); 

    public static EventJournal journal = UO.CreateEventJournal();

    public static ModelId WalkableType { get; set; } = 0x0E21;
    public static Color? WalkableColor { get; set; } = null;
    public static ModelId BlockingType { get; set; } = 0x1363;
    public static Color? BlockingColor { get; set; } = null;
    
    private const int previousLocationsCapacity = 32;
    private static Location3D? locationBeforeJump;
    private static Queue<Location3D> previousLocations = new Queue<Location3D>(previousLocationsCapacity);
    
    public static Color? CurrentColor
    {
        get => currentColor;
        set
        {
            currentColor = value;
        
            foreach (var phantom in phantoms)
            {
                phantom.Color = currentColor;
            }
        }
    }
    
    public static ModelId CurrentType
    {
        get => currentType;
        set
        {
            currentType = value;
            foreach (var phantom in phantoms)
            {
                phantom.Type = currentType;
            }
        }
    }
    
    private static Color? currentColor = null;
    private static ModelId currentType = BlockingType;
    private static string currentFileName;
    
    public static void Load(string fileName)
    {
        currentFileName = fileName;

        var lineNumber = 1;
        foreach (var line in File.ReadAllLines(fileName))
        {
            try
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var phantom = ParsePhantom(line);
                        
                    AddPhantom(phantom);
                }
                else
                {
                    UO.Log($"Line {lineNumber} is empty, ignoring.");
                }
            }
            catch (Exception e)
            {
                UO.Log($"Cannot parse line {lineNumber}: {line}");
                UO.Log(e.Message);
            }
            lineNumber++;
        }
        
        UO.ClientPrint($"{fileName} loaded.");
    }
    
    public static void Reset()
    {
        currentFileName = null;
        phantoms = new List<Phantom>();
    }
    
    private static Phantom ParsePhantom(string locationString)
    {
        var parts = locationString.Split(',');
        var location = new Location3D(
            ushort.Parse(parts[0].Trim()),
            ushort.Parse(parts[1].Trim()),
            sbyte.Parse(parts[2].Trim()));
        var type = BlockingType;
        var color = BlockingColor;
        if (parts.Length > 3)
            type = Convert.ToUInt16(parts[3].Trim(), 16);
        if (parts.Length > 4)
            color = (Color)Convert.ToUInt16(parts[4].Trim(), 16);

        return new Phantom(location, type, color);
    }
    
    public static void Save(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
            currentFileName = fileName;
        Save();
    }
    
    private static void Save()
    {
        if (string.IsNullOrEmpty(currentFileName))
        {
            UO.Log("No file name for phantoms specified");
            return;
        }
    
        var content = new StringBuilder();
    
        foreach (var phantom in phantoms)
        {
            content.AppendLine($"{phantom.Location}, {phantom.Type}");
        }
        
        UO.ClientPrint($"Saving to {currentFileName}");
        File.WriteAllText(currentFileName, content.ToString());
    }

    public static void Run()
    {
        journal.When<PlayerLocationChangedEvent>(HandlePlayerMove)
            .Incomming();
    }
    
    public static void Enable()
    {
        if (!UO.CommandHandler.IsCommandRunning("phantoms"))
        {
            UO.CommandHandler.Invoke("phantoms");
        }
    }
    
    public static void Disable()
    {
        UO.CommandHandler.Terminate("phantoms");
    }
    
    public static void Toggle()
    {
        if (UO.CommandHandler.IsCommandRunning("phantoms"))
        {
            UO.Log("Disabling phantoms");
            Disable();
        }
        else
        {
            UO.Log("Enabling phantoms");        
            Enable();
        }
    }
    
    public static void AddPhantomAtCurrentPosition()
    {
        Enable();
        AddPhantomAndSave(UO.Me.Location);
    }
    
    public static void LocationDump()
    {
        foreach (var location in previousLocations)
        {
            UO.Log(location.ToString());
        }
    }
    
    public static void AddPhantomAtLocationCommand(string parameters)
    {
        Enable();
        var location = ParsePhantom(parameters);
        AddPhantomAndSave(location);
    }
    
    public static void AddPhantomAtLastJump()
    {
        Enable();
        if (locationBeforeJump.HasValue)
        {
            AddPhantomAndSave(locationBeforeJump.Value);
        }
        else
            UO.Log("No previous jump");
    }
    
    public static void AddPhantomCommand()
    {
        HandlePhantomCommand(false);
    }

    public static void AddPassablePhantomCommand()
    {
        HandlePhantomCommand(true);
    }
    
    private static void HandlePhantomCommand(bool passable)
    {
        Enable();
        var (type, color) = passable
            ? (WalkableType, WalkableColor)
            : (BlockingType, BlockingColor);
        UO.ClientPrint("Select positions to add phantoms, press esc to cancel phantoms adding");
        var targetInfo = UO.Info();
        while (targetInfo.HasValue)
        {
            Trace.Log($"Phantom location: {targetInfo.Value.Location}");
            AddPhantomAndSave(new Phantom(targetInfo.Value.Location, type, color));
            targetInfo = UO.Info();
        }
        
        Trace.Log("no targetInfo");
    }
    
    public static void RefreshAll()
    {
        foreach (var phantom in phantoms)
        {
            phantom.UpdateVisibility(UO.Me.Location);
        }
    }

    public static void AddPhantomAndSave(Location3D location)
    {
        AddPhantom(location);
        Save();
        
        UO.Log($"Phantom added at {location}");
    }

    private static void AddPhantomAndSave(Phantom phantom)
    {
        AddPhantom(phantom);
        Save();
        
        UO.Log($"Phantom added at {phantom.Location}");
    }

    public static void AddPhantom(Location3D location)
    {
        var phantom = new Phantom(location, currentType, currentColor);
        AddPhantom(phantom);
    }
    
    private static void AddPhantom(Phantom phantom)
    {
        var location = phantom.Location;
        var existingPhantom = phantoms.FirstOrDefault(x => x.Location == location);
        if (existingPhantom != null)
        {
            UO.Log($"Phantom already exists at location {location}, replacing it");
            phantoms.Remove(existingPhantom);
        }
        
        phantoms.Add(phantom);        
        phantom.UpdateVisibility(UO.Me.Location);
    }

    public static void RemovePhantomCommand()
    {
        UO.ClientPrint("Select phantoms to remove, press esc to cancel phantoms removing");

        bool anyChange = false;
        var targetInfo = UO.Info();

        while (targetInfo.HasValue)
        {
            var phantomsToRemove = (targetInfo?.Id != null) ?
            phantoms.Where(x => x.Id == targetInfo.Value.Id) :
            phantoms.Where(x => (Location2D)x.Location == (Location2D)targetInfo.Value.Location);
    
            foreach (var phantom in phantomsToRemove.ToArray())
            {
                phantom.Remove();
                phantoms.Remove(phantom);
            }

            anyChange = true;
            targetInfo = UO.Info();
        }

        UO.Log("targeting cancelled");
        if (anyChange)
            Save();        
    }

    public static void MakeWalkable()
    {    
        Phantoms.CurrentType = Phantoms.WalkableType;
        Phantoms.CurrentColor = Phantoms.WalkableColor;
    }
    
    public static void MakeBlocking()
    {
        Phantoms.CurrentType = Phantoms.BlockingType;
        Phantoms.CurrentColor = Phantoms.BlockingColor;
    }
    
    private static void HandlePlayerMove(PlayerLocationChangedEvent ev)
    {
        if (previousLocations.Count >= previousLocationsCapacity - 1)
            previousLocations.Dequeue();
    
        previousLocations.Enqueue(ev.NewLocation);
        
        if (ev.NewLocation.GetDistance(ev.OldLocation) > 3)
        {
            locationBeforeJump = ev.OldLocation.LocationInDirection(UO.Me.Direction);
            UO.Log($"Jump {locationBeforeJump} -> {ev.NewLocation}");
        }
    
        foreach (var phantom in phantoms)
        {
            phantom.UpdateVisibility(ev.NewLocation);
        }
    }
    
    private static ObjectId nextPhantomId = 0x4FFFFFFF;
    
    private static ObjectId NextPhantomId() => nextPhantomId--;

    private class Phantom
    {
        public Location3D Location { get; }
        public ObjectId? Id { get; set; }
        private ModelId type;
        private Color? color;
        
        public Color? Color
        {
            get => this.color;
            set
            {
                if (color != value)
                {
                    color = value;
                    Show();
                }
            }
        }
        
        public ModelId Type
        {
            get => this.type;
            set
            {
                if (value != type)
                {
                   type = value;
                   Show();
                }
            }
        }
        
        public Phantom(Location3D location, ModelId type, Color? color)
        {
            this.type = type;
            Location = location;
            Color = color;
        }
        
        public void UpdateVisibility(Location3D viewCenter)
        {
            if (viewCenter.WithZ(0).GetDistance(Location.WithZ(0)) < 23)
            {
                Trace.Log($"Showing at location {Location}, viewCenter {viewCenter}");
                Show();
            }
            
            if (Id.HasValue && viewCenter.WithZ(0).GetDistance(Location.WithZ(0)) >= 23)
            {
                Trace.Log($"Removing at location {Location}, viewCenter {viewCenter}");
                Remove();
            }
        }
        
        public void Remove()
        {
            if (Id.HasValue)
            {
                UO.Phantoms.Remove(Id.Value);
                Id = null;
            }
        }

        
        private void Show()
        {
            if (!Id.HasValue)
            {
                Id = NextPhantomId();
            }
        
            UO.Phantoms.Show(Id.Value, Type, Location, color);
        }
    }    
}

UO.RegisterBackgroundCommand("phantoms", Phantoms.Run);
UO.RegisterCommand("phantoms-enable", Phantoms.Enable);
UO.RegisterCommand("phantoms-disable", Phantoms.Disable);
UO.RegisterCommand("phantoms-toggle", Phantoms.Toggle);

UO.RegisterCommand("phantoms-locdump", Phantoms.LocationDump);
UO.RegisterCommand("phantoms-addcurrent", Phantoms.AddPhantomAtCurrentPosition);
UO.RegisterCommand("phantoms-addloc", Phantoms.AddPhantomAtLocationCommand);
UO.RegisterCommand("phantoms-addjump", Phantoms.AddPhantomAtLastJump);
UO.RegisterCommand("phantoms-save", fileName => Phantoms.Save(fileName));
UO.RegisterCommand("phantoms-load", fileName => Phantoms.Load(fileName));
UO.RegisterCommand("phantoms-refresh", Phantoms.RefreshAll);
UO.RegisterCommand("phantoms-blocking", Phantoms.MakeBlocking);
UO.RegisterCommand("phantoms-walkable", Phantoms.MakeWalkable);
UO.RegisterCommand("phantoms-add", Phantoms.AddPhantomCommand);
UO.RegisterCommand("phantoms-add-passable", Phantoms.AddPassablePhantomCommand);
UO.RegisterCommand("phantoms-remove", Phantoms.RemovePhantomCommand);
