using Infusion.Packets;
using Infusion.Packets.Server;
using System;
using Ultima;

namespace Infusion.LegacyApi
{
    internal sealed class JournalObservers
    {
        private readonly SpeechJournalSource journalSource;
        private static readonly Lazy<StringList> clilocDictionary = new Lazy<StringList>(() => new StringList("ENU"));

        public JournalObservers(SpeechJournalSource journalSource, IServerPacketSubject serverPacketSubject)
        {
            this.journalSource = journalSource;
            serverPacketSubject.Subscribe(PacketDefinitions.SpeechMessage, HandleSpeechMessagePacket);
            serverPacketSubject.Subscribe(PacketDefinitions.SendSpeech, HanldeSendSpeechPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.ClilocMessage, HandleClilocMessage);
            serverPacketSubject.Subscribe(PacketDefinitions.ClilocMessageAffix, HandleClilocMessageAffix);
        }

        private void HandleClilocMessageAffix(ClilocMessageAffixPacket packet)
        {
            journalSource.AddMessage(packet.Name, clilocDictionary.Value.GetString(packet.MessageId.Value) + packet.Affix, packet.SpeakerId, packet.SpeakerBody);
        }

        private void HandleClilocMessage(ClilocMessagePacket packet)
        {
            journalSource.AddMessage(packet.Name, clilocDictionary.Value.GetString(packet.MessageId.Value), packet.SpeakerId, packet.SpeakerBody);
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
