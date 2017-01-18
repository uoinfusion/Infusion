using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.PacketParser
{
    public static class PacketLogEntryEnumerableExtensions
    {
        public static IEnumerable<PacketLogEntry> OfName(this IEnumerable<PacketLogEntry> entries, params string[] names)
            => entries.Where(e => names.Contains(e.Name));

        public static IEnumerable<PacketLogEntry> OfDirection(this IEnumerable<PacketLogEntry> entries, params PacketDirection[] directions)
            => entries.Where(e => directions.Contains(e.Direction));

        public static void ToConsole(this IEnumerable<PacketLogEntry> entries, Func<PacketLogEntry, string> formatter)
        {
            foreach (var entry in entries)
            {
                Console.WriteLine(formatter(entry));
            }
        }
    }
}
