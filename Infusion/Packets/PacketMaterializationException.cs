using System;

namespace Infusion.Packets
{
    public class PacketMaterializationException : Exception
    {
        public PacketMaterializationException(string message, Exception innerExcpetion) : base(message, innerExcpetion)
        {
        }
    }
}