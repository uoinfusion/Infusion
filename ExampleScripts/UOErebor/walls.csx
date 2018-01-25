#load "countdown.csx"
#load "counter.csx"
#load "colors.csx"

using System;

public static class Walls
{
    public static ScriptTrace Trace { get; } = UO.Trace.Create();

    private static TargetInfo? wallPreviewTargetInfo;
    private static ObjectId[] previewWallIds = new ObjectId[7];
    private static EventJournal eventJournal = UO.CreateEventJournal();

    public static void PreviewWall(ModelId eastWestWallType, ModelId northSouthWallType)
    {
        Trace.Log("PreviewWall");
        wallPreviewTargetInfo = UO.AskForLocation();
        DeletePreview();
        
        if (!wallPreviewTargetInfo.HasValue)
        {
            UO.ClientPrint("wall cancelled", "wall", UO.Me);
            return;
        }
        
        var targetLocation = wallPreviewTargetInfo.Value.Location;
        var playerLocation = UO.Me.Location;
        var wallDistance = playerLocation.WithZ(0).GetDistance(targetLocation.WithZ(0));
        
        Trace.Log($"{playerLocation} -> {targetLocation} (distance {wallDistance})");
        if (wallDistance > 14)
        {
            UO.ClientPrint("Too far away", "wall", UO.Me);
            return;
        }
    
        int dx = Math.Abs(targetLocation.X - playerLocation.X);
        int dy = Math.Abs(targetLocation.Y - playerLocation.Y);
        var wallType = ( dx > dy ) ? northSouthWallType : eastWestWallType;

        
        for (int i = -3; i <= 3; ++i)
        {    
            Location3D loc = new Location3D(
                (dx > dy) ? targetLocation.X : targetLocation.X + i,
                (dx > dy) ? targetLocation.Y + i : targetLocation.Y,
                targetLocation.Z);
        
            ObjectId wallId = (uint)(0x5FFFFFFF - i);
            UO.Client.ObjectInfo(wallId, wallType, loc, Colors.Green);
            previewWallIds[i + 3] = wallId;
        }

        Trace.Log($"targetInfo: {wallPreviewTargetInfo}");
    }
    
    public static void ApproveWall(ModelId eastWestWallType, ModelId northSouthWallType, Spell wallSpell, TimeSpan wallDurability, int wallDuplicates)
    {
        Trace.Log($"ApproveWall {eastWestWallType}/{northSouthWallType}, {wallDurability}");
        if (!wallPreviewTargetInfo.HasValue)
        {
            UO.ClientPrint("No preview position set", "wall", UO.Me);
            return;
        }

        var currentTargetInfo = wallPreviewTargetInfo;
        var currentTargetLocation = wallPreviewTargetInfo.Value.Location;
        
        UO.CastSpell(wallSpell);
        if (!UO.WaitForTarget("You don't know that spell.", 
            "You don't have enough mana to cast that spell!",
            "You can't see that.the target"))
        {
            Trace.Log("Waiting for target failed");
            return;
        }
        
        UO.Target(currentTargetInfo.Value.Location);

        ObjectId? targetObjectId = null;
        ModelId? targetObjectType = null;
        Item targetObject = null;
        int wallCounter = 0;        
    
        eventJournal
            .When<ItemEnteredViewEvent>(
                i =>
                {
                    if ((i.NewItem.Type == northSouthWallType || i.NewItem.Type == eastWestWallType)
                        && i.NewItem.Location == currentTargetLocation)
                    {
                        Trace.Log($"wall found {i.NewItem.Id}, {i.NewItem.Type}, wallCounter = {wallCounter}");

                        // Sphere sends wall twice, not sure why. So we have to choose wall that arrives
                        // at the target location.
                        if (wallCounter == wallDuplicates)
                            return true;
                        wallCounter++;
                        
                    }
                    return false;
                },
                i => 
                {
                    targetObjectId = i.NewItem.Id;
                    targetObjectType = i.NewItem.Type;
                    targetObject = i.NewItem;
                    Trace.Log($"choosing wall {targetObjectId.Value}, {targetObjectType.Value}, wallCounter = {wallCounter}");
                })
            .When<SpeechReceivedEvent>(
                s => IsFailMessage(s.Speech.Message),
                s => { })
            .WhenTimeout(() => UO.ClientPrint("Approve wall timeout"))
            .WaitAny(TimeSpan.FromSeconds(7.5));
            
        if (targetObjectId.HasValue && targetObjectType.HasValue && targetObject != null)
        {
            Trace.Log("Newly created wall found, starting count down!");
            
            if (wallPreviewTargetInfo.HasValue && wallPreviewTargetInfo.Value.Location == currentTargetLocation)
            {
                wallPreviewTargetInfo = null;
                DeletePreview();
            }

            new Counter("wall", Colors.Blue, targetObject, 500).Start();
        }
    }
    
    private static bool IsFailMessage(string message)
        => message.Contains("Kouzlo se nezdarilo")
            || message.Contains("Target is not in line of sight")
            || message.Contains("You can't see that.the target");
    
    public static void DeletePreview()
    {
        for (int i = 0; i < 7; i++)
        {
            if (previewWallIds[i] != 0)
            {
                UO.Client.DeleteItem(previewWallIds[i]);
                previewWallIds[i] = 0;
            }
        }
    }

    public static void PreviewWallOfStone()
    {
        PreviewWall(0x0080, 0x0080);
    }

    public static void ApproveWallOfStone()
    {
        ApproveWall(0x0080, 0x0080, Spell.WallOfStone, TimeSpan.FromSeconds(140), 1);
    }

    public static void PreviewEnergyField()
    {
        PreviewWall(0x3947, 0x3956);
    }
    
    public static void ApproveEnergyField()
    {
        ApproveWall(0x3947, 0x3956, Spell.EnergyField, TimeSpan.FromSeconds(520), 0);
    }
}

UO.RegisterCommand("wallofstone-preview", Walls.PreviewWallOfStone);
UO.RegisterCommand("wallofstone-approve", Walls.ApproveWallOfStone);
UO.RegisterCommand("energyfield-preview", Walls.PreviewEnergyField);
UO.RegisterCommand("energyfield-approve", Walls.ApproveEnergyField);