using Infusion.LegacyApi.Console;
using Infusion.LegacyApi.Keywords;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.LegacyApi
{
    public sealed class Chat
    {
        private readonly UltimaServer ultimaServer;
        private readonly Action notifyAction;
        private readonly IConsole console;
        private readonly KeywordParser keywordParser;

        private const ushort defaultFont = 0x03;

        internal Chat(UltimaServer ultimaServer, Action notifyAction, IConsole console, IKeywordSource keywordSource)
        {
            this.ultimaServer = ultimaServer;
            this.notifyAction = notifyAction;
            this.console = console;
            keywordParser = new KeywordParser(keywordSource);
        }

        public string Language { get; set; } = "ENU";

        public Color NormalMessageColor { get; set; } = (Color)0x02B2;
        public Color AllianceMessageColor { get; set; } = (Color)0x02B2;
        public Color GuildMessageColor { get; set; } = (Color)0x02B2;
        public Color EmoteMessageColor { get; set; } = (Color)0x0225;
        public Color WhisperMessageColor { get; set; } = (Color)0x02B2;
        public Color YellMessageColor { get; set; } = (Color)0x02B2;

        public void ChannelSay(string message) => this.ultimaServer.SendChatMessage(message);

        public void Say(string message) => Say(message, NormalMessageColor, defaultFont);
        public void Say(string message, Color color) => Say(message, color, defaultFont);
        public void Say(string message, Color color, ushort font) => Say(message, SpeechType.Normal, color, font);

        public void AllianceSay(string message) => AllianceSay(message, AllianceMessageColor, defaultFont);
        public void AllianceSay(string message, Color color) => AllianceSay(message, color, defaultFont);
        public void AllianceSay(string message, Color color, ushort font) => Say(message, SpeechType.AllianceChat, color, font);

        public void GuildSay(string message) => GuildSay(message, GuildMessageColor, defaultFont);
        public void GuildSay(string message, Color color) => GuildSay(message, color, defaultFont);
        public void GuildSay(string message, Color color, ushort font) => Say(message, SpeechType.GuildChat, color, font);

        public void Emote(string message) => Emote(message, EmoteMessageColor, defaultFont);
        public void Emote(string message, Color color) => Emote(message, color, defaultFont);
        public void Emote(string message, Color color, ushort font) => Say(message, SpeechType.Emote, color, font);

        public void Whisper(string message) => Whisper(message, WhisperMessageColor, defaultFont);
        public void Whisper(string message, Color color) => Whisper(message, color, defaultFont);
        public void Whisper(string message, Color color, ushort font) => Say(message, SpeechType.Whisper, color, font);

        public void Yell(string message) => Yell(message, YellMessageColor, defaultFont);
        public void Yell(string message, Color color) => Yell(message, color, defaultFont);
        public void Yell(string message, Color color, ushort font) => Say(message, SpeechType.Yell, color, font);

        private void Say(string message, SpeechType type, Color color, ushort font)
        {
            notifyAction();
            console.Debug(message);

            var keywordIds = keywordParser.GetKeywordIds(message);
            ultimaServer.Say(message, keywordIds, type, color, font, Language);
        }
    }
}
