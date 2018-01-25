using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class ServerRequestedGumpCloseEvent : IEvent
    {
        public GumpTypeId GumpTypeId { get; }

        internal ServerRequestedGumpCloseEvent(GumpTypeId gumpTypeId)
        {
            GumpTypeId = gumpTypeId;
        }
    }
}
