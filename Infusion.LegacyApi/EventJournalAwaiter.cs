using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi
{
    public class EventJournalAwaiter
    {
        private readonly Func<CancellationToken?> cancellationTokenProvider;
        private readonly EventJournal journal;
        private readonly AutoResetEvent eventReceivedEvent = new AutoResetEvent(false);
        private readonly object eventReceivedLock = new object();

        private readonly Dictionary<Type, List<EventSubscription>> eventSubscriptions =
            new Dictionary<Type, List<EventSubscription>>();

        private readonly List<IEvent> preallocatedAllEvents;

        private Queue<Tuple<Delegate, IEvent>> incommingEventQueue;
        private IEvent receivedEvent;

        private Delegate whenActionToExecute;

        internal EventJournalAwaiter(IEventJournalSource source, Func<CancellationToken?> cancellationTokenProvider,
            EventJournal journal)
        {
            this.cancellationTokenProvider = cancellationTokenProvider;
            this.journal = journal;
            source.NewEventReceived += HandleNewEvent;
            preallocatedAllEvents = new List<IEvent>(source.MaximumCapacity);
        }

        public void ClearSubscriptions()
        {
            eventSubscriptions.Clear();
        }

        private void HandleNewEvent(object sender, IEvent ev)
        {
            if (eventSubscriptions.TryGetValue(ev.GetType(), out var subscriptionsList))
            {
                var subscription =
                    subscriptionsList.FirstOrDefault(x => x.Predicate == null || (bool) x.Predicate.DynamicInvoke(ev));
                if (subscription == null)
                    return;

                lock (eventReceivedLock)
                {
                    incommingEventQueue?.Enqueue(new Tuple<Delegate, IEvent>(subscription.WhenAction, ev));

                    if (whenActionToExecute == null)
                    {
                        whenActionToExecute = subscription.WhenAction;
                        receivedEvent = ev;
                    }
                }
                eventReceivedEvent.Set();
            }
        }

        internal EventJournalAwaiter When<T>(Func<T, bool> whenPredicate, Action<T> whenAction) where T : IEvent
        {
            if (!eventSubscriptions.TryGetValue(typeof(T), out var subscriptionList))
            {
                subscriptionList = new List<EventSubscription>();
                eventSubscriptions.Add(typeof(T), subscriptionList);
            }

            subscriptionList.Add(new EventSubscription(whenPredicate, whenAction));

            return this;
        }

        public void WaitAny(TimeSpan? timeout = null)
        {
            var startedTime = DateTime.UtcNow;

            journal.AwaitingStarted.Set();

            while (!eventReceivedEvent.WaitOne(10))
            {
                var elapsed = DateTime.UtcNow - startedTime;
                if (timeout.HasValue && timeout.Value < elapsed)
                    throw new TimeoutException();

                if (cancellationTokenProvider != null)
                {
                    var token = cancellationTokenProvider();
                    token?.ThrowIfCancellationRequested();
                }
            }

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
            if (!eventSubscriptions.TryGetValue(typeof(T), out var subscriptionList))
            {
                subscriptionList = new List<EventSubscription>();
                eventSubscriptions.Add(typeof(T), subscriptionList);
            }

            subscriptionList.Add(new EventSubscription(null, action));

            return this;
        }

        public void Incomming()
        {
            try
            {
                lock (eventReceivedLock)
                {
                    incommingEventQueue = new Queue<Tuple<Delegate, IEvent>>();
                }

                journal.AwaitingStarted.Set();

                while (true)
                {
                    if (eventReceivedEvent.WaitOne(10))
                    {
                        Tuple<Delegate, IEvent>[] eventTuples = null;

                        lock (eventReceivedLock)
                        {
                            if (incommingEventQueue.Count > 0)
                            {
                                eventTuples = incommingEventQueue.ToArray();
                                incommingEventQueue.Clear();
                            }
                        }

                        if (eventTuples != null)
                        {
                            foreach (var eventTuple in eventTuples)
                            {
                                eventTuple.Item1.DynamicInvoke(eventTuple.Item2);
                            }
                        }
                    }

                    if (cancellationTokenProvider != null)
                    {
                        var token = cancellationTokenProvider();
                        token?.ThrowIfCancellationRequested();
                    }
                }
            }
            finally
            {
                lock (eventReceivedLock)
                {
                    incommingEventQueue = null;
                }
            }
        }

        public void All()
        {
            var lastProcessedEventId = journal.LastEventId;

            journal.AwaitingStarted.Set();
            preallocatedAllEvents.Clear();

            journal.GatherEvents(preallocatedAllEvents, lastProcessedEventId);

            foreach (var ev in preallocatedAllEvents)
            {
                if (eventSubscriptions.TryGetValue(ev.GetType(), out var subscriptionsList))
                {
                    foreach (var subscription in subscriptionsList)
                    {
                        if (subscription.Predicate == null || (bool)subscription.Predicate.DynamicInvoke(ev))
                            subscription.WhenAction.DynamicInvoke(ev);
                    }
                }
            }

            journal.JournalStartEventId = lastProcessedEventId;
        }

        private class EventSubscription
        {
            public EventSubscription(Delegate predicate, Delegate whenAction)
            {
                Predicate = predicate;
                WhenAction = whenAction;
            }

            public Delegate WhenAction { get; }
            public Delegate Predicate { get; }
        }

    }
}