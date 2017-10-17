using System;
using System.Collections.Generic;
using System.Threading;
using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi
{
    public class EventJournalAwaiter
    {
        private readonly IEventJournalSource source;
        private readonly Func<CancellationToken?> cancellationTokenProvider;
        private readonly Dictionary<Type,  Delegate> whenActions = new Dictionary<Type, Delegate>();
        private readonly ManualResetEvent eventReceivedEvent = new ManualResetEvent(false);
        private readonly object eventReceivedLock = new object();

        private Delegate whenActionToExecute;
        private IEvent receivedEvent;

        internal EventJournalAwaiter(IEventJournalSource source, Func<CancellationToken?> cancellationTokenProvider)
        {
            this.source = source;
            this.cancellationTokenProvider = cancellationTokenProvider;
            source.NewEventReceived += HandleNewEvent;
        }

        private void HandleNewEvent(object sender, IEvent ev)
        {
            if (whenActions.TryGetValue(ev.GetType(), out var whenDelegate))
            {
                lock (eventReceivedLock)
                {
                    if (whenActionToExecute != null)
                        return;

                    whenActionToExecute = whenDelegate;
                    receivedEvent = ev;
                }
                eventReceivedEvent.Set();
            }
        }

        public void WaitAny(TimeSpan? timeout = null)
        {
            var totalWaitingMillieseconds = 0;

            while (!eventReceivedEvent.WaitOne(100))
            {
                totalWaitingMillieseconds += 100;
                if (timeout.HasValue && timeout.Value.TotalMilliseconds < totalWaitingMillieseconds)
                    throw new TimeoutException();

                if (cancellationTokenProvider != null)
                {
                    var token = cancellationTokenProvider();
                    token?.ThrowIfCancellationRequested();
                }
            }

            if (!eventReceivedEvent.WaitOne((int) (timeout?.TotalMilliseconds ?? -1)))
                throw new TimeoutException();

            Delegate whenAction;
            IEvent ev;
            lock (eventReceivedLock)
            {
                whenAction = whenActionToExecute;
                whenActionToExecute = null;

                ev = receivedEvent;
                receivedEvent = null;
            }

            whenAction.DynamicInvoke(ev);

            eventReceivedEvent.Reset();
        }

        public EventJournalAwaiter When<T>(Action<T> action)
        {
            whenActions[typeof(T)] = action;

            return this;
        }
    }
}