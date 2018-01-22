using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class ItemWornEvent : IEvent
    {
        public ObjectId ItemId { get; }
        public ObjectId MobileId { get; }
        public Layer Layer { get; }

        internal ItemWornEvent(ObjectId itemId, ObjectId mobileId, Layer layer)
        {
            ItemId = itemId;
            MobileId = mobileId;
            Layer = layer;
        }
    }
}
