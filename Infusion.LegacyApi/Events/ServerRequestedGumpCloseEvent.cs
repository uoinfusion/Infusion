using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class ServerRequestedGumpCloseEvent : IEvent
    {
        public GumpInstanceId GumpId { get; }

        internal ServerRequestedGumpCloseEvent(GumpInstanceId gumpId)
        {
            GumpId = gumpId;
        }
    }
}
