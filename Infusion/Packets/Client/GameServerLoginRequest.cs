using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Packets.Client
{
    public class GameServerLoginRequest
    {
        public byte[] Key { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }

        public Packet Serialize()
        {
            var payload = new byte[65];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.GameServerLoginRequest.Id);
            writer.Write(Key, 0, 4);
            writer.WriteString(30, AccountName);
            writer.WriteString(30, Password);

            return new Packet(PacketDefinitions.GameServerLoginRequest.Id, payload);
        }
    }
}
