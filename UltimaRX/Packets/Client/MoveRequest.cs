namespace UltimaRX.Packets.Client
{
    public class MoveRequest : MaterializedPacket
    {
        public Movement Movement { get; set; }
        public byte SequenceKey { get; set; }

        public override Packet RawPacket
        {
            get
            {
                var payload = new byte[7];

                payload[0] = (byte) PacketDefinitions.MoveRequest.Id;
                payload[1] = Movement.Type == MovementType.Walk
                    ? (byte) Movement.Direction
                    : (byte) (((byte) Movement.Direction) | 0x80);
                payload[2] = SequenceKey;
                payload[3] = 0;
                payload[4] = 0;
                payload[5] = 0;
                payload[6] = 0;

                return new Packet(PacketDefinitions.MoveRequest.Id, payload);
            }
        }

        public override void Deserialize(Packet rawPacket)
        {
            var directionByte = rawPacket.Payload[1];
            Movement = (directionByte & 0x80) != 0
                ? new Movement((Direction) (directionByte - 0x80), MovementType.Run)
                : new Movement((Direction) directionByte, MovementType.Walk);
            SequenceKey = rawPacket.Payload[2];
        }
    }
}