using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class MoveItemRequestRejectedEvent : IEvent
    {
        public DragResult Reason { get; }

        internal MoveItemRequestRejectedEvent(DragResult reason)
        {
            Reason = reason;
        }
    }
}
