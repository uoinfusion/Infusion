using System;
using System.Text;
using UltimaRX.Packets;

namespace UltimaRX.IO
{
    public class DiagnosticStream : IDiagnosticStream
    {
        StringBuilder builder = new StringBuilder();

        public void Dispose()
        {
            BaseStream.Dispose();
        }

        public bool DataAvailable => BaseStream.DataAvailable;

        public int ReadByte()
        {
            int value = BaseStream.ReadByte();

            if (value >= byte.MinValue && value <= byte.MaxValue)
            {
                OnByteReceived((byte)value);
            }

            return value;
        }

        private int columns;

        private const int MaxColumns = 16;
        private void OnByteReceived(byte value)
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

        public int Read(byte[] buffer, int offset, int count)
        {
            Header();

            int length = BaseStream.Read(buffer, offset, count);

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    OnByteReceived(buffer[offset + i]);
                }
            }

            return length;
        }

        private void Header()
        {
            if (requiresHeader)
            {
                builder.AppendFormat($"{DateTime.Now} Starting packet receive");
                builder.AppendLine();
                requiresHeader = false;
            }
        }

        public IPullStream BaseStream { get; set; }
        private bool requiresHeader = true;

        public void FinishPacket(Packet packet)
        {
            builder.AppendLine();
            builder.AppendFormat($"{DateTime.Now} Packet {packet.Id:X2}, length = {packet.Length}");
            builder.AppendLine();

            for (int i = 0; i < packet.Length; i++)
            {
                builder.AppendFormat("0x{0:X2}, ", packet.Payload[i]);
                if ((i + 1)%MaxColumns == 0)
                {
                    builder.AppendLine();
                }
            }

            builder.AppendLine();
            builder.AppendLine();
            columns = 0;
            requiresHeader = true;
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}