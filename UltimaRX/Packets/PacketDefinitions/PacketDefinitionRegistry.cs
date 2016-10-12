using System.Collections.Generic;
using UltimaRX.Packets.PacketDefinitions.Client;
using UltimaRX.Packets.PacketDefinitions.Server;

namespace UltimaRX.Packets.PacketDefinitions
{
    public static class PacketDefinitionRegistry
    {
        private static readonly Dictionary<int, PacketDefinition> Definitions = new Dictionary<int, PacketDefinition>();

        static PacketDefinitionRegistry()
        {
            Register(new AttackRequestDefinition());
            Register(new CreateCharacterRequestDefinition());
            Register(new DisconnectNotificationDefinition());
            Register(new GodModeRequestDefinition());
            Register(new LoginRequestDefinition());
            Register(new MoveRequestDefinition());
            Register(new SelectServerRequestDefinition());
            Register(new TalkRequestDefinition());
            Register(new GameServerListDefinition());
            Register(new ConnectToGameServerDefinition());
            Register(new EnableLockedClientFeaturesDefinition());
            Register(new CharactersStartingLocationsDefinition());
        }

        public static void Register(PacketDefinition definition)
        {
            Definitions.Add(definition.Id, definition);
        }

        public static PacketDefinition Find(int id)
        {
            return Definitions[id];
        }

        public static bool TryFind(int packedId, out PacketDefinition packetDefinition)
        {
            return Definitions.TryGetValue(packedId, out packetDefinition);
        }
    }
}