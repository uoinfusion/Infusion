using System;
using System.Collections.Generic;
using UltimaRX.Packets.PacketDefinitions.Both;
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
            Register(new GeneralInformationPacketDefinition());
            Register(new DeleteObjectDefinition());
            Register(new CharacterLocaleAndBodyDefinition());
            Register(new LoginCompleteDefinition());
            Register(new ClientViewRangeDefinition());
            Register(new PlaySoundEffectDefinition());
            Register(new GetClientStatusDefinition());
            Register(new ClientVersionDefinition());
            Register(new RequestWarModeDefinition());
            Register(new DrawGamePlayerDefinition());
            Register(new DrawObjectDefinition());
            Register(new SendSpeechDefinition());
            Register(new TipNoticeWindowDefinition());
            Register(new SendSkillsDefinition());
            Register(new PingMessageDefinition());
            Register(new CharacterMoveAckDefinition());
            Register(new SpeechRequestDefinition());
            Register(new SpeechMessageDefinition());
            Register(new LoginDeniedDefinition());
            Register(new PauseClientDefinition());
            Register(new ObjectInfoDefinition());
            Register(new GraphicalEffectDefinition());
            Register(new RejectMoveItemRequestDefinition());
            Register(new KickPlayerDefinition());
            Register(new AddItemToContainerDefinition());
            Register(new DrawContainerDefinition());
            Register(new DraggingOfItemDefinition());
            Register(new CharMoveRejectionDefinition());
            Register(new ExplosionDefinition());
            Register(new HealthBarStatusUpdateDefinition());
            Register(new NewHealthBarStatusUpdateDefinition());
            Register(new StatusBarInfoDefinition());
            Register(new DamageDefinition());
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