using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class MobileFlagsUpdatedEvent : IEvent
    {
        public Mobile BeforeUpdate { get; }
        public Mobile Updated { get; }

        internal MobileFlagsUpdatedEvent(Mobile beforeUpdate, Mobile updated)
        {
            BeforeUpdate = beforeUpdate;
            Updated = updated;
        }
    }
}
