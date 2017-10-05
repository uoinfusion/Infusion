using System;
using System.IO;
using Infusion.IO;

namespace Infusion.Diagnostic
{
    internal sealed class InfusionDiagnosticPushStreamProvider : IDisposable
    {
        private readonly Configuration configuration;
        private BinaryDiagnosticPushStream outputStream;
        private readonly object providerLock = new object();

        public InfusionDiagnosticPushStreamProvider(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public ServerConnection ServerConnection { get; set; }
        public UltimaClientConnection ClientConnection { get; set; }

        public void Dispose()
        {
            outputStream?.Dispose();
        }

        public BinaryDiagnosticPushStream GetStream()
        {
            lock (providerLock)
            {
                // Packets may be interpreted differently
                if (ServerConnection == null || ClientConnection == null ||
                    ServerConnection.Status != ServerConnectionStatus.Game ||
                    ClientConnection.Status != UltimaClientConnectionStatus.Game)
                {
                    return null;
                }

                if (outputStream == null && string.IsNullOrEmpty(configuration.LogPath) &&
                    !configuration.LogPacketsToFileEnabled)
                {
                    return null;
                }
                if (outputStream != null && (string.IsNullOrEmpty(configuration.LogPath) ||
                                             !configuration.LogPacketsToFileEnabled))
                {
                    outputStream = null;
                    return null;
                }
                if (outputStream == null && !string.IsNullOrEmpty(configuration.LogPath) &&
                    configuration.LogPacketsToFileEnabled)
                {
                    var fileName = Path.Combine(configuration.LogPath,
                        $"{DateTime.UtcNow:yyyyMMdd-HH.mm.ss.ffff}.packets");
                    outputStream =
                        new BinaryDiagnosticPushStream(
                            new SynchronizedPushStream(new StreamToPushStreamAdapter(File.OpenWrite(fileName))));
                }

                return outputStream;
            }
        }
    }
}