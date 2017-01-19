using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace UltimaRX.Packets.Parsers
{
    public class PacketLogParser
    {
        private string log;
        private int position;

        public static IEnumerable<PacketLogEntry> ParseFile(string fileName)
        {
            string log = File.ReadAllText(fileName);

            return new PacketLogParser().Parse(log);
        }

        public IEnumerable<PacketLogEntry> Parse(string log)
        {
            this.log = log;
            position = 0;

            while (position < log.Length)
            {
                var packet = ParsePacket();
                if (packet != null)
                    yield return packet;
                if (position < log.Length)
                    NextLine();
            }
        }

        private PacketLogEntry ParsePacket()
        {
            var created = ParseTime();
            if (!created.HasValue)
                return null;

            if (!ConsumeString(" >>>> "))
                return null;

            var direction = ParseDirection();
            if (!direction.HasValue)
                return null;

            string name;
            byte[] payload;

            switch (direction.Value)
            {
                case PacketDirection.ProxyServer:
                    if (!ConsumeString(": RawPacket "))
                        return null;
                    name = ParseIdentifier();
                    if (name == null)
                        return null;
                    NextLine();
                    payload = ParsePayload();
                    if (payload == null)
                        return null;
                    NextLine();
                    if (ParsePayload() == null)
                        return null;
                    break;

                case PacketDirection.ProxyClient:
                    if (!ConsumeString(": RawPacket "))
                        return null;
                    name = ParseIdentifier();
                    if (name == null)
                        return null;
                    NextLine();
                    payload = ParsePayload();
                    if (payload == null)
                        return null;
                    NextLine();
                    if (ParsePayload() == null)
                        return null;
                    break;

                case PacketDirection.ServerProxy:
                    NextLine();
                    if (ParsePayload() == null)
                        return null;
                    if (!ParseTime().HasValue)
                        return null;
                    if (!ConsumeString(" >>>> server -> proxy: RawPacket "))
                        return null;
                    name = ParseIdentifier();
                    if (name == null)
                        return null;
                    NextLine();
                    payload = ParsePayload();
                    if (payload == null)
                        return null;
                    break;

                case PacketDirection.ClientProxy:
                    NextLine();
                    if (ParsePayload() == null)
                        return null;
                    if (!ParseTime().HasValue)
                        return null;
                    if (!ConsumeString(" >>>> client -> proxy: RawPacket "))
                        return null;
                    name = ParseIdentifier();
                    if (name == null)
                        return null;
                    NextLine();
                    payload = ParsePayload();
                    if (payload == null)
                        return null;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return new PacketLogEntry(created.Value, name, direction.Value, payload);
        }

        private byte[] ParsePayload()
        {
            var b = ParsePayloadByte();

            if (!b.HasValue)
                return null;

            var payload = new List<byte> {b.Value};

            do
            {
                if (log[position] == '\r' || log[position] == '\n')
                    NextLine();

                b = ParsePayloadByte();
                if (!b.HasValue)
                    break;
                payload.Add(b.Value);
            } while (position < log.Length);

            return payload.ToArray();
        }

        private byte? ParsePayloadByte()
        {
            if (!ConsumeString("0x"))
                return null;

            var digit1 = ParseHexDigit();
            if (!digit1.HasValue)
                return null;

            var digit2 = ParseHexDigit();
            if (!digit2.HasValue)
                return null;

            var hexNumberString = new string(new[] {digit1.Value, digit2.Value});
            byte number;
            if (!byte.TryParse(hexNumberString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out number))
                return null;

            if (!ConsumeString(", "))
                return null;

            return number;
        }

        private char? ParseHexDigit()
        {
            var currentChar = char.ToLower(log[position], CultureInfo.InvariantCulture);

            if (char.IsDigit(currentChar) || (currentChar >= 'a' && currentChar <= 'f'))
            {
                position++;
                return currentChar;
            }

            return null;
        }

        private void NextLine()
        {
            while (log[position] != '\r' && log[position] != '\n')
            {
                position++;
            }

            if (log[position] == '\r')
                position += 2;
            else if (log[position] == '\n')
                position++;
        }

        private string ParseIdentifier()
        {
            var startPosition = position;

            while (char.IsLetter(log[position]))
            {
                position++;
            }

            if (position == startPosition)
                return null;

            return log.Substring(startPosition, position - startPosition);
        }

        private PacketDirection? ParseDirection()
        {
            if (ConsumeString("proxy -> server"))
                return PacketDirection.ProxyServer;

            if (ConsumeString("server -> proxy"))
                return PacketDirection.ServerProxy;

            if (ConsumeString("proxy -> client"))
                return PacketDirection.ProxyClient;

            if (ConsumeString("client -> proxy"))
                return PacketDirection.ClientProxy;

            return null;
        }

        private bool ConsumeString(string s)
        {
            var startPosition = position;

            if (!s.All(t => ConsumeChar(t)))
            {
                position = startPosition;
                return false;
            }

            return true;
        }

        private TimeSpan? ParseTime()
        {
            var startPosition = position;
            var success = ConsumeNumber(2) && ConsumeChar(':') && ConsumeNumber(2) && ConsumeChar(':') &&
                          ConsumeNumber(2) && ConsumeChar('.') && ConsumeNumber(3);

            if (!success)
            {
                position = startPosition;
                return null;
            }

            var time = log.Substring(startPosition, position - startPosition);

            return TimeSpan.Parse(time);
        }

        private bool ConsumeChar(char ch)
        {
            if (position >= log.Length)
                return false;

            if (log[position] == ch)
            {
                position++;
                return true;
            }

            return false;
        }

        private bool ConsumeNumber(int numberOfDigits)
        {
            for (var i = 0; i < numberOfDigits; i++)
                if (!ConsumeDigit())
                    return false;

            return true;
        }

        private bool ConsumeDigit()
        {
            if (char.IsDigit(log[position]))
            {
                position++;
                return true;
            }

            return false;
        }
    }

    public enum PacketDirection
    {
        ProxyServer,
        ServerProxy,
        ProxyClient,
        ClientProxy
    }
}