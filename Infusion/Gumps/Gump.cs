using Infusion.Packets;

namespace Infusion.Gumps
{
    public sealed class Gump
    {
        public Gump(GumpTypeId id, GumpInstanceId gumpId, string commands, string[] textLines)
        {
            Id = id;
            GumpId = gumpId;
            Commands = commands;
            TextLines = textLines;
        }

        public string Commands { get; }

        public string[] TextLines { get; }

        public GumpTypeId Id { get; set; }

        public GumpInstanceId GumpId { get; set; }
    }
}