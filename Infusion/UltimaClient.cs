using System;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion
{
    public sealed class UltimaClient : IClientPacketSubject
    {
        private readonly IClientPacketSubject packetSubject;
        private readonly Action<Packet> packetSender;

        public UltimaClient(IClientPacketSubject packetSubject, Action<Packet> packetSender)
        {
            this.packetSubject = packetSubject;
            this.packetSender = packetSender;
        }

        private void Send(Packet rawPacket)
        {
            packetSender(rawPacket);
        }

        public void RegisterFilter(Func<Packet, Packet?> filter)
        {
            packetSubject.RegisterFilter(filter);
        }

        public void Subscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket
        {
            packetSubject.Subscribe(definition, observer);
        }

        public void Unsubscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket
        {
            packetSubject.Unsubscribe(definition, observer);
        }

        public void CloseContainer(uint containerId)
        {
            Send(new CloseContainerPacket(containerId).RawPacket);
        }

        public void SendSpeech(string message, string name, uint itemId, ModelId itemModel, SpeechType type, Color color)
        {
            var packet = new SendSpeechPacket
            {
                Id = itemId,
                Model = itemModel,
                Type = type,
                Color = color,
                Font = 0x0003,
                Name = name,
                Message = message
            };

            packet.Serialize();

            Send(packet.RawPacket);
        }

        public void CloseGump(uint gumpId)
        {
            Send(new CloseGenericGumpPacket(gumpId).RawPacket);
        }

        public void DrawGamePlayer(uint playerId, ModelId bodyType, Location3D location, Movement movement, Color color)
        {
            var drawGamePlayerPacket = new DrawGamePlayerPacket(playerId, bodyType,
                location, movement, color);
            Send(drawGamePlayerPacket.RawPacket);
        }

        public void TargetCursor(CursorTarget location, uint cursorId, CursorType type)
        {
            var packet = new TargetCursorPacket(location, cursorId, type);
            Send(packet.RawPacket);
        }

        public void TargetLocation(uint cursorId, Location3D location, ModelId tileType, CursorType cursorType)
        {
            var targetRequest = new TargetLocationRequest(cursorId, location, tileType, cursorType);
            Send(targetRequest.RawPacket);
        }

        public void CancelTarget(uint lastCursorId, uint itemId, Location3D location, ModelId type)
        {
            var cancelRequest = new TargetLocationRequest(lastCursorId, itemId, CursorType.Cancel, location,
                type);
            Send(cancelRequest.RawPacket);
        }
    }
}