using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class PlayerMoveRequestedEvent : IEvent
    {
        public Direction Direction { get; }

        internal PlayerMoveRequestedEvent(Direction direction)
        {
            Direction = direction;
        }
    }
}
