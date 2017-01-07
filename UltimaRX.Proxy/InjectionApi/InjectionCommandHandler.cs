using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.Packets;
using UltimaRX.Packets.Client;

namespace UltimaRX.Proxy.InjectionApi
{
    internal sealed class InjectionCommandHandler
    {
        public event EventHandler<string> CommandReceived;

        public InjectionCommandHandler(ClientPacketHandler clientPacketHandler)
        {
            clientPacketHandler.RegisterFilter(FilterClientSpeech);
        }

        private Packet? FilterClientSpeech(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SpeechRequest.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<SpeechRequest>(rawPacket);
                if (packet.Text.StartsWith(","))
                {
                    if (CommandReceived == null)
                    {
                        Console.WriteLine($"Unhandled command: {packet.Text}");
                    }
                    else
                    {
                        CommandReceived?.Invoke(null, packet.Text.TrimStart(','));
                    }

                    return null;
                }
            }

            return rawPacket;
        }
    }
}
