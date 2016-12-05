using System;

namespace UltimaRX.Packets
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