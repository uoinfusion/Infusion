using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class GumpSubrutines
    {
        private readonly Legacy infusionApi;
        private int? nextTriggerId;
        private readonly object gumpLock = new object();

        public GumpSubrutines(Legacy infusionApi, GumpObservers gumpObservers)
        {
            this.infusionApi = infusionApi;
            gumpObservers.GumpReceived += OnGumpReceived;
        }

        private void OnGumpReceived()
        {
            // very nasty hack
            // This method is called before sending gump packet to client,
            // so the gump is closed before client receives gump packet and
            // cannot close it. Core is completely rotten and needs to be rewritten.
            Task.Run(() =>
            {
                Thread.Sleep(10);

                lock (gumpLock)
                {
                    if (nextTriggerId.HasValue)
                    {
                        infusionApi.TriggerGump(new GumpControlId((uint)nextTriggerId));
                        nextTriggerId = null;
                    }
                }
            });
        }

        internal void WaitGump(int triggerId)
        {
            lock (gumpLock)
            {
                nextTriggerId = triggerId;
            }
        }

        internal void SendGumpSelect(int triggerId)
        {
            lock (gumpLock)
            {
                infusionApi.GumpResponse()
                    .Trigger(new GumpControlId((uint)triggerId));
                nextTriggerId = null;
            }
        }
    }
}
