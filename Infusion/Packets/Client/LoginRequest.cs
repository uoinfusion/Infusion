using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Packets.Client
{
    internal sealed class LoginRequest : MaterializedPacket
    {
        private Packet rawPacket;

        public string Account { get; set; }
        public string Password { get; set; }

        public override Packet RawPacket => rawPacket;

        public Packet Serialize()
        {
            var payload = new byte[62];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.LoginRequest.Id);
            writer.WriteString(30, Account);
            writer.WriteString(30, Password);
            writer.WriteByte(0xFF);

            rawPacket = new Packet(PacketDefinitions.LoginRequest.Id, payload);
            return rawPacket;
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);
            Account = reader.ReadString(30);
            Password = reader.ReadString(30);
        }
    }
}
