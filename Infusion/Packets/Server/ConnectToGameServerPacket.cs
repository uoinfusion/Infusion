using System;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class ConnectToGameServerPacket : MaterializedPacket
    {
        private byte[] payload;

        public byte[] GameServerIp { get; set; }

        public byte[] Seed { get; set; }

        public ushort GameServerPort { get; set; }

        public override Packet RawPacket
        {
            get
            {
                var modifiedPayload = new byte[payload.Length];
                payload.CopyTo(modifiedPayload, 0);

                var writer = new ArrayPacketWriter(modifiedPayload) {Position = 1};
                writer.Write(GameServerIp, 0, 4);
                writer.Position = 5;
                writer.WriteUShort(GameServerPort);

                payload = modifiedPayload;
                return new Packet(PacketDefinitions.ConnectToGameServer.Id, payload);
            }
        }

        public override void Deserialize(Packet rawPacket)
        {
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);
            GameServerIp = new byte[4];
            reader.Read(GameServerIp, 0, 4);
            GameServerPort = reader.ReadUShort();
            Seed = new byte[4];
            reader.Read(Seed, 0, 4);
            payload = rawPacket.Payload;
        }

        public Packet Serialize()
        {
            var payload = new byte[0x0B];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.ConnectToGameServer.Id);
            writer.Write(GameServerIp, 0, 4);
            writer.WriteUShort(GameServerPort);
            writer.Write(Seed, 0, 4);

            return new Packet(PacketDefinitions.ConnectToGameServer.Id, payload);
        }
    }
}