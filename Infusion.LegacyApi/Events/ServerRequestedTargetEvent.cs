using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class ServerRequestedTargetEvent : IEvent
    {
        public CursorId CursorId { get; }

        internal ServerRequestedTargetEvent(CursorId cursorId)
        {
            CursorId = cursorId;
        }
    }
}
