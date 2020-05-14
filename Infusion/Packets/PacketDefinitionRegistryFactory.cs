using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets
{
    internal static class PacketDefinitionRegistryFactory
    {
        private static readonly Version[] protocolVersions =
        {
            new Version(0, 0, 0, 0),
            new Version(3, 0, 0, 0),
            new Version(5, 0, 0, 0),
            new Version(5, 0, 9, 0),
            new Version(6, 0, 6, 0),
            new Version(6, 0, 1, 3),
            new Version(6, 0, 1, 7),
            new Version(6, 0, 14, 2),
            new Version(7, 0, 9, 0),
            new Version(7, 0, 16, 0),
            new Version(7, 0, 18, 0),
            new Version(7, 0, 33, 0)
        };

        public static Version GetProtocolVersion(Version clientVersion)
        {
            var protocolVersion = protocolVersions.LastOrDefault(v => clientVersion >= v);

            if (protocolVersion == null)
                throw new ArgumentException($"Unknown protocol version for client version {clientVersion}", nameof(clientVersion));

            return protocolVersion;
        }

        public static void CreateClassicClient(PacketDefinitionRegistry registry, Version version)
        {
            registry.Reset();

            registry.Register(PacketDefinitions.AddItemToContainer);
            registry.Register(PacketDefinitions.AttackRequest);
            registry.Register(PacketDefinitions.CharMoveRejection);
            registry.Register(PacketDefinitions.CharacterLocaleAndBody);
            registry.Register(PacketDefinitions.CharacterMoveAck);
            registry.Register(PacketDefinitions.CharactersStartingLocations);
            registry.Register(PacketDefinitions.ClientSpy);
            registry.Register(PacketDefinitions.ClientVersion);
            registry.Register(PacketDefinitions.ClientViewRange);
            registry.Register(PacketDefinitions.TripTime);
            registry.Register(PacketDefinitions.UTripTime);
            registry.Register(PacketDefinitions.CreateCharacterRequest);
            registry.Register(PacketDefinitions.Damage);
            registry.Register(PacketDefinitions.DeleteObject);
            registry.Register(PacketDefinitions.DisconnectNotification);
            registry.Register(PacketDefinitions.DraggingOfItem);
            registry.Register(PacketDefinitions.DrawContainer);
            registry.Register(PacketDefinitions.DrawGamePlayer);
            registry.Register(PacketDefinitions.DrawObject);
            registry.Register(PacketDefinitions.ResourceQuery);
            registry.Register(PacketDefinitions.EnableLockedClientFeatures);
            registry.Register(PacketDefinitions.Explosion);
            registry.Register(PacketDefinitions.GameServerList);
            registry.Register(PacketDefinitions.GameServerLoginRequest);
            registry.Register(PacketDefinitions.GeneralInformationPacket);
            registry.Register(PacketDefinitions.GetClientStatus);
            registry.Register(PacketDefinitions.ResourceType);
            registry.Register(PacketDefinitions.ResourceTileData);
            registry.Register(PacketDefinitions.MoveObject);
            registry.Register(PacketDefinitions.ToggleHackMover);
            registry.Register(PacketDefinitions.GodModeRequest);
            registry.Register(PacketDefinitions.GraphicalEffect);
            registry.Register(PacketDefinitions.HealthBarStatusUpdate);
            registry.Register(PacketDefinitions.AddScript);
            registry.Register(PacketDefinitions.EditNpcSpeech);
            registry.Register(PacketDefinitions.KickPlayer);
            registry.Register(PacketDefinitions.LoginSeed);
            registry.Register(PacketDefinitions.ExtendedLoginSeed);
            registry.Register(PacketDefinitions.LoginCharacter);
            registry.Register(PacketDefinitions.LoginComplete);
            registry.Register(PacketDefinitions.LoginDenied);
            registry.Register(PacketDefinitions.LoginRequest);
            registry.Register(PacketDefinitions.ChangeCharacter);
            registry.Register(PacketDefinitions.MoveRequest);
            registry.Register(PacketDefinitions.NewHealthBarStatusUpdate);
            registry.Register(PacketDefinitions.ObjectInfo);
            registry.Register(PacketDefinitions.PauseClient);
            registry.Register(PacketDefinitions.PingMessage);
            registry.Register(PacketDefinitions.PlaySoundEffect);
            registry.Register(PacketDefinitions.RejectMoveItemRequest);
            registry.Register(PacketDefinitions.RequestWarMode);
            registry.Register(PacketDefinitions.SelectServerRequest);
            registry.Register(PacketDefinitions.SendSkills);
            registry.Register(PacketDefinitions.SendSpeech);
            registry.Register(PacketDefinitions.SpeechMessage);
            registry.Register(PacketDefinitions.SpeechRequest);
            registry.Register(PacketDefinitions.StatusBarInfo);
            registry.Register(PacketDefinitions.TalkRequest);
            registry.Register(PacketDefinitions.TipNoticeWindow);
            registry.Register(PacketDefinitions.ConnectToGameServer);
            registry.Register(PacketDefinitions.UpdateMultiData);
            registry.Register(PacketDefinitions.DropItemFailed);
            registry.Register(PacketDefinitions.DropItemApproved);
            registry.Register(PacketDefinitions.Blood);
            registry.Register(PacketDefinitions.ToggleGodMode);
            registry.Register(PacketDefinitions.MobAttributes);
            registry.Register(PacketDefinitions.WornItem);
            registry.Register(PacketDefinitions.FightOccuring);
            registry.Register(PacketDefinitions.AttackOk);
            registry.Register(PacketDefinitions.AttackEnded);
            registry.Register(PacketDefinitions.PersonalLightLevel);
            registry.Register(PacketDefinitions.OverallLightLevel);
            registry.Register(PacketDefinitions.RejectCharacterLogon);
            registry.Register(PacketDefinitions.Time);
            registry.Register(PacketDefinitions.SetWeather);
            registry.Register(PacketDefinitions.BooksPages);
            registry.Register(PacketDefinitions.Simped);
            registry.Register(PacketDefinitions.PlayMidiMusic);
            registry.Register(PacketDefinitions.CharacterAnimation);
            registry.Register(PacketDefinitions.OpenBuyWindow);
            registry.Register(PacketDefinitions.NewSubserver);
            registry.Register(PacketDefinitions.UpdatePlayer);
            registry.Register(PacketDefinitions.OpenDialogBox);
            registry.Register(PacketDefinitions.ResendCharactersAfterDelete);
            registry.Register(PacketDefinitions.SendResources);
            registry.Register(PacketDefinitions.OpenPaperdoll);
            registry.Register(PacketDefinitions.CorpseClothing);
            registry.Register(PacketDefinitions.TriggerEdit);
            registry.Register(PacketDefinitions.DisplaySign);
            registry.Register(PacketDefinitions.MapMessage);
            registry.Register(PacketDefinitions.MovePlayer);
            registry.Register(PacketDefinitions.RequestAssistance);
            registry.Register(PacketDefinitions.GMSingle);
            registry.Register(PacketDefinitions.SellList);
            registry.Register(PacketDefinitions.UpdateCurrentHealth);
            registry.Register(PacketDefinitions.UpdateCurrentMana);
            registry.Register(PacketDefinitions.UpdateCurrentStamina);
            registry.Register(PacketDefinitions.OpenWebBrowser);
            registry.Register(PacketDefinitions.AllowRefuseAttack);
            registry.Register(PacketDefinitions.GumpTextEntryDialog);
            registry.Register(PacketDefinitions.DisplayDeathAction);
            registry.Register(PacketDefinitions.SendGumpMenuDialog);
            registry.Register(PacketDefinitions.ChatMessage);
            registry.Register(PacketDefinitions.HelpTileData);
            registry.Register(PacketDefinitions.QuestArrow);
            registry.Register(PacketDefinitions.SeasonalInformation);
            registry.Register(PacketDefinitions.GraphicalEffect2);
            registry.Register(PacketDefinitions.ClilocMessage);
            registry.Register(PacketDefinitions.Semivisible);
            registry.Register(PacketDefinitions.InvalidMapEnable);
            registry.Register(PacketDefinitions.GlobalQueueCount);
            registry.Register(PacketDefinitions.ClilocMessageAffix);
            registry.Register(PacketDefinitions.UnknownCD);
            registry.Register(PacketDefinitions.UnknownCE);
            registry.Register(PacketDefinitions.UnknownCF);
            registry.Register(PacketDefinitions.ExtendedDrawGamePlayer);
            registry.Register(PacketDefinitions.ExtendedDrawObject);
            registry.Register(PacketDefinitions.SendCustomHouse);
            registry.Register(PacketDefinitions.CharacterTransferLog);
            registry.Register(PacketDefinitions.SecondAgeRevision);
            registry.Register(PacketDefinitions.CompressedGump);
            registry.Register(PacketDefinitions.UpdateMobileStatus);
            registry.Register(PacketDefinitions.BuffSystem);
            registry.Register(PacketDefinitions.BugReport);
            registry.Register(PacketDefinitions.NewCharacterAnimation);
            registry.Register(PacketDefinitions.KrEncryptionResponse);
            registry.Register(PacketDefinitions.SecondAgeObjectInformation);
            registry.Register(PacketDefinitions.UnknownE4);
            registry.Register(PacketDefinitions.UnknownE5);
            registry.Register(PacketDefinitions.UnknownE6);
            registry.Register(PacketDefinitions.UnknownE7);
            registry.Register(PacketDefinitions.UnknownE8);
            registry.Register(PacketDefinitions.UnknownE9);
            registry.Register(PacketDefinitions.UnknownEA);
            registry.Register(PacketDefinitions.UnknownEB);
            registry.Register(PacketDefinitions.EquipMacro);
            registry.Register(PacketDefinitions.UnequipMacro);
            registry.Register(PacketDefinitions.UnknownEE);
            registry.Register(PacketDefinitions.KrClientSeed);
            registry.Register(PacketDefinitions.NewMapMessage);
            registry.Register(PacketDefinitions.BoatMoving);
            registry.Register(PacketDefinitions.UnknownF7);
            registry.Register(PacketDefinitions.DoubleClick);
            registry.Register(PacketDefinitions.UnknownF4);
            registry.Register(PacketDefinitions.PickUpItem);
            registry.Register(PacketDefinitions.Edit);
            registry.Register(PacketDefinitions.EditNpcData);
            registry.Register(PacketDefinitions.EditHueData);
            registry.Register(PacketDefinitions.EditTemplateData);
            registry.Register(PacketDefinitions.Unknown0F);
            registry.Register(PacketDefinitions.DropItem);
            registry.Register(PacketDefinitions.SingleClick);
            registry.Register(PacketDefinitions.RequestSkills);
            registry.Register(PacketDefinitions.DropWearItem);
            registry.Register(PacketDefinitions.ControlAnimation);
            registry.Register(PacketDefinitions.PathfinidingInClient);
            registry.Register(PacketDefinitions.BuyItems);
            registry.Register(PacketDefinitions.UpdateContainedItems);
            registry.Register(PacketDefinitions.Ship);
            registry.Register(PacketDefinitions.Versions);
            registry.Register(PacketDefinitions.UpdateStatics);
            registry.Register(PacketDefinitions.UpdateTerrains);
            registry.Register(PacketDefinitions.UpdateTerrains2);
            registry.Register(PacketDefinitions.UpdateArt);
            registry.Register(PacketDefinitions.UpdateAnimation);
            registry.Register(PacketDefinitions.UpdateHues);
            registry.Register(PacketDefinitions.VersionOK);
            registry.Register(PacketDefinitions.NewArt);
            registry.Register(PacketDefinitions.NewTerrain);
            registry.Register(PacketDefinitions.NewAnimation);
            registry.Register(PacketDefinitions.NewHues);
            registry.Register(PacketDefinitions.DeleteArt);
            registry.Register(PacketDefinitions.CheckClientVersion);
            registry.Register(PacketDefinitions.ScriptNames);
            registry.Register(PacketDefinitions.EditScriptFile);
            registry.Register(PacketDefinitions.BoardHeader);
            registry.Register(PacketDefinitions.BoardMessage);
            registry.Register(PacketDefinitions.BoardPostMessage);
            registry.Register(PacketDefinitions.UpdateRegions);
            registry.Register(PacketDefinitions.AddRegion);
            registry.Register(PacketDefinitions.NewContextFx);
            registry.Register(PacketDefinitions.UpdateContextFx);
            registry.Register(PacketDefinitions.RestartVersion);
            registry.Register(PacketDefinitions.ServerListing);
            registry.Register(PacketDefinitions.ServerListAddEntry);
            registry.Register(PacketDefinitions.ServerListRemoveEntry);
            registry.Register(PacketDefinitions.RemoveStaticObject);
            registry.Register(PacketDefinitions.MoveStaticObject);
            registry.Register(PacketDefinitions.LoadArea);
            registry.Register(PacketDefinitions.LoadAreaRequest);
            registry.Register(PacketDefinitions.ScriptAttach);
            registry.Register(PacketDefinitions.Friends);
            registry.Register(PacketDefinitions.NotifyFriend);
            registry.Register(PacketDefinitions.RenameCharacter);
            registry.Register(PacketDefinitions.ResponseToDialogBox);
            registry.Register(PacketDefinitions.GodViewQuery);
            registry.Register(PacketDefinitions.GodViewData);
            registry.Register(PacketDefinitions.DeleteCharacter);
            registry.Register(PacketDefinitions.ChangePassword);
            registry.Register(PacketDefinitions.CharacterListNotification);
            registry.Register(PacketDefinitions.CharacterCreation);
            registry.Register(PacketDefinitions.MoveCharacter);
            registry.Register(PacketDefinitions.Unknown8F);
            registry.Register(PacketDefinitions.RequestHelp);
            registry.Register(PacketDefinitions.SellRequest);
            registry.Register(PacketDefinitions.RequestTipNoticeWindow);
            registry.Register(PacketDefinitions.GumpTextEntryDialogReply);
            registry.Register(PacketDefinitions.GumpMenuSelection);
            registry.Register(PacketDefinitions.ChatText);
            registry.Register(PacketDefinitions.TargetObjectList);
            registry.Register(PacketDefinitions.OpenChatWindow);
            registry.Register(PacketDefinitions.SendHelpTipRequest);
            registry.Register(PacketDefinitions.InvalidMapRequest);
            registry.Register(PacketDefinitions.ParticleEffect);
            registry.Register(PacketDefinitions.SpyOnClient);
            registry.Register(PacketDefinitions.MahjongGameCommand);
            registry.Register(PacketDefinitions.UnknownD5);
            registry.Register(PacketDefinitions.Follow);
            registry.Register(PacketDefinitions.ResurrectionMenu);
            registry.Register(PacketDefinitions.RemoveGroup);
            registry.Register(PacketDefinitions.MapPacket);
            registry.Register(PacketDefinitions.BooksPages);
            registry.Register(PacketDefinitions.TargetCursor);
            registry.Register(PacketDefinitions.SecureTraiding);
            registry.Register(PacketDefinitions.BulletinBoardMessage);
            registry.Register(PacketDefinitions.BookHeader);
            registry.Register(PacketDefinitions.UpdateSkillsData);
            registry.Register(PacketDefinitions.DyeWindow);
            registry.Register(PacketDefinitions.GameCentralMonitor);
            registry.Register(PacketDefinitions.GiveBoatHousePlacementView);
            registry.Register(PacketDefinitions.ConsoleEntryPrompt);
            registry.Register(PacketDefinitions.RequestCharProfile);
            registry.Register(PacketDefinitions.UltimaMessenger);
            registry.Register(PacketDefinitions.AssistVersion);
            registry.Register(PacketDefinitions.UnicodeTextEntry);
            registry.Register(PacketDefinitions.UnknownC3);
            registry.Register(PacketDefinitions.ConfigurationFile);
            registry.Register(PacketDefinitions.LogoutStatus);
            registry.Register(PacketDefinitions.BookHeaderNew);
            registry.Register(PacketDefinitions.MegaCliloc);
            registry.Register(PacketDefinitions.GenericAosCommands);
            registry.Register(PacketDefinitions.RunUOProtocolExtension);
            registry.Register(PacketDefinitions.EditTileData);
            registry.Register(PacketDefinitions.SendElevation);
            registry.Register(PacketDefinitions.AllNamesPacket);
            registry.Register(PacketDefinitions.UnknownF9);
            registry.Register(PacketDefinitions.UOStoreRequest);
            registry.Register(PacketDefinitions.UnknownFB);
            registry.Register(PacketDefinitions.CrossUOMessages);
            registry.Register(PacketDefinitions.UnknownFD);
            registry.Register(PacketDefinitions.RazorHandshake);
            registry.Register(PacketDefinitions.UnknownFF);

            if (version >= new Version(5, 0, 0, 0))
            {
                registry.Register(PacketDefinitions.Damage500a);
                registry.Register(PacketDefinitions.AttackEnded500a);
                registry.Register(PacketDefinitions.NewHealthBarStatusUpdate500a);
            }

            if (version >= new Version(5, 0, 9, 0))
                registry.Register(PacketDefinitions.ClientType5090);
            else
                registry.Register(PacketDefinitions.ClientType);

            if (version >= new Version(6, 0, 6, 0))
                registry.Register(PacketDefinitions.TimeSynchronizationRequest6060);

            if (version >= new Version(6, 0, 1, 3))
                registry.Register(PacketDefinitions.KrEncryptionResponse6013);
            else
                registry.Register(PacketDefinitions.KrEncryptionResponse);

            if (version >= new Version(6, 0, 1, 7))
            {
                registry.Register(PacketDefinitions.DropItem6017);
                registry.Register(PacketDefinitions.AddItemToContainer6017);
                registry.Register(PacketDefinitions.AddMultipleItemsInContainer6017);
            }
            else
            {
                registry.Register(PacketDefinitions.AddMultipleItemsInContainer);
            }

            if (version >= new Version(6, 0, 14, 2))
                registry.Register(PacketDefinitions.EnableLockedClientFeaturesSince6_0_14_2);

            if (version >= new Version(7, 0, 9, 0))
            {
                registry.Register(PacketDefinitions.DrawContainer7090);
                registry.Register(PacketDefinitions.SecondAgeObjectInformation7090);
                registry.Register(PacketDefinitions.GiveBoatHousePlacementView7090);
                registry.Register(PacketDefinitions.QuestArrow7090);
                registry.Register(PacketDefinitions.TimeSynchronizationRequest7090);
                registry.Register(PacketDefinitions.TimeSynchronizationResponse7090);
            }

            if (version >= new Version(7, 0, 16, 0))
                registry.Register(PacketDefinitions.CharacterCreation70160);

            if (version >= new Version(7, 0, 18, 0))
                registry.Register(PacketDefinitions.CreateCharacterRequest7018);

            if (version >= new Version(7, 0, 33, 0))
                registry.Register(PacketDefinitions.DrawObject7033);
        }

        public static PacketDefinitionRegistry CreateClassicClient(Version version)
        {
            var registry = new PacketDefinitionRegistry();

            CreateClassicClient(registry, version);

            return registry;
        }

        internal static PacketDefinitionRegistry CreateClassicClient() => CreateClassicClient(new Version(0, 0, 0, 0));
    }
}
