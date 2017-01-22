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
        private readonly JournalEntries journal;

        public JournalObservers(JournalEntries journal, ServerPacketHandler serverPacketHandler)
        {
            this.journal = journal;
            serverPacketHandler.Subscribe(PacketDefinitions.SpeechMessage, HandleSpeechMessagePacket);
            serverPacketHandler.Subscribe(PacketDefinitions.SendSpeech, HanldeSendSpeechPacket);
        }

        private void HandleSpeechMessagePacket(SpeechMessagePacket packet)
        {
            journal.AddMessage(new JournalEntry(packet.Name, packet.Message, packet.Id, packet.Model));
        }

        private void HanldeSendSpeechPacket(SendSpeechPacket packet)
        {
            journal.AddMessage(new JournalEntry(packet.Name, packet.Message, packet.Id, packet.Model));
        }
    }
}
