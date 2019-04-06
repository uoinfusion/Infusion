using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets.Client
{
    internal sealed class LoginCharacterRequest : MaterializedPacket
    {
        private Packet rawPacket;

        public string CharacterName { get; set; }
        public uint LoginCount { get; set; }
        public uint SlotChosen { get; set; }
        public byte[] ClientIp { get; set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(5);

            CharacterName = reader.ReadString(30);
            reader.Skip(2); // unknown
            reader.Skip(4); // client flag
            reader.Skip(4); // unknown
            LoginCount = reader.ReadUInt();
            reader.Skip(16); // unknown
            SlotChosen = reader.ReadUInt();

            ClientIp = new byte[4];
            reader.Read(ClientIp, 0, 4);
        }
    }
}
