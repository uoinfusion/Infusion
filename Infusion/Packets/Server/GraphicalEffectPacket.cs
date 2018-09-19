using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class GraphicalEffectPacket : MaterializedPacket
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

        public GraphicalEffectPacket()
        {
        }

        public GraphicalEffectPacket(ObjectId characterId, ObjectId targetId, ModelId type, Location3D location,
            Location3D targetLocation, byte animationSpeed, EffectDirectionType directionType, byte duration, bool adjustDirection, bool explodeOnImpact)
        {
            DirectionType = directionType;
            CharacterId = characterId;
            TargetId = targetId;
            Type = type;
            Location = location;
            TargetLocation = targetLocation;
            AnimationSpeed = animationSpeed;
            AdjustDirection = adjustDirection;
            ExplodeOnImpact = explodeOnImpact;

            var payload = new byte[28];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.GraphicalEffect.Id);
            writer.WriteByte((byte)DirectionType);
            writer.WriteId(characterId);
            writer.WriteId(targetId);
            writer.WriteModelId(type);

            writer.WriteUShort((ushort)location.X);
            writer.WriteUShort((ushort)location.Y);
            writer.WriteByte((byte)location.Z);

            writer.WriteUShort((ushort)targetLocation.X);
            writer.WriteUShort((ushort)targetLocation.Y);
            writer.WriteByte((byte)targetLocation.Z);

            writer.WriteByte(animationSpeed);
            writer.WriteByte(duration);
            writer.WriteUShort(0); // unknwon
            writer.WriteByte((byte)(adjustDirection ? 1 : 0));
            writer.WriteByte((byte)(explodeOnImpact ? 1 : 0));

            rawPacket = new Packet(PacketDefinitions.GraphicalEffect.Id, payload);

        }

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
