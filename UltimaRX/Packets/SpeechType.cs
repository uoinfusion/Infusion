namespace UltimaRX.Packets
{
    public enum SpeechType : byte
    {
        Normal = 0x00,
        BroadcastSystem = 0x01,
        Emote = 0x02,
        LowerCorner = 0x06,
        CornerWithName = 0x07,
        Whisper = 0x08,
        Yell = 0x09,
        Spell = 0x0A,
        GuildChat = 0x0D,
        AllianceChat = 0x0E,
        CommandPrompts = 0x0F,
        EncodedCommands = 0xC0
    }
}
