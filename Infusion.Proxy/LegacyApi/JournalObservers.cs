using Infusion.Packets;
using Infusion.Packets.Server;
using Ultima;

namespace Infusion.Proxy.LegacyApi
{
    internal sealed class JournalObservers
    {
        private readonly JournalSource journalSource;
        private static readonly StringList clilocDictionary = new StringList("ENU");

        public JournalObservers(JournalSource journalSource, ServerPacketHandler serverPacketHandler)
        {
            this.journalSource = journalSource;
            serverPacketHandler.Subscribe(PacketDefinitions.SpeechMessage, HandleSpeechMessagePacket);
            serverPacketHandler.Subscribe(PacketDefinitions.SendSpeech, HanldeSendSpeechPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.ClilocMessage, HandleClilocMessage);
            serverPacketHandler.Subscribe(PacketDefinitions.ClilocMessageAffix, HandleClilocMessageAffix);
        }

        private void HandleClilocMessageAffix(ClilocMessageAffixPacket packet)
        {
            journalSource.AddMessage(packet.Name, clilocDictionary.GetString(packet.MessageId) + packet.Affix, packet.SpeakerId, packet.SpeakerBody);
        }

        private void HandleClilocMessage(ClilocMessagePacket packet)
        {
            journalSource.AddMessage(packet.Name, clilocDictionary.GetString(packet.MessageId), packet.SpeakerId, packet.SpeakerBody);
        }

        private void HandleSpeechMessagePacket(SpeechMessagePacket packet)
        {
            journalSource.AddMessage(packet.Name, packet.Message, packet.Id, packet.Model);
        }

        private void HanldeSendSpeechPacket(SendSpeechPacket packet)
        {
            journalSource.AddMessage(packet.Name, packet.Message, packet.Id, packet.Model);
        }
    }
}
