using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.Packets
{
    internal static class PacketDefinitions
    {
        public static readonly PacketDefinition LoginSeed = new PacketDefinition(-1,
            new StaticPacketLength(4), "LoginSeed");

        public static readonly PacketDefinition CreateCharacterRequest = new PacketDefinition(0x00,
            new StaticPacketLength(0x68), "CreateCharacterRequest");
        public static readonly PacketDefinition CreateCharacterRequest7018 = new PacketDefinition(0x00,
            new StaticPacketLength(0x6A), "CreateCharacterRequest");

        public static readonly PacketDefinition DisconnectNotification = new PacketDefinition(0x01,
            new StaticPacketLength(5), "DisconnectNotification");

        public static readonly PacketDefinition<MoveRequest> MoveRequest = new PacketDefinition<MoveRequest>(0x02,
            new StaticPacketLength(7), "MoveRequest", () => new MoveRequest());

        public static readonly PacketDefinition TalkRequest = new PacketDefinition<TalkRequest>(0x03,
            new VariablePacketLength(), "TalkRequest", () => new TalkRequest());

        public static readonly PacketDefinition GodModeRequest = new PacketDefinition(0x04,
            new StaticPacketLength(2), "GodModeRequest");

        public static readonly PacketDefinition AttackRequest = new PacketDefinition(0x05,
            new StaticPacketLength(5), "AttackRequest");

        public static readonly PacketDefinition<DoubleClickRequest> DoubleClick = new PacketDefinition<DoubleClickRequest>(0x06,
            new StaticPacketLength(5), "DoubleClick", () => new DoubleClickRequest());

        public static readonly PacketDefinition PickUpItem = new PacketDefinition(0x07,
            new StaticPacketLength(7), "PickUpItem");

        public static readonly PacketDefinition<DropItemRequest> DropItem = new PacketDefinition<DropItemRequest>(0x08,
            new StaticPacketLength(14), "DropItem", () => new DropItemRequest(false, 14));
        public static readonly PacketDefinition<DropItemRequest> DropItem6017 = new PacketDefinition<DropItemRequest>(0x08,
            new StaticPacketLength(15), "DropItem", () => new DropItemRequest(true, 15));

        public static readonly PacketDefinition<SingleClickRequest> SingleClick = new PacketDefinition<SingleClickRequest>(0x09,
            new StaticPacketLength(5), "SingleClick", () => new SingleClickRequest());

        public static readonly PacketDefinition Edit = new PacketDefinition(0x0A,
            new StaticPacketLength(0x0B), "Edit");

        public static readonly PacketDefinition Damage = new PacketDefinition(0x0B,
            new StaticPacketLength(0x10A), "Damage");
        public static readonly PacketDefinition Damage500a = new PacketDefinition(0x0B,
            new StaticPacketLength(7), "Damage");

        public static readonly PacketDefinition EditTileData = new PacketDefinition(0x0C,
            new VariablePacketLength(), "EditTileData");

        public static readonly PacketDefinition EditNpcData = new PacketDefinition(0x0D,
            new StaticPacketLength(0x03), "EditNpcData");

        public static readonly PacketDefinition EditTemplateData = new PacketDefinition(0x0E,
            new StaticPacketLength(0x01), "EditTemplateData");

        public static readonly PacketDefinition Unknown0F = new PacketDefinition(0x0F,
            new StaticPacketLength(0x3d), "Unknown0F");

        public static readonly PacketDefinition EditHueData = new PacketDefinition(0x10,
            new StaticPacketLength(0xd7), "EditHueData");

        public static readonly PacketDefinition<StatusBarInfoPacket> StatusBarInfo = new PacketDefinition<StatusBarInfoPacket>(0x11,
                new VariablePacketLength(), "StatusBarInfo", () => new StatusBarInfoPacket());

        public static readonly PacketDefinition<SkillRequest> RequestSkills = new PacketDefinition<SkillRequest>(0x12,
            new VariablePacketLength(), "RequestSkills", () => new SkillRequest());

        public static readonly PacketDefinition DropWearItem = new PacketDefinition(0x13,
            new StaticPacketLength(0x0A), "DropWearItem");

        public static readonly PacketDefinition SendElevation = new PacketDefinition(0x14,
            new StaticPacketLength(6), "SendElevation");

        public static readonly PacketDefinition Follow = new PacketDefinition(0x15,
            new StaticPacketLength(9), "Follow");

        public static readonly PacketDefinition NewHealthBarStatusUpdate = new PacketDefinition(0x16,
            new StaticPacketLength(1), "NewHealthBarStatusUpdate");
        public static readonly PacketDefinition NewHealthBarStatusUpdate500a = new PacketDefinition(0x16,
            new VariablePacketLength(), "NewHealthBarStatusUpdate");

        public static readonly PacketDefinition HealthBarStatusUpdate = new PacketDefinition(0x17,
            new VariablePacketLength(), "HealthBarStatusUpdate");

        public static readonly PacketDefinition AddScript = new PacketDefinition(0x18,
            new VariablePacketLength(), "AddScript");

        public static readonly PacketDefinition EditNpcSpeech = new PacketDefinition(0x19,
            new VariablePacketLength(), "EditNpcSpeech");

        public static readonly PacketDefinition<ObjectInfoPacket> ObjectInfo = new PacketDefinition<ObjectInfoPacket>(0x1A,
            new VariablePacketLength(), "ObjectInfo", () => new ObjectInfoPacket());

        public static readonly PacketDefinition<CharLocaleAndBodyPacket> CharacterLocaleAndBody = new PacketDefinition<CharLocaleAndBodyPacket>(0x1B,
            new StaticPacketLength(0x25), "CharacterLocaleAndBody", () => new CharLocaleAndBodyPacket());

        public static readonly PacketDefinition<SendSpeechPacket> SendSpeech = new PacketDefinition<SendSpeechPacket>(0x1C,
            new VariablePacketLength(), "SendSpeech", () => new SendSpeechPacket());

        public static readonly PacketDefinition<DeleteObjectPacket> DeleteObject = new PacketDefinition<DeleteObjectPacket>(0x1D,
            new StaticPacketLength(5), "DeleteObject", () => new DeleteObjectPacket());

        public static readonly PacketDefinition ControlAnimation = new PacketDefinition(0x1E,
            new StaticPacketLength(4), "ControlAnimation");

        public static readonly PacketDefinition Explosion = new PacketDefinition(0x1F,
            new StaticPacketLength(8), "Explosion");

        public static readonly PacketDefinition<DrawGamePlayerPacket> DrawGamePlayer = new PacketDefinition<DrawGamePlayerPacket>(0x20,
            new StaticPacketLength(0x13), "DrawGamePlayer", () => new DrawGamePlayerPacket());

        public static readonly PacketDefinition<CharMoveRejectionPacket> CharMoveRejection = new PacketDefinition<CharMoveRejectionPacket>(0x21,
            new StaticPacketLength(8), "CharMoveRejection", () => new CharMoveRejectionPacket());

        public static readonly PacketDefinition<CharacterMoveAckPacket> CharacterMoveAck = new PacketDefinition<CharacterMoveAckPacket>(0x22,
            new StaticPacketLength(3), "CharacterMoveAck", () => new CharacterMoveAckPacket());

        public static readonly PacketDefinition DraggingOfItem = new PacketDefinition(0x23,
            new StaticPacketLength(0x1A), "DraggingOfItem");

        public static readonly PacketDefinition<DrawContainerPacket> DrawContainer = new PacketDefinition<DrawContainerPacket>(0x24,
            new StaticPacketLength(7), "DrawContainer", () => new DrawContainerPacket());
        public static readonly PacketDefinition<DrawContainerPacket> DrawContainer7090 = new PacketDefinition<DrawContainerPacket>(0x24,
            new StaticPacketLength(9), "DrawContainer", () => new DrawContainerPacket(7090));

        public static readonly PacketDefinition<AddItemToContainerPacket> AddItemToContainer = new PacketDefinition<AddItemToContainerPacket>(0x25,
            new StaticPacketLength(20), "AddItemToContainer", () => new AddItemToContainerPacket(false));
        public static readonly PacketDefinition<AddItemToContainerPacket> AddItemToContainer6017 = new PacketDefinition<AddItemToContainerPacket>(0x25,
            new StaticPacketLength(21), "AddItemToContainer", () => new AddItemToContainerPacket(true));

        public static readonly PacketDefinition KickPlayer = new PacketDefinition(0x26,
            new StaticPacketLength(5), "KickPlayer");

        public static readonly PacketDefinition<RejectMoveItemRequestPacket> RejectMoveItemRequest = new PacketDefinition<RejectMoveItemRequestPacket>(0x27,
            new StaticPacketLength(2), "RejectMoveItemRequest", () => new RejectMoveItemRequestPacket());

        public static readonly PacketDefinition DropItemFailed = new PacketDefinition(0x28,
            new StaticPacketLength(5), "DropItemFailed");

        public static readonly PacketDefinition DropItemApproved = new PacketDefinition(0x29,
            new StaticPacketLength(1), "DropItemApproved");

        public static readonly PacketDefinition Blood = new PacketDefinition(0x2A,
            new StaticPacketLength(5), "Blood");

        public static readonly PacketDefinition ToggleGodMode = new PacketDefinition(0x2B,
            new StaticPacketLength(2), "ToggleGodMode");

        public static readonly PacketDefinition ResurrectionMenu = new PacketDefinition(0x2C,
            new StaticPacketLength(2), "ResurrectionMenu");

        public static readonly PacketDefinition MobAttributes = new PacketDefinition(0x2D,
            new StaticPacketLength(0x11), "MobAttributes");

        public static readonly PacketDefinition<WornItemPacket> WornItem = new PacketDefinition<WornItemPacket>(0x2E,
            new StaticPacketLength(0x0F), "WornItem", () => new WornItemPacket());

        public static readonly PacketDefinition FightOccuring = new PacketDefinition(0x2F,
            new StaticPacketLength(0x0A), "FightOccuring");

        public static readonly PacketDefinition AttackOk = new PacketDefinition(0x30,
            new StaticPacketLength(5), "AttackOk");

        public static readonly PacketDefinition AttackEnded = new PacketDefinition(0x31,
            new StaticPacketLength(1), "AttackEnded");
        public static readonly PacketDefinition AttackEnded500a = new PacketDefinition(0x31,
            new VariablePacketLength(), "AttackEnded");

        public static readonly PacketDefinition ToggleHackMover = new PacketDefinition(0x32,
            new StaticPacketLength(2), "ToggleHackMover");

        public static readonly PacketDefinition<PauseClientPacket> PauseClient = new PacketDefinition<PauseClientPacket>(0x33,
            new StaticPacketLength(2), "PauseClient", () => new PauseClientPacket());

        public static readonly PacketDefinition<GetClientStatusRequest> GetClientStatus
            = new PacketDefinition<GetClientStatusRequest>(0x34, new StaticPacketLength(10), "GetClientStatus", () => new GetClientStatusRequest());

        public static readonly PacketDefinition ResourceType = new PacketDefinition(0x35,
            new StaticPacketLength(0x28d), "ResourceType");

        public static readonly PacketDefinition ResourceTileData = new PacketDefinition(0x36,
            new VariablePacketLength(), "ResourceTileData");

        public static readonly PacketDefinition MoveObject = new PacketDefinition(0x37,
            new StaticPacketLength(8), "MoveObject");

        public static readonly PacketDefinition PathfinidingInClient = new PacketDefinition(0x38,
            new StaticPacketLength(7), "PathfinidingInClient");

        public static readonly PacketDefinition RemoveGroup = new PacketDefinition(0x39,
            new StaticPacketLength(9), "RemoveGroup");

        public static readonly PacketDefinition<SendSkillsPacket> SendSkills = new PacketDefinition<SendSkillsPacket>(0x3A,
            new VariablePacketLength(), "SendSkills", () => new SendSkillsPacket());

        public static readonly PacketDefinition BuyItems = new PacketDefinition(0x3B,
            new VariablePacketLength(), "BuyItems");

        public static readonly PacketDefinition UpdateContainedItems = new PacketDefinition(0x3C,
            new VariablePacketLength(), "UpdateContainedItems");
        public static readonly PacketDefinition<AddMultipleItemsInContainerPacket> AddMultipleItemsInContainer
            = new PacketDefinition<AddMultipleItemsInContainerPacket>(0x3C, new VariablePacketLength(),
                "AddMultipleItemsInContainer", () => new AddMultipleItemsInContainerPacket());
        public static readonly PacketDefinition<AddMultipleItemsInContainerPacket> AddMultipleItemsInContainer6017
            = new PacketDefinition<AddMultipleItemsInContainerPacket>(0x3C, new VariablePacketLength(),
                "AddMultipleItemsInContainer", () => new AddMultipleItemsInContainerPacket(6017));

        public static readonly PacketDefinition Ship = new PacketDefinition(0x3D,
            new StaticPacketLength(2), "Ship");

        public static readonly PacketDefinition Versions = new PacketDefinition(0x3E,
            new StaticPacketLength(0x25), "Versions");

        public static readonly PacketDefinition UpdateStatics = new PacketDefinition(0x3F,
            new VariablePacketLength(), "UpdateStatics");

        public static readonly PacketDefinition UpdateTerrains = new PacketDefinition(0x40,
            new StaticPacketLength(0xc9), "UpdateTerrains");

        public static readonly PacketDefinition UpdateTerrains2 = new PacketDefinition(0x41,
            new VariablePacketLength(), "UpdateTerrains2");

        public static readonly PacketDefinition UpdateArt = new PacketDefinition(0x42,
            new VariablePacketLength(), "UpdateArt");

        public static readonly PacketDefinition UpdateAnimation = new PacketDefinition(0x43,
            new StaticPacketLength(0x229), "UpdateAnimation");

        public static readonly PacketDefinition UpdateHues = new PacketDefinition(0x44,
            new StaticPacketLength(0x2c9), "UpdateHues");

        public static readonly PacketDefinition VersionOK = new PacketDefinition(0x45,
            new StaticPacketLength(5), "VersionOK");

        public static readonly PacketDefinition NewArt = new PacketDefinition(0x46,
            new VariablePacketLength(), "NewArt");

        public static readonly PacketDefinition NewTerrain = new PacketDefinition(0x47,
            new StaticPacketLength(0x0B), "NewTerrain");

        public static readonly PacketDefinition NewAnimation = new PacketDefinition(0x48,
            new StaticPacketLength(0x49), "NewAnimation");

        public static readonly PacketDefinition NewHues = new PacketDefinition(0x49,
            new StaticPacketLength(0x5d), "NewHues");

        public static readonly PacketDefinition DeleteArt = new PacketDefinition(0x4A,
            new StaticPacketLength(5), "DeleteArt");

        public static readonly PacketDefinition CheckClientVersion = new PacketDefinition(0x4B,
            new StaticPacketLength(9), "CheckClientVersion");

        public static readonly PacketDefinition ScriptNames = new PacketDefinition(0x4C,
            new VariablePacketLength(), "ScriptNames");

        public static readonly PacketDefinition EditScriptFile = new PacketDefinition(0x4D,
            new VariablePacketLength(), "EditScriptFile");

        public static readonly PacketDefinition<PersonalLightLevelPacket> PersonalLightLevel = new PacketDefinition<PersonalLightLevelPacket>(0x4E,
            new StaticPacketLength(6), "PersonalLightLevel", () => new PersonalLightLevelPacket());

        public static readonly PacketDefinition<OverallLightLevelPacket> OverallLightLevel = new PacketDefinition<OverallLightLevelPacket>(0x4F,
            new StaticPacketLength(2), "OverallLightLevel", () => new OverallLightLevelPacket());

        public static readonly PacketDefinition BoardHeader = new PacketDefinition(0x50,
            new VariablePacketLength(), "BoardHeader");

        public static readonly PacketDefinition BoardMessage = new PacketDefinition(0x51,
            new VariablePacketLength(), "BoardMessage");

        public static readonly PacketDefinition BoardPostMessage = new PacketDefinition(0x52,
            new VariablePacketLength(), "BoardPostMessage");

        public static readonly PacketDefinition RejectCharacterLogon = new PacketDefinition(0x53,
            new StaticPacketLength(2), "RejectCharacterLogon");

        public static readonly PacketDefinition<PlaySoundEffectPacket> PlaySoundEffect = new PacketDefinition<PlaySoundEffectPacket>(0x54,
            new StaticPacketLength(0x0C), "PlaySoundEffect", () => new PlaySoundEffectPacket());

        public static readonly PacketDefinition LoginComplete = new PacketDefinition(0x55,
            new StaticPacketLength(1), "LoginComplete");

        public static readonly PacketDefinition MapPacket = new PacketDefinition(0x56,
            new StaticPacketLength(0x0b), "MapPacket");

        public static readonly PacketDefinition UpdateRegions = new PacketDefinition(0x57,
            new StaticPacketLength(0x6E), "UpdateRegions");

        public static readonly PacketDefinition AddRegion = new PacketDefinition(0x58,
            new StaticPacketLength(0x6A), "AddRegion");

        public static readonly PacketDefinition NewContextFx = new PacketDefinition(0x59,
            new VariablePacketLength(), "NewContextFx");

        public static readonly PacketDefinition UpdateContextFx = new PacketDefinition(0x5A,
            new VariablePacketLength(), "UpdateContextFx");

        public static readonly PacketDefinition Time = new PacketDefinition(0x5B,
            new StaticPacketLength(4), "Time");

        public static readonly PacketDefinition RestartVersion = new PacketDefinition(0x5C,
            new StaticPacketLength(2), "RestartVersion");

        public static readonly PacketDefinition LoginCharacter = new PacketDefinition(0x5D,
            new StaticPacketLength(0x49), "LoginCharacter");

        public static readonly PacketDefinition<ServerListingPacket> ServerListing 
            = new PacketDefinition<ServerListingPacket>(0x5E, new VariablePacketLength(), "ServerListing", () => new ServerListingPacket());

        public static readonly PacketDefinition ServerListAddEntry = new PacketDefinition(0x5F,
            new StaticPacketLength(0x31), "ServerListAddEntry");

        public static readonly PacketDefinition ServerListRemoveEntry = new PacketDefinition(0x60,
            new StaticPacketLength(5), "ServerListRemoveEntry");

        public static readonly PacketDefinition RemoveStaticObject = new PacketDefinition(0x61,
            new StaticPacketLength(9), "RemoveStaticObject");

        public static readonly PacketDefinition MoveStaticObject = new PacketDefinition(0x62,
            new StaticPacketLength(0x0F), "MoveStaticObject");

        public static readonly PacketDefinition LoadArea = new PacketDefinition(0x63,
            new StaticPacketLength(0x0D), "LoadArea");

        public static readonly PacketDefinition LoadAreaRequest = new PacketDefinition(0x64,
            new StaticPacketLength(1), "LoadAreaRequest");

        public static readonly PacketDefinition SetWeather = new PacketDefinition(0x65,
            new StaticPacketLength(4), "SetWeather");

        public static readonly PacketDefinition BooksPages = new PacketDefinition(0x66,
            new VariablePacketLength(), "BooksPages");

        public static readonly PacketDefinition Simped = new PacketDefinition(0x67,
            new StaticPacketLength(0x15), "Simped");

        public static readonly PacketDefinition ScriptAttach = new PacketDefinition(0x68,
            new VariablePacketLength(), "ScriptAttach");

        public static readonly PacketDefinition Friends = new PacketDefinition(0x69,
            new VariablePacketLength(), "Friends");

        public static readonly PacketDefinition NotifyFriend = new PacketDefinition(0x6A,
            new StaticPacketLength(3), "NotifyFriend");

        public static readonly PacketDefinition<TargetCursorPacket> TargetCursor = new PacketDefinition<TargetCursorPacket>(0x6C,
            new StaticPacketLength(0x13), "TargetCursor", () => new TargetCursorPacket());

        public static readonly PacketDefinition PlayMidiMusic = new PacketDefinition(0x6D,
            new StaticPacketLength(3), "PlayMidiMusic");

        public static readonly PacketDefinition CharacterAnimation = new PacketDefinition(0x6E,
            new StaticPacketLength(0x0E), "CharacterAnimation");

        public static readonly PacketDefinition SecureTraiding = new PacketDefinition(0x6F,
            new VariablePacketLength(), "SecureTraiding");

        public static readonly PacketDefinition<GraphicalEffectPacket> GraphicalEffect = new PacketDefinition<GraphicalEffectPacket>(0x70,
            new StaticPacketLength(0x1c), "GraphicalEffect", () => new GraphicalEffectPacket());

        public static readonly PacketDefinition BulletinBoardMessage = new PacketDefinition(0x71,
            new VariablePacketLength(), "BulletinBoardMessage");

        public static readonly PacketDefinition RequestWarMode = new PacketDefinition(0x72,
            new StaticPacketLength(5), "RequestWarMode");

        public static readonly PacketDefinition PingMessage = new PacketDefinition(0x73,
            new StaticPacketLength(2), "PingMessage");

        public static readonly PacketDefinition OpenBuyWindow = new PacketDefinition(0x74,
            new VariablePacketLength(), "OpenBuyWindow");

        public static readonly PacketDefinition RenameCharacter = new PacketDefinition(0x75,
            new StaticPacketLength(0x23), "RenameCharacter");

        public static readonly PacketDefinition NewSubserver = new PacketDefinition(0x76,
            new StaticPacketLength(0x10), "NewSubserver");

        public static readonly PacketDefinition<UpdatePlayerPacket> UpdatePlayer = new PacketDefinition<UpdatePlayerPacket>(0x77,
            new StaticPacketLength(0x11), "UpdatePlayer", () => new UpdatePlayerPacket());

        public static readonly PacketDefinition<DrawObjectPacket> DrawObject = new PacketDefinition<DrawObjectPacket>(0x78,
            new VariablePacketLength(), "DrawObject", () => new DrawObjectPacket());
        public static readonly PacketDefinition<DrawObjectPacket> DrawObject7033 = new PacketDefinition<DrawObjectPacket>(0x78,
            new VariablePacketLength(), "DrawObject", () => new DrawObjectPacket7033());

        public static readonly PacketDefinition ResourceQuery = new PacketDefinition(0x79,
            new StaticPacketLength(9), "ResourceQuery");

        public static readonly PacketDefinition ResourceData = new PacketDefinition(0x7A,
            new VariablePacketLength(), "ResourceData");

        public static readonly PacketDefinition Unknown7B = new PacketDefinition(0x7B,
            new StaticPacketLength(0x02), "Unknown7B");

        public static readonly PacketDefinition<OpenDialogBoxPacket> OpenDialogBox = new PacketDefinition<OpenDialogBoxPacket>(0x7C,
            new VariablePacketLength(), "OpenDialogBox", () => new OpenDialogBoxPacket());

        public static readonly PacketDefinition ResponseToDialogBox = new PacketDefinition(0x7D,
            new StaticPacketLength(0x0D), "ResponseToDialogBox");

        public static readonly PacketDefinition GodViewQuery = new PacketDefinition(0x7E,
            new StaticPacketLength(0x02), "GodViewQuery");

        public static readonly PacketDefinition GodViewData = new PacketDefinition(0x7F,
            new VariablePacketLength(), "GodViewData");

        public static readonly PacketDefinition LoginRequest = new PacketDefinition(0x80,
            new StaticPacketLength(0x3e), "LoginRequest");

        public static readonly PacketDefinition ChangeCharacter = new PacketDefinition(0x81,
            new VariablePacketLength(), "ChangeCharacter");

        public static readonly PacketDefinition LoginDenied = new PacketDefinition(0x82,
            new StaticPacketLength(2), "LoginDenied");

        public static readonly PacketDefinition DeleteCharacter = new PacketDefinition(0x83,
            new StaticPacketLength(0x27), "DeleteCharacter");

        public static readonly PacketDefinition ChangePassword = new PacketDefinition(0x84,
            new StaticPacketLength(0x45), "ChangePassword");

        public static readonly PacketDefinition CharacterListNotification = new PacketDefinition(0x85,
            new StaticPacketLength(2), "CharacterListNotification");

        public static readonly PacketDefinition ResendCharactersAfterDelete = new PacketDefinition(0x86,
            new VariablePacketLength(), "ResendCharactersAfterDelete");

        public static readonly PacketDefinition SendResources = new PacketDefinition(0x87,
            new VariablePacketLength(), "SendResources");

        public static readonly PacketDefinition OpenPaperdoll = new PacketDefinition(0x88,
            new StaticPacketLength(0x42), "OpenPaperdoll");

        public static readonly PacketDefinition CorpseClothing = new PacketDefinition(0x89,
            new VariablePacketLength(), "CorpseClothing");

        public static readonly PacketDefinition TriggerEdit = new PacketDefinition(0x8a,
            new VariablePacketLength(), "TriggerEdit");

        public static readonly PacketDefinition DisplaySign = new PacketDefinition(0x8b,
            new VariablePacketLength(), "DisplaySign");

        public static readonly PacketDefinition<ConnectToGameServerPacket> ConnectToGameServer = new PacketDefinition<ConnectToGameServerPacket>(0x8C,
            new StaticPacketLength(0x0B), "ConnectToGameServer", () => new ConnectToGameServerPacket());

        public static readonly PacketDefinition CharacterCreation = new PacketDefinition(0x8D,
            new VariablePacketLength(), "CharacterCreation");

        public static readonly PacketDefinition MoveCharacter = new PacketDefinition(0x8E,
            new VariablePacketLength(), "MoveCharacter");

        public static readonly PacketDefinition Unknown8F = new PacketDefinition(0x8F,
            new VariablePacketLength(), "Unknown8F");

        public static readonly PacketDefinition<MapMessagePacket> MapMessage = new PacketDefinition<MapMessagePacket>(0x90,
            new StaticPacketLength(0x13), "MapMessage", () => new MapMessagePacket());

        public static readonly PacketDefinition GameServerLoginRequest = new PacketDefinition(0x91,
            new StaticPacketLength(0x41), "GameServerLoginRequest");

        public static readonly PacketDefinition UpdateMultiData = new PacketDefinition(0x92,
            new VariablePacketLength(), "UpdateMultiData");

        public static readonly PacketDefinition BookHeader = new PacketDefinition(0x93,
            new StaticPacketLength(0x63), "BookHeader");

        public static readonly PacketDefinition UpdateSkillsData = new PacketDefinition(0x94,
            new VariablePacketLength(), "UpdateSkillsData");

        public static readonly PacketDefinition DyeWindow = new PacketDefinition(0x95,
            new StaticPacketLength(9), "DyeWindow");

        public static readonly PacketDefinition GameCentralMonitor = new PacketDefinition(0x96,
            new VariablePacketLength(), "GameCentralMonitor");

        public static readonly PacketDefinition MovePlayer = new PacketDefinition(0x97,
            new StaticPacketLength(2), "MovePlayer");

        public static readonly PacketDefinition AllNamesPacket = new PacketDefinition(0x98,
            new VariablePacketLength(), "AllNames");

        public static readonly PacketDefinition GiveBoatHousePlacementView = new PacketDefinition(0x99,
            new StaticPacketLength(0x1A), "GiveBoatHousePlacementView");
        public static readonly PacketDefinition GiveBoatHousePlacementView7090 = new PacketDefinition(0x99,
            new StaticPacketLength(0x1E), "GiveBoatHousePlacementView");

        public static readonly PacketDefinition ConsoleEntryPrompt = new PacketDefinition(0x9A,
            new VariablePacketLength(), "ConsoleEntryPrompt");

        public static readonly PacketDefinition RequestHelp = new PacketDefinition(0x9B,
            new StaticPacketLength(0x102), "RequestHelp");

        public static readonly PacketDefinition RequestAssistance = new PacketDefinition(0x9C,
            new StaticPacketLength(0x135), "RequestAssistance");

        public static readonly PacketDefinition GMSingle = new PacketDefinition(0x9D,
            new StaticPacketLength(0x33), "GMSingle");

        public static readonly PacketDefinition SellList = new PacketDefinition(0x9E,
            new VariablePacketLength(), "SellList");

        public static readonly PacketDefinition SellListReply = new PacketDefinition(0x9F,
            new VariablePacketLength(), "SellListReply");

        public static readonly PacketDefinition<SelectServerRequest> SelectServerRequest 
            = new PacketDefinition<SelectServerRequest>(0xA0, new StaticPacketLength(3), "SelectServerRequest", () => new Client.SelectServerRequest());

        public static readonly PacketDefinition<UpdateCurrentHealthPacket> UpdateCurrentHealth =
            new PacketDefinition<UpdateCurrentHealthPacket>(0xA1,
                new StaticPacketLength(9), "UpdateCurrentHealth", () => new UpdateCurrentHealthPacket());

        public static readonly PacketDefinition<UpdateCurrentManaPacket> UpdateCurrentMana = new PacketDefinition<UpdateCurrentManaPacket>(0xA2,
            new StaticPacketLength(9), "UpdateCurrentMana", () => new UpdateCurrentManaPacket());

        public static readonly PacketDefinition<UpdateCurrentStaminaPacket> UpdateCurrentStamina =
            new PacketDefinition<UpdateCurrentStaminaPacket>(0xA3,
                new StaticPacketLength(9), "UpdateCurrentStamina", () => new UpdateCurrentStaminaPacket());

        public static readonly PacketDefinition ClientSpy = new PacketDefinition(0xA4,
            new StaticPacketLength(0x95), "ClientSpy");

        public static readonly PacketDefinition OpenWebBrowser = new PacketDefinition(0xA5,
            new VariablePacketLength(), "OpenWebBrowser");

        public static readonly PacketDefinition TipNoticeWindow = new PacketDefinition(0xA6,
            new VariablePacketLength(), "TipNoticeWindow");

        public static readonly PacketDefinition RequestTipNoticeWindow = new PacketDefinition(0xA7,
            new StaticPacketLength(4), "RequestTipNoticeWindow");

        public static readonly PacketDefinition<GameServerListPacket> GameServerList = new PacketDefinition<GameServerListPacket>(0xA8,
            new VariablePacketLength(), "GameServerList", () => new GameServerListPacket());

        public static readonly PacketDefinition CharactersStartingLocations = new PacketDefinition(0xA9,
            new VariablePacketLength(), "CharactersStartingLocations");

        public static readonly PacketDefinition<AllowRefuseAttackPacket> AllowRefuseAttack = new PacketDefinition<AllowRefuseAttackPacket>(0xAA,
            new StaticPacketLength(5), "AllowRefuseAttack", () => new AllowRefuseAttackPacket());

        public static readonly PacketDefinition GumpTextEntryDialog = new PacketDefinition(0xAB,
            new VariablePacketLength(), "GumpTextEntryDialog");

        public static readonly PacketDefinition GumpTextEntryDialogReply = new PacketDefinition(0xAC,
            new VariablePacketLength(), "GumpTextEntryDialogReply");

        public static readonly PacketDefinition<SpeechRequest> SpeechRequest = new PacketDefinition<SpeechRequest>(0xAD,
            new VariablePacketLength(), "SpeechRequest", () => new SpeechRequest());

        public static readonly PacketDefinition<SpeechMessagePacket> SpeechMessage = new PacketDefinition<SpeechMessagePacket>(0xAE,
            new VariablePacketLength(), "SpeechMessage", () => new SpeechMessagePacket());

        public static readonly PacketDefinition DisplayDeathAction = new PacketDefinition(0xAF,
            new StaticPacketLength(0x0D), "DisplayDeathAction");

        public static readonly PacketDefinition<SendGumpMenuDialogPacket> SendGumpMenuDialog = new PacketDefinition<SendGumpMenuDialogPacket>(0xB0,
            new VariablePacketLength(), "SendGumpMenuDialog", () => new SendGumpMenuDialogPacket());

        public static readonly PacketDefinition<GumpMenuSelectionRequest> GumpMenuSelection = new PacketDefinition<GumpMenuSelectionRequest>(0xB1,
            new VariablePacketLength(), "GumpMenuSelection", () => new GumpMenuSelectionRequest());

        public static readonly PacketDefinition ChatMessage = new PacketDefinition(0xB2,
            new VariablePacketLength(), "ChatMessage");

        public static readonly PacketDefinition ChatText = new PacketDefinition(0xB3,
            new VariablePacketLength(), "ChatText");

        public static readonly PacketDefinition TargetObjectList = new PacketDefinition(0xB4,
            new VariablePacketLength(), "TargetObjectList");

        public static readonly PacketDefinition OpenChatWindow = new PacketDefinition(0xB5,
            new StaticPacketLength(64), "OpenChatWindow");

        public static readonly PacketDefinition SendHelpTipRequest = new PacketDefinition(0xB6,
            new StaticPacketLength(9), "SendHelpTipRequest");

        public static readonly PacketDefinition HelpTileData = new PacketDefinition(0xB7,
            new VariablePacketLength(), "HelpTileData");

        public static readonly PacketDefinition RequestCharProfile = new PacketDefinition(0xB8,
            new VariablePacketLength(), "RequestCharProfile");

        public static readonly PacketDefinition EnableLockedClientFeatures = new PacketDefinition(0xB9,
            new StaticPacketLength(3), "EnableLockedClientFeatures");
        public static readonly PacketDefinition EnableLockedClientFeaturesSince6_0_14_2 = new PacketDefinition(0xB9,
            new StaticPacketLength(5), "EnableLockedClientFeatures");

        public static readonly PacketDefinition<QuestArrowPacket> QuestArrow = new PacketDefinition<QuestArrowPacket>(0xBA,
            new StaticPacketLength(6), "QuestArrow", () => new QuestArrowPacket());
        public static readonly PacketDefinition<QuestArrowPacket> QuestArrow7090 = new PacketDefinition<QuestArrowPacket>(0xBA,
            new StaticPacketLength(10), "QuestArrow", () => new QuestArrowPacket());

        public static readonly PacketDefinition UltimaMessenger = new PacketDefinition(0xBB,
            new StaticPacketLength(9), "UltimaMessenger");

        public static readonly PacketDefinition SeasonalInformation = new PacketDefinition(0xBC,
            new StaticPacketLength(3), "SeasonalInformation");

        public static readonly PacketDefinition ClientVersion = new PacketDefinition(0xBD,
            new VariablePacketLength(), "ClientVersion");

        public static readonly PacketDefinition AssistVersion = new PacketDefinition(0xBE,
            new VariablePacketLength(), "AssistVersion");

        public static readonly PacketDefinition GeneralInformationPacket = new PacketDefinition(0xBF,
            new VariablePacketLength(), "GeneralInformationPacket");

        public static readonly PacketDefinition GraphicalEffect2 = new PacketDefinition(0xC0,
            new StaticPacketLength(0x24), "GraphicalEffect2");

        public static readonly PacketDefinition<ClilocMessagePacket> ClilocMessage = new PacketDefinition<ClilocMessagePacket>(0xC1,
            new VariablePacketLength(), "ClilocMessage", () => new ClilocMessagePacket());

        public static readonly PacketDefinition UnicodeTextEntry = new PacketDefinition(0xC2,
            new VariablePacketLength(), "UnicodeTextEntry");

        public static readonly PacketDefinition UnknownC3 = new PacketDefinition(0xC3,
            new VariablePacketLength(), "UnknownC3");

        public static readonly PacketDefinition Semivisible = new PacketDefinition(0xC4,
            new StaticPacketLength(6), "Semivisible");

        public static readonly PacketDefinition InvalidMapRequest = new PacketDefinition(0xC5,
            new StaticPacketLength(0xcb), "InvalidMapRequest");

        public static readonly PacketDefinition InvalidMapEnable = new PacketDefinition(0xC6,
            new StaticPacketLength(1), "InvalidMapEnable");

        public static readonly PacketDefinition ParticleEffect = new PacketDefinition(0xC7,
            new StaticPacketLength(0x31), "ParticleEffect");

        public static readonly PacketDefinition ClientViewRange = new PacketDefinition(0xC8,
            new StaticPacketLength(2), "ClientViewRange");

        public static readonly PacketDefinition TripTime = new PacketDefinition(0xC9,
            new StaticPacketLength(6), "TripTime");

        public static readonly PacketDefinition UTripTime = new PacketDefinition(0xCA,
            new StaticPacketLength(6), "UTripTime");

        public static readonly PacketDefinition GlobalQueueCount = new PacketDefinition(0xCB,
            new StaticPacketLength(7), "GlobalQueueCount");

        public static readonly PacketDefinition<ClilocMessageAffixPacket> ClilocMessageAffix = new PacketDefinition<ClilocMessageAffixPacket>(0xCC,
            new VariablePacketLength(), "ClilocMessageAffix", () => new ClilocMessageAffixPacket());

        public static readonly PacketDefinition UnknownCD = new PacketDefinition(0xCD,
            new StaticPacketLength(1), "UnknownCD");

        public static readonly PacketDefinition UnknownCE = new PacketDefinition(0xCE,
            new VariablePacketLength(), "UnknownCE");

        public static readonly PacketDefinition UnknownCF = new PacketDefinition(0xCF,
            new StaticPacketLength(0x4E), "UnknownCF");

        public static readonly PacketDefinition ConfigurationFile = new PacketDefinition(0xD0,
            new VariablePacketLength(), "ConfigurationFile");

        public static readonly PacketDefinition LogoutStatus = new PacketDefinition(0xD1,
            new StaticPacketLength(2), "LogoutStatus");

        public static readonly PacketDefinition ExtendedDrawGamePlayer = new PacketDefinition(0xD2,
            new StaticPacketLength(0x19), "ExtendedDrawGamePlayer");

        public static readonly PacketDefinition ExtendedDrawObject = new PacketDefinition(0xD3,
            new VariablePacketLength(), "ExtendedDrawObject");

        public static readonly PacketDefinition BookHeaderNew = new PacketDefinition(0xD4,
            new VariablePacketLength(), "BookHeaderNew");

        public static readonly PacketDefinition UnknownD5 = new PacketDefinition(0xD5,
            new VariablePacketLength(), "UnknownD5");

        public static readonly PacketDefinition MegaCliloc = new PacketDefinition(0xD6,
            new VariablePacketLength(), "MegaCliloc");

        public static readonly PacketDefinition GenericAosCommands = new PacketDefinition(0xD7,
            new VariablePacketLength(), "GenericAosCommands");

        public static readonly PacketDefinition SendCustomHouse = new PacketDefinition(0xD8,
            new VariablePacketLength(), "SendCustomHouse");

        public static readonly PacketDefinition SpyOnClient = new PacketDefinition(0xD9,
            new StaticPacketLength(0x10c), "SpyOnClient");

        public static readonly PacketDefinition MahjongGameCommand = new PacketDefinition(0xDA,
            new VariablePacketLength(), "MahjongGameCommand");

        public static readonly PacketDefinition CharacterTransferLog = new PacketDefinition(0xDB,
            new VariablePacketLength(), "CharacterTransferLog");

        public static readonly PacketDefinition SecondAgeRevision = new PacketDefinition(0xDC,
            new StaticPacketLength(9), "SecondAgeRevision");

        public static readonly PacketDefinition<CompressedGumpPacket> CompressedGump = new PacketDefinition<CompressedGumpPacket>(0xDD,
            new VariablePacketLength(), "CompressedGump", () => new CompressedGumpPacket());

        public static readonly PacketDefinition UpdateMobileStatus = new PacketDefinition(0xDE,
            new VariablePacketLength(), "UpdateMobileStatus");

        public static readonly PacketDefinition BuffSystem = new PacketDefinition(0xDF,
            new VariablePacketLength(), "BuffSystem");

        public static readonly PacketDefinition BugReport = new PacketDefinition(0xE0,
            new VariablePacketLength(), "BugReport");

        public static readonly PacketDefinition ClientType = new PacketDefinition(0xE1,
            new StaticPacketLength(9), "ClientType");
        public static readonly PacketDefinition ClientType5090 = new PacketDefinition(0xE1,
            new VariablePacketLength(), "ClientType");

        public static readonly PacketDefinition NewCharacterAnimation = new PacketDefinition(0xE2,
            new StaticPacketLength(10), "NewCharacterAnimation");

        public static readonly PacketDefinition KrEncryptionResponse = new PacketDefinition(0xE3,
            new StaticPacketLength(77), "KrEncryptionResponse");
        public static readonly PacketDefinition KrEncryptionResponse6013 = new PacketDefinition(0xE3,
            new VariablePacketLength(), "KrEncryptionResponse");

        public static readonly PacketDefinition UnknownE4 = new PacketDefinition(0xE4,
            new VariablePacketLength(), "UnknownE4");
        public static readonly PacketDefinition UnknownE5 = new PacketDefinition(0xE5,
            new VariablePacketLength(), "UnknownE5");
        public static readonly PacketDefinition UnknownE6 = new PacketDefinition(0xE6,
            new StaticPacketLength(5), "UnknownE6");
        public static readonly PacketDefinition UnknownE7 = new PacketDefinition(0xE7,
            new StaticPacketLength(0x0C), "UnknownE7");
        public static readonly PacketDefinition UnknownE8 = new PacketDefinition(0xE8,
            new StaticPacketLength(0x0D), "UnknownE8");
        public static readonly PacketDefinition UnknownE9 = new PacketDefinition(0xE9,
            new StaticPacketLength(0x4B), "UnknownE9");
        public static readonly PacketDefinition UnknownEA = new PacketDefinition(0xEA,
            new StaticPacketLength(0x03), "UnknownEA");
        public static readonly PacketDefinition UnknownEB = new PacketDefinition(0xEB,
            new VariablePacketLength(), "UnknownEB");

        public static readonly PacketDefinition EquipMacro = new PacketDefinition(0xEC,
            new VariablePacketLength(), "EquipMacro");

        public static readonly PacketDefinition UnequipMacro = new PacketDefinition(0xED,
            new VariablePacketLength(), "UnequipMacro");

        public static readonly PacketDefinition UnknownEE = new PacketDefinition(0xEE,
            new VariablePacketLength(), "UnknownEE");

        public static readonly PacketDefinition KrClientSeed = new PacketDefinition(0xEF,
            new StaticPacketLength(0x15), "KrClientSeed");

        public static readonly PacketDefinition KrriosClientSpecial = new PacketDefinition(0xF0,
            new VariablePacketLength(), "KrriosClientSpecial");

        public static readonly PacketDefinition TimeSynchronizationRequest = new PacketDefinition(0xF1,
            new VariablePacketLength(), "TimeSynchronizationRequest");
        public static readonly PacketDefinition TimeSynchronizationRequest6060 = new PacketDefinition(0xF1,
            new StaticPacketLength(9), "TimeSynchronizationRequest");
        public static readonly PacketDefinition TimeSynchronizationRequest7090 = new PacketDefinition(0xF1,
            new StaticPacketLength(9), "TimeSynchronizationRequest");

        public static readonly PacketDefinition TimeSynchronizationResponse = new PacketDefinition(0xF2,
            new VariablePacketLength(), "TimeSynchronizationResponse");
        public static readonly PacketDefinition TimeSynchronizationResponse7090 = new PacketDefinition(0xF2,
            new StaticPacketLength(0x19), "TimeSynchronizationResponse");

        public static readonly PacketDefinition SecondAgeObjectInformation = new PacketDefinition(0xF3,
            new StaticPacketLength(24), "SecondAgeObjectInformation");
        public static readonly PacketDefinition<SecondAgeObjectInformationPacket> SecondAgeObjectInformation7090
            = new PacketDefinition<SecondAgeObjectInformationPacket>(0xF3, new StaticPacketLength(26),
                "SecondAgeObjectInformation", () => new SecondAgeObjectInformationPacket());

        public static readonly PacketDefinition UnknownF4 = new PacketDefinition(0xF4,
            new VariablePacketLength(), "UnknownF4");

        public static readonly PacketDefinition NewMapMessage = new PacketDefinition(0xF5,
            new StaticPacketLength(0x15), "NewMapMessage");

        public static readonly PacketDefinition BoatMoving = new PacketDefinition(0xF6,
            new VariablePacketLength(), "BoatMoving");

        public static readonly PacketDefinition UnknownF7 = new PacketDefinition(0xF7,
            new VariablePacketLength(), "UnknownF7");

        public static readonly PacketDefinition CharacterCreation70160 = new PacketDefinition(0xF8,
           new StaticPacketLength(0x6a), "CharacterCreation");

        public static readonly PacketDefinition UnknownF9 = new PacketDefinition(0xF9,
            new VariablePacketLength(), "UnknownF9");
        public static readonly PacketDefinition UnknownFA = new PacketDefinition(0xFA,
            new VariablePacketLength(), "UnknownFA");
        public static readonly PacketDefinition UnknownFB = new PacketDefinition(0xFB,
            new VariablePacketLength(), "UnknownFB");

        public static readonly PacketDefinition CrossUOMessages = new PacketDefinition(0xFC,
            new VariablePacketLength(), "UnknownFC");

        public static readonly PacketDefinition UnknownFD = new PacketDefinition(0xFD,
            new VariablePacketLength(), "UnknownFD");

        public static readonly PacketDefinition RazorHandshake = new PacketDefinition(0xFE,
            new StaticPacketLength(8), "RazorHandshake");

        public static readonly PacketDefinition UnknownFF = new PacketDefinition(0xFF,
            new VariablePacketLength(), "UnknownFF");
    }
}