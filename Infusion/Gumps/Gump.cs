using Infusion.Packets;

namespace Infusion.Gumps
{
    public sealed class Gump
    {
        public Gump(GumpTypeId gumpTypeId, GumpInstanceId id, string commands, string[] textLines)
            : this(id, gumpTypeId, commands, textLines)
        {

        }
        public Gump(GumpInstanceId id, GumpTypeId gumpTypeId, string commands, string[] textLines)
        {
            Id = id;
            GumpTypeId = gumpTypeId;
            Commands = commands;
            TextLines = textLines;
        }

        public string Commands { get; }

        public string[] TextLines { get; }

        public GumpInstanceId Id { get; set; }

        public GumpTypeId GumpTypeId { get; set; }
    }
}