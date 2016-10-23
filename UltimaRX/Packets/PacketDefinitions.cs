using UltimaRX.Packets.Server;

namespace UltimaRX.Packets
{
    public static class PacketDefinitions
    {
        #region Client

        public static readonly PacketDefinition LoginSeed = new PacketDefinition(-1,
            new StaticPacketLength(4), "LoginSeed");

        public static readonly PacketDefinition CreateCharacterRequest = new PacketDefinition(0x00,
            new StaticPacketLength(104), "CreateCharacterRequest");

        public static readonly PacketDefinition DisconnectNotification = new PacketDefinition(0x01,
            new StaticPacketLength(5), "DisconnectNotification");

        public static readonly PacketDefinition MoveRequest = new PacketDefinition(0x02,
            new StaticPacketLength(7), "MoveRequest");

        public static readonly PacketDefinition TalkRequest = new PacketDefinition(0x03,
            new StandardVariablePacketLength(), "TalkRequest");

        public static readonly PacketDefinition GodModeRequest = new PacketDefinition(0x04,
            new StaticPacketLength(2), "GodModeRequest");

        public static readonly PacketDefinition AttackRequest = new PacketDefinition(0x05,
            new StaticPacketLength(5), "AttackRequest");

        public static readonly PacketDefinition GetClientStatus = new PacketDefinition(0x34,
            new StaticPacketLength(10), "GetClientStatus");

        public static readonly PacketDefinition LoginCharacter = new PacketDefinition(0x5D,
            new StaticPacketLength(73), "LoginCharacter");

        public static readonly PacketDefinition LoginRequest = new PacketDefinition(0x80,
            new StaticPacketLength(62), "LoginRequest");

        public static readonly PacketDefinition GameServerLoginRequest = new PacketDefinition(0x91,
            new StaticPacketLength(65), "GameServerLoginRequest");

        public static readonly PacketDefinition SelectServerRequest = new PacketDefinition(0xA0,
            new StaticPacketLength(3), "SelectServerRequest");

        public static readonly PacketDefinition ClientSpy = new PacketDefinition(0xA4,
            new StaticPacketLength(149), "ClientSpy");

        public static readonly PacketDefinition SpeechRequest = new PacketDefinition(0xAD,
            new StandardVariablePacketLength(), "SpeechRequest");

        #endregion

        #region Server

        public static readonly PacketDefinition Damage = new PacketDefinition(0x0B,
            new StandardVariablePacketLength(), "Damage");

        public static readonly PacketDefinition StatusBarInfo = new PacketDefinition(0x11,
            new StandardVariablePacketLength(), "StatusBarInfo");

        public static readonly PacketDefinition NewHealthBarStatusUpdate = new PacketDefinition(0x16,
            new StandardVariablePacketLength(), "NewHealthBarStatusUpdate");

        public static readonly PacketDefinition HealthBarStatusUpdate = new PacketDefinition(0x17,
            new StaticPacketLength(12), "HealthBarStatusUpdate");

        public static readonly PacketDefinition ObjectInfo = new PacketDefinition(0x1A,
            new StandardVariablePacketLength(), "ObjectInfo");

        public static readonly PacketDefinition CharacterLocaleAndBody = new PacketDefinition(0x1B,
            new StaticPacketLength(37), "CharacterLocaleAndBody");

        public static readonly PacketDefinition SendSpeech = new PacketDefinition(0x1C,
            new StandardVariablePacketLength(), "SendSpeech");

        public static readonly PacketDefinition DeleteObject = new PacketDefinition(0x1D,
            new StaticPacketLength(5), "DeleteObject");

        public static readonly PacketDefinition Explosion = new PacketDefinition(0x1F,
            new StaticPacketLength(8), "Explosion");

        public static readonly PacketDefinition DrawGamePlayer = new PacketDefinition(0x20,
            new StaticPacketLength(19), "DrawGamePlayer");

