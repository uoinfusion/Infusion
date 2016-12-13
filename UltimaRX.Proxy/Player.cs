using UltimaRX.Packets;

namespace UltimaRX.Proxy
{
    public class Player
    {
        public uint PlayerId { get; set; }

        public Location3D Location { get; set; }
        public Direction Direction { get; set; }
    }
}