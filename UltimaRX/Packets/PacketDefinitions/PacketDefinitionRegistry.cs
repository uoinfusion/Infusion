using System;
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
            Register(new GameServerLoginRequestDefinition());
            Register(new ClientSpyDefinition());
            Register(new LoginCharacterDefinition());
            Register(new LoginSeedDefinition());
        }

        public static void Register(PacketDefinition definition)
        {
            Definitions.Add(definition.Id, definition);
        }

        public static PacketDefinition Find(int id)
        {
            PacketDefinition definition;

            if (!Definitions.TryGetValue(id, out definition))
            {
                throw new InvalidOperationException($"Unknown packet id {id:X2}.");
            }

            return definition;
        }

        public static bool TryFind(int packedId, out PacketDefinition packetDefinition)
        {
            return Definitions.TryGetValue(packedId, out packetDefinition);
        }

        public static T Materialize<T>(Packet rawPacket) where T : MaterializedPacket
        {
            PacketDefinition definition = Find(rawPacket.Id);
            return (T)definition.Materialize(rawPacket);
        }
    }
}