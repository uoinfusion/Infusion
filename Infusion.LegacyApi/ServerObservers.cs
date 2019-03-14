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

        public event Action<ServerListItem> ServerSelected;

        public ServerObservers(IServerPacketSubject server, IClientPacketSubject client)
        {
            server.Subscribe(PacketDefinitions.ServerListing, HandleServerListing);
            server.Subscribe(PacketDefinitions.GameServerList, HandleGameServerList);
            client.Subscribe(PacketDefinitions.SelectServerRequest, HandleSelectServerRequest);
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
            var selectedServer = servers.First(s => s.Id == packet.ChosenServerId);
            ServerSelected?.Invoke(selectedServer);
        }
    }
}
