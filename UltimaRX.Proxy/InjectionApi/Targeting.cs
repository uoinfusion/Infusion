using System;
using System.Threading;
using UltimaRX.Packets;
using UltimaRX.Packets.Both;
using UltimaRX.Packets.Client;

namespace UltimaRX.Proxy.InjectionApi
{
    internal sealed class Targeting
    {
        private readonly AutoResetEvent receivedTargetInfoEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent targetFromServerReceivedEvent = new AutoResetEvent(false);
        private bool discardNextTargetLocationRequestIfEmpty;

        private string lastTargetInfo;
        private ushort lastTypeInfo;

        public Targeting(ServerPacketHandler serverPacketHandler, ClientPacketHandler clientPacketHandler)
        {
            serverPacketHandler.Subscribe(PacketDefinitions.TargetCursor, HanldeServerTargetCursorPacket);
            clientPacketHandler.RegisterFilter(FilterClientTargetCursorPacket);
        }

        private void HanldeServerTargetCursorPacket(TargetCursorPacket packet)
        {
            Program.Diagnostic.WriteLine("TargetCursorPacket received from server");
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
                    Program.Diagnostic.WriteLine("discarding empty TargetCursorPacket sent from client");
                    return null;
                }
                Program.Diagnostic.WriteLine("non empty TargetCursorPacket sent from client - discarding cancelled");
            }

            if (packet.CursorId == 0xDEADBEEF)
            {
                switch (packet.CursorTarget)
                {
                    case CursorTarget.Location:
                        lastTargetInfo =
                            $"{packet.ClickedOnType} {packet.Location.X} {packet.Location.Y} {packet.Location.Z}";
                        break;
                    case CursorTarget.Object:
                        lastTargetInfo =
                            $"{packet.ClickedOnType:X4} {packet.ClickedOnId:X8}";
                        break;
                }

                receivedTargetInfoEvent.Set();
                return null;
            } else if (packet.CursorId == 0xDEADBEFF)
            {
                switch (packet.CursorTarget)
                {
                    case CursorTarget.Object:
                        lastTypeInfo = packet.ClickedOnType;
                        break;
                }

                receivedTargetInfoEvent.Set();
                return null;
            }

            return rawPacket;
        }

        public void WaitForTarget()
        {
            Program.Diagnostic.WriteLine("WaitForTarget");
            targetFromServerReceivedEvent.Reset();
            while (!targetFromServerReceivedEvent.WaitOne(TimeSpan.FromSeconds(1)))
            {
                Injection.CheckCancellation();
            }

            Program.Diagnostic.WriteLine("WaitForTarget - done");
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

        public void TargetTile(Location3D location, ushort tileType)
        {
            Program.Diagnostic.WriteLine("TargetTile");
            var targetRequest = new TargetLocationRequest(0x00000025, location, tileType, CursorType.Harmful);
            Program.SendToServer(targetRequest.RawPacket);

            Program.Diagnostic.WriteLine(
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

            ushort type;
            if (!ushort.TryParse(parts[0], out type))
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

            TargetTile(xloc, yloc, zloc, type);
        }

        public void TargetTile(ushort xloc, ushort yloc, byte zloc, ushort tileType)
        {
            TargetTile(new Location3D(xloc, yloc, zloc), tileType);
        }

        public void Target(Item item)
        {
            Program.Diagnostic.WriteLine("Target");
            var targetRequest = new TargetLocationRequest(0x00000025, item.Id, CursorType.Harmful, item.Location, item.Type);
            Program.SendToServer(targetRequest.RawPacket);

            Program.Diagnostic.WriteLine(
                "Cancelling cursor on client, next TargetLocation request will be cancelled if it is empty");
            var cancelRequest = new TargetLocationRequest(0x00000025, item.Id, CursorType.Cancel, item.Location, item.Type);
            discardNextTargetLocationRequestIfEmpty = true;
            Program.SendToClient(cancelRequest.RawPacket);
        }

        public ushort TypeInfo()
        {
            var packet = new TargetCursorPacket(CursorTarget.Location, 0xDEADBEFF, CursorType.Neutral);

            receivedTargetInfoEvent.Reset();
            Program.SendToClient(packet.RawPacket);

            while (!receivedTargetInfoEvent.WaitOne(TimeSpan.FromSeconds(1)))
            {
                Injection.CheckCancellation();
            }

            return lastTypeInfo;
        }
    }
}