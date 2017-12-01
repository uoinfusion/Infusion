using System;
using Infusion.Commands;
using Infusion.LegacyApi.Events;
using Infusion.Logging;
using Infusion.Packets;
using Infusion.Packets.Client;

namespace Infusion.LegacyApi
{
    internal sealed class SpeechRequestObserver
    {
        private readonly CommandHandler commandHandler;
        private readonly IEventJournalSource eventSource;

        public SpeechRequestObserver(IClientPacketSubject clientPacketSubject, CommandHandler commandHandler, IEventJournalSource eventSource)
        {
            this.commandHandler = commandHandler;
            this.eventSource = eventSource;
            clientPacketSubject.RegisterFilter(FilterClientSpeech);
        }

        private Packet? FilterClientSpeech(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SpeechRequest.Id)
            {

                var packet = PacketDefinitionRegistry.Materialize<SpeechRequest>(rawPacket);
                if (commandHandler.IsInvocationSyntax(packet.Text))
                {
                    commandHandler.InvokeSyntax(packet.Text);
                    eventSource.Publish(new CommandRequestedEvent(packet.Text));

                    return null;
                }

                eventSource.Publish(new SpeechRequestedEvent(packet.Text));
            }

            return rawPacket;
        }
    }
}
