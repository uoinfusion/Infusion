using Infusion.IO;
using System.Collections.Generic;


namespace Infusion.Packets.Server
{
    internal sealed class BoatMovingPacket : MaterializedPacket
    {
        private PositionEntity[] PositionEntities { get; set; }
        public uint Serial { get; set; }
        public byte BoatSpeed { get; set; }
        public Direction MovingDirection { get; set; }
        public Direction FacingDirection { get; set; }
        
        private Packet _rawPacket;
        public BoatMovingPacket()
        {
            
        }
        public override Packet RawPacket => _rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            _rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(3);

            Serial = reader.ReadUInt();
            BoatSpeed = reader.ReadByte();
            (MovingDirection, _) = reader.ReadDirection();
            (FacingDirection, _) = reader.ReadDirection();
            ushort x = reader.ReadUShort();
            ushort y = reader.ReadUShort();
            ushort z = reader.ReadUShort();

            int count = reader.ReadUShort();
            PositionEntities = new PositionEntity[count];
            for (int i = 0; i < count; i++)
            {
                uint cSerial = reader.ReadUInt();
                ushort cx = reader.ReadUShort();
                ushort cy = reader.ReadUShort();
                ushort cz = reader.ReadUShort();

                PositionEntities[i] = new PositionEntity(cSerial, new Location3D(cx, cy, 0));
            }


        }
    }

    internal class PositionEntity
    {
        internal uint Serial { get; set; }
        internal Location3D Location { get; set; }

        public PositionEntity(uint serial, Location3D location)
        {
            Serial = serial;
            Location = location;
        }
    }
}