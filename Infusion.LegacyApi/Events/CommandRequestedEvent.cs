namespace Infusion.LegacyApi.Events
{
    public sealed class CommandRequestedEvent : IEvent
    {
        internal CommandRequestedEvent(string invocationSyntax)
        {
            InvocationSyntax = invocationSyntax;
        }

        public string InvocationSyntax { get; }
    }
}