using System;

namespace UltimaRX.Packets.Server
{
    public class MovePlayer : MaterializedPacket
    {
        public Direction Direction { get; set; }

        public override Packet RawPacket
        {
            get
            {
                var payload = new byte[2];
                payload[0] = (byte) PacketDefinitions.MovePlayer.Id;
                payload[1] = (byte) Direction;

                return new Packet(PacketDefinitions.MovePlayer.Id, payload);
            }
        }

        public override void Deserialize(Packet rawPacket)
        {
            throw new NotImplementedException();
        }
    }
}