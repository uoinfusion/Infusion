using Infusion.Packets;

namespace Infusion.LegacyApi
{
    internal struct WalkRequest
    {
        public WalkRequest(byte sequenceKey, Direction direction, MovementType movementType, bool issuedByProxy)
        {
            SequenceKey = sequenceKey;
            Direction = direction;
            MovementType = movementType;
            IssuedByProxy = issuedByProxy;
        }

        public MovementType MovementType { get; set; }
        public byte SequenceKey { get; }
        public Direction Direction { get; }
        public bool IssuedByProxy { get; }
    }
}
