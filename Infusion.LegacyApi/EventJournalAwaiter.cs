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
        private readonly EventJournal journal;
        private readonly Func<TimeSpan?> defaultTimeout;
        private readonly AutoResetEvent eventReceivedEvent = new AutoResetEvent(false);
        private readonly object eventReceivedLock = new object();

        private readonly Dictionary<Type, List<EventSubscription>> eventSubscriptions =
            new Dictionary<Type, List<EventSubscription>>();

        private readonly List<IEvent> preallocatedAllEvents;

        private Queue<Tuple<Delegate, IEvent>> incommingEventQueue;
        private IEvent receivedEvent;

        private Delegate whenActionToExecute;
        private readonly IEventJournalSource source;
        private readonly Cancellation cancellation;
        private Action timeoutAction;

        internal EventJournalAwaiter(IEventJournalSource source, Cancellation cancellation,
            EventJournal journal, Func<TimeSpan?> defaultTimeout)
        {
            this.source = source;
            this.cancellation = cancellation;
            this.journal = journal;
            this.defaultTimeout = defaultTimeout;
            source.NewEventReceived += HandleNewEvent;
            preallocatedAllEvents = new List<IEvent>();
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

                    eventReceivedEvent.Set();
                }
            }
        }

        public EventJournalAwaiter When<T>(Func<T, bool> whenPredicate, Action<T> whenAction) where T : IEvent
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
            preallocatedAllEvents.Clear();
            journal.GetherWaitEnyEvents(preallocatedAllEvents);

            journal.AwaitingStarted.Set();

            foreach (var ev in preallocatedAllEvents)
            {
                cancellation?.Check();

                if (eventSubscriptions.TryGetValue(ev.GetType(), out var subscriptionsList))
                {
                    foreach (var subscription in subscriptionsList)
                    {
                        if (subscription.Predicate == null || (bool) subscription.Predicate.DynamicInvoke(ev))
                        {
                            journal.NotifyWait();
                            subscription.WhenAction.DynamicInvoke(ev);
                            return;
                        }
                    }
                }
            }

            try
            {
                timeout = timeout ?? defaultTimeout?.Invoke();
                var startedTime = DateTime.UtcNow;

                while (!eventReceivedEvent.WaitOne(10))
                {
                    var elapsed = DateTime.UtcNow - startedTime;
                    if (timeout.HasValue && timeout.Value < elapsed)
                    {
                        if (timeoutAction != null)
                        {
                            timeoutAction();
                            return;
                        }
                        else
                            throw new TimeoutException("Event journal WaitAny timeout.");
                    }

                    cancellation?.Check();
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
                journal.NotifyWait();
            }
            finally
            {
                lock (eventReceivedLock)
                {
                    source.NewEventReceived -= HandleNewEvent;
                    whenActionToExecute = null;
                    receivedEvent = null;
                    eventSubscriptions.Clear();
                    eventReceivedEvent.Reset();
                }
            }
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

        public EventJournalAwaiter WhenTimeout(Action timeoutAction)
        {
            this.timeoutAction = timeoutAction;

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

                    cancellation?.Check();
                }
            }
            finally
            {
                lock (eventReceivedLock)
                {
                    source.NewEventReceived -= HandleNewEvent;
                    eventSubscriptions.Clear();
                    incommingEventQueue = null;
                    eventReceivedEvent.Reset();
                }
            }
        }

        public void All()
        {
            try
            {
                var lastProcessedEventId = journal.LastEventId;

                journal.AwaitingStarted.Set();
                preallocatedAllEvents.Clear();

                journal.GatherEvents(preallocatedAllEvents, EventId.MinValue, lastProcessedEventId);

                cancellation?.Check();

                foreach (var ev in preallocatedAllEvents)
                {
                    cancellation?.Check();

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
            finally
            {
                lock (eventReceivedLock)
                {
                    source.NewEventReceived -= HandleNewEvent;
                    eventSubscriptions.Clear();
                }
            }
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