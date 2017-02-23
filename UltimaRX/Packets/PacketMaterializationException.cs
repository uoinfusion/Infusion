using System;

namespace UltimaRX.Packets
{
    public class PacketMaterializationException : Exception
    {
        public PacketMaterializationException(string message, Exception innerExcpetion) : base(message, innerExcpetion)
        {
        }
    }
}