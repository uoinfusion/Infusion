namespace Infusion.LegacyApi.Events
{
    public sealed class ContainerOpenedEvent : IEvent
    {
        internal ContainerOpenedEvent(ObjectId containerId)
        {
            ContainerId = containerId;
        }

        public ObjectId ContainerId { get; }
    }
}