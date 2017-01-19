using System;

namespace UltimaRX.Packets.Parsers
{
    public class PacketLogEntry
    {
        public PacketLogEntry(TimeSpan created, string name, PacketDirection direction, byte[] payload)
        {
            Created = created;
            Name = name;
            Direction = direction;
            Payload = payload;
        }

        public TimeSpan Created { get; }
        public string Name { get; }
        public PacketDirection Direction { get; }

        public byte[] Payload { get; }
    }
}
