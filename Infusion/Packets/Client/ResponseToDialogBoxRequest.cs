using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Client
{
    internal sealed class ResponseToDialogBoxRequest
    {
        private Packet rawPacket;

        public uint DialogId { get; set; }
        public ushort MenuId { get; set; }
        public byte ResponseIndex { get; set; }

        public ResponseToDialogBoxRequest(uint dialogId, ushort menuId, byte responseIndex, ModelId responseType, Color responseColor)
        {
            DialogId = dialogId;
            MenuId = menuId;
            ResponseIndex = responseIndex;

            byte[] payload = new byte[13];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.ResponseToDialogBox.Id);
            writer.WriteUInt(dialogId);
            writer.WriteUShort(menuId);
            writer.WriteUShort(responseIndex);
            writer.WriteModelId(responseType);
            writer.WriteColor(responseColor);

            rawPacket = new Packet(PacketDefinitions.DoubleClick.Id, payload);
        }

        public Packet RawPacket => rawPacket;
    }
}
