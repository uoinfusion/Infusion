using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class ObjectDeletedEvent : IEvent
    {
        public ObjectId DeletedObjectId { get; }

        internal ObjectDeletedEvent(ObjectId deletedObjectId)
        {
            DeletedObjectId = deletedObjectId;
        }
    }
}
