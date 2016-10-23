using System;
using UltimaRX.IO;

namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class ConnectToGameServer : MaterializedPacket
    {
        private readonly byte[] gameServerIp = new byte[4];
        private readonly ArrayPacketWriter writer;

        private ushort gameServerPort;

        public ConnectToGameServer(Packet rawPacket) : base(rawPacket)
        {
            var reader = new ArrayPacketReader(rawPacket.Payload) {Position = 1};
            reader.Read(gameServerIp, 0, 4);
            gameServerPort = reader.ReadUShort();

            writer = new ArrayPacketWriter(rawPacket.Payload);
        }

        public byte[] GameServerIp
        {
            get { return gameServerIp; }

            set
            {
                if (value.Length != 4)
                {
                    throw new InvalidOperationException(
                        $"GameServerIp has to be array of length 4, it is {value.Length} instead");
                }

                Array.Copy(value, gameServerIp, 4);

                writer.Position = 1;
                writer.Write(value, 0, 4);
            }
        }

        public ushort GameServerPort
        {
            get { return gameServerPort; }

            set
            {
                this.gameServerPort = value;
                writer.Position = 5;
                writer.Write(value);
            }
        }
    }
}