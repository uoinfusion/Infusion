using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets
{
    internal static class PacketDefinitionRegistryFactory
    {
        public static PacketDefinitionRegistry CreateClassicClient(Version version)
        {
            var registry = new PacketDefinitionRegistry();

            registry.Register(PacketDefinitions.AddItemToContainer);
            registry.Register(PacketDefinitions.AttackRequest);
            registry.Register(PacketDefinitions.CharMoveRejection);
            registry.Register(PacketDefinitions.CharacterLocaleAndBody);
            registry.Register(PacketDefinitions.CharacterMoveAck);
            registry.Register(PacketDefinitions.CharactersStartingLocations);
            registry.Register(PacketDefinitions.ClientSpy);
            registry.Register(PacketDefinitions.ClientVersion);
            registry.Register(PacketDefinitions.ClientViewRange);
            registry.Register(PacketDefinitions.CreateCharacterRequest);
            registry.Register(PacketDefinitions.Damage);
            registry.Register(PacketDefinitions.DeleteObject);
            registry.Register(PacketDefinitions.DisconnectNotification);
            registry.Register(PacketDefinitions.DraggingOfItem);
            registry.Register(PacketDefinitions.DrawContainer);
            registry.Register(PacketDefinitions.DrawGamePlayer);
            registry.Register(PacketDefinitions.DrawObject);
            registry.Register(PacketDefinitions.EnableLockedClientFeatures);
            registry.Register(PacketDefinitions.Explosion);
            registry.Register(PacketDefinitions.GameServerList);
            registry.Register(PacketDefinitions.GameServerLoginRequest);
            registry.Register(PacketDefinitions.GeneralInformationPacket);
            registry.Register(PacketDefinitions.GetClientStatus);
            registry.Register(PacketDefinitions.GodModeRequest);
            registry.Register(PacketDefinitions.GraphicalEffect);
            registry.Register(PacketDefinitions.HealthBarStatusUpdate);
            registry.Register(PacketDefinitions.KickPlayer);
            registry.Register(PacketDefinitions.LoginSeed);
            registry.Register(PacketDefinitions.LoginCharacter);
            registry.Register(PacketDefinitions.LoginComplete);
            registry.Register(PacketDefinitions.LoginDenied);
            registry.Register(PacketDefinitions.LoginRequest);
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
            registry.Register(PacketDefinitions.DropItemFailed);
            registry.Register(PacketDefinitions.DropItemApproved);
            registry.Register(PacketDefinitions.Blood);
            registry.Register(PacketDefinitions.MobAttributes);
            registry.Register(PacketDefinitions.WornItem);
            registry.Register(PacketDefinitions.FightOccuring);
            registry.Register(PacketDefinitions.AttackOk);
            registry.Register(PacketDefinitions.AttackEnded);
            registry.Register(PacketDefinitions.AddMultipleItemsInContainer);
            registry.Register(PacketDefinitions.PersonalLightLevel);
            registry.Register(PacketDefinitions.OverallLightLevel);
            registry.Register(PacketDefinitions.RejectCharacterLogon);
            registry.Register(PacketDefinitions.Time);
            registry.Register(PacketDefinitions.SetWeather);
            registry.Register(PacketDefinitions.PlayMidiMusic);
            registry.Register(PacketDefinitions.CharacterAnimation);
            registry.Register(PacketDefinitions.OpenBuyWindow);
            registry.Register(PacketDefinitions.NewSubserver);
            registry.Register(PacketDefinitions.UpdatePlayer);
            registry.Register(PacketDefinitions.OpenDialogBox);
            registry.Register(PacketDefinitions.ResendCharactersAfterDelete);
            registry.Register(PacketDefinitions.OpenPaperdoll);
            registry.Register(PacketDefinitions.CorpseClothing);
            registry.Register(PacketDefinitions.DisplaySign);
            registry.Register(PacketDefinitions.MapMessage);
            registry.Register(PacketDefinitions.MovePlayer);
            registry.Register(PacketDefinitions.RequestAssistance);
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
            registry.Register(PacketDefinitions.ExtendedDrawGamePlayer);
            registry.Register(PacketDefinitions.ExtendedDrawObject);
            registry.Register(PacketDefinitions.SendCustomHouse);
            registry.Register(PacketDefinitions.CharacterTransferLog);
            registry.Register(PacketDefinitions.SecondAgeRevision);
            registry.Register(PacketDefinitions.CompressedGump);
            registry.Register(PacketDefinitions.UpdateMobileStatus);
            registry.Register(PacketDefinitions.BuffSystem);
            registry.Register(PacketDefinitions.NewCharacterAnimation);
            registry.Register(PacketDefinitions.KrEncryptionResponse);
            registry.Register(PacketDefinitions.SecondAgeObjectInformation);
            registry.Register(PacketDefinitions.NewMapMessage);
            registry.Register(PacketDefinitions.DoubleClick);
            registry.Register(PacketDefinitions.PickUpItem);
            registry.Register(PacketDefinitions.DropItem);
            registry.Register(PacketDefinitions.SingleClick);
            registry.Register(PacketDefinitions.RequestSkills);
            registry.Register(PacketDefinitions.DropWearItem);
            registry.Register(PacketDefinitions.ControlAnimation);
            registry.Register(PacketDefinitions.PathfinidingInClient);
            registry.Register(PacketDefinitions.BuyItems);
            registry.Register(PacketDefinitions.VersionOk);
            registry.Register(PacketDefinitions.NewArtwork);
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
            registry.Register(PacketDefinitions.ChangeTextColor);
            registry.Register(PacketDefinitions.RenameCharacter);
            registry.Register(PacketDefinitions.ResponseToDialogBox);
            registry.Register(PacketDefinitions.DeleteCharacter);
            registry.Register(PacketDefinitions.CharacterCreation);
            registry.Register(PacketDefinitions.RequestHelp);
            registry.Register(PacketDefinitions.SellListReply);
            registry.Register(PacketDefinitions.RequestTipNoticeWindow);
            registry.Register(PacketDefinitions.GumpTextEntryDialogReply);
            registry.Register(PacketDefinitions.GumpMenuSelection);
            registry.Register(PacketDefinitions.ChatText);
            registry.Register(PacketDefinitions.OpenChatWindow);
            registry.Register(PacketDefinitions.SendHelpTipRequest);
            registry.Register(PacketDefinitions.InvalidMapRequest);
            registry.Register(PacketDefinitions.ParticleEffect);
            registry.Register(PacketDefinitions.SpyOnClient);
            registry.Register(PacketDefinitions.Follow);
            registry.Register(PacketDefinitions.ResurrectionMenu);
            registry.Register(PacketDefinitions.RemoveGroup);
            registry.Register(PacketDefinitions.MapPacket);
            registry.Register(PacketDefinitions.BooksPages);
            registry.Register(PacketDefinitions.TargetCursor);
            registry.Register(PacketDefinitions.SecureTraiding);
            registry.Register(PacketDefinitions.BulletinBoardMessage);
            registry.Register(PacketDefinitions.BookHeader);
            registry.Register(PacketDefinitions.DyeWindow);
            registry.Register(PacketDefinitions.GiveBoatHousePlacementView);
            registry.Register(PacketDefinitions.ConsoleEntryPrompt);
            registry.Register(PacketDefinitions.RequestCharProfile);
            registry.Register(PacketDefinitions.UltimaMessenger);
            registry.Register(PacketDefinitions.AssistVersion);
            registry.Register(PacketDefinitions.UnicodeTextEntry);
            registry.Register(PacketDefinitions.ConfigurationFile);
            registry.Register(PacketDefinitions.LogoutStatus);
            registry.Register(PacketDefinitions.BookHeaderNew);
            registry.Register(PacketDefinitions.MegaCliloc);
            registry.Register(PacketDefinitions.GenericAosCommands);
            registry.Register(PacketDefinitions.KrriosClientSpecial);
            registry.Register(PacketDefinitions.FreeShardList);
            registry.Register(PacketDefinitions.EditTileData);
            registry.Register(PacketDefinitions.SendElevation);
            registry.Register(PacketDefinitions.AllNamesPacket);

            if (version >= new Version(5, 0, 0, 0))
                registry.Register(PacketDefinitions.Damage500a);

            if (version >= new Version(5, 0, 9, 0))
                registry.Register(PacketDefinitions.ClientType5090);
            else
                registry.Register(PacketDefinitions.ClientType);

            if (version >= new Version(6, 0, 1, 3))
                registry.Register(PacketDefinitions.KrEncryptionResponse6013);
            else
                registry.Register(PacketDefinitions.KrEncryptionResponse);

            if (version >= new Version(6, 0, 1, 7))
            {
                registry.Register(PacketDefinitions.DropItem6017);
                registry.Register(PacketDefinitions.AddItemToContainer6017);
            }

            if (version >= new Version(6, 0, 14, 2))
                registry.Register(PacketDefinitions.EnableLockedClientFeaturesSince6_0_14_2);

            if (version >= new Version(7, 0, 0, 0))
                registry.Register(PacketDefinitions.DrawObject7000);

            if (version >= new Version(7, 0, 9, 0))
            {
                registry.Register(PacketDefinitions.DrawContainer7090);
                registry.Register(PacketDefinitions.SecondAgeObjectInformation7090);
                registry.Register(PacketDefinitions.GiveBoatHousePlacementView7090);
                registry.Register(PacketDefinitions.QuestArrow7090);
            }

            if (version >= new Version(7, 0, 16, 0))
                registry.Register(PacketDefinitions.CharacterCreation70160);


            return registry;
        }

        internal static PacketDefinitionRegistry CreateClassicClient() => CreateClassicClient(new Version(0, 0, 0, 0));
    }
}