        public static readonly PacketDefinition CharMoveRejection = new PacketDefinition(0x21,
            new StaticPacketLength(8), "CharMoveRejection");

        public static readonly PacketDefinition DraggingOfItem = new PacketDefinition(0x23,
            new StaticPacketLength(26), "DraggingOfItem");

        public static readonly PacketDefinition DrawContainer = new PacketDefinition(0x24,
            new StaticPacketLength(7), "DrawContainer");

        public static readonly PacketDefinition AddItemToContainer = new PacketDefinition(0x25,
            new StaticPacketLength(20), "AddItemToContainer");

        public static readonly PacketDefinition KickPlayer = new PacketDefinition(0x26,
            new StaticPacketLength(5), "KickPlayer");

        public static readonly PacketDefinition RejectMoveItemRequest = new PacketDefinition(0x27,
            new StaticPacketLength(2), "RejectMoveItemRequest");

        public static readonly PacketDefinition PauseClient = new PacketDefinition(0x33,
            new StaticPacketLength(2), "PauseClient");

        public static readonly PacketDefinition PlaySoundEffect = new PacketDefinition(0x54,
            new StaticPacketLength(12), "PlaySoundEffect");

        public static readonly PacketDefinition LoginComplete = new PacketDefinition(0x55,
            new StaticPacketLength(1), "LoginComplete");

        public static readonly PacketDefinition GraphicalEffect = new PacketDefinition(0x70,
            new StaticPacketLength(28), "CharactersStartingLocations");

        public static readonly PacketDefinition DrawObject = new PacketDefinition(0x78,
            new StandardVariablePacketLength(), "DrawObject");

        public static readonly PacketDefinition LoginDenied = new PacketDefinition(0x82,
            new StaticPacketLength(2), "LoginDenied");

        public static readonly PacketDefinition ConnectToGameServer = new PacketDefinition<ConnectToGameServer>(0x8C,
            new StaticPacketLength(11), "ConnectToGameServer", rawPacket => new ConnectToGameServer(rawPacket));

        public static readonly PacketDefinition TipNoticeWindow = new PacketDefinition(0xA6,
            new StandardVariablePacketLength(), "TipNoticeWindow");

        public static readonly PacketDefinition GameServerList = new PacketDefinition(0xA8,
            new StandardVariablePacketLength(), "GameServerList");

        public static readonly PacketDefinition CharactersStartingLocations = new PacketDefinition(0xA9,
            new StandardVariablePacketLength(), "CharactersStartingLocations");

        public static readonly PacketDefinition SpeechMessage = new PacketDefinition(0xAE,
            new StandardVariablePacketLength(), "SpeechMessage");

        public static readonly PacketDefinition EnableLockedClientFeatures = new PacketDefinition(0xB9,
            new StaticPacketLength(3), "EnableLockedClientFeatures");

        #endregion

        #region Both

        public static readonly PacketDefinition CharacterMoveAck = new PacketDefinition(0x22,
            new StaticPacketLength(3), "CharacterMoveAck");

        public static readonly PacketDefinition ClientVersion = new PacketDefinition(0xBD,
            new StandardVariablePacketLength(), "ClientVersion");

        public static readonly PacketDefinition ClientViewRange = new PacketDefinition(0xC8,
            new StaticPacketLength(2), "ClientViewRange");

        public static readonly PacketDefinition GeneralInformationPacket = new PacketDefinition(0xBF,
            new StandardVariablePacketLength(), "GeneralInformationPacket");

        public static readonly PacketDefinition PingMessage = new PacketDefinition(0x73,
            new StaticPacketLength(2), "PingMessage");

        public static readonly PacketDefinition RequestWarMode = new PacketDefinition(0x72,
            new StaticPacketLength(5), "RequestWarMode");

        public static readonly PacketDefinition SendSkills = new PacketDefinition(0x3A,
            new StandardVariablePacketLength(), "SendSkills");

        #endregion
    }
}