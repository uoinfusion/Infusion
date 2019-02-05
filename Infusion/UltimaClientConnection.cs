using System;
using System.IO;
using System.Linq;
using Infusion.Diagnostic;
using Infusion.IO;
using Infusion.Packets;
using PacketLogParser = Infusion.Parsers.PacketLogParser;

namespace Infusion
{
    internal sealed class UltimaClientConnection
    {
        private readonly IDiagnosticPullStream diagnosticPullStream;
        private readonly IDiagnosticPushStream diagnosticPushStream;
        private readonly Parsers.PacketLogParser packetLogParser;
        private LoginPullStream loginStream;
        private ClientNewGamePullStream receiveNewGameStream;
        private NewGameStream sendNewGameStream;
        private uint loginSeed;
        private byte[] receivedSeed = new byte[21];
        private int receivedPosition = 0;

        public UltimaClientConnection() : this(
            UltimaClientConnectionStatus.Initial, NullDiagnosticPullStream.Instance,
            NullDiagnosticPushStream.Instance, PacketDefinitionRegistryFactory.CreateClassicClient())
        {
        }

        public UltimaClientConnection(UltimaClientConnectionStatus status)
            : this(status, NullDiagnosticPullStream.Instance, NullDiagnosticPushStream.Instance, PacketDefinitionRegistryFactory.CreateClassicClient())
        {
        }

        public UltimaClientConnection(UltimaClientConnectionStatus status, IDiagnosticPullStream diagnosticPullStream,
            IDiagnosticPushStream diagnosticPushStream, PacketDefinitionRegistry packetRegistry)
        {
            this.diagnosticPullStream = diagnosticPullStream;
            this.diagnosticPushStream = diagnosticPushStream;
            Status = status;
            packetLogParser = new PacketLogParser(packetRegistry);
            loginStream = new LoginPullStream();
            loginStream.BaseStream = diagnosticPullStream;
            receiveNewGameStream = new ClientNewGamePullStream();
            receiveNewGameStream.BaseStream = diagnosticPullStream;
        }

        public UltimaClientConnectionStatus Status { get; private set; }

        public event EventHandler<Packet> PacketReceived;

        public void ReceiveBatch(IPullStream inputStream, int batchLength)
        {
            diagnosticPullStream.BaseStream = inputStream;
            receiveNewGameStream.BaseStream = diagnosticPullStream;
            // IPullStream currentStream = new StreamToPullStreamAdapter(receiveNewGameStream);
            IPullStream currentStream = diagnosticPullStream;

            switch (Status)
            {
                case UltimaClientConnectionStatus.Initial:
                    int processedBytes = ReceiveSeed(diagnosticPullStream, batchLength, UltimaClientConnectionStatus.AfterInitialSeed);
                    currentStream = loginStream;
                    DetectEncryption(diagnosticPullStream);
                    break;
                case UltimaClientConnectionStatus.AfterInitialSeed:
                    DetectEncryption(diagnosticPullStream);
                    currentStream = loginStream;
                    break;
                case UltimaClientConnectionStatus.ServerLogin:
                    currentStream = loginStream;
                    break;
                case UltimaClientConnectionStatus.PreGameLogin:
                    ReceiveSeed(diagnosticPullStream, batchLength, UltimaClientConnectionStatus.GameLogin);
                    currentStream = receiveNewGameStream;
                    break;
                case UltimaClientConnectionStatus.Game:
                    currentStream = receiveNewGameStream;
                    break;
            }

            foreach (var packet in packetLogParser.ParseBatch(currentStream))
            {
                OnPacketReceived(packet);

                switch (Status)
                {
                    case UltimaClientConnectionStatus.ServerLogin:
                        if (packet.Id == PacketDefinitions.SelectServerRequest.Id)
                            Status = UltimaClientConnectionStatus.PreGameLogin;
                        break;
                    case UltimaClientConnectionStatus.GameLogin:
                        if (packet.Id == PacketDefinitions.GameServerLoginRequest.Id)
                            Status = UltimaClientConnectionStatus.Game;
                        break;
                }
            }
        }

        private readonly LoginEncryptionDetector loginEncryptionDetector
            = new LoginEncryptionDetector();

        private void DetectEncryption(IDiagnosticPullStream inputStream)
        {
            if (!inputStream.DataAvailable)
                return;

            var result = loginEncryptionDetector.Detect(this.loginSeed, inputStream);
            loginStream = new LoginPullStream(this.loginSeed, result.Key);
            loginStream.BaseStream = diagnosticPullStream;

            var packet = packetLogParser.ParsePacket(result.DecryptedPacket);
            OnPacketReceived(packet);

            Status = UltimaClientConnectionStatus.ServerLogin;
        }

        private int ReceiveSeed(IPullStream inputStream, int batchLength, UltimaClientConnectionStatus nextStatus)
        {
            int byteReceived = 0;

            if (receivedPosition == 0)
            {
                var firstByte = inputStream.ReadByte();
                byteReceived++;
                receivedPosition++;
                receivedSeed[0] = (byte)firstByte;
                if (firstByte != 0xEF)
                {
                    var seed = new byte[4];
                    inputStream.Read(seed, 1, 3);
                    seed[0] = (byte)firstByte;
                    var packet = new Packet(PacketDefinitions.LoginSeed.Id, seed);
                    OnPacketReceived(packet);
                    Status = nextStatus;
                    receivedPosition = 0;
                    this.loginSeed = BitConverter.ToUInt32(seed.Reverse().ToArray(), 0);
                    receiveNewGameStream = new ClientNewGamePullStream(seed);
                    receiveNewGameStream.BaseStream = diagnosticPullStream;
                    sendNewGameStream = new NewGameStream(seed, true, true);
                    return 4;
                }
            }

            if (batchLength > byteReceived)
            {
                var remaining = 21 - receivedPosition;
                var len = batchLength - byteReceived;
                len = (len > remaining) ? remaining : len;
                inputStream.Read(receivedSeed, receivedPosition, len);
                receivedPosition += len;
                byteReceived += len;
            }

            if (receivedPosition >= 21)
            {
                var packet = new Packet(PacketDefinitions.LoginSeed.Id, receivedSeed);
                OnPacketReceived(packet);
                Status = nextStatus;
                receivedPosition = 0;
            }

            return byteReceived;
        }

        private void OnPacketReceived(Packet packet)
        {
            diagnosticPullStream.FinishPacket(packet);
            PacketReceived?.Invoke(this, packet);
        }

        public void Send(Packet packet, Stream outputStream)
        {
            diagnosticPushStream.DumpPacket(packet);

            switch (Status)
            {
                case UltimaClientConnectionStatus.Initial:
                case UltimaClientConnectionStatus.ServerLogin:
                case UltimaClientConnectionStatus.PreGameLogin:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);
                    diagnosticPushStream.Write(packet.Payload, 0, packet.Length);
                    break;
                case UltimaClientConnectionStatus.Game:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);
                    sendNewGameStream.BasePushStream = new PushStreamToStreamAdapter(diagnosticPushStream);
                    var huffmanStream = new HuffmanStream(sendNewGameStream);
                    huffmanStream.Write(packet.Payload, 0, packet.Length);

                    break;
                default:
                    throw new NotImplementedException($"Sending packets while in {Status} Status.");
            }

            diagnosticPushStream.Finish();
        }
    }
}