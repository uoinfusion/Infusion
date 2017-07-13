using System;
using System.IO;
using Infusion.IO;

namespace Infusion.Diagnostic
{
    public sealed class InfusionDiagnosticPushStreamProvider : IDisposable
    {
        private readonly Configuration configuration;
        private BinaryDiagnosticPushStream outputStream;

        public InfusionDiagnosticPushStreamProvider(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void Dispose()
        {
            outputStream?.Dispose();
        }

        public BinaryDiagnosticPushStream GetStream()
        {
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
                var fileName = Path.Combine(configuration.LogPath, $"{DateTime.UtcNow:yyyyMMdd-HH.mm.ss.ffff}.packets");
                outputStream =
                    new BinaryDiagnosticPushStream(
                        new SynchronizedPushStream(new StreamToPushStreamAdapter(File.Create(fileName))));
            }

            return outputStream;
        }
    }
}