using System;
using System.IO;
using Infusion.IO;
using Infusion.Logging;
using Infusion.Utilities;

namespace Infusion.Diagnostic
{
    internal sealed class InfusionDiagnosticPushStreamProvider : IDisposable
    {
        private readonly IDiagnosticConfiguration configuration;
        private readonly ILogger logger;
        private BinaryDiagnosticPushStream outputStream;
        private readonly object providerLock = new object();

        public InfusionDiagnosticPushStreamProvider(IDiagnosticConfiguration configuration, ILogger logger)
        {
            this.configuration = configuration;
            this.logger = logger;

            loggingBreaker = new CircuitBreaker(HandleLoggingException);
        }

        public ServerConnection ServerConnection { get; set; }
        public UltimaClientConnection ClientConnection { get; set; }

        public void Dispose()
        {
            outputStream?.Dispose();
        }

        private readonly CircuitBreaker loggingBreaker;

        private void HandleLoggingException(Exception ex)
        {
            logger.Error($"Error while logging packets to disk. Please, check that Infusion can write to {configuration.LogPath}.");
            logger.Important("You can change the log path by setting UO.Configuration.LogPath property or disable packet logging by setting UO.Configuration.LogPacketsToFileEnabled = false in your initial script.");
            logger.Debug(ex.ToString());
        }


        public BinaryDiagnosticPushStream GetStream()
        {
            BinaryDiagnosticPushStream result = null;

            loggingBreaker.Protect(() =>
            {
                lock (providerLock)
                {
                    // Packets may be interpreted differently
                    if (ServerConnection == null || ClientConnection == null ||
                        ServerConnection.Status != ServerConnectionStatus.Game ||
                        ClientConnection.Status != UltimaClientConnectionStatus.Game)
                    {
                        result = null;
                        return;
                    }

                    if (outputStream == null && string.IsNullOrEmpty(configuration.LogPath) &&
                        !configuration.LogPacketsToFileEnabled)
                    {
                        result = null;
                        return;
                    }
                    if (outputStream != null && (string.IsNullOrEmpty(configuration.LogPath) ||
                                                 !configuration.LogPacketsToFileEnabled))
                    {
                        outputStream = null;
                        result = null;
                        return;
                    }
                    if (outputStream == null && !string.IsNullOrEmpty(configuration.LogPath) &&
                        configuration.LogPacketsToFileEnabled)
                    {
                        var fileName = Path.Combine(configuration.LogPath,
                            $"{DateTime.UtcNow:yyyyMMdd-HH.mm.ss.ffff}.packets");
                        outputStream =
                            new BinaryDiagnosticPushStream(
                                new SynchronizedPushStream(new StreamToPushStreamAdapter(new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))), loggingBreaker);
                    }

                    result = outputStream;
                }

            });

            return result;
        }
    }
}