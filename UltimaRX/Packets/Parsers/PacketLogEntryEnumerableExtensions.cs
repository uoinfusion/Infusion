using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimaRX.Packets.Parsers
{
    public static class PacketLogEntryEnumerableExtensions
    {
        public static IEnumerable<PacketLogEntry> OfName(this IEnumerable<PacketLogEntry> entries, params string[] names)
            => entries.Where(e => names.Contains(e.Name));

        public static IEnumerable<PacketLogEntry> OfDirection(this IEnumerable<PacketLogEntry> entries,
            params PacketDirection[] directions)
            => entries.Where(e => directions.Contains(e.Direction));

        public static IEnumerable<string> Format(this IEnumerable<PacketLogEntry> entries,
            Func<PacketLogEntry, string> formatter) => entries.Select(e => formatter(e));

        public static IEnumerable<string> ShortFormat(this IEnumerable<PacketLogEntry> entries)
            => Format(entries, LogFormat.Short);

        public static IEnumerable<string> Format(this IEnumerable<PacketLogEntry> entries, LogFormat format)
        {
            Func<PacketLogEntry, string> formatter;
            switch (format)
            {
                case LogFormat.Short:
                    formatter = e => $"{e.Created}\t{e.Direction}\t\t{e.Name}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), $"Unknown format {format}.");
            }

            return Format(entries, formatter);
        }

        public static void ToConsole(this IEnumerable<string> entries)
        {
            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }
        }
    }

    public enum LogFormat
    {
        Short
    }
}