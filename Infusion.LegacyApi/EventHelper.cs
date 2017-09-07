using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi
{
    internal static class EventHelper
    {
        public static void RaiseScriptEvent<T>(this Delegate eventDelegate, object sender, T args)
        {
            if (eventDelegate != null)
                Task.Run(() => eventDelegate.DynamicInvoke(sender, args));
        }
    }
}
