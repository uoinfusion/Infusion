using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.Proxy.LegacyApi
{
    internal class JournalObservers
    {
        private readonly JournalSource journalSource;

        public JournalObservers(JournalSource journalSource, ServerPacketHandler serverPacketHandler)
        {
            this.journalSource = journalSource;
            serverPacketHandler.Subscribe(PacketDefinitions.SpeechMessage, HandleSpeechMessagePacket);
            serverPacketHandler.Subscribe(PacketDefinitions.SendSpeech, HanldeSendSpeechPacket);
        }

        private void HandleSpeechMessagePacket(SpeechMessagePacket packet)
        {
            journalSource.AddMessage(new JournalEntry(packet.Name, packet.Message, packet.Id, packet.Model));
        }

        private void HanldeSendSpeechPacket(SendSpeechPacket packet)
        {
            journalSource.AddMessage(new JournalEntry(packet.Name, packet.Message, packet.Id, packet.Model));
        }
    }
}
