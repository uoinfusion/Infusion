using Infusion.Packets;

namespace Infusion.Proxy.LegacyApi
{
    internal struct WalkRequest
    {
        public WalkRequest(byte sequenceKey, Movement movement, bool issuedByProxy)
        {
            SequenceKey = sequenceKey;
            Movement = movement;
            IssuedByProxy = issuedByProxy;
        }

        public byte SequenceKey { get; }
        public Movement Movement { get; }
        public bool IssuedByProxy { get; }
    }
}
