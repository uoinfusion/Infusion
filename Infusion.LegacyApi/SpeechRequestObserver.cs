using System;
using System.Threading.Tasks;
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
        private readonly ILogger logger;

        public SpeechRequestObserver(IClientPacketSubject clientPacketSubject, CommandHandler commandHandler, IEventJournalSource eventSource, ILogger logger)
        {
            this.commandHandler = commandHandler;
            this.eventSource = eventSource;
            this.logger = logger;
            clientPacketSubject.RegisterFilter(FilterClientSpeech);
        }

        private Packet? FilterClientSpeech(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SpeechRequest.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<SpeechRequest>(rawPacket);
                if (commandHandler.IsInvocationSyntax(packet.Text))
                {
                    eventSource.Publish(new CommandRequestedEvent(packet.Text));

                    Task.Run(() =>
                    {
                        logger.Debug(packet.Text);
                        commandHandler.InvokeSyntax(packet.Text);
                    });

                    return null;
                }

                logger.Debug(packet.Text);
                eventSource.Publish(new SpeechRequestedEvent(packet.Text));
            }

            return rawPacket;
        }
    }
}
