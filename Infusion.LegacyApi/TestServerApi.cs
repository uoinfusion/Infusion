using Infusion.IO;
using Infusion.Packets;
using Infusion.Packets.Server;
using System;

namespace Infusion.LegacyApi
{
    public sealed class TestServerApi
    {
        private readonly Func<byte[], Packet?> sendPacket;
        private readonly Legacy api;
        private ObjectId lastMobileId = 0x00000001;
        private ObjectId lastItemId = 0x40000001;

        public TestServerApi(Func<byte[], Packet?> packetReceivedFromServer, Legacy api)
        {
            sendPacket = packetReceivedFromServer;
            this.api = api;
        }

        public void PlayerEntersWorld(Location2D location)
        {
            var playerId = NewMobileId();

            var entersWorldPayload = new byte[37];
            var writer = new ArrayPacketWriter(entersWorldPayload);

            writer.WriteByte(0x1B); // packet
            writer.WriteId(playerId); // player id
            writer.WriteInt(0); // unknown
            writer.WriteModelId(0x190); // body type
            writer.WriteUShort((ushort)location.X);
            writer.WriteUShort((ushort)location.Y);
            writer.WriteByte(0); // unknown
            writer.WriteByte(0); // zlock
            writer.WriteByte(0x03); // facing
            writer.WriteInt(0x00007F00); // unknown
            writer.WriteInt(0x00061000); // unknown
            writer.WriteByte(0); // unkwnown
            writer.WriteUShort(0x0470); // server boundary width - 8
            writer.WriteUShort(0x0470); // server boundary height
            writer.WriteUShort(0x0500); // server boundary height
            writer.WriteUShort(0x0000); // unknown

            sendPacket(entersWorldPayload);

            var drawPlayerPayload = new byte[30];
            writer = new ArrayPacketWriter(drawPlayerPayload);
            writer.WriteByte(0x78); // packet
            writer.WriteUShort(30); // size
            writer.WriteId(playerId); // player id
            writer.WriteUShort(0x190); // graphics id
            writer.WriteUShort((ushort)location.X); // X
            writer.WriteUShort((ushort)location.Y); // Y
            writer.WriteByte(0); // Z
            writer.WriteByte(0x06); // facing
            writer.WriteColor((Color)0x0909); // color
            writer.WriteByte(0x00); // flag
            writer.WriteByte(0x01); // notoriety

            var backpackId = NewItemId();
            writer.WriteId(backpackId);
            writer.WriteUShort(0x0E75);
            writer.WriteByte(0x15);
            writer.WriteInt(0);

            sendPacket(drawPlayerPayload);
        }

        public ObjectId AddNewItemToBackpack(ModelId type, int amount = 1, Color? color = null)
            => AddNewItemToContainer(type, amount, color: color, containerId: api.Me.BackPack.Id);

        public ObjectId AddNewItemToContainer(ModelId type, int amount = 1, Location2D? location = null, ObjectId? containerId = null, Color? color = null)
        {
            var newObjectId = NewItemId();
            var payload = new byte[20];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte(0x25);
            writer.WriteId(newObjectId);
            writer.WriteUShort(type);
            writer.WriteByte(0x00);
            writer.WriteUShort((ushort)amount);
            if (location.HasValue)
            {
                writer.WriteUShort((ushort)location.Value.X);
                writer.WriteUShort((ushort)location.Value.Y);
            }
            else
            {
                writer.WriteUShort(0);
                writer.WriteUShort(0);
            }

            if (containerId.HasValue)
                writer.WriteId(containerId.Value);
            else
                writer.WriteInt(0);

            if (color.HasValue)
                writer.WriteColor(color.Value);
            else
                writer.WriteUShort(0);

            sendPacket(payload);

            return newObjectId;
        }

        public ObjectId AddNewItemToGround(ModelId type, Location2D location, int amount = 1, Color? color = null)
        {
            var newItemId = NewItemId();

            var packet = new ObjectInfoPacket(newItemId, type, (Location3D)location, color, (ushort?)amount);
            sendPacket(packet.RawPacket.Payload);

            return newItemId;
        }

        public void Say(ObjectId id, string name, string message, Color? color = null, ModelId? modelId = null,
            SpeechType type = SpeechType.Normal, ushort font = 0)
        {
            var packet = new SendSpeechPacket()
            {
                Id = id,
                Name = name,
                Message = message,
                Color = color ?? (Color)0,
                Model = modelId ?? 0,
                Font = font,
                Type = type,
            };

            packet.Serialize();

            sendPacket(packet.RawPacket.Payload);
        }

        private ObjectId NewMobileId() => lastMobileId++;
        private ObjectId NewItemId() => lastItemId++;
    }
}