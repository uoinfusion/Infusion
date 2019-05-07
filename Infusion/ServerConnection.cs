using System;
using System.IO;
using System.Linq;
using Infusion.Diagnostic;
using Infusion.IO;
using Infusion.IO.Encryption.Login;
using Infusion.IO.Encryption.NewGame;
using Infusion.Packets;

namespace Infusion
{
    internal sealed class ServerConnection
    {
        private readonly IDiagnosticPullStream diagnosticPullStream;
        private readonly IDiagnosticPushStream diagnosticPushStream;
        private readonly PacketDefinitionRegistry packetRegistry;
        private readonly EncryptionSetup encryptionSetup;
        private readonly PullStreamToStreamAdapter preLoginStream;

        private HuffmanStream huffmanStream;
        private ServerNewGamePullStream receiveNewGameStream;
        private ServerNewGamePushStream sendNewGameStream;
        private LoginPushStream loginStream;

        public ServerConnectionStatus Status { get; private set; }

        public ServerConnection()
            : this(ServerConnectionStatus.PreLogin, NullDiagnosticPullStream.Instance, NullDiagnosticPushStream.Instance,
                  PacketDefinitionRegistryFactory.CreateClassicClient(), EncryptionSetup.Autodetect)
        {
        }

        public ServerConnection(ServerConnectionStatus status)
            : this(status, NullDiagnosticPullStream.Instance, NullDiagnosticPushStream.Instance,
                  PacketDefinitionRegistryFactory.CreateClassicClient(), EncryptionSetup.Autodetect)
        {
        }

        public ServerConnection(ServerConnectionStatus status, IDiagnosticPullStream diagnosticPullStream,
            IDiagnosticPushStream diagnosticPushStream)
            : this(status, diagnosticPullStream, diagnosticPushStream, PacketDefinitionRegistryFactory.CreateClassicClient(), EncryptionSetup.Autodetect)
        {
        }

        public ServerConnection(ServerConnectionStatus status, IDiagnosticPullStream diagnosticPullStream,
            IDiagnosticPushStream diagnosticPushStream, byte[] newGameKey, uint loginSeed, LoginEncryptionKey loginKey)
            : this(status, diagnosticPullStream, diagnosticPushStream, PacketDefinitionRegistryFactory.CreateClassicClient(), EncryptionSetup.Autodetect)
        {
            InitNewGameEncryption(newGameKey);
            this.loginStream = new LoginPushStream(loginSeed, loginKey);
        }

        public ServerConnection(ServerConnectionStatus status, IDiagnosticPullStream diagnosticPullStream,
            IDiagnosticPushStream diagnosticPushStream, PacketDefinitionRegistry packetRegistry, EncryptionSetup encryptionSetup)
        {
            this.Status = status;
            this.diagnosticPullStream = diagnosticPullStream;
            this.diagnosticPushStream = diagnosticPushStream;
            this.packetRegistry = packetRegistry;
            this.encryptionSetup = encryptionSetup;
            this.loginStream = new LoginPushStream();

            this.receiveNewGameStream = new ServerNewGamePullStream();
            this.sendNewGameStream = new ServerNewGamePushStream();

            huffmanStream = new HuffmanStream(new PullStreamToStreamAdapter(receiveNewGameStream));
            preLoginStream = new PullStreamToStreamAdapter(diagnosticPullStream);
        }

        public event EventHandler<Packet> PacketReceived;
        private readonly byte[] receiveBuffer = new byte[65535];

        public void Receive(IPullStream inputStream)
        {
            var processingStream = GetReceiveStream(inputStream);

            while (inputStream.DataAvailable)
            {
                var packetReader = new StreamPacketReader(processingStream, receiveBuffer);
                int packetId = packetReader.ReadByte();
                if ((packetId < 0) || (packetId > 255))
                    throw new EndOfStreamException();

                diagnosticPullStream.StartPacket();
                var packetDefinition = packetRegistry.Find(packetId);
                var packetSize = packetDefinition.GetSize(packetReader);
                packetReader.ReadBytes(packetSize - packetReader.Position);
                var payload = new byte[packetSize];
                Array.Copy(receiveBuffer, 0, payload, 0, packetSize);

                var packet = new Packet(packetId, payload);

                if (packetId == PacketDefinitions.ConnectToGameServer.Id)
                {
                    Status = ServerConnectionStatus.PreGame;
                }

                diagnosticPullStream.FinishPacket(packet);

                OnPacketReceived(packet);
            }
        }

        private void OnPacketReceived(Packet packet)
        {
            PacketReceived?.Invoke(this, packet);
        }

        private Stream GetReceiveStream(IPullStream inputStream)
        {
            diagnosticPullStream.BaseStream = inputStream;

            switch (Status)
            {
                case ServerConnectionStatus.PreLogin:
                    return preLoginStream;

                case ServerConnectionStatus.Game:
                    receiveNewGameStream.BaseStream = diagnosticPullStream;

                    return huffmanStream;

                default:
                    throw new NotImplementedException($"Unknown Status: {Status}");
            }
        }

        public void Send(Packet packet, Stream outputStream)
        {
            diagnosticPushStream.DumpPacket(packet);

            switch (Status)
            {
                case ServerConnectionStatus.Initial:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);
                    diagnosticPushStream.Write(packet.Payload, 0, packet.Length);
                    Status = ServerConnectionStatus.PreLogin;
                    break;
                case ServerConnectionStatus.PreLogin:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);
                    loginStream.BaseStream = diagnosticPushStream;
                    loginStream.Write(packet.Payload, 0, packet.Length);
                    break;
                case ServerConnectionStatus.PreGame:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);
                    diagnosticPushStream.Write(packet.Payload, 0, packet.Length);
                    Status = ServerConnectionStatus.Game;
                    break;
                case ServerConnectionStatus.Game:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);
                    sendNewGameStream.BaseStream = diagnosticPushStream;
                    sendNewGameStream.Write(packet.Payload, 0, packet.Length);
                    break;
            }

            diagnosticPushStream.Finish();
        }

        internal void InitNewGameEncryption(byte[] key)
        {
            if (encryptionSetup != EncryptionSetup.EncryptedClient)
            {
                receiveNewGameStream = new ServerNewGamePullStream(key);
                sendNewGameStream = new ServerNewGamePushStream(key);

                huffmanStream = new HuffmanStream(new PullStreamToStreamAdapter(receiveNewGameStream));
            }
        }

        internal void InitLoginEncryption(uint seed, LoginEncryptionKey key)
        {
            if (encryptionSetup != EncryptionSetup.EncryptedClient)
            {
                loginStream = new LoginPushStream(seed, key);
            }
        }
    }
}