using System;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class ObjectNameReceiver
    {
        private readonly Legacy api;
        private readonly EventJournal journal;

        public ObjectNameReceiver(Legacy api)
        {
            this.api = api;
            journal = api.CreateEventJournal();
        }

        public void Receive(int id, int timeout)
        {
            var target = api.GameObjects[(ObjectId)id];

            if (target == null)
                return;

            api.Click(target);

            if (string.IsNullOrEmpty(target.Name) && timeout > 0)
            {
                journal.When<Events.SpeechReceivedEvent>(
                        ev => ev.Speech.SpeakerId == target.Id,
                        ev => { })
                    .WhenTimeout(() => { })
                    .WaitAny(TimeSpan.FromMilliseconds(timeout));
            }
        }
    }
}
