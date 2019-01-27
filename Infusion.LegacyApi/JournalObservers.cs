using Infusion.LegacyApi.Console;
using Infusion.Packets;
using Infusion.Packets.Server;
using System;
using Ultima;

namespace Infusion.LegacyApi
{
    internal sealed class JournalObservers
    {
        private readonly SpeechJournalSource journalSource;
        private readonly IConsole console;
        private static readonly Lazy<StringList> clilocDictionary = new Lazy<StringList>(() => new StringList("ENU"));

        public JournalObservers(SpeechJournalSource journalSource, IServerPacketSubject serverPacketSubject, IConsole console)
        {
            this.journalSource = journalSource;
            this.console = console;
            serverPacketSubject.Subscribe(PacketDefinitions.SpeechMessage, HandleSpeechMessagePacket);
            serverPacketSubject.Subscribe(PacketDefinitions.SendSpeech, HanldeSendSpeechPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.ClilocMessage, HandleClilocMessage);
            serverPacketSubject.Subscribe(PacketDefinitions.ClilocMessageAffix, HandleClilocMessageAffix);
        }

        private void HandleClilocMessageAffix(ClilocMessageAffixPacket packet)
        {
            var message = clilocDictionary.Value.GetString(packet.MessageId.Value) ?? $"Unknown Cliloc #{packet.MessageId.Value}";
            if (!string.IsNullOrEmpty(packet.Affix))
                message += packet.Affix;
            
            journalSource.AddMessage(packet.Name, message, packet.SpeakerId, packet.SpeakerBody, packet.Color, packet.Type);
            console.WriteSpeech(packet.Name, message, packet.SpeakerId, packet.Color, packet.SpeakerBody, packet.Type);
        }

        private void HandleClilocMessage(ClilocMessagePacket packet)
        {
            var message = clilocDictionary.Value.GetString(packet.MessageId.Value) ?? $"Unknown Cliloc #{packet.MessageId.Value}";

            journalSource.AddMessage(packet.Name, message, packet.SpeakerId, packet.SpeakerBody, packet.Color, packet.Type);
            console.WriteSpeech(packet.Name, message, packet.SpeakerId, packet.Color, packet.SpeakerBody, packet.Type);
        }

        private void HandleSpeechMessagePacket(SpeechMessagePacket packet)
        {
            journalSource.AddMessage(packet.Name, packet.Message, packet.Id, packet.Model, packet.Color, packet.Type);
            console.WriteSpeech(packet.Name, packet.Message, packet.Id, packet.Color, packet.Model, packet.Type);
        }

        private void HanldeSendSpeechPacket(SendSpeechPacket packet)
        {
            journalSource.AddMessage(packet.Name, packet.Message, packet.Id, packet.Model, packet.Color, packet.Type);
            console.WriteSpeech(packet.Name, packet.Message, packet.Id, packet.Color, packet.Model, packet.Type);
        }
    }
}
