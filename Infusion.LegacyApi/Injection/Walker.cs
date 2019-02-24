using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class Walker
    {
        private readonly Legacy api;
        private readonly EventJournal journal;

        public Walker(Legacy api)
        {
            this.api = api;
            this.journal = api.CreateEventJournal();
        }

        public void MakeStepByKey(int key)
        {
            DateTime startTime;
            int attempts = 0;
            int maxAttempts = 10;

            bool moveRequestFailed = false;
            do
            {
                bool moveRequestSent = false;
                do
                {
                    api.NotifyAction();
                    this.api.ClientWindow.PressKey((KeyCode)key);

                    journal.When<Events.PlayerMoveRequestedEvent>(request =>
                        {
                            moveRequestSent = true;
                        })
                        .WhenTimeout(() =>
                        {
                            moveRequestSent = false;
                        })
                        .WaitAny(TimeSpan.FromSeconds(1));
                } while (!moveRequestSent);

                startTime = DateTime.UtcNow;

                journal.When<Events.PlayerMoveRejectedEvent>(p =>
                    {
                        moveRequestFailed = true;
                    })
                    .When<Events.PlayerMoveAcceptedEvent>(p =>
                    {
                        moveRequestFailed = false;
                    })
                    .WhenTimeout(() => {
                        if (attempts > maxAttempts)
                            throw new InjectionException("Cannot walk");
                        moveRequestFailed = true;
                    })
                    .WaitAny(TimeSpan.FromSeconds(30));

                api.Wait(25);
                attempts++;
             } while (moveRequestFailed);

            var endTime = DateTime.UtcNow;
            var minDuration = TimeSpan.FromMilliseconds(150);
            var duration = endTime - startTime;
            if (duration < minDuration)
            {
                api.Wait((int)(minDuration - duration).TotalMilliseconds);
            }
        }
    }
}
