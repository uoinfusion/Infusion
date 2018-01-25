using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class GraphicalEffectPacket : MaterializedPacket
    {
        public EffectDirectionType DirectionType { get; private set; }
        public ObjectId CharacterId { get; private set; }
        public ObjectId TargetId { get; private set; }
        public ModelId Type { get; private set; }
        public Location3D Location { get; private set; }
        public Location3D TargetLocation { get; private set; }

        public byte AnimationSpeed { get; private set; }
        public byte Duration { get; private set; }
        public bool AdjustDirection { get; private set; }
        public bool ExplodeOnImpact { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            DirectionType = (EffectDirectionType)reader.ReadByte();
            CharacterId = reader.ReadObjectId();
            TargetId = reader.ReadObjectId();
            Type = reader.ReadModelId();

            Location = new Location3D(reader.ReadUShort(), reader.ReadUShort(), reader.ReadSByte());
            TargetLocation = new Location3D(reader.ReadUShort(), reader.ReadUShort(), reader.ReadSByte());
            AnimationSpeed = reader.ReadByte();
            AdjustDirection = reader.ReadBool();
            ExplodeOnImpact = reader.ReadBool();
        }

        private Packet rawPacket;
        public override Packet RawPacket => rawPacket;
    }

    public enum EffectDirectionType : byte
    {
        SourceToDest = 0,
        StrikeAtSource = 1,
        StayAtLocation = 2,
        StayWithSource = 3
    }
}
