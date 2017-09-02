using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Server;

namespace Infusion
{
    internal sealed class ServerPacketHandler : IServerPacketSubject
    {
        private readonly PacketHandler packetHandler = new PacketHandler();

        public void RegisterFilter(Func<Packet, Packet?> filter)
        {
            packetHandler.RegisterFilter(filter);
        }

        public void Subscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer) where TPacket : MaterializedPacket
        {
            packetHandler.Subscribe(definition, observer);
        }

        public void Unsubscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer) where TPacket : MaterializedPacket
        {
            packetHandler.Subscribe(definition, observer);
        }

        public Packet? HandlePacket(Packet rawPacket)
        {
            var filteredPacket = packetHandler.Filter(rawPacket);
            if (!filteredPacket.HasValue)
                return null;
            rawPacket = filteredPacket.Value;

            if (rawPacket.Id == PacketDefinitions.AddMultipleItemsInContainer.Id)
                packetHandler.Publish<AddMultipleItemsInContainerPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.AddItemToContainer.Id)
                packetHandler.Publish<AddItemToContainerPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.DeleteObject.Id)
                packetHandler.Publish<DeleteObjectPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.ObjectInfo.Id)
                packetHandler.Publish<ObjectInfoPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.DrawObject.Id)
                packetHandler.Publish<DrawObjectPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.TargetCursor.Id)
                packetHandler.Publish<TargetCursorPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.CharacterMoveAck.Id)
                packetHandler.Publish<CharacterMoveAckPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.CharMoveRejection.Id)
                packetHandler.Publish<CharMoveRejectionPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.DrawGamePlayer.Id)
                packetHandler.Publish<DrawGamePlayerPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.SpeechMessage.Id)
                packetHandler.Publish<SpeechMessagePacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.SendSpeech.Id)
                packetHandler.Publish<SendSpeechPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.CharacterLocaleAndBody.Id)
                packetHandler.Publish<CharLocaleAndBodyPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.UpdatePlayer.Id)
                packetHandler.Publish<UpdatePlayerPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.UpdateCurrentHealth.Id)
                packetHandler.Publish<UpdateCurrentHealthPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.UpdateCurrentMana.Id)
                packetHandler.Publish<UpdateCurrentManaPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.UpdateCurrentStamina.Id)
                packetHandler.Publish<UpdateCurrentStaminaPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.SendGumpMenuDialog.Id)
                packetHandler.Publish<SendGumpMenuDialogPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.WornItem.Id)
                packetHandler.Publish<WornItemPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.StatusBarInfo.Id)
                packetHandler.Publish<StatusBarInfoPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.SendSkills.Id)
                packetHandler.Publish<SendSkillsPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.RejectMoveItemRequest.Id)
                packetHandler.Publish<RejectMoveItemRequestPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.ClilocMessage.Id)
                packetHandler.Publish<ClilocMessagePacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.ClilocMessageAffix.Id)
                packetHandler.Publish<ClilocMessageAffixPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.AllowRefuseAttack.Id)
                packetHandler.Publish<AllowRefuseAttackPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.DrawContainer.Id)
                packetHandler.Publish<DrawContainerPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.PauseClient.Id)
                packetHandler.Publish<PauseClientPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.PersonalLightLevel.Id)
                packetHandler.Publish<PersonalLightLevelPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.OverallLightLevel.Id)
                packetHandler.Publish<OverallLightLevelPacket>(rawPacket);

            return rawPacket;
        }
    }
}
