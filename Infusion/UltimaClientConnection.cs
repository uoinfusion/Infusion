using System;
using System.IO;
using System.Linq;
using Infusion.Diagnostic;
using Infusion.IO;
using Infusion.IO.Encryption.Login;
using Infusion.IO.Encryption.NewGame;
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
        private ClientNewGamePushStream sendNewGameStream;
        private uint loginSeed;
        private byte[] receivedSeed = new byte[21];
        private int receivedPosition = 0;
        private bool requiresEncryption = false;

        public event Action<byte[]> NewGameEncryptionStarted;
        public event Action<uint, LoginEncryptionKey> LoginEncryptionStarted;

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
            sendNewGameStream = new ClientNewGamePushStream();
        }

        public UltimaClientConnectionStatus Status { get; private set; }

        public event EventHandler<Packet> PacketReceived;

        public void ReceiveBatch(IPullStream inputStream, int batchLength)
        {
            diagnosticPullStream.BaseStream = inputStream;
            IPullStream currentStream = diagnosticPullStream;

            switch (Status)
            {
                case UltimaClientConnectionStatus.Initial:
                    ReceiveSeed(diagnosticPullStream, batchLength, UltimaClientConnectionStatus.AfterInitialSeed);
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
                case UltimaClientConnectionStatus.GameLogin:
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
            if (result.IsEncrypted)
            {
                requiresEncryption = true;
                loginStream = new LoginPullStream(this.loginSeed, result.Key);
                loginStream.BaseStream = diagnosticPullStream;
                LoginEncryptionStarted?.Invoke(loginSeed, result.Key.Value);
            }

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
                    if (requiresEncryption)
                    {
                        receiveNewGameStream = new ClientNewGamePullStream(seed);
                        receiveNewGameStream.BaseStream = diagnosticPullStream;
                        sendNewGameStream = new ClientNewGamePushStream(seed);
                        NewGameEncryptionStarted?.Invoke(seed);
                    }
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
                    sendNewGameStream.BaseStream = diagnosticPushStream;
                    var huffmanStream = new HuffmanStream(new PushStreamToStreamAdapter(sendNewGameStream));
                    huffmanStream.Write(packet.Payload, 0, packet.Length);

                    break;
                default:
                    throw new NotImplementedException($"Sending packets while in {Status} Status.");
            }

            diagnosticPushStream.Finish();
        }
    }
}