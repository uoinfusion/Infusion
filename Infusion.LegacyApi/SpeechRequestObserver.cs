using System;
using Infusion.Commands;
using Infusion.Logging;
using Infusion.Packets;
using Infusion.Packets.Client;

namespace Infusion.LegacyApi
{
    internal sealed class SpeechRequestObserver
    {
        private readonly CommandHandler commandHandler;
        private readonly ILogger logger;

        public SpeechRequestObserver(UltimaClient clientPacketHandler, CommandHandler commandHandler, ILogger logger)
        {
            this.commandHandler = commandHandler;
            this.logger = logger;
            clientPacketHandler.RegisterFilter(FilterClientSpeech);
        }

        public event EventHandler<SpeechRequestedArgs> SpeechRequested;
        public event EventHandler<CommandRequestedArgs> CommandRequested;

        private Packet? FilterClientSpeech(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SpeechRequest.Id)
            {

                var packet = PacketDefinitionRegistry.Materialize<SpeechRequest>(rawPacket);
                if (commandHandler.IsInvocationSyntax(packet.Text))
                {
                    CommandRequested?.Invoke(this, new CommandRequestedArgs(packet.Text));

                    return null;
                }

                SpeechRequested?.Invoke(this, new SpeechRequestedArgs());
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
