using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi
{
    internal static class EventHelper
    {
        public static void RaiseScriptEvent(Delegate eventDelegate, Action raiseAction)
        {
            if (eventDelegate != null)
                Task.Run(raiseAction);
        }
    }
}
