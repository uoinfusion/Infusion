using System;

namespace Infusion.Diagnostic
{
    public sealed class PacketLogEntry
    {
        public PacketLogEntry(DateTime created, string name, PacketDirection direction, byte[] payload)
        {
            Created = created;
            Name = name;
            Direction = direction;
            Payload = payload;
        }

        public DateTime Created { get; }
        public string Name { get; }
        public PacketDirection Direction { get; }

        public byte[] Payload { get; }
    }
}
