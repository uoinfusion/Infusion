using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.IO;
using UltimaRX.Packets;
using UltimaRX.Packets.PacketDefinitions;

namespace UltimaRX
{
    public class ServerConnection
    {
        private UltimaClientConnectionStatus status = UltimaClientConnectionStatus.BeforeInitialSeed;
        private readonly HuffmanStream huffmanStream;
        private readonly NewGameStream newGameStream = new NewGameStream(null, new byte[] {127,0,0,1});

        public ServerConnection()
        {
            huffmanStream = new HuffmanStream(newGameStream);
        }

        public event EventHandler<Packet> PacketReceived;

        public void Receive(IPullStream inputStream)
        {
            newGameStream.BaseStream = inputStream;
            var received = new byte[1024];

            while (inputStream.DataAvailable)
            {
                var packetReader = new StreamPacketReader(huffmanStream, received);
                int packetId = packetReader.ReadByte();
                if (packetId < 0 || packetId > 255)
                {
                    throw new EndOfStreamException();
                }

                var packetDefinition = PacketDefinitionRegistry.Find(packetId);
                int packetSize = packetDefinition.GetSize(packetReader);
                packetReader.ReadBytes(packetSize - packetReader.Position);
                var payload = new byte[packetSize];
                Array.Copy(received, 0, payload, 0, packetSize);

                PacketReceived?.Invoke(this, new Packet(packetId, payload));
            }
        }
    }

    public class StreamPacketReader : IPacketReader
    {
        private readonly Stream sourceStream;
        private readonly byte[] targetBuffer;

        public int Position { get; private set; }

        public StreamPacketReader(Stream sourceStream, byte[] targetBuffer)
        {
            this.sourceStream = sourceStream;
            this.targetBuffer = targetBuffer;
        }

        public byte ReadByte()
        {
            int result = sourceStream.ReadByte();

            if (result < byte.MinValue || result > byte.MaxValue)
            {
                throw new EndOfStreamException();
            }

            byte resultByte = (byte) result;
            targetBuffer[Position++] = resultByte;

            return resultByte;
        }

        public ushort ReadUShort()
        {
            return (ushort)((ReadByte() << 8) + ReadByte());
        }

        public void ReadBytes(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ReadByte();
            }
        }
    }
}
