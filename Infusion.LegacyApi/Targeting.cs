using System;
using System.Threading;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;

namespace Infusion.LegacyApi
{
    internal sealed class Targeting
    {
        private readonly UltimaClient client;
        private readonly Legacy legacyApi;
        private readonly AutoResetEvent receivedTargetInfoEvent = new AutoResetEvent(false);
        private readonly UltimaServer server;
        private readonly AutoResetEvent targetFromServerReceivedEvent = new AutoResetEvent(false);
        private bool discardNextTargetLocationRequestIfEmpty;
        private uint lastCursorId = 0x00000025;
        private uint lastItemIdInfo;

        private string lastTargetInfo;
        private ModelId lastTypeInfo;

        public Targeting(UltimaServer server, UltimaClient client, Legacy legacyApi)
        {
            this.server = server;
            this.client = client;
            this.legacyApi = legacyApi;
            server.Subscribe(PacketDefinitions.TargetCursor, HanldeServerTargetCursorPacket);
            client.RegisterFilter(FilterClientTargetCursorPacket);
        }

        private void HanldeServerTargetCursorPacket(TargetCursorPacket packet)
        {
            targetFromServerReceivedEvent.Set();
            lastCursorId = packet.CursorId;
        }

        private Packet? FilterClientTargetCursorPacket(Packet rawPacket)
        {
            if (rawPacket.Id != PacketDefinitions.TargetCursor.Id)
                return rawPacket;

            var packet = PacketDefinitionRegistry.Materialize<TargetCursorPacket>(rawPacket);

            if (discardNextTargetLocationRequestIfEmpty)
            {
                discardNextTargetLocationRequestIfEmpty = false;
                if (packet.Location.X == 0xFFFF && packet.Location.Y == 0xFFFF &&
                    packet.ClickedOnId == 0)
                {
                    return null;
                }
            }

            if (packet.CursorId == 0xDEADBEEF)
            {
                switch (packet.CursorTarget)
                {
                    case CursorTarget.Location:
                        lastTargetInfo =
                            $"{packet.ClickedOnType.Value} {packet.Location.X} {packet.Location.Y} {packet.Location.Z}";
                        break;
                    case CursorTarget.Object:
                        lastTypeInfo = packet.ClickedOnType;
                        lastItemIdInfo = packet.ClickedOnId;
                        var lastItem = legacyApi.Items[lastItemIdInfo];
                        lastTargetInfo = lastItem?.ToString() ??
                                         $"{packet.ClickedOnId:X8} {packet.ClickedOnType} {packet.ClickedOnType}";
                        break;
                }

                receivedTargetInfoEvent.Set();
                return null;
            }

            return rawPacket;
        }

        public void WaitForTarget()
        {
            targetFromServerReceivedEvent.Reset();
            while (!targetFromServerReceivedEvent.WaitOne(TimeSpan.FromSeconds(1)))
            {
                legacyApi.CheckCancellation();
            }
        }

        public string Info()
        {
            receivedTargetInfoEvent.Reset();

            client.TargetCursor(CursorTarget.Location, 0xDEADBEEF, CursorType.Neutral);

            while (!receivedTargetInfoEvent.WaitOne(TimeSpan.FromSeconds(1)))
            {
                legacyApi.CheckCancellation();
            }

            return lastTargetInfo;
        }

        public void TargetTile(Location3D location, ModelId tileType)
        {
            server.TargetLocation(0x00000025, location, tileType, CursorType.Harmful);

            client.TargetLocation(0x00000025, location, tileType, CursorType.Cancel);

            discardNextTargetLocationRequestIfEmpty = true;
        }

        public void TargetTile(string tileInfo)
        {
            var errorMessage =
                $"Invalid tile info: '{tileInfo}'. Expecting <type> <xloc> <yloc> <zloc>. All numbers has to be decimal. Example: 3295 982 1007 0";
            var parts = tileInfo.Split(' ');
            if (parts.Length != 4)
            {
                throw new InvalidOperationException(errorMessage);
            }

            ushort rawType;
            if (!ushort.TryParse(parts[0], out rawType))
                throw new InvalidOperationException(errorMessage);

            ushort xloc;
            if (!ushort.TryParse(parts[1], out xloc))
                throw new InvalidOperationException(errorMessage);

            ushort yloc;
            if (!ushort.TryParse(parts[2], out yloc))
                throw new InvalidOperationException(errorMessage);

            byte zloc;
            if (!byte.TryParse(parts[3], out zloc))
                throw new InvalidOperationException(errorMessage);

            TargetTile(xloc, yloc, zloc, rawType);
        }

        public void TargetTile(ushort xloc, ushort yloc, byte zloc, ModelId tileType)
        {
            TargetTile(new Location3D(xloc, yloc, zloc), tileType);
        }

        public void Target(Player player)
        {
            Target(player.PlayerId, player.BodyType, player.Location);
        }

        private void Target(uint itemId, ModelId type, Location3D location)
        {
            server.TargetItem(lastCursorId, itemId, CursorType.Harmful, location, type);

            discardNextTargetLocationRequestIfEmpty = true;
            client.CancelTarget(lastCursorId, itemId, location, type);
        }

        public void Target(Item item)
        {
            Target(item.Id, item.Type, item.Location);
        }

        public ModelId TypeInfo()
        {
            receivedTargetInfoEvent.Reset();
            client.TargetCursor(CursorTarget.Location, 0xDEADBEEF, CursorType.Neutral);

            while (!receivedTargetInfoEvent.WaitOne(TimeSpan.FromSeconds(1)))
            {
                legacyApi.CheckCancellation();
            }

            return lastTypeInfo;
        }

        public uint ItemIdInfo()
        {
            receivedTargetInfoEvent.Reset();
            client.TargetCursor(CursorTarget.Location, 0xDEADBEEF, CursorType.Neutral);

            while (!receivedTargetInfoEvent.WaitOne(TimeSpan.FromSeconds(1)))
            {
                legacyApi.CheckCancellation();
            }

            return lastItemIdInfo;
        }
    }
}