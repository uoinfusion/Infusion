using System;
using System.Collections.Generic;
using System.Linq;
using Infusion.IO;

namespace Infusion.Packets.Client
{
    public class GumpMenuSelectionRequest : MaterializedPacket
    {
        public GumpMenuSelectionRequest()
        {
            
        }

        public GumpMenuSelectionRequest(uint id, uint gumpId, uint triggerId, uint[] selectedCheckBoxeIds, Tuple<ushort, string>[] textEntries)
        {
            Id = id;
            GumpId = gumpId;
            TriggerId = triggerId;

            var packetLength = (ushort)(23 + selectedCheckBoxeIds.Length * 4 +
                 textEntries.Length * 4 + textEntries.Sum(textEntryValue => textEntryValue.Item2.Length * 2));
            var payload = new byte[packetLength];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.GumpMenuSelection.Id);
            writer.WriteUShort(packetLength);
            writer.WriteUInt(id);
            writer.WriteUInt(gumpId);
            writer.WriteUInt(triggerId);
            writer.WriteUInt((uint)selectedCheckBoxeIds.Length);
            foreach (uint checkBoxId in selectedCheckBoxeIds)
                writer.WriteUInt(checkBoxId);
            writer.WriteUInt((uint)textEntries.Length);
            foreach (var textEntry in textEntries)
            {
                writer.WriteUShort(textEntry.Item1);
                writer.WriteUShort((ushort)textEntry.Item2.Length);
                writer.WriteUnicodeString(textEntry.Item2);
            }

            rawPacket = new Packet(PacketDefinitions.GumpMenuSelection.Id, payload);
        }

        private Packet rawPacket;
        private IEnumerable<uint> selectedCheckBoxes;

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(3);
            Id = reader.ReadUInt();
            GumpId = reader.ReadUInt();
            TriggerId = reader.ReadUInt();
        }

        public uint TriggerId { get; private set; }

        public uint GumpId { get; private set; }

        public uint Id { get; private set; }
    }
}
