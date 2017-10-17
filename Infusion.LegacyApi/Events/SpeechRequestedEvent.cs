namespace Infusion.LegacyApi.Events
{
    public sealed class SpeechRequestedEvent : IEvent
    {
        public string Message { get; }

        internal SpeechRequestedEvent(string message)
        {
            Message = message;
        }
    }
}