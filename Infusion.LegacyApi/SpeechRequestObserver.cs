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
            string text = null;

            if (rawPacket.Id == PacketDefinitions.SpeechRequest.Id)
                text = PacketDefinitionRegistry.Materialize<SpeechRequest>(rawPacket).Text;
            else if (rawPacket.Id == PacketDefinitions.TalkRequest.Id)
                text = PacketDefinitionRegistry.Materialize<TalkRequest>(rawPacket).Message;

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
