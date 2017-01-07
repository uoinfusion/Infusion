using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.Packets;

namespace UltimaRX.Proxy.InjectionApi
{
    internal struct WalkRequest
    {
        public WalkRequest(byte sequenceKey, Direction direction, bool issuedByProxy)
        {
            SequenceKey = sequenceKey;
            Direction = direction;
            IssuedByProxy = issuedByProxy;
        }

        public byte SequenceKey { get; }
        public Direction Direction { get; }
        public bool IssuedByProxy { get; }
    }
}
