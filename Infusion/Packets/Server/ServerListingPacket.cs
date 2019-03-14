using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets.Server
{
    internal sealed class ServerListingPacket : MaterializedPacket
    {
        public ServerListItem[] Servers { get; set; }

        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(3);

            var count = reader.ReadUShort();
            Servers = new ServerListItem[count];
            for (int i = 0; i < count; i++)
            {
                var id = reader.ReadUShort();
                var name = reader.ReadString(32);
                var fullPercent = reader.ReadByte();
                var timeZone = reader.ReadByte();
                var ip = reader.ReadUInt();

                Servers[i] = new ServerListItem(id, name, fullPercent, timeZone, ip);
            }
        }
    }

    internal sealed class ServerListItem
    {
        public ushort Id { get; }
        public string Name { get; }
        public byte FullPercent { get; }
        public byte TimeZone { get; }
        public uint IP { get; }

        public ServerListItem(ushort id, string name, byte fullPercent, byte timeZone, uint ip)
        {
            Id = id;
            Name = name;
            FullPercent = fullPercent;
            TimeZone = timeZone;
            IP = ip;
        }
    }
}
