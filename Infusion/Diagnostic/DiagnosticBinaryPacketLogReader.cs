using System;
using System.Collections.Generic;
using System.IO;
using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Diagnostic
{
    public class DiagnosticPacketReader : IDisposable
    {
        private readonly Stream inputStream;
        private readonly BinaryReader inputStreamReader;
        private readonly IPullStream pullStream;

        public DiagnosticPacketReader(string fileName)
            : this(File.OpenRead(fileName))
        {
        }

        public DiagnosticPacketReader(Stream inputStream)
        {
            this.inputStream = inputStream;
            inputStreamReader = new BinaryReader(inputStream);
            pullStream = new StreamToPullStreamAdaptet(this.inputStream);
        }

        public void Dispose()
        {
            inputStreamReader?.Dispose();
        }

        public PacketLogEntry ReadNextLogEntry()
        {
            try
            {
                var ticks = inputStreamReader.ReadInt64();
                var directionCode = (DiagnosticStreamDirection)inputStreamReader.ReadUInt32();
                var direction = directionCode == DiagnosticStreamDirection.ClientToServer
                    ? PacketDirection.ProxyServer
                    : directionCode == DiagnosticStreamDirection.ServerToClient
                        ? PacketDirection.ProxyClient
                        : throw new InvalidOperationException();

                var packet = Parsers.PacketLogParser.ParsePacket(pullStream);
                var name = PacketDefinitionRegistry.Find(packet.Id).Name;

                return new PacketLogEntry(new DateTime(ticks), name, direction, packet.Payload);
            }
            catch (EndOfStreamException)
            {
                return null;
            }
        }

        public IEnumerable<PacketLogEntry> ReadAll()
        {
            PacketLogEntry entry;

            while ((entry = ReadNextLogEntry()) != null)
                yield return entry;
        }
    }
}