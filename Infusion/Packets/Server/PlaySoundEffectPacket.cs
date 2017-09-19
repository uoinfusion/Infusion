using System;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class PlaySoundEffectPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public SoundId Id { get; private set; }
        public Location3D Location { get; set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(2);
            Id = reader.ReadUShort();
            reader.Skip(2);

            var x = reader.ReadUShort();
            var y = reader.ReadUShort();

            Location = new Location3D(x, y, 0);
        }
    }
}