namespace Infusion.LegacyApi
{
    public struct SpeechRequestedArgs
    {
        public string Message { get; }

        public SpeechRequestedArgs(string message)
        {
            Message = message;
        }
    }
}