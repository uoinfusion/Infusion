using System;
using System.IO;
using System.Linq;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Proxy;
using Infusion.Proxy.LegacyApi;
using static Infusion.Proxy.LegacyApi.Legacy;

public class MapRecorder : IDisposable
{
    private readonly FileStream mapFileStream;
    private readonly StreamWriter mapWriter;
    private Location3D lastLocation;
    private Direction? lastDirection;
    private uint? lastUsedItemId;
    private ModelId? lastUsedItemType;

    public MapRecorder(string mapFileName)
    {
        mapFileStream = File.Create(mapFileName, 1024, FileOptions.WriteThrough);
        mapWriter = new StreamWriter(mapFileStream);

        Program.ClientPacketHandler.Subscribe(PacketDefinitions.DoubleClick, HandleDoubleClickRequest);
        Program.ClientPacketHandler.Subscribe(PacketDefinitions.TargetCursor, HandleTargetCursor);

        lastLocation = Me.Location;
        stepBeforeLocation = Me.Location;
        RecordWalkTo(Me.Location);

        Me.LocationChanged += OnPlayerLocationChanged;
        lastUseCommand = Legacy.CommandHandler.RegisterCommand("lastuse", LastUse);
    }

    private void LastUse()
    {
        if (lastUsedItemId.HasValue && lastUsedItemType.HasValue)
        {
            itemTypeAboutToBeUsed = lastUsedItemType.Value;
            Use(lastUsedItemId.Value);
        }
        else
        {
            Log("No last used item.");
        }
    }

    private void HandleTargetCursor(TargetCursorPacket packet)
    {
        if (packet.CursorType == CursorType.Cancel || !itemTypeAboutToBeUsed.HasValue)
            return;

        Log($"Target cursor: {packet.ClickedOnType}, {packet.Location}, {packet.CursorTarget}");

        try
        {
            if (packet.CursorTarget == CursorTarget.Location)
            {
                if (ItemTypes.Hatchets.Contains(itemTypeAboutToBeUsed.Value))
                {
                    if (lastLocation != Me.Location)
                        RecordWalkTo(Me.Location);

                    string targetInfo =
                        ($"lumber: {packet.ClickedOnType.Value} {packet.Location.X} {packet.Location.Y} {packet.Location.Z}");
                    Log(targetInfo);
                    WriteLine(targetInfo);
                }
            }
        }
        finally
        {
            itemTypeAboutToBeUsed = null;
        }
    }

    private void RecordWalkTo(Location3D targetLocation)
    {
        Log(
            $"Position: {lastLocation} -> {targetLocation}, Direction: {lastDirection?.ToString() ?? "<none>"} -> {Me.Movement.Direction}");
        WriteLine($"walk: {(Location2D)targetLocation}");

        lastDirection = Me.Movement.Direction;
        lastLocation = targetLocation;
    }

    private ModelId? itemTypeAboutToBeUsed;
    private Location3D stepBeforeLocation;
    private readonly Command lastUseCommand;

    private void HandleDoubleClickRequest(DoubleClickRequest packet)
    {
        itemTypeAboutToBeUsed = null;

        var item = Items[packet.ItemId];
        if (item == null)
        {
            Log($"Item about to be used {packet.ItemId:X8} not found");
            return;
        }

        lastUsedItemId = item.Id;
        lastUsedItemType = item.Type;
        itemTypeAboutToBeUsed = item.Type;
        Log($"Item type about to be used {item.Type}");
    }

    private void OnPlayerLocationChanged(object sender, Location3D newLocation)
    {
        if (!lastDirection.HasValue || lastDirection.Value != Me.Movement.Direction)
        {
            RecordWalkTo(stepBeforeLocation);
        }

        stepBeforeLocation = newLocation;
    }

    private void WriteLine(string str)
    {
        mapWriter.WriteLine(str);
    }

    public void Dispose()
    {
        Me.LocationChanged -= OnPlayerLocationChanged;
        Legacy.CommandHandler.Unregister(lastUseCommand);
        Program.ClientPacketHandler.Unsubscribe(PacketDefinitions.DoubleClick, HandleDoubleClickRequest);

        if (lastLocation != Me.Location)
            RecordWalkTo(Me.Location);

        mapWriter.Dispose();
        mapFileStream.Dispose();
    }
}