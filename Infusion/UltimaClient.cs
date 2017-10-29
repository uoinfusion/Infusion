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

        internal UltimaClient(IClientPacketSubject packetSubject, Action<Packet> packetSender)
        {
            this.packetSubject = packetSubject;
            this.packetSender = packetSender;
        }

        internal void Send(Packet rawPacket)
        {
            packetSender(rawPacket);
        }

        void IClientPacketSubject.RegisterFilter(Func<Packet, Packet?> filter)
        {
            packetSubject.RegisterFilter(filter);
        }

        void IClientPacketSubject.Subscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
        {
            packetSubject.Subscribe(definition, observer);
        }

        void IClientPacketSubject.Unsubscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
        {
            packetSubject.Unsubscribe(definition, observer);
        }

        public void CloseContainer(ObjectId containerId)
        {
            Send(new CloseContainerPacket(containerId).RawPacket);
        }

        public void SendSpeech(string message, string name, ObjectId itemId, ModelId itemModel, SpeechType type, Color color)
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

        internal void CloseGump(GumpInstanceId gumpId)
        {
            Send(new CloseGenericGumpPacket(gumpId).RawPacket);
        }

        public void DrawGamePlayer(ObjectId playerId, ModelId bodyType, Location3D location, Direction direction, MovementType movementType, Color color)
        {
            var drawGamePlayerPacket = new DrawGamePlayerPacket(playerId, bodyType,
                location, direction, movementType, color);
            Send(drawGamePlayerPacket.RawPacket);
        }

        public void TargetCursor(CursorTarget location, CursorId cursorId, CursorType type)
        {
            var packet = new TargetCursorPacket(location, cursorId, type);
            Send(packet.RawPacket);
        }

        public void TargetLocation(CursorId cursorId, Location3D location, ModelId tileType, CursorType cursorType)
        {
            var targetRequest = new TargetLocationRequest(cursorId, location, tileType, cursorType);
            Send(targetRequest.RawPacket);
        }

        public void CancelTarget(CursorId lastCursorId, ObjectId itemId, Location3D location, ModelId type)
        {
            var cancelRequest = new TargetLocationRequest(lastCursorId, itemId, CursorType.Cancel, location,
                type);
            Send(cancelRequest.RawPacket);
        }

        public void ObjectInfo(ObjectId id, ModelId type, Location3D location)
        {
            var packet = new ObjectInfoPacket(id, type, location);
            Send(packet.RawPacket);
        }

        public Item CreatePhantom(ObjectId id, ModelId modelId, Location3D location)
        {
            var item = new Item(id, modelId, 1, location, null, null, null);

            ObjectInfo(id, modelId, location);

            return item;
        }

        public void DeleteItem(ObjectId id)
        {
            var packet = new DeleteObjectPacket(id);
            Send(packet.RawPacket);
        }
    }
}