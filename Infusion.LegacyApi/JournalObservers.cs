using Infusion.LegacyApi.Cliloc;
using Infusion.LegacyApi.Console;
using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal sealed class JournalObservers
    {
        private readonly SpeechJournalSource journalSource;
        private readonly IConsole console;
        private readonly IClilocSource clilocSource;
        private readonly ClilocTranslator translator;

        public JournalObservers(SpeechJournalSource journalSource, IServerPacketSubject serverPacketSubject, IConsole console,
            IClilocSource clilocSource)
        {
            this.journalSource = journalSource;
            this.console = console;
            this.clilocSource = clilocSource;
            translator = new ClilocTranslator(clilocSource);

            serverPacketSubject.Subscribe(PacketDefinitions.SpeechMessage, HandleSpeechMessagePacket);
            serverPacketSubject.Subscribe(PacketDefinitions.SendSpeech, HanldeSendSpeechPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.ClilocMessage, HandleClilocMessage);
            serverPacketSubject.Subscribe(PacketDefinitions.ClilocMessageAffix, HandleClilocMessageAffix);
        }

        private void HandleClilocMessageAffix(ClilocMessageAffixPacket packet)
        {
            var message = clilocSource.GetString(packet.MessageId.Value);
            if (!string.IsNullOrEmpty(packet.Affix))
                message += packet.Affix;
            
            journalSource.AddMessage(packet.Name, message, packet.SpeakerId, packet.SpeakerBody, packet.Color, packet.Type);
            console.WriteSpeech(packet.Name, message, packet.SpeakerId, packet.Color, packet.SpeakerBody, packet.Type);
        }

        private void HandleClilocMessage(ClilocMessagePacket packet)
        {
            var message = translator.Translate(packet.MessageId.Value, packet.Arguments);

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
