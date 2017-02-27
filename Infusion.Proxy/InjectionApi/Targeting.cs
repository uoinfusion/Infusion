using System;
using System.Threading;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;

namespace Infusion.Proxy.InjectionApi
{
    internal sealed class Targeting
    {
        private readonly AutoResetEvent receivedTargetInfoEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent targetFromServerReceivedEvent = new AutoResetEvent(false);
        private bool discardNextTargetLocationRequestIfEmpty;
        private uint lastItemIdInfo;

        private string lastTargetInfo;
        private ModelId lastTypeInfo;

        public Targeting(ServerPacketHandler serverPacketHandler, ClientPacketHandler clientPacketHandler)
        {
            serverPacketHandler.Subscribe(PacketDefinitions.TargetCursor, HanldeServerTargetCursorPacket);
            clientPacketHandler.RegisterFilter(FilterClientTargetCursorPacket);
        }

        private void HanldeServerTargetCursorPacket(TargetCursorPacket packet)
        {
            Program.Diagnostic.Debug("TargetCursorPacket received from server");
            targetFromServerReceivedEvent.Set();
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
                    Program.Diagnostic.Debug("discarding empty TargetCursorPacket sent from client");
                    return null;
                }
                Program.Diagnostic.Debug("non empty TargetCursorPacket sent from client - discarding cancelled");
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
                        lastTargetInfo =
                            $"{packet.ClickedOnType} {packet.ClickedOnId:X8}";
                        break;
                }

                receivedTargetInfoEvent.Set();
                return null;
            }

            return rawPacket;
        }

        public void WaitForTarget()
        {
            Program.Diagnostic.Debug("WaitForTarget");
            targetFromServerReceivedEvent.Reset();
            while (!targetFromServerReceivedEvent.WaitOne(TimeSpan.FromSeconds(1)))
            {
                Injection.CheckCancellation();
            }

            Program.Diagnostic.Debug("WaitForTarget - done");
        }

        public string Info()
        {
            var packet = new TargetCursorPacket(CursorTarget.Location, 0xDEADBEEF, CursorType.Neutral);

            receivedTargetInfoEvent.Reset();
            Program.SendToClient(packet.RawPacket);

            while (!receivedTargetInfoEvent.WaitOne(TimeSpan.FromSeconds(1)))
            {
                Injection.CheckCancellation();
            }

            return lastTargetInfo;
        }

        public void TargetTile(Location3D location, ModelId tileType)
        {
            Program.Diagnostic.Debug("TargetTile");
            var targetRequest = new TargetLocationRequest(0x00000025, location, tileType, CursorType.Harmful);
            Program.SendToServer(targetRequest.RawPacket);

            Program.Diagnostic.Debug(
                "Cancelling cursor on client, next TargetLocation request will be cancelled if it is empty");
            var cancelRequest = new TargetLocationRequest(0x00000025, location, tileType, CursorType.Cancel);
            discardNextTargetLocationRequestIfEmpty = true;
            Program.SendToClient(cancelRequest.RawPacket);
        }

        public void TargetTile(string tileInfo)
        {
            string errorMessage =
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

            TargetTile(xloc, yloc, zloc, (ModelId) rawType);
        }

        public void TargetTile(ushort xloc, ushort yloc, byte zloc, ModelId tileType)
        {
            TargetTile(new Location3D(xloc, yloc, zloc), tileType);
        }

        public void Target(Item item)
        {
            Program.Diagnostic.Debug("Target");
            var targetRequest = new TargetLocationRequest(0x00000025, item.Id, CursorType.Harmful, item.Location,
                item.Type);
            Program.SendToServer(targetRequest.RawPacket);

            Program.Diagnostic.Debug(
                "Cancelling cursor on client, next TargetLocation request will be cancelled if it is empty");
            var cancelRequest = new TargetLocationRequest(0x00000025, item.Id, CursorType.Cancel, item.Location,
                item.Type);
            discardNextTargetLocationRequestIfEmpty = true;
            Program.SendToClient(cancelRequest.RawPacket);
        }

        public ModelId TypeInfo()
        {
            var packet = new TargetCursorPacket(CursorTarget.Location, 0xDEADBEEF, CursorType.Neutral);

            receivedTargetInfoEvent.Reset();
            Program.SendToClient(packet.RawPacket);

            while (!receivedTargetInfoEvent.WaitOne(TimeSpan.FromSeconds(1)))
            {
                Injection.CheckCancellation();
            }

            return lastTypeInfo;
        }

        public uint ItemIdInfo()
        {
            var packet = new TargetCursorPacket(CursorTarget.Location, 0xDEADBEEF, CursorType.Neutral);

            receivedTargetInfoEvent.Reset();
            Program.SendToClient(packet.RawPacket);

            while (!receivedTargetInfoEvent.WaitOne(TimeSpan.FromSeconds(1)))
            {
                Injection.CheckCancellation();
            }

            return lastItemIdInfo;
        }
    }
}