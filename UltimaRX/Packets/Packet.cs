namespace Infusion.Packets
{
    public struct Packet
    {
        public byte[] Payload { get; }

        public int Id { get; }

        public int Length => Payload.Length;

        public Packet(int id, byte[] payload)
        {
            Payload = payload;
            Id = id;
        }
    }
}