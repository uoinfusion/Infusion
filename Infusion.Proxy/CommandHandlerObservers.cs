using Infusion.Commands;
using Infusion.Packets;
using Infusion.Packets.Client;

namespace Infusion.Proxy
{
    internal sealed class CommandHandlerObservers
    {
        private readonly CommandHandler commandHandler;

        public CommandHandlerObservers(ClientPacketHandler clientPacketHandler, CommandHandler commandHandler)
        {
            this.commandHandler = commandHandler;
            clientPacketHandler.RegisterFilter(FilterClientSpeech);
        }

        private Packet? FilterClientSpeech(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SpeechRequest.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<SpeechRequest>(rawPacket);
                if (commandHandler.IsInvocationSyntax(packet.Text))
                {
                    commandHandler.Invoke(packet.Text);

                    return null;
                }
            }

            return rawPacket;
        }
    }
}
