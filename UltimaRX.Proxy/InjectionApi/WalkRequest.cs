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
