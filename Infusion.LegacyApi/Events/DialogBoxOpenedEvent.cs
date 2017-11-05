using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class DialogBoxOpenedEvent : IEvent
    {
        public DialogBox DialogBox { get; }

        internal DialogBoxOpenedEvent(DialogBox dialogBox)
        {
            DialogBox = dialogBox;
        }
    }
}
