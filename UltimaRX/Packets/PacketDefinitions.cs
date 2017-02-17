using UltimaRX.Packets.Both;
using UltimaRX.Packets.Client;
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

        public static readonly PacketDefinition<MoveRequest> MoveRequest = new PacketDefinition<MoveRequest>(0x02,
            new StaticPacketLength(7), "MoveRequest", packet => new MoveRequest());

        public static readonly PacketDefinition TalkRequest = new PacketDefinition(0x03,
            new VariablePacketLength(), "TalkRequest");

        public static readonly PacketDefinition GodModeRequest = new PacketDefinition(0x04,
            new StaticPacketLength(2), "GodModeRequest");

        public static readonly PacketDefinition AttackRequest = new PacketDefinition(0x05,
            new StaticPacketLength(5), "AttackRequest");

        public static readonly PacketDefinition<DoubleClickRequest> DoubleClick = new PacketDefinition<DoubleClickRequest>(0x06,
            new StaticPacketLength(5), "DoubleClick", packet => new DoubleClickRequest());

        public static readonly PacketDefinition PickUpItem = new PacketDefinition(0x07,
            new StaticPacketLength(7), "PickUpItem");

        public static readonly PacketDefinition DropItem = new PacketDefinition(0x08,
            new StaticPacketLength(14), "DropItem");

        public static readonly PacketDefinition SingleClick = new PacketDefinition(0x09,
            new StaticPacketLength(5), "SingleClick");

        public static readonly PacketDefinition RequestSkills = new PacketDefinition(0x12,
            new VariablePacketLength(), "RequestSkills");

        public static readonly PacketDefinition DropWearItem = new PacketDefinition(0x13,
            new StaticPacketLength(10), "DropWearItem");

        public static readonly PacketDefinition ControlAnimation = new PacketDefinition(0x1E,
            new StaticPacketLength(4), "ControlAnimation");

        public static readonly PacketDefinition GetClientStatus = new PacketDefinition(0x34,
            new StaticPacketLength(10), "GetClientStatus");

        public static readonly PacketDefinition PathfinidingInClient = new PacketDefinition(0x38,
            new StaticPacketLength(7), "PathfinidingInClient");

        public static readonly PacketDefinition BuyItems = new PacketDefinition(0x3B,
            new VariablePacketLength(), "BuyItems");

        public static readonly PacketDefinition VersionOk = new PacketDefinition(0x45,
            new StaticPacketLength(5), "VersionOk");

        public static readonly PacketDefinition NewArtwork = new PacketDefinition(0x46,
            new VariablePacketLength(), "NewArtwork");

        public static readonly PacketDefinition NewTerrain = new PacketDefinition(0x47,
            new StaticPacketLength(11), "NewTerrain");

        public static readonly PacketDefinition NewAnimation = new PacketDefinition(0x48,
            new StaticPacketLength(73), "NewAnimation");

        public static readonly PacketDefinition NewHues = new PacketDefinition(0x49,
            new StaticPacketLength(93), "NewHues");

        public static readonly PacketDefinition DeleteArt = new PacketDefinition(0x4A,
            new StaticPacketLength(5), "DeleteArt");

        public static readonly PacketDefinition CheckClientVersion = new PacketDefinition(0x4B,
            new StaticPacketLength(9), "CheckClientVersion");

        public static readonly PacketDefinition ScriptNames = new PacketDefinition(0x4C,
            new VariablePacketLength(), "ScriptNames");

        public static readonly PacketDefinition EditScriptFile = new PacketDefinition(0x4D,
            new VariablePacketLength(), "EditScriptFile");

        public static readonly PacketDefinition BoardHeader = new PacketDefinition(0x50,
            new VariablePacketLength(), "BoardHeader");

        public static readonly PacketDefinition BoardMessage = new PacketDefinition(0x51,
            new VariablePacketLength(), "BoardMessage");

        public static readonly PacketDefinition BoardPostMessage = new PacketDefinition(0x52,
            new VariablePacketLength(), "BoardPostMessage");

        public static readonly PacketDefinition UpdateRegions = new PacketDefinition(0x57,
            new StaticPacketLength(110), "UpdateRegions");

        public static readonly PacketDefinition AddRegion = new PacketDefinition(0x58,
            new StaticPacketLength(106), "AddRegion");

        public static readonly PacketDefinition NewContextFx = new PacketDefinition(0x59,
            new VariablePacketLength(), "NewContextFx");

        public static readonly PacketDefinition UpdateContextFx = new PacketDefinition(0x5A,
            new VariablePacketLength(), "UpdateContextFx");

        public static readonly PacketDefinition RestartVersion = new PacketDefinition(0x5C,
            new StaticPacketLength(2), "RestartVersion");

        public static readonly PacketDefinition LoginCharacter = new PacketDefinition(0x5D,
            new StaticPacketLength(73), "LoginCharacter");

        public static readonly PacketDefinition ServerListing = new PacketDefinition(0x5E,
            new VariablePacketLength(), "ServerListing");

        public static readonly PacketDefinition ServerListAddEntry = new PacketDefinition(0x5F,
            new StaticPacketLength(49), "ServerListAddEntry");

        public static readonly PacketDefinition ServerListRemoveEntry = new PacketDefinition(0x60,
            new StaticPacketLength(5), "ServerListRemoveEntry");

        public static readonly PacketDefinition RemoveStaticObject = new PacketDefinition(0x61,
            new StaticPacketLength(9), "RemoveStaticObject");

        public static readonly PacketDefinition MoveStaticObject = new PacketDefinition(0x62,
            new StaticPacketLength(15), "MoveStaticObject");

        public static readonly PacketDefinition LoadArea = new PacketDefinition(0x63,
            new StaticPacketLength(13), "LoadArea");

        public static readonly PacketDefinition LoadAreaRequest = new PacketDefinition(0x64,
            new StaticPacketLength(1), "LoadAreaRequest");

        public static readonly PacketDefinition ChangeTextColor = new PacketDefinition(0x69,
            new StaticPacketLength(5), "ChangeTextColor");

        public static readonly PacketDefinition RenameCharacter = new PacketDefinition(0x75,
            new StaticPacketLength(35), "RenameCharacter");

        public static readonly PacketDefinition ResponseToDialogBox = new PacketDefinition(0x7D,
            new StaticPacketLength(13), "ResponseToDialogBox");

        public static readonly PacketDefinition LoginRequest = new PacketDefinition(0x80,
            new StaticPacketLength(62), "LoginRequest");

        public static readonly PacketDefinition DeleteCharacter = new PacketDefinition(0x83,
            new StaticPacketLength(39), "DeleteCharacter");

        public static readonly PacketDefinition CharacterCreation = new PacketDefinition(0x8D,
            new StaticPacketLength(146), "CharacterCreation");

        public static readonly PacketDefinition GameServerLoginRequest = new PacketDefinition(0x91,
            new StaticPacketLength(65), "GameServerLoginRequest");

        public static readonly PacketDefinition RequestHelp = new PacketDefinition(0x9B,
            new StaticPacketLength(258), "RequestHelp");

        public static readonly PacketDefinition SellListReply = new PacketDefinition(0x9F,
            new VariablePacketLength(), "SellListReply");

        public static readonly PacketDefinition SelectServerRequest = new PacketDefinition(0xA0,
            new StaticPacketLength(3), "SelectServerRequest");

        public static readonly PacketDefinition ClientSpy = new PacketDefinition(0xA4,
            new StaticPacketLength(149), "ClientSpy");

        public static readonly PacketDefinition RequestTipNoticeWindow = new PacketDefinition(0xA7,
            new StaticPacketLength(4), "RequestTipNoticeWindow");

        public static readonly PacketDefinition GumpTextEntryDialogReply = new PacketDefinition(0xAC,
            new VariablePacketLength(), "GumpTextEntryDialogReply");

        public static readonly PacketDefinition<SpeechRequest> SpeechRequest = new PacketDefinition<SpeechRequest>(0xAD,
            new VariablePacketLength(), "SpeechRequest", (packet) => new SpeechRequest());

        public static readonly PacketDefinition<GumpMenuSelectionRequest> GumpMenuSelection = new PacketDefinition<GumpMenuSelectionRequest>(0xB1,
            new VariablePacketLength(), "GumpMenuSelection", packet => new GumpMenuSelectionRequest());

        public static readonly PacketDefinition ChatText = new PacketDefinition(0xB3,
            new VariablePacketLength(), "ChatText");

        public static readonly PacketDefinition OpenChatWindow = new PacketDefinition(0xB5,
            new StaticPacketLength(64), "OpenChatWindow");

        public static readonly PacketDefinition SendHelpTipRequest = new PacketDefinition(0xB6,
            new StaticPacketLength(9), "SendHelpTipRequest");

        public static readonly PacketDefinition InvalidMapRequest = new PacketDefinition(0xC5,
            new StaticPacketLength(1), "InvalidMapRequest");

        public static readonly PacketDefinition SpyOnClient = new PacketDefinition(0xD9,
            new VariablePacketLength(), "SpyOnClient");

        #endregion

        #region Server

        public static readonly PacketDefinition Damage = new PacketDefinition(0x0B,
            new VariablePacketLength(), "Damage");

        public static readonly PacketDefinition<StatusBarInfoPacket> StatusBarInfo = new PacketDefinition<StatusBarInfoPacket>(0x11,
            new VariablePacketLength(), "StatusBarInfo", packet => new StatusBarInfoPacket());

        public static readonly PacketDefinition NewHealthBarStatusUpdate = new PacketDefinition(0x16,
            new VariablePacketLength(), "NewHealthBarStatusUpdate");

        public static readonly PacketDefinition HealthBarStatusUpdate = new PacketDefinition(0x17,
            new StaticPacketLength(12), "HealthBarStatusUpdate");

        public static readonly PacketDefinition<ObjectInfoPacket> ObjectInfo = new PacketDefinition<ObjectInfoPacket>(0x1A,
            new VariablePacketLength(), "ObjectInfo", (packet) => new ObjectInfoPacket());

        public static readonly PacketDefinition<CharLocaleAndBodyPacket> CharacterLocaleAndBody = new PacketDefinition<CharLocaleAndBodyPacket>(0x1B,
            new StaticPacketLength(37), "CharacterLocaleAndBody", packet => new CharLocaleAndBodyPacket());

        public static readonly PacketDefinition<SendSpeechPacket> SendSpeech = new PacketDefinition<SendSpeechPacket>(0x1C,
            new VariablePacketLength(), "SendSpeech", packet => new SendSpeechPacket());

        public static readonly PacketDefinition<DeleteObjectPacket> DeleteObject = new PacketDefinition<DeleteObjectPacket>(0x1D,
            new StaticPacketLength(5), "DeleteObject", (packet) => new DeleteObjectPacket());

        public static readonly PacketDefinition Explosion = new PacketDefinition(0x1F,
            new StaticPacketLength(8), "Explosion");

        public static readonly PacketDefinition<DrawGamePlayerPacket> DrawGamePlayer = new PacketDefinition<DrawGamePlayerPacket>(0x20,
            new StaticPacketLength(19), "DrawGamePlayer", packet => new DrawGamePlayerPacket());

        public static readonly PacketDefinition<CharMoveRejectionPacket> CharMoveRejection = new PacketDefinition<CharMoveRejectionPacket>(0x21,
            new StaticPacketLength(8), "CharMoveRejection", packet => new CharMoveRejectionPacket());

        public static readonly PacketDefinition DraggingOfItem = new PacketDefinition(0x23,
            new StaticPacketLength(26), "DraggingOfItem");

        public static readonly PacketDefinition DrawContainer = new PacketDefinition(0x24,
            new StaticPacketLength(7), "DrawContainer");

        public static readonly PacketDefinition<AddItemToContainerPacket> AddItemToContainer = new PacketDefinition<AddItemToContainerPacket>(0x25,
            new StaticPacketLength(20), "AddItemToContainer", packet => new AddItemToContainerPacket());

        public static readonly PacketDefinition KickPlayer = new PacketDefinition(0x26,
            new StaticPacketLength(5), "KickPlayer");

        public static readonly PacketDefinition RejectMoveItemRequest = new PacketDefinition(0x27,
            new StaticPacketLength(2), "RejectMoveItemRequest");

        public static readonly PacketDefinition DropItemFailed = new PacketDefinition(0x28,
            new StaticPacketLength(5), "DropItemFailed");

        public static readonly PacketDefinition DropItemApproved = new PacketDefinition(0x29,
            new StaticPacketLength(1), "DropItemApproved");

        public static readonly PacketDefinition Blood = new PacketDefinition(0x2A,
            new StaticPacketLength(5), "Blood");

        public static readonly PacketDefinition MobAttributes = new PacketDefinition(0x2D,
            new StaticPacketLength(17), "MobAttributes");

        public static readonly PacketDefinition<WornItemPacket> WornItem = new PacketDefinition<WornItemPacket>(0x2E,
            new StaticPacketLength(15), "WornItem", packet => new WornItemPacket());

        public static readonly PacketDefinition FightOccuring = new PacketDefinition(0x2F,
            new StaticPacketLength(10), "FightOccuring");

        public static readonly PacketDefinition AttackOk = new PacketDefinition(0x30,
            new StaticPacketLength(5), "AttackOk");

        public static readonly PacketDefinition AttackEnded = new PacketDefinition(0x31,
            new StaticPacketLength(1), "AttackEnded");

        public static readonly PacketDefinition<AddMultipleItemsInContainerPacket> AddMultipleItemsInContainer
            = new PacketDefinition<AddMultipleItemsInContainerPacket>(0x3C, new VariablePacketLength(),
                "AddMultipleItemsInContainer", (packet) => new AddMultipleItemsInContainerPacket());

        public static readonly PacketDefinition PauseClient = new PacketDefinition(0x33,
            new StaticPacketLength(2), "PauseClient");

        public static readonly PacketDefinition PersonalLightLevel = new PacketDefinition(0x4E,
            new StaticPacketLength(6), "PersonalLightLevel");

        public static readonly PacketDefinition OverallLightLevel = new PacketDefinition(0x4F,
            new StaticPacketLength(2), "OverallLightLevel");

        public static readonly PacketDefinition RejectCharacterLogon = new PacketDefinition(0x53,
            new StaticPacketLength(2), "RejectCharacterLogon");

        public static readonly PacketDefinition PlaySoundEffect = new PacketDefinition(0x54,
            new StaticPacketLength(12), "PlaySoundEffect");

        public static readonly PacketDefinition LoginComplete = new PacketDefinition(0x55,
            new StaticPacketLength(1), "LoginComplete");

        public static readonly PacketDefinition Time = new PacketDefinition(0x5B,
            new StaticPacketLength(4), "Time");

        public static readonly PacketDefinition SetWeather = new PacketDefinition(0x65,
            new StaticPacketLength(4), "SetWeather");

        public static readonly PacketDefinition PlayMidiMusic = new PacketDefinition(0x6D,
            new StaticPacketLength(3), "PlayMidiMusic");

        public static readonly PacketDefinition CharacterAnimation = new PacketDefinition(0x6E,
            new StaticPacketLength(14), "CharacterAnimation");

        public static readonly PacketDefinition GraphicalEffect = new PacketDefinition(0x70,
            new StaticPacketLength(28), "GraphicalEffect");

        public static readonly PacketDefinition OpenBuyWindow = new PacketDefinition(0x74,
            new VariablePacketLength(), "OpenBuyWindow");

        public static readonly PacketDefinition NewSubserver = new PacketDefinition(0x76,
            new StaticPacketLength(16), "NewSubserver");

        public static readonly PacketDefinition<UpdatePlayerPacket> UpdatePlayer = new PacketDefinition<UpdatePlayerPacket>(0x77,
            new StaticPacketLength(17), "UpdatePlayer", packet => new UpdatePlayerPacket());

        public static readonly PacketDefinition<DrawObjectPacket> DrawObject = new PacketDefinition<DrawObjectPacket>(0x78,
            new VariablePacketLength(), "DrawObject", (packet) => new DrawObjectPacket());

        public static readonly PacketDefinition OpenDialogBox = new PacketDefinition(0x7C,
            new VariablePacketLength(), "OpenDialogBox");

        public static readonly PacketDefinition LoginDenied = new PacketDefinition(0x82,
            new StaticPacketLength(2), "LoginDenied");

        public static readonly PacketDefinition ResendCharactersAfterDelete = new PacketDefinition(0x86,
            new StaticPacketLength(304), "ResendCharactersAfterDelete");

        public static readonly PacketDefinition OpenPaperdoll = new PacketDefinition(0x88,
            new StaticPacketLength(66), "OpenPaperdoll");

        public static readonly PacketDefinition CorpseClothing = new PacketDefinition(0x89,
            new VariablePacketLength(), "CorpseClothing");

        public static readonly PacketDefinition<ConnectToGameServerPacket> ConnectToGameServer = new PacketDefinition<ConnectToGameServerPacket>(0x8C,
            new StaticPacketLength(11), "ConnectToGameServer", rawPacket => new ConnectToGameServerPacket());

        public static readonly  PacketDefinition DisplaySign = new PacketDefinition(0x8b, new VariablePacketLength(), "DisplaySign");

        public static readonly PacketDefinition MapMessage = new PacketDefinition(0x90,
            new StaticPacketLength(19), "MapMessage");

        public static readonly PacketDefinition MovePlayer = new PacketDefinition(0x97,
            new StaticPacketLength(2), "MovePlayer");

        public static readonly PacketDefinition RequestAssistance = new PacketDefinition(0x9C,
            new StaticPacketLength(53), "RequestAssistance");

        public static readonly PacketDefinition SellList = new PacketDefinition(0x9E,
            new VariablePacketLength(), "SellList");

        public static readonly PacketDefinition<UpdateCurrentHealthPacket> UpdateCurrentHealth =
            new PacketDefinition<UpdateCurrentHealthPacket>(0xA1,
                new StaticPacketLength(9), "UpdateCurrentHealth", packet => new UpdateCurrentHealthPacket());

        public static readonly PacketDefinition UpdateCurrentMana = new PacketDefinition(0xA2,
            new StaticPacketLength(9), "UpdateCurrentMana");

        public static readonly PacketDefinition<UpdateCurrentStaminaPacket> UpdateCurrentStamina =
            new PacketDefinition<UpdateCurrentStaminaPacket>(0xA3,
                new StaticPacketLength(9), "UpdateCurrentStamina", packet => new UpdateCurrentStaminaPacket());

        public static readonly PacketDefinition OpenWebBrowser = new PacketDefinition(0xA5,
            new VariablePacketLength(), "OpenWebBrowser");

        public static readonly PacketDefinition TipNoticeWindow = new PacketDefinition(0xA6,
            new VariablePacketLength(), "TipNoticeWindow");

        public static readonly PacketDefinition GameServerList = new PacketDefinition(0xA8,
            new VariablePacketLength(), "GameServerList");

        public static readonly PacketDefinition CharactersStartingLocations = new PacketDefinition(0xA9,
            new VariablePacketLength(), "CharactersStartingLocations");

        public static readonly PacketDefinition AllowRefuseAttack = new PacketDefinition(0xAA,
            new StaticPacketLength(5), "AllowRefuseAttack");

        public static readonly PacketDefinition GumpTextEntryDialog = new PacketDefinition(0xAB,
            new VariablePacketLength(), "GumpTextEntryDialog");

        public static readonly PacketDefinition<SpeechMessagePacket> SpeechMessage = new PacketDefinition<SpeechMessagePacket>(0xAE,
            new VariablePacketLength(), "SpeechMessage", packet => new SpeechMessagePacket());

        public static readonly PacketDefinition DisplayDeathAction = new PacketDefinition(0xAF,
            new StaticPacketLength(13), "DisplayDeathAction");

        public static readonly PacketDefinition<SendGumpMenuDialogPacket> SendGumpMenuDialog = new PacketDefinition<SendGumpMenuDialogPacket>(0xB0,
            new VariablePacketLength(), "SendGumpMenuDialog", packet => new SendGumpMenuDialogPacket());

        public static readonly PacketDefinition ChatMessage = new PacketDefinition(0xB2,
            new VariablePacketLength(), "ChatMessage");

        public static readonly PacketDefinition HelpTileData = new PacketDefinition(0xB7,
            new VariablePacketLength(), "HelpTileData");

        public static readonly PacketDefinition EnableLockedClientFeatures = new PacketDefinition(0xB9,
            new StaticPacketLength(3), "EnableLockedClientFeatures");

        public static readonly PacketDefinition QuestArrow = new PacketDefinition(0xBA,
            new StaticPacketLength(6), "QuestArrow");

        public static readonly PacketDefinition SeasonalInformation = new PacketDefinition(0xBC,
            new StaticPacketLength(3), "SeasonalInformation");

        public static readonly PacketDefinition GraphicalEffect2 = new PacketDefinition(0xC0,
            new StaticPacketLength(36), "GraphicalEffect2");

        public static readonly PacketDefinition ClilocMessage = new PacketDefinition(0xC1,
            new VariablePacketLength(), "ClilocMessage");

        public static readonly PacketDefinition Semivisible = new PacketDefinition(0xC4,
            new StaticPacketLength(6), "Semivisible");

        public static readonly PacketDefinition InvalidMapEnable = new PacketDefinition(0xC6,
            new StaticPacketLength(1), "InvalidMapEnable");

        public static readonly PacketDefinition GlobalQueueCount = new PacketDefinition(0xCB,
            new StaticPacketLength(7), "GlobalQueueCount");

        public static readonly PacketDefinition ClilocMessageAffix = new PacketDefinition(0xCC,
            new VariablePacketLength(), "ClilocMessageAffix");

        public static readonly PacketDefinition ExtendedDrawGamePlayer = new PacketDefinition(0xD2,
            new StaticPacketLength(25), "ExtendedDrawGamePlayer");

        public static readonly PacketDefinition ExtendedDrawObject = new PacketDefinition(0xD3,
            new VariablePacketLength(), "ExtendedDrawObject");

        public static readonly PacketDefinition SendCustomHouse = new PacketDefinition(0xD8,
            new VariablePacketLength(), "SendCustomHouse");

        public static readonly PacketDefinition CharacterTransferLog = new PacketDefinition(0xDB,
            new VariablePacketLength(), "CharacterTransferLog");

        public static readonly PacketDefinition SecondAgeRevision = new PacketDefinition(0xDC,
            new StaticPacketLength(9), "SecondAgeRevision");

        public static readonly PacketDefinition CompressedGump = new PacketDefinition(0xDD,
            new VariablePacketLength(), "CompressedGump");

        public static readonly PacketDefinition UpdateMobileStatus = new PacketDefinition(0xDE,
            new VariablePacketLength(), "UpdateMobileStatus");

        public static readonly PacketDefinition BuffSystem = new PacketDefinition(0xDF,
            new VariablePacketLength(), "BuffSystem");

        public static readonly PacketDefinition NewCharacterAnimation = new PacketDefinition(0xE2,
            new StaticPacketLength(10), "NewCharacterAnimation");

        public static readonly PacketDefinition KrEncryptionResponse = new PacketDefinition(0xE3,
            new StaticPacketLength(77), "KrEncryptionResponse");

        public static readonly PacketDefinition SecondAgeObjectInformation = new PacketDefinition(0xF3,
            new StaticPacketLength(24), "SecondAgeObjectInformation");

        public static readonly PacketDefinition NewMapMessage = new PacketDefinition(0xF5,
            new StaticPacketLength(21), "NewMapMessage");

        #endregion

        #region Both

        public static readonly PacketDefinition Follow = new PacketDefinition(0x15,
            new StaticPacketLength(9), "Follow");

        public static readonly PacketDefinition<CharacterMoveAckPacket> CharacterMoveAck = new PacketDefinition<CharacterMoveAckPacket>(0x22,
            new StaticPacketLength(3), "CharacterMoveAck", packet => new CharacterMoveAckPacket());

        public static readonly PacketDefinition ResurrectionMenu = new PacketDefinition(0x2C,
            new StaticPacketLength(2), "ResurrectionMenu");

        public static readonly PacketDefinition RemoveGroup = new PacketDefinition(0x39,
            new StaticPacketLength(9), "RemoveGroup");

        public static readonly PacketDefinition SendSkills = new PacketDefinition(0x3A,
            new VariablePacketLength(), "SendSkills");

        public static readonly PacketDefinition MapPacket = new PacketDefinition(0x56,
            new StaticPacketLength(11), "MapPacket");

        public static readonly PacketDefinition BooksPages = new PacketDefinition(0x66,
            new VariablePacketLength(), "BooksPages");

        public static readonly PacketDefinition<TargetCursorPacket> TargetCursor = new PacketDefinition<TargetCursorPacket>(0x6C,
            new StaticPacketLength(19), "TargetCursor", (packet) => new TargetCursorPacket());

        public static readonly PacketDefinition SecureTraiding = new PacketDefinition(0x6F,
            new VariablePacketLength(), "SecureTraiding");

        public static readonly PacketDefinition BulletinBoardMessage = new PacketDefinition(0x71,
            new VariablePacketLength(), "BulletinBoardMessage");

        public static readonly PacketDefinition RequestWarMode = new PacketDefinition(0x72,
            new StaticPacketLength(5), "RequestWarMode");

        public static readonly PacketDefinition PingMessage = new PacketDefinition(0x73,
            new StaticPacketLength(2), "PingMessage");

        public static readonly PacketDefinition BookHeader = new PacketDefinition(0x93,
            new StaticPacketLength(99), "BookHeader");

        public static readonly PacketDefinition DyeWindow = new PacketDefinition(0x95,
            new StaticPacketLength(9), "DyeWindow");

        public static readonly PacketDefinition GiveBoatHousePlacementView = new PacketDefinition(0x99,
            new StaticPacketLength(26), "GiveBoatHousePlacementView");

        public static readonly PacketDefinition ConsoleEntryPrompt = new PacketDefinition(0x9A,
            new VariablePacketLength(), "ConsoleEntryPrompt");

        public static readonly PacketDefinition RequestCharProfile = new PacketDefinition(0xB8,
            new VariablePacketLength(), "RequestCharProfile");

        public static readonly PacketDefinition UltimaMessenger = new PacketDefinition(0xBB,
            new StaticPacketLength(9), "UltimaMessenger");

        public static readonly PacketDefinition ClientVersion = new PacketDefinition(0xBD,
            new VariablePacketLength(), "ClientVersion");

        public static readonly PacketDefinition AssistVersion = new PacketDefinition(0xBE,
            new VariablePacketLength(), "AssistVersion");

        public static readonly PacketDefinition GeneralInformationPacket = new PacketDefinition(0xBF,
            new VariablePacketLength(), "GeneralInformationPacket");

        public static readonly PacketDefinition UnicodeTextEntry = new PacketDefinition(0xC2,
            new VariablePacketLength(), "UnicodeTextEntry");

        public static readonly PacketDefinition ClientViewRange = new PacketDefinition(0xC8,
            new StaticPacketLength(2), "ClientViewRange");

        public static readonly PacketDefinition ConfigurationFile = new PacketDefinition(0xD0,
            new VariablePacketLength(), "ConfigurationFile");

        public static readonly PacketDefinition LogoutStatus = new PacketDefinition(0xD1,
            new StaticPacketLength(2), "LogoutStatus");

        public static readonly PacketDefinition BookHeaderNew = new PacketDefinition(0xD4,
            new VariablePacketLength(), "BookHeaderNew");

        public static readonly PacketDefinition MegaCliloc = new PacketDefinition(0xD6,
            new VariablePacketLength(), "MegaCliloc");

        public static readonly PacketDefinition GenericAosCommands = new PacketDefinition(0xD7,
            new VariablePacketLength(), "GenericAosCommands");

        public static readonly PacketDefinition FreeShardList = new PacketDefinition(0xF1,
            new VariablePacketLength(), "FreeShardList");

        #endregion
    }
}