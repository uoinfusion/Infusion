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

        public ServerListItem SelectedServer { get; private set; }
        public string SelectedCharacterName { get; private set; }

        public ServerObservers(IServerPacketSubject server, IClientPacketSubject client)
        {
            server.Subscribe(PacketDefinitions.ServerListing, HandleServerListing);
            server.Subscribe(PacketDefinitions.GameServerList, HandleGameServerList);
            client.Subscribe(PacketDefinitions.SelectServerRequest, HandleSelectServerRequest);
            client.Subscribe(PacketDefinitions.LoginCharacter, HandleSelectLoginCharacterRequest);
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
