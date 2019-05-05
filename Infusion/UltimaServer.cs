using System;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;

namespace Infusion
{
    internal sealed class UltimaServer : IServerPacketSubject
    {
        private readonly Action<Packet> packetSender;
        private readonly PacketDefinitionRegistry packetRegistry;
        private readonly IServerPacketSubject packetSubject;

        public UltimaServer(IServerPacketSubject packetSubject, Action<Packet> packetSender,
            PacketDefinitionRegistry packetRegistry)
        {
            this.packetSubject = packetSubject;
            this.packetSender = packetSender;
            this.packetRegistry = packetRegistry;
        }

        public void RegisterFilter(Func<Packet, Packet?> filter)
        {
            packetSubject.RegisterFilter(filter);
        }

        public void RegisterOutputFilter(Func<Packet, Packet?> filter)
        {
            packetSubject.RegisterOutputFilter(filter);
        }

        public void Subscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket
        {
            packetSubject.Subscribe(definition, observer);
        }

        public void Unsubscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket
        {
            packetSubject.Subscribe(definition, observer);
        }

        private void Send(Packet rawPacket)
        {
            packetSender(rawPacket);
        }

        public void Say(string message)
        {
            var packet = new SpeechRequest
            {
                Type = SpeechType.Normal,
                Text = message,
                Font = 0x02b2,
                Color = 0x0003,
                Language = "ENU"
            };

            Send(packet.RawPacket);
        }

        internal void AnswerRazorNegitiation(byte responseCode)
        {
            var response = new RazorNegotiateResponse(responseCode);
            Send(response.RawPacket);
        }

        public void DoubleClick(ObjectId itemId)
        {
            var packet = new DoubleClickRequest(itemId);
            Send(packet.RawPacket);
        }

        public void Click(ObjectId itemId)
        {
            Send(new SingleClickRequest(itemId).RawPacket);
        }

        public void RequestStatus(ObjectId id)
        {
            var packet = new GetClientStatusRequest(id);

            Send(packet.RawPacket);
        }

        public void RequestWarMode(WarMode mode)
        {
            var packet = new RequestWarMode(mode);
            Send(packet.RawPacket);
        }

        public void DropItem(ObjectId itemId, ObjectId targetContainerId)
        {
            var dropPacket = packetRegistry.Instantiate<DropItemRequest>();

            Send(dropPacket.Serialize(itemId, targetContainerId));
        }

        public void DropItem(ObjectId itemId, Location3D location)
        {
            var dropPacket = packetRegistry.Instantiate<DropItemRequest>();

            Send(dropPacket.Serialize(itemId, location));
        }

        public void DropItem(ObjectId itemId, ObjectId targetContainerId, Location2D targetContainerLocation)
        {
            var dropPacket = packetRegistry.Instantiate<DropItemRequest>();

            Send(dropPacket.Serialize(itemId, targetContainerId, targetContainerLocation));
        }

        public void DragItem(ObjectId itemId, int amount)
        {
            if (amount < ushort.MinValue || amount > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(amount), $"amount cannot be less than {ushort.MinValue} or more than {ushort.MaxValue}, current value is {amount}");

            var pickupPacket = new PickupItemRequest(itemId, (ushort)amount);
            Send(pickupPacket.RawPacket);
        }

        public void Wear(ObjectId itemId, Layer layer, ObjectId playerId)
        {
            var request = new WearItemRequest(itemId, layer, playerId);
            Send(request.RawPacket);
        }

        public void CastSpell(Spell spell)
        {
            var request = new SkillRequest(spell);
            Send(request.RawPacket);
        }

        public void UseSkill(Skill skill)
        {
            var request = new SkillRequest(skill);
            Send(request.RawPacket);
        }

        public void OpenDoor()
        {
            var request = new SkillRequest(0x58, null);
            Send(request.RawPacket);
        }

        public void AttackRequest(ObjectId targetId)
        {
            var packet = new AttackRequest(targetId);
            Send(packet.RawPacket);
        }

        public void TargetLocation(CursorId cursorId, Location3D location, ModelId tileType, CursorType cursorType)
        {
            var targetRequest = new TargetLocationRequest(cursorId, location, tileType, cursorType);
            Send(targetRequest.RawPacket);
        }

        public void TargetItem(CursorId cursorId, ObjectId itemId, CursorType cursorType, Location3D location,
            ModelId type)
        {
            var targetRequest = new TargetLocationRequest(cursorId, itemId, CursorType.Harmful, location,
                type);

            Send(targetRequest.RawPacket);
        }

        public void CancelTarget(CursorId cursorId)
        {
            var targetRequest = new TargetLocationRequest(cursorId, 0, CursorType.Harmful, new Location3D(0xFFFF, 0xFFFF, 0), 0);
            Send(targetRequest.RawPacket);
        }

        public void Move(Direction direction, MovementType movementType, byte sequenceKey)
        {
            var packet = new MoveRequest
            {
                Direction = direction,
                MovementType = movementType,
                SequenceKey = sequenceKey
            };

            Send(packet.RawPacket);
        }

        internal void RequestGumpSelection(GumpMenuSelectionRequest packet)
        {
            Send(packet.RawPacket);
        }

        public void DialogBoxResponse(uint dialogId, ushort menuId, byte responseIndex, ModelId responseType,
            Color responseColor)
        {
            var packet = new ResponseToDialogBoxRequest(dialogId, menuId, responseIndex, responseType, responseColor);
            Send(packet.RawPacket);
        }
    }
}