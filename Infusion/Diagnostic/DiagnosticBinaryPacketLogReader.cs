using System;
using System.Collections.Generic;
using System.IO;
using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Diagnostic
{
    internal sealed class DiagnosticPacketReader : IDisposable
    {
        private readonly Stream inputStream;
        private readonly PacketDefinitionRegistry packetRegistry;
        private readonly BinaryReader inputStreamReader;
        private readonly IPullStream pullStream;
        private readonly Parsers.PacketLogParser packetLogParser;

        public DiagnosticPacketReader(string fileName, PacketDefinitionRegistry packetRegistry)
            : this(File.OpenRead(fileName), packetRegistry)
        {
        }

        public DiagnosticPacketReader(Stream inputStream, PacketDefinitionRegistry packetRegistry)
        {
            this.inputStream = inputStream;
            this.packetRegistry = packetRegistry;
            inputStreamReader = new BinaryReader(inputStream);
            pullStream = new StreamToPullStreamAdapter(this.inputStream);
            packetLogParser = new Parsers.PacketLogParser(packetRegistry);
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

                var packet = packetLogParser.ParsePacket(pullStream);
                var name = packetRegistry.Find(packet.Id).Name;

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