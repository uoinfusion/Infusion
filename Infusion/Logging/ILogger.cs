namespace Infusion.Logging
{
    public interface ILogger
    {
        void Info(string message);
        void Important(string message);
        void Debug(string message);
        void Critical(string message);
        void Error(string message);
    }
}
