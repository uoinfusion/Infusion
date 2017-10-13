namespace Infusion.LegacyApi.Events
{
    public struct SpeechRequestedEvent
    {
        public string Message { get; }

        public SpeechRequestedEvent(string message)
        {
            Message = message;
        }
    }
}