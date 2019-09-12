using Infusion.Diagnostic;
using Infusion.IO;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Infusion.Logging;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Proxy
{
    public class HeadlessClient
    {
        private class ReloginInfo
        {
            public byte ServerListSystemFlag { get; set; }
            public ServerListItem SelectedServer { get; set; }
        }

        private readonly Legacy legacyApi;
        private readonly IConsole console;
        private readonly HeadlessStartConfig startConfig;
        private readonly IDiagnosticPushStream serverDiagnosticPushStream;
        private readonly InfusionDiagnosticPushStreamProvider diagnosticProvider;
        private readonly ConsoleDiagnosticPullStream serverDiagnosticPullStream;
        private readonly RingBufferLogger packetLogger = new RingBufferLogger(10000);
        private readonly PacketDefinitionRegistry packetRegistry;
        private readonly ServerPacketHandler serverPacketHandler;
        private readonly UltimaServer ultimaServer;
        private readonly ClientPacketHandler clientPacketHandler;
        private readonly UltimaClient ultimaClient;
        private IPEndPoint serverEndpoint;
        private CancellationTokenSource disconnectTokenSource;

        private readonly ReloginInfo reloginInfo = new ReloginInfo();

        private Task pingLoopTask;

        private Socket serverSocket;
        private readonly ServerConnection serverConnection;
        private Task serverLoopTask;
        private NetworkStream serverStream;
        private readonly object serverStreamLock = new object();

        private UltimaClientConnection clientConnection;
        private Task clientLoopTask;
        private NetworkStream clientStream;
        private readonly object clientLock = new object();
        private bool transmitClientPackets = false;

        public LogConfiguration LogConfig { get; } = new LogConfiguration();

        public HeadlessClient(Legacy legacyApi, IConsole console, HeadlessStartConfig startConfig)
        {
            this.legacyApi = legacyApi;
            this.console = console;
            this.startConfig = startConfig;
            packetRegistry = PacketDefinitionRegistryFactory.CreateClassicClient(startConfig.ProtocolVersion);
            diagnosticProvider = new InfusionDiagnosticPushStreamProvider(LogConfig, console);
            serverDiagnosticPushStream =
                new CompositeDiagnosticPushStream(new ConsoleDiagnosticPushStream(packetLogger, "headless -> server", packetRegistry),
                    new InfusionBinaryDiagnosticPushStream(DiagnosticStreamDirection.ClientToServer, diagnosticProvider.GetStream));
            serverDiagnosticPullStream = new ConsoleDiagnosticPullStream(packetLogger, "server -> headless", packetRegistry);

            serverConnection = new ServerConnection(ServerConnectionStatus.Initial, serverDiagnosticPullStream,
                serverDiagnosticPushStream, packetRegistry, startConfig.Encryption);
            serverConnection.PacketReceived += ServerConnectionOnPacketReceived;
            ultimaServer = new UltimaServer(serverPacketHandler, SendToServer, packetRegistry);

            serverPacketHandler = new ServerPacketHandler(packetRegistry);
            clientPacketHandler = new ClientPacketHandler(packetRegistry);
            serverEndpoint = startConfig.ServerEndPoint;

            ultimaClient = new UltimaClient(clientPacketHandler, SendToClient);

            serverPacketHandler.Subscribe(PacketDefinitions.GameServerList, HandleGameServerList);
            serverPacketHandler.Subscribe(PacketDefinitions.CharactersStartingLocations, HandleCharactersStartingLocationsPacket);

            clientPacketHandler.Subscribe(PacketDefinitions.LoginRequest, HandleLoginRequest);
            clientPacketHandler.Subscribe(PacketDefinitions.GameServerLoginRequest, HandleGameServerLoginRequest);
            clientPacketHandler.Subscribe(PacketDefinitions.SelectServerRequest, HandleSelectServerRequest);
        }

        private void HandleGameServerLoginRequest(GameServerLoginRequest packet)
        {
            if (packet.AccountName.Equals(startConfig.AccountName) && packet.Password.Equals(this.startConfig.Password))
            {
                clientConnection.Status = UltimaClientConnectionStatus.Game;

                Task.Run(() =>
                {
                    var loginConfirmPacket = new CharLocaleAndBodyPacket()
                    {
                        PlayerId = legacyApi.Me.PlayerId,
                        BodyType = legacyApi.Me.BodyType,
                        Direction = legacyApi.Me.Direction,
                        MovementType = legacyApi.Me.MovementType,
                        Location = legacyApi.Me.Location,
                    };

                    SendToClient(new Packet(new byte[] { 0xB9, 0x80, 0x1F }));

                    SendToClient(loginConfirmPacket.Serialize());
                    SendToClient(new Packet(new byte[] { 0xB9, 0x00, 0x03, }));

                    SendToClient(new Packet(new byte[] { 0x55 }));

                    var drawPlayer = new DrawGamePlayerPacket(legacyApi.Me.PlayerId, legacyApi.Me.BodyType, legacyApi.Me.Location,
                        legacyApi.Me.Direction, legacyApi.Me.MovementType, legacyApi.Me.Color);
                    SendToClient(drawPlayer.Serialize());

                    foreach (var obj in UO.Items.OnGround())
                    {
                        var objectInfo = new ObjectInfoPacket(obj.Id, obj.Type, obj.Location, obj.Color, obj.Amount);
                        SendToClient(objectInfo.RawPacket);
                    }

                    SendToClient(new Packet(new byte[] { 0xBF, 0x00, 0x0D, 0x00, 0x05, 0x00, 0x00, 0x02, 0x80, 0x01, 0xFF, 0xFF, 0xA7, }));

                    transmitClientPackets = true;
                });
            }
            else
            {
                throw new NotImplementedException("account/password mismatch");
            }
        }

            private void HandleSelectServerRequest(SelectServerRequest packet)
        {
            if (packet.ChosenServerId == 0)
            {
                var connectPacket = new ConnectToGameServerPacket();
                connectPacket.GameServerIp = new byte[] { 127, 0, 0, 1 };
                connectPacket.Seed = connectPacket.GameServerIp;
                connectPacket.GameServerPort = 30000;

                SendToClient(connectPacket.Serialize());
            }
            else
            {
                throw new NotImplementedException($"Unexpected server id {packet.ChosenServerId}, it must be always 0");
            }
        }

        private void HandleLoginRequest(LoginRequest packet)
        {
            if (packet.Account.Equals(startConfig.AccountName) && packet.Password.Equals(this.startConfig.Password))
            {
                var serverListPacket = new GameServerListPacket();
                serverListPacket.Servers = new[]
                {
                    new ServerListItem(0, reloginInfo.SelectedServer.Name, reloginInfo.SelectedServer.FullPercent, reloginInfo.SelectedServer.TimeZone, 0x7F000001)
                };
                serverListPacket.SystemInfoFlag = reloginInfo.ServerListSystemFlag;

                SendToClient(serverListPacket.Serialize());
            }
            else
            {
                throw new NotImplementedException("account/password mismatch");
            }
        }

        private void HandleCharactersStartingLocationsPacket(CharactersStartingLocationsPacket packet)
        {
            var character = packet.Characters.Select((x, i) => new { x.Name, Index = i })
                .FirstOrDefault(x => x.Name.Equals(startConfig.CharacterName, StringComparison.OrdinalIgnoreCase));
            if (character == null)
                throw new InvalidOperationException($"Character {startConfig.CharacterName} not found.");

            var loginCharacterRequest = packetRegistry.Instantiate<LoginCharacterRequest>();
            loginCharacterRequest.ClientIp = new byte[] { 0x01, 0x89, 0xA8, 0xC0 };
            loginCharacterRequest.CharacterName = startConfig.CharacterName;
            loginCharacterRequest.Flags = (ClientFlags)7;
            loginCharacterRequest.SlotChosen = (uint)character.Index;

            SendToServer(loginCharacterRequest.Serialize());
        }

        private void HandleGameServerList(GameServerListPacket packet)
        {
            var server = packet.Servers.FirstOrDefault(x => x.Name.Equals(startConfig.ShardName, StringComparison.OrdinalIgnoreCase));
            if (server == null)
                throw new InvalidOperationException($"Cannot find shard {startConfig.ShardName}.");

            reloginInfo.SelectedServer = server;
            reloginInfo.ServerListSystemFlag = packet.SystemInfoFlag;

            var selectServerRequest = packetRegistry.Instantiate<SelectServerRequest>();
            selectServerRequest.ChosenServerId = server.Id;
            SendToServer(selectServerRequest.Serialize());
        }

        private void HandleConnectToGameServer(ConnectToGameServerPacket packet)
        {
            lock (serverStreamLock)
            {
                DisconnectFromServer();
                Thread.Sleep(1000);
                serverEndpoint = new IPEndPoint(new IPAddress(packet.GameServerIp), packet.GameServerPort);
                serverStream = ConnectToServer();
            }

            SendToServer(new Packet(PacketDefinitions.LoginSeed.Id, packet.GameServerIp));

            var loginRequest = new GameServerLoginRequest
            {
                Key = packet.GameServerIp,
                AccountName = startConfig.AccountName,
                Password = startConfig.Password
            };
            SendToServer(loginRequest.Serialize());
        }

        private void ServerConnectionOnPacketReceived(object sender, Packet rawPacket)
        {
            try
            {
                var handledPacket = serverPacketHandler.HandlePacket(rawPacket);

                if (transmitClientPackets && handledPacket.HasValue)
                {
                    SendToClient(handledPacket.Value);
                }
            }
            catch (PacketMaterializationException ex)
            {
                // just log exception and continue, do not interrupt proxy
                console.Error(ex.ToString());
            }
            catch (Exception ex)
            {
                // just log exception and continue, do not interrupt proxy
                console.Debug(ex.ToString());
            }
        }

        public void Connect()
        {
            lock (serverStreamLock)
            {
                serverStream = ConnectToServer();
            }

            disconnectTokenSource = new CancellationTokenSource();
            serverLoopTask = Task.Run(() => ServerLoop(), disconnectTokenSource.Token);
            clientLoopTask = Task.Run(() => ClientLoop(), disconnectTokenSource.Token);
            pingLoopTask = Task.Run(() => PingLoop(), disconnectTokenSource.Token);

            SendPreLoginSeed();
            SendFirstLogin();
        }

        public void Disconnect()
        {
            disconnectTokenSource.Cancel();
            if (!Task.WaitAll(new Task[] { pingLoopTask, serverLoopTask }, 5000))
                console.Error("Disconnect timeout.");
        }

        private void SendPreLoginSeed()
        {
            SendToServer(new Packet(PacketDefinitions.LoginSeed.Id, new byte[] { 0x01, 0x89, 0xA8, 0xC0 }));
        }

        private void SendFirstLogin()
        {
            SendToServer(new Packet(PacketDefinitions.LoginRequest.Id, new byte[]
            {
                0x80, 0x69, 0x76, 0x61, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70,
                0x61, 0x73, 0x73, 0x77, 0x6F, 0x72, 0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x99 }));
        }

        private void PingLoop()
        {
            try
            {
                while (true)
                {
                    Task.Delay(55000, disconnectTokenSource.Token).Wait();
                    if (disconnectTokenSource.IsCancellationRequested)
                        break;
                    SendToServer(new PingPacket().Serialize());
                }
            }
            catch (AggregateException ex) when (ex.InnerExceptions.All(x => x is TaskCanceledException))
            {
                // just ignore it
            }
        }

        private void SendToClient(Packet rawPacket)
        {
            if (clientStream == null)
                return;

            var filteredPacket = serverPacketHandler.FilterOutput(rawPacket);
            if (filteredPacket.HasValue)
            {
                lock (clientLock)
                {
                    using (var memoryStream = new MemoryStream(1024))
                    {
                        clientConnection.Send(filteredPacket.Value, memoryStream);
                        var buffer = memoryStream.GetBuffer();

#if DUMP_RAW
                        Console.Info("proxy -> client");
                        Console.Info(buffer.Take((int)memoryStream.Length).Select(x => x.ToString("X2")).Aggregate((l, r) => l + " " + r));
#endif

                        clientStream.Write(buffer, 0, (int)memoryStream.Length);
                    }
                }
            }
        }

        private void ClientLoop()
        {
            clientConnection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial,
                new ConsoleDiagnosticPullStream(packetLogger, "client -> proxy", packetRegistry),
                new CompositeDiagnosticPushStream(new ConsoleDiagnosticPushStream(packetLogger, "proxy -> client", packetRegistry),
                    new InfusionBinaryDiagnosticPushStream(DiagnosticStreamDirection.ServerToClient, diagnosticProvider.GetStream)), packetRegistry,
                    EncryptionSetup.Autodetect, null);
            clientConnection.PacketReceived += ClientConnectionOnPacketReceived;

            var listener = new TcpListener(new IPEndPoint(IPAddress.Any, 30000));
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                lock (clientLock)
                {
                    clientStream = client.GetStream();
                }

                int receivedLength;
                var receiveBuffer = new byte[65535];

                while ((receivedLength = clientStream.Read(receiveBuffer, 0, receiveBuffer.Length)) > 0)
                {
#if DUMP_RAW
                        Console.Info("client -> proxy");
                        Console.Info(receiveBuffer.Take(receivedLength).Select(x => x.ToString("X2")).Aggregate((l, r) => l + " " + r));
#endif

                    var memoryStream = new MemoryStream(receiveBuffer, 0, receivedLength, false);
                    clientConnection.ReceiveBatch(new MemoryStreamToPullStreamAdapter(memoryStream), receivedLength);

                    if (disconnectTokenSource.Token.IsCancellationRequested)
                        return;

                    Thread.Yield();
                }
                client.Dispose();
                clientStream.Dispose();
            }
        }

        private void ClientConnectionOnPacketReceived(object sender, Packet rawPacket)
        {
            try
            {
                var handledPacket = clientPacketHandler.HandlePacket(rawPacket);

                if (transmitClientPackets && handledPacket != null)
                    SendToServer(handledPacket.Value);
            }
            catch (Exception ex)
            {
                console.Error(ex.ToString());
            }
        }

        private void ServerLoop()
        {
            try
            {
                console.Info($"Headless connecting to {startConfig.ServerAddress}");

                while (true)
                {
                    lock (serverStreamLock)
                    {
                        if (serverStream == null)
                            serverStream = ConnectToServer();
                    }

                    while (serverStream.DataAvailable)
                    {
                        try
                        {
                            var packet = serverConnection.Receive(new NetworkStreamToPullStreamAdapter(serverStream));
                            if (packet.Id == PacketDefinitions.ConnectToGameServer.Id)
                            {
                                HandleConnectToGameServer(packetRegistry.Materialize<ConnectToGameServerPacket>(packet));
                            }
                            else
                                ServerConnectionOnPacketReceived(this, packet);
                        }
                        catch (EndOfStreamException ex)
                        {
                            console.Debug(ex.ToString());
                            // just swallow this exception, wait for the next batch
                        }
                    }
                    if (disconnectTokenSource.Token.IsCancellationRequested)
                        return;
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                console.Debug(serverDiagnosticPullStream.Flush());
                console.Debug(ex.ToString());
                throw;
            }
        }

        private NetworkStream ConnectToServer()
        {
            serverSocket = new Socket(serverEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(serverEndpoint);

            return new NetworkStream(serverSocket);
        }

        private void DisconnectFromServer()
        {
            serverStream.Dispose();
            serverStream = null;
            serverSocket.Dispose();
            serverSocket = null;
        }

        public void SendToServer(Packet rawPacket)
        {
            if (serverConnection == null)
                return;

            var filteredPacket = clientPacketHandler.FilterOutput(rawPacket);
            if (filteredPacket.HasValue)
            {
                lock (serverStreamLock)
                {
                    using (var memoryStream = new MemoryStream(1024))
                    {
                        serverConnection.Send(filteredPacket.Value, memoryStream);
                        var buffer = memoryStream.GetBuffer();

#if DUMP_RAW
                        console.Info("headless -> server");
                        console.Info(buffer.Take((int)memoryStream.Length).Select(x => x.ToString("X2")).Aggregate((l, r) => l + " " + r));
#endif

                        serverStream.Write(buffer, 0, (int)memoryStream.Length);
                    }
                }
            }
        }
    }
}
