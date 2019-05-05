using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.Packets;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal sealed class ServerObservers
    {
        private ServerListItem[] servers = Array.Empty<ServerListItem>();
        private readonly PacketDefinitionRegistry packetRegistry;
        private readonly UltimaServer server;

        public ServerListItem SelectedServer { get; private set; }
        public string SelectedCharacterName { get; private set; }

        public ServerObservers(UltimaServer server, IClientPacketSubject client, PacketDefinitionRegistry packetRegistry)
        {
            server.Subscribe(PacketDefinitions.ServerListing, HandleServerListing);
            server.Subscribe(PacketDefinitions.GameServerList, HandleGameServerList);
            server.RegisterFilter(HandleRunUOProtocolExtension);
            client.Subscribe(PacketDefinitions.SelectServerRequest, HandleSelectServerRequest);
            client.Subscribe(PacketDefinitions.LoginCharacter, HandleSelectLoginCharacterRequest);
            this.packetRegistry = packetRegistry;
            this.server = server;
        }

        private Packet? HandleRunUOProtocolExtension(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.RunUOProtocolExtension.Id)
            {
                var packet = packetRegistry.Materialize<RunUOProtocolExtensionPacket>(rawPacket);
                if (packet.Type == 0xFE)
                {
                    server.AnswerRazorNegitiation(0xFF);
                    return null;
                }
            }

            return rawPacket;
        }

        private void HandleSelectLoginCharacterRequest(LoginCharacterRequest packet)
        {
            SelectedCharacterName = packet.CharacterName;
        }

        private void HandleServerListing(ServerListingPacket packet)
        {
            servers = packet.Servers;
        }

        private void HandleGameServerList(GameServerListPacket packet)
        {
            servers = packet.Servers;
        }

        private void HandleSelectServerRequest(SelectServerRequest packet)
        {
            SelectedServer = servers.First(s => s.Id == packet.ChosenServerId);
        }
    }
}
