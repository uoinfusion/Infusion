using Infusion.IO;
using System;

namespace Infusion.Packets.Client
{
    internal sealed class LoginCharacterRequest : MaterializedPacket
    {
        private Packet rawPacket;

        public string CharacterName { get; set; }
        public uint LoginCount { get; set; }
        public uint SlotChosen { get; set; }
        public byte[] ClientIp { get; set; }
        public ClientFlags Flags { get; set; }

        public override Packet RawPacket => rawPacket;

        public Packet Serialize()
        {
            var payload = new byte[73];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.LoginCharacter.Id);
            writer.WriteUInt(0xEDEDEDED);
            writer.WriteString(30, CharacterName);

            // unknown
            writer.WriteByte(0x00);
            writer.WriteByte(0x00);

            writer.WriteUInt((uint)Flags);

            // unknown
            writer.FillBytes(0, 24);

            writer.WriteUInt(SlotChosen);
            writer.Write(ClientIp, 0, 4);

            rawPacket = new Packet(PacketDefinitions.LoginCharacter.Id, payload);
            return rawPacket;
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(5);

            CharacterName = reader.ReadString(30);
            reader.Skip(2); // unknown
            Flags = (ClientFlags)reader.ReadUInt();
            reader.Skip(4); // unknown
            LoginCount = reader.ReadUInt();
            reader.Skip(16); // unknown
            SlotChosen = reader.ReadUInt();

            ClientIp = new byte[4];
            reader.Read(ClientIp, 0, 4);
        }
    }

    [Flags]
    public enum ClientFlags : uint
    {
        T2A = 0x00,
        Renaissance = 0x01,
        ThirdDawn = 0x02,
        Lbr = 0x04,
        Aos = 0x08,
        SE = 0x10,
        SA = 0x20,
        UO3D = 0x40,
        Client3D = 0x100,
    }
}
