using System;

namespace Infusion.Packets
{
    public class PacketParsingException : Exception
    {
        private readonly Packet rawPacket;

        public PacketParsingException(Packet rawPacket, string message) : base(message)
        {
            this.rawPacket = rawPacket;
        }
    }
}