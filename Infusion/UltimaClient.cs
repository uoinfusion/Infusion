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
                Name = string.IsNullOrEmpty(name) ? "<null>" : name,
                Message = string.IsNullOrEmpty(message) ? "<null>" : message,
            };

            packet.Serialize();

            Send(packet.RawPacket);
        }

        internal void UpdateCurrentStamina(ObjectId playerId, ushort currentStamina, ushort maxStamina)
        {
            Send(new UpdateCurrentStaminaPacket(playerId, currentStamina, maxStamina).RawPacket);
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

        public void ObjectInfo(ObjectId id, ModelId type, Location3D location, Color? color)
        {
            var packet = new ObjectInfoPacket(id, type, location, color);
            Send(packet.RawPacket);
        }

        public Item CreatePhantom(ObjectId id, ModelId modelId, Location3D location, Color? color)
        {
            var item = new Item(id, modelId, 1, location, color, null, null);

            ObjectInfo(id, modelId, location, color);

            return item;
        }

        public void DeleteItem(ObjectId id)
        {
            var packet = new DeleteObjectPacket(id);
            Send(packet.RawPacket);
        }

        internal void PauseClient(PauseClientChoice pause)
        {
            var packet = new PauseClientPacket(pause);
            Send(packet.RawPacket);
        }

        public void AllowAttack(ObjectId id)
        {
            var packet = new AllowRefuseAttackPacket(id);
            Send(packet.RawPacket);
        }
    }
}