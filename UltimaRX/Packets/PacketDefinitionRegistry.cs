using System;
using System.Collections.Generic;
using UltimaRX.Packets.Server;

namespace UltimaRX.Packets
{
    public static class PacketDefinitionRegistry
    {
        private static readonly Dictionary<int, PacketDefinition> Definitions = new Dictionary<int, PacketDefinition>();

        static PacketDefinitionRegistry()
        {
            Register(PacketDefinitions.AddItemToContainer);
            Register(PacketDefinitions.AttackRequest);
            Register(PacketDefinitions.CharMoveRejection);
            Register(PacketDefinitions.CharacterLocaleAndBody);
            Register(PacketDefinitions.CharacterMoveAck);
            Register(PacketDefinitions.CharactersStartingLocations);
            Register(PacketDefinitions.ClientSpy);
            Register(PacketDefinitions.ClientVersion);
            Register(PacketDefinitions.ClientViewRange);
            Register(PacketDefinitions.CreateCharacterRequest);
            Register(PacketDefinitions.Damage);
            Register(PacketDefinitions.DeleteObject);
            Register(PacketDefinitions.DisconnectNotification);
            Register(PacketDefinitions.DraggingOfItem);
            Register(PacketDefinitions.DrawContainer);
            Register(PacketDefinitions.DrawGamePlayer);
            Register(PacketDefinitions.DrawObject);
            Register(PacketDefinitions.EnableLockedClientFeatures);
            Register(PacketDefinitions.Explosion);
            Register(PacketDefinitions.GameServerList);
            Register(PacketDefinitions.GameServerLoginRequest);
            Register(PacketDefinitions.GeneralInformationPacket);
            Register(PacketDefinitions.GetClientStatus);
            Register(PacketDefinitions.GodModeRequest);
            Register(PacketDefinitions.GraphicalEffect);
            Register(PacketDefinitions.HealthBarStatusUpdate);
            Register(PacketDefinitions.KickPlayer);
            Register(PacketDefinitions.LoginSeed);
            Register(PacketDefinitions.LoginCharacter);
            Register(PacketDefinitions.LoginComplete);
            Register(PacketDefinitions.LoginDenied);
            Register(PacketDefinitions.LoginRequest);
            Register(PacketDefinitions.MoveRequest);
            Register(PacketDefinitions.NewHealthBarStatusUpdate);
            Register(PacketDefinitions.ObjectInfo);
            Register(PacketDefinitions.PauseClient);
            Register(PacketDefinitions.PingMessage);
            Register(PacketDefinitions.PlaySoundEffect);
            Register(PacketDefinitions.RejectMoveItemRequest);
            Register(PacketDefinitions.RequestWarMode);
            Register(PacketDefinitions.SelectServerRequest);
            Register(PacketDefinitions.SendSkills);
            Register(PacketDefinitions.SendSpeech);
            Register(PacketDefinitions.SpeechMessage);
            Register(PacketDefinitions.SpeechRequest);
            Register(PacketDefinitions.StatusBarInfo);
            Register(PacketDefinitions.TalkRequest);
            Register(PacketDefinitions.TipNoticeWindow);
            Register(PacketDefinitions.ConnectToGameServer);
            Register(PacketDefinitions.DropItemFailed);
            Register(PacketDefinitions.DropItemApproved);
            Register(PacketDefinitions.Blood);
            Register(PacketDefinitions.MobAttributes);
            Register(PacketDefinitions.WornItem);
            Register(PacketDefinitions.FightOccuring);
            Register(PacketDefinitions.AttackOk);
            Register(PacketDefinitions.AttackEnded);
            Register(PacketDefinitions.AddMultipleItemsInContainer);
            Register(PacketDefinitions.PersonalLightLevel);
            Register(PacketDefinitions.OverallLightLevel);
            Register(PacketDefinitions.RejectCharacterLogon);
            Register(PacketDefinitions.Time);
            Register(PacketDefinitions.SetWeather);
            Register(PacketDefinitions.PlayMidiMusic);
            Register(PacketDefinitions.CharacterAnimation);
            Register(PacketDefinitions.OpenBuyWindow);
            Register(PacketDefinitions.NewSubserver);
            Register(PacketDefinitions.UpdatePlayer);
            Register(PacketDefinitions.OpenDialogBox);
            Register(PacketDefinitions.ResendCharactersAfterDelete);
            Register(PacketDefinitions.OpenPaperdoll);
            Register(PacketDefinitions.CorpseClothing);
            Register(PacketDefinitions.MapMessage);
            Register(PacketDefinitions.MovePlayer);
            Register(PacketDefinitions.RequestAssistance);
            Register(PacketDefinitions.SellList);
            Register(PacketDefinitions.UpdateCurrentHealth);
            Register(PacketDefinitions.UpdateCurrentMana);
            Register(PacketDefinitions.UpdateCurrentStamina);
            Register(PacketDefinitions.OpenWebBrowser);
            Register(PacketDefinitions.AllowRefuseAttack);
            Register(PacketDefinitions.GumpTextEntryDialog);
            Register(PacketDefinitions.DisplayDeathAction);
            Register(PacketDefinitions.SendGumpMenuDialog);
            Register(PacketDefinitions.ChatMessage);
            Register(PacketDefinitions.HelpTileData);
            Register(PacketDefinitions.QuestArrow);
            Register(PacketDefinitions.SeasonalInformation);
            Register(PacketDefinitions.GraphicalEffect2);
            Register(PacketDefinitions.ClilocMessage);
            Register(PacketDefinitions.Semivisible);
            Register(PacketDefinitions.InvalidMapEnable);
            Register(PacketDefinitions.GlobalQueueCount);
            Register(PacketDefinitions.ClilocMessageAffix);
            Register(PacketDefinitions.ExtendedDrawGamePlayer);
            Register(PacketDefinitions.ExtendedDrawObject);
            Register(PacketDefinitions.SendCustomHouse);
            Register(PacketDefinitions.CharacterTransferLog);
            Register(PacketDefinitions.SecondAgeRevision);
            Register(PacketDefinitions.CompressedGump);
            Register(PacketDefinitions.UpdateMobileStatus);
            Register(PacketDefinitions.BuffSystem);
            Register(PacketDefinitions.NewCharacterAnimation);
            Register(PacketDefinitions.KrEncryptionResponse);
            Register(PacketDefinitions.SecondAgeObjectInformation);
            Register(PacketDefinitions.NewMapMessage);
            Register(PacketDefinitions.DoubleClick);
            Register(PacketDefinitions.PickUpItem);
            Register(PacketDefinitions.DropItem);
            Register(PacketDefinitions.SingleClick);
            Register(PacketDefinitions.RequestSkills);
            Register(PacketDefinitions.DropWearItem);
            Register(PacketDefinitions.ControlAnimation);
            Register(PacketDefinitions.PathfinidingInClient);
            Register(PacketDefinitions.BuyItems);
            Register(PacketDefinitions.VersionOk);
            Register(PacketDefinitions.NewArtwork);
            Register(PacketDefinitions.NewTerrain);
            Register(PacketDefinitions.NewAnimation);
            Register(PacketDefinitions.NewHues);
            Register(PacketDefinitions.DeleteArt);
            Register(PacketDefinitions.CheckClientVersion);
            Register(PacketDefinitions.ScriptNames);
            Register(PacketDefinitions.EditScriptFile);
            Register(PacketDefinitions.BoardHeader);
            Register(PacketDefinitions.BoardMessage);
            Register(PacketDefinitions.BoardPostMessage);
            Register(PacketDefinitions.UpdateRegions);
            Register(PacketDefinitions.AddRegion);
            Register(PacketDefinitions.NewContextFx);
            Register(PacketDefinitions.UpdateContextFx);
            Register(PacketDefinitions.RestartVersion);
            Register(PacketDefinitions.ServerListing);
            Register(PacketDefinitions.ServerListAddEntry);
            Register(PacketDefinitions.ServerListRemoveEntry);
            Register(PacketDefinitions.RemoveStaticObject);
            Register(PacketDefinitions.MoveStaticObject);
            Register(PacketDefinitions.LoadArea);
            Register(PacketDefinitions.LoadAreaRequest);
            Register(PacketDefinitions.ChangeTextColor);
            Register(PacketDefinitions.RenameCharacter);
            Register(PacketDefinitions.ResponseToDialogBox);
            Register(PacketDefinitions.DeleteCharacter);
            Register(PacketDefinitions.CharacterCreation);
            Register(PacketDefinitions.RequestHelp);
            Register(PacketDefinitions.SellListReply);
            Register(PacketDefinitions.RequestTipNoticeWindow);
            Register(PacketDefinitions.GumpTextEntryDialogReply);
            Register(PacketDefinitions.GumpMenuSelection);
            Register(PacketDefinitions.ChatText);
            Register(PacketDefinitions.OpenChatWindow);
            Register(PacketDefinitions.SendHelpTipRequest);
            Register(PacketDefinitions.InvalidMapRequest);
            Register(PacketDefinitions.SpyOnClient);
            Register(PacketDefinitions.Follow);
            Register(PacketDefinitions.ResurrectionMenu);
            Register(PacketDefinitions.RemoveGroup);
            Register(PacketDefinitions.MapPacket);
            Register(PacketDefinitions.BooksPages);
            Register(PacketDefinitions.TargetCursor);
            Register(PacketDefinitions.SecureTraiding);
            Register(PacketDefinitions.BulletinBoardMessage);
            Register(PacketDefinitions.BookHeader);
            Register(PacketDefinitions.DyeWindow);
            Register(PacketDefinitions.GiveBoatHousePlacementView);
            Register(PacketDefinitions.ConsoleEntryPrompt);
            Register(PacketDefinitions.RequestCharProfile);
            Register(PacketDefinitions.UltimaMessenger);
            Register(PacketDefinitions.AssistVersion);
            Register(PacketDefinitions.UnicodeTextEntry);
            Register(PacketDefinitions.ConfigurationFile);
            Register(PacketDefinitions.LogoutStatus);
            Register(PacketDefinitions.BookHeaderNew);
            Register(PacketDefinitions.MegaCliloc);
            Register(PacketDefinitions.GenericAosCommands);
            Register(PacketDefinitions.FreeShardList);
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