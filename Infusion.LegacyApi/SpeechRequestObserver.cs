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
        private readonly PacketDefinitionRegistry packetRegistry;

        public SpeechRequestObserver(IClientPacketSubject clientPacketSubject, CommandHandler commandHandler,
            IEventJournalSource eventSource, ILogger logger, PacketDefinitionRegistry packetRegistry)
        {
            this.commandHandler = commandHandler;
            this.eventSource = eventSource;
            this.logger = logger;
            this.packetRegistry = packetRegistry;
            clientPacketSubject.RegisterFilter(FilterClientSpeech);
        }

        private Packet? FilterClientSpeech(Packet rawPacket)
        {
            string text = null;

            if (rawPacket.Id == PacketDefinitions.SpeechRequest.Id)
                text = packetRegistry.Materialize<SpeechRequest>(rawPacket).Text;
            else if (rawPacket.Id == PacketDefinitions.TalkRequest.Id)
                text = packetRegistry.Materialize<TalkRequest>(rawPacket).Message;

            if (text != null)
            {
                if (commandHandler.IsInvocationSyntax(text))
                {
                    eventSource.Publish(new CommandRequestedEvent(text));

                    Task.Run(() =>
                    {
                        logger.Debug(text);
                        commandHandler.InvokeSyntax(text);
                    });

                    return null;
                }

                logger.Debug(text);
                eventSource.Publish(new SpeechRequestedEvent(text));
            }

            return rawPacket;
        }
    }
}
