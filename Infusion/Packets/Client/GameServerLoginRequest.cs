using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Packets.Client
{
    public class GameServerLoginRequest : MaterializedPacket
    {
        public byte[] Key { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }

        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);
            Key = new byte[4];
            reader.Read(Key, 0, 4);
            AccountName = reader.ReadString(30);
            Password = reader.ReadString(30);
        }

        public Packet Serialize()
        {
            var payload = new byte[65];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.GameServerLoginRequest.Id);
            writer.Write(Key, 0, 4);
            writer.WriteString(30, AccountName);
            writer.WriteString(30, Password);

            rawPacket = new Packet(PacketDefinitions.GameServerLoginRequest.Id, payload);
            return rawPacket;
        }
    }
}
