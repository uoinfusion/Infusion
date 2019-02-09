using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Diagnostic
{
    public class BinaryPacketFileParser
    {
        private readonly PacketDefinitionRegistry packetRegistry;
        private readonly Parsers.PacketLogParser packetLogParser;

        public BinaryPacketFileParser()
            : this(PacketDefinitionRegistryFactory.CreateClassicClient())
        {
        }

        internal BinaryPacketFileParser(PacketDefinitionRegistry packetRegistry)
        {
            this.packetRegistry = packetRegistry;
            this.packetLogParser = new Parsers.PacketLogParser(packetRegistry);
        }

        public IEnumerable<PacketLogEntry> ParseFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var pullStream = new StreamToPullStreamAdapter(stream))
                    {
                        while (stream.Position < stream.Length)
                        {
                            var time = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
                            var binaryDirection = (DiagnosticStreamDirection)reader.ReadInt32();
                            PacketDirection packetDirection;

                            switch (binaryDirection)
                            {
                                case DiagnosticStreamDirection.ClientToServer:
                                    packetDirection = PacketDirection.ProxyServer;
                                    break;
                                case DiagnosticStreamDirection.ServerToClient:
                                    packetDirection = PacketDirection.ProxyClient;
                                    break;
                                default:
                                    throw new NotImplementedException($"Unknown binnary packet direction: {((int)binaryDirection):X8} at position {stream.Position}");
                            }

                            var packet = packetLogParser.ParsePacket(pullStream);
                            var definition = packetRegistry.Find(packet.Id);

                            yield return new PacketLogEntry(time, definition.Name, packetDirection, packet.Payload);
                        }
                    }
                }
            }
        }
    }
}
