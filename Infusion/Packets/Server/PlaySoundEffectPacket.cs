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
            var z = reader.ReadUShort();
            if (z > byte.MaxValue)
                throw new NotImplementedException(
                    $"z coordinate of a sound is {z}. Only byte z coordinates implemented.");

            Location = new Location3D(x, y, (byte) z);
        }
    }
}