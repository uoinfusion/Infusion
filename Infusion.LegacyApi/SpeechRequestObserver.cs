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
        private readonly ILogger logger;
        private readonly IEventJournalSource eventSource;

        public SpeechRequestObserver(UltimaClient clientPacketHandler, CommandHandler commandHandler, ILogger logger, IEventJournalSource eventSource)
        {
            this.commandHandler = commandHandler;
            this.logger = logger;
            this.eventSource = eventSource;
            clientPacketHandler.RegisterFilter(FilterClientSpeech);
        }

        public event EventHandler<SpeechRequestedEvent> SpeechRequested;
        public event EventHandler<CommandRequestedEvent> CommandRequested;

        private Packet? FilterClientSpeech(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SpeechRequest.Id)
            {

                var packet = PacketDefinitionRegistry.Materialize<SpeechRequest>(rawPacket);
                if (commandHandler.IsInvocationSyntax(packet.Text))
                {
                    var commandEvent = new CommandRequestedEvent(packet.Text);
                    eventSource.Publish(commandEvent);
                    CommandRequested?.Invoke(this, commandEvent);

                    return null;
                }

                var speechEvent = new SpeechRequestedEvent(packet.Text);
                eventSource.Publish(speechEvent);
                SpeechRequested?.Invoke(this, speechEvent);
            }

            return rawPacket;
        }

        public void ResetEvents()
        {
            //SpeechRequested = null;
            //CommandRequested = null;
        }
    }
}
