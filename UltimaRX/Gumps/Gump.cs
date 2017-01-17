namespace UltimaRX.Gumps
{
    public sealed class Gump
    {
        public Gump(uint id, uint gumpId, string commands, string[] textLines)
        {
            Id = id;
            GumpId = gumpId;
            Commands = commands;
            TextLines = textLines;
        }

        public string Commands { get; }

        public string[] TextLines { get; }

        public uint Id { get; set; }

        public uint GumpId { get; set; }
    }
}