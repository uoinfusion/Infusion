namespace Infusion.Proxy.Logging
{
    public interface ILogger
    {
        void Info(string message);
        void Speech(SpeechMessage message);
        void Debug(string message);
        void Critical(string message);
        void Error(string message);
    }
}
