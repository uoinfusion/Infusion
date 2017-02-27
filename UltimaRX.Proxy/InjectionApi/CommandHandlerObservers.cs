using Infusion.Packets;
using Infusion.Packets.Client;

namespace Infusion.Proxy.InjectionApi
{
    internal sealed class CommandHandlerObservers
    {
        private readonly CommandHandler scriptHandler;

        public CommandHandlerObservers(ClientPacketHandler clientPacketHandler, CommandHandler scriptHandler)
        {
            this.scriptHandler = scriptHandler;
            clientPacketHandler.RegisterFilter(FilterClientSpeech);
        }

        private Packet? FilterClientSpeech(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SpeechRequest.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<SpeechRequest>(rawPacket);
                if (scriptHandler.IsInvocationSyntax(packet.Text))
                {
                    scriptHandler.Invoke(packet.Text);

                    return null;
                }
            }

            return rawPacket;
        }
    }
}
