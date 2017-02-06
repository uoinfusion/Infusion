using UltimaRX.Nazghul.Common;
using UltimaRX.Proxy;
using UltimaRX.Proxy.InjectionApi;
using UltimaRX.Proxy.Logging;

namespace UltimaRX.Nazghul.Proxy
{
    internal sealed class NazghulLogger : ILogger
    {
        private readonly NazghulProxy nazghul;

        public NazghulLogger(NazghulProxy nazghul)
        {
            this.nazghul = nazghul;
        }

        public void Info(string message)
        {
            WriteLine(message);
        }

        public void Speech(SpeechMessage message)
        {
            var type = message.SpeakerId == 0 || message.SpeakerId == Injection.Me.PlayerId || message.IsName
                ? LogMessageType.Info
                : LogMessageType.Speech;

            nazghul.SendLog(new LogMessage
            {
                Message = message.Text,
                Type = type,
            });
        }

        public void Debug(string message)
        {
            WriteLine(message);
        }

        public void Critical(string message)
        {
            nazghul.SendLog(new LogMessage
            {
                Type = LogMessageType.Alert,
                Message = message
            });
        }

        public void Error(string message)
        {
            WriteLine(message);
        }

        public void WriteLine(string message)
        {
            nazghul.SendLog(message);
        }
    }
}