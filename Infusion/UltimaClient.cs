using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Server;
using System;

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

        void IClientPacketSubject.RegisterFilter(Func<Packet, Packet?> filter) => packetSubject.RegisterFilter(filter);

        public void RegisterOutputFilter(Func<Packet, Packet?> filter) => packetSubject.RegisterOutputFilter(filter);


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
            SendSpeechPacket packet = new SendSpeechPacket
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

        public void UpdateCurrentStamina(ObjectId playerId, ushort currentStamina, ushort maxStamina)
        {
            Send(new UpdateCurrentStaminaPacket(playerId, currentStamina, maxStamina).RawPacket);
        }

        public void CloseGump(GumpTypeId gumpTypeId)
        {
            Send(new CloseGenericGumpPacket(gumpTypeId).RawPacket);
        }

        public void DrawGamePlayer(ObjectId playerId, ModelId bodyType, Location3D location, Direction direction, MovementType movementType, Color color)
        {
            var drawGamePlayerPacket = new DrawGamePlayerPacket(playerId, bodyType,
                location, direction, movementType, color);
            Send(drawGamePlayerPacket.RawPacket);
        }

        public void UpdatePlayer(ObjectId playerId, ModelId bodyType, Location3D location, Direction direction, Color color)
        {
            var packet = new UpdatePlayerPacket(playerId, bodyType, location, direction, color);
            Send(packet.RawPacket);
        }

        public void TargetCursor(CursorTarget location, CursorId cursorId, CursorType type)
        {
            TargetCursorPacket packet = new TargetCursorPacket(location, cursorId, type);
            Send(packet.RawPacket);
        }

        public void TargetLocation(CursorId cursorId, Location3D location, ModelId tileType, CursorType cursorType)
        {
            TargetLocationRequest targetRequest = new TargetLocationRequest(cursorId, location, tileType, cursorType);
            Send(targetRequest.RawPacket);
        }

        public void CancelTarget(CursorId lastCursorId, ObjectId itemId, Location3D location, ModelId type)
        {
            TargetLocationRequest cancelRequest = new TargetLocationRequest(lastCursorId, itemId, CursorType.Cancel, location,
                type);
            Send(cancelRequest.RawPacket);
        }

        public void ObjectInfo(ObjectId id, ModelId type, Location3D location, Color? color)
        {
            ObjectInfoPacket packet = new ObjectInfoPacket(id, type, location, color);
            Send(packet.RawPacket);
        }

        public void DeleteItem(ObjectId id)
        {
            DeleteObjectPacket packet = new DeleteObjectPacket(id);
            Send(packet.RawPacket);
        }

        internal void PauseClient(PauseClientChoice pause)
        {
            PauseClientPacket packet = new PauseClientPacket(pause);
            Send(packet.RawPacket);
        }

        public void AllowAttack(ObjectId id)
        {
            AllowRefuseAttackPacket packet = new AllowRefuseAttackPacket(id);
            Send(packet.RawPacket);
        }

        public void CancelQuest()
        {
            QuestArrowPacket packet = new QuestArrowPacket(new Location2D(0, 0), false);
            Send(packet.RawPacket);
        }

        public void PlayGraphicalEffect(EffectDirectionType directionType, ObjectId characterId, ModelId type,
            Location3D location, byte animationSpeed, byte duration, bool adjustDirection, bool explodeOnImpact)
        {
            GraphicalEffectPacket packet = new GraphicalEffectPacket(characterId, 0, type, location, location,
                animationSpeed, directionType, duration, adjustDirection, explodeOnImpact);
            Send(packet.RawPacket);
        }
    }
}