using Infusion.Gumps;
using Infusion.LegacyApi.Console;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class GumpSubrutines
    {
        private readonly Legacy infusionApi;
        private readonly IConsole console;
        private int? nextTriggerId;
        private readonly object gumpLock = new object();

        public GumpSubrutines(Legacy infusionApi, GumpObservers gumpObservers, IConsole console)
        {
            this.infusionApi = infusionApi;
            this.console = console;
            gumpObservers.GumpReceived += OnGumpReceived;
        }

        private void OnGumpReceived()
        {
            // very nasty hack
            // This method is called before sending gump packet to client,
            // so the gump is closed before client receives gump packet and
            // cannot close it. Core is completely rotten and needs to be rewritten
            // e.g. publishing events after completely processing a packet - maybe the
            // behavior has to be considered case by case and asking what state on client
            // does script expect.
            Task.Run(() =>
            {
                Thread.Sleep(10);

                lock (gumpLock)
                {
                    try
                    {
                        if (nextTriggerId.HasValue)
                        {
                            infusionApi.TriggerGump(new GumpControlId((uint)nextTriggerId));
                            nextTriggerId = null;
                        }
                    }
                    catch (GumpException ex)
                    {
                        console.Debug(ex.ToString());
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
                try
                {
                    infusionApi.GumpResponse()
                        .Trigger(new GumpControlId((uint)triggerId));
                    nextTriggerId = null;
                }
                catch (GumpException ex)
                {
                    console.Debug(ex.ToString());
                }
            }
        }
    }
}
