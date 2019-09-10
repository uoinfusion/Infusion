using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;

namespace Infusion
{
    internal sealed class ClientPacketHandler : IClientPacketSubject
    {
        private readonly PacketHandler packetHandler;

        public ClientPacketHandler(PacketDefinitionRegistry packetRegistry)
        {
            packetHandler = new PacketHandler(packetRegistry);
        }

        public void RegisterFilter(Func<Packet, Packet?> filter)
        {
            packetHandler.RegisterFilter(filter);
        }

        public void RegisterOutputFilter(Func<Packet, Packet?> filter)
        {
            packetHandler.RegisterOutputFilter(filter);
        }

        public void Subscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket
        {
            packetHandler.Subscribe(definition, observer);
        }

        public void Unsubscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket
        {
            packetHandler.Unsubscribe(definition, observer);
        }

        public Packet? FilterOutput(Packet rawPacket) => packetHandler.FilterOutput(rawPacket);

        public Packet? HandlePacket(Packet rawPacket)
        {
            var filteredPacket = packetHandler.Filter(rawPacket);
            if (!filteredPacket.HasValue)
                return null;
            rawPacket = filteredPacket.Value;

            if (rawPacket.Id == PacketDefinitions.MoveRequest.Id)
                packetHandler.Publish<MoveRequest>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.TalkRequest.Id)
                packetHandler.Publish<TalkRequest>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.SpeechRequest.Id)
                packetHandler.Publish<SpeechRequest>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.TargetCursor.Id)
                packetHandler.Publish<TargetCursorPacket>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.GumpMenuSelection.Id)
                packetHandler.Publish<GumpMenuSelectionRequest>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.DoubleClick.Id)
                packetHandler.Publish<DoubleClickRequest>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.RequestSkills.Id)
                packetHandler.Publish<SkillRequest>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.SelectServerRequest.Id)
                packetHandler.Publish<SelectServerRequest>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.ExtendedLoginSeed.Id)
                packetHandler.Publish<ExtendedLoginSeed>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.LoginCharacter.Id)
                packetHandler.Publish<LoginCharacterRequest>(rawPacket);
            else if (rawPacket.Id == PacketDefinitions.LoginRequest.Id)
                packetHandler.Publish<LoginRequest>(rawPacket);

            return rawPacket;
        }
    }
}
