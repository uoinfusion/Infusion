using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class PlayerLocationChangedEvent : IEvent
    {
        public Location3D NewLocation { get; }
        public Location3D OldLocation { get; }

        public PlayerLocationChangedEvent(Location3D newLocation, Location3D oldLocation)
        {
            NewLocation = newLocation;
            OldLocation = oldLocation;
        }
    }
}
