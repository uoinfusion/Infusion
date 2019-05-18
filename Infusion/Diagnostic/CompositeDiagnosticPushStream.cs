using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Diagnostic
{
    internal sealed class CompositeDiagnosticPushStream : IDiagnosticPushStream
    {
        private readonly IDiagnosticPushStream[] diagnosticStreams;
        public IPushStream BaseStream { get; set; }

        public CompositeDiagnosticPushStream(params IDiagnosticPushStream[] diagnosticStreams)
        {
            this.diagnosticStreams = diagnosticStreams;
        }

        public void DumpPacket(Packet packet)
        {
            foreach (var stream in diagnosticStreams)
            {
                stream.DumpPacket(packet);
            }
        }

        public void Finish()
        {
            foreach (var stream in diagnosticStreams)
            {
                stream.Finish();
            }
        }

        public void Dispose()
        {
            BaseStream.Dispose();
            foreach (var stream in diagnosticStreams)
            {
                stream.Dispose();
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
            foreach (var stream in diagnosticStreams)
            {
                stream.Write(buffer, offset, count);
            }
        }

        public void WriteByte(byte value)
        {
            BaseStream.WriteByte(value);
            foreach (var stream in diagnosticStreams)
            {
                stream.WriteByte(value);
            }
        }

        public void Flush()
        {
            BaseStream.Flush();
            foreach (var stream in diagnosticStreams)
            {
                stream.Dispose();
            }
        }
    }
}
