using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets.Server
{
    internal sealed class GameServerListPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public byte SystemInfoFlag { get; set; }
        public ServerListItem[] Servers { get; set; }

        public override Packet RawPacket => rawPacket;

        public Packet Serialize()
        {
            int size = 3 + 3 + Servers.Length * 40;
            var payload = new byte[size];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.GameServerList.Id);
            writer.WriteUShort((ushort)size);

            writer.WriteByte(SystemInfoFlag);
            writer.WriteUShort((ushort)Servers.Length);

            foreach (var server in Servers)
            {
                writer.WriteUShort(server.Id);
                writer.WriteString(32, server.Name);
                writer.WriteByte(server.FullPercent);
                writer.WriteByte(server.TimeZone);
                writer.WriteUInt(server.IP);
            }

            rawPacket = new Packet(PacketDefinitions.GameServerList.Id, payload);
            return rawPacket;
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(3);

            SystemInfoFlag = reader.ReadByte();
            var count = reader.ReadUShort();

            Servers = new ServerListItem[count];
            for (int i = 0; i < count; i++)
            {
                var id = reader.ReadUShort();
                var name = reader.ReadString(32);
                var percent = reader.ReadByte();
                var timezone = reader.ReadByte();
                var ip = reader.ReadUInt();

                Servers[i] = new ServerListItem(id, name, percent, timezone, ip);
            }
        }
    }
}
