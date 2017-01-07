using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.Packets;
using UltimaRX.Packets.Server;

namespace UltimaRX.Proxy.InjectionApi
{
    internal class JournalObservers
    {
        private readonly Journal journal;

        public JournalObservers(Journal journal, ServerPacketHandler serverPacketHandler)
        {
            this.journal = journal;
            serverPacketHandler.Subscribe(PacketDefinitions.SpeechMessage, HandleSpeechMessagePacket);
            serverPacketHandler.Subscribe(PacketDefinitions.SendSpeech, HanldeSendSpeechPacket);
        }

        private void HandleSpeechMessagePacket(SpeechMessagePacket packet)
        {
            string name = string.IsNullOrEmpty(packet.Name) ? string.Empty : $"{packet.Name}: ";
            string message = name + packet.Message;

            journal.AddMessage(message);
        }

        private void HanldeSendSpeechPacket(SendSpeechPacket packet)
        {
            string name = string.IsNullOrEmpty(packet.Name) ? string.Empty : $"{packet.Name}: ";
            string message = name + packet.Message;

            journal.AddMessage(message);
        }
    }
}
