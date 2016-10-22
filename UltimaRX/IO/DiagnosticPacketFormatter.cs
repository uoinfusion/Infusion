using System;
using System.Text;
using UltimaRX.Packets;
using UltimaRX.Packets.PacketDefinitions;

namespace UltimaRX.IO
{
    public class DiagnosticPacketFormatter
    {
        private const int MaxColumns = 16;
        private readonly StringBuilder builder = new StringBuilder();
        private readonly string header;
        private int columns;
        private bool requiresHeader = true;

        public DiagnosticPacketFormatter(string header)
        {
            this.header = header;
        }

        public void Header()
        {
            if (requiresHeader)
            {
                builder.AppendFormat($"{DateTime.Now} {header} >>>> Starting");
                builder.AppendLine();
                requiresHeader = false;
            }
        }

        public void FinishPacket(Packet packet)
        {
            builder.AppendLine();
            builder.AppendFormat(
                $"{DateTime.Now} {header} >>>> Packet {PacketDefinitionRegistry.Find(packet.Id).GetType().Name}, length = {packet.Length}");
            builder.AppendLine();

            for (var i = 0; i < packet.Length; i++)
            {
                builder.AppendFormat("0x{0:X2}, ", packet.Payload[i]);
                if ((i + 1)%MaxColumns == 0)
                    builder.AppendLine();
            }

            builder.AppendLine();
            builder.AppendLine();
            columns = 0;
            requiresHeader = true;
        }

        public void AddByte(byte value)
        {
            Header();

            builder.AppendFormat("0x{0:X2}, ", value);
            columns++;

            if (columns > MaxColumns)
            {
                columns = 0;
                builder.AppendLine();
            }
        }

        public string Flush()
        {
            var result = builder.ToString();

            builder.Clear();

            return result;
        }
    }
}