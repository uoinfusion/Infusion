using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class SpeechReceivedEvent : IEvent
    {
        internal SpeechReceivedEvent(JournalEntry speech)
        {
            Speech = speech;
        }

        public JournalEntry Speech { get; }
    }
}
