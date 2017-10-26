namespace Infusion.LegacyApi.Events
{
    public struct CommandRequestedEvent
    {
        public string InvocationSyntax { get; }

        public CommandRequestedEvent(string invocationSyntax)
        {
            InvocationSyntax = invocationSyntax;
        }
    }
}