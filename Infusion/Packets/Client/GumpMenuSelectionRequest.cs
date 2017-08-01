using System;
using System.Linq;
using Infusion.IO;

namespace Infusion.Packets.Client
{
    public class GumpMenuSelectionRequest : MaterializedPacket
    {
        private Packet rawPacket;

        public GumpMenuSelectionRequest()
        {
        }

        public GumpMenuSelectionRequest(GumpTypeId id, GumpInstanceId gumpId, GumpControlId triggerId,
            GumpControlId[] selectedCheckBoxeIds, Tuple<ushort, string>[] textEntries)
        {
            Id = id;
            GumpId = gumpId;
            TriggerId = triggerId;

            var packetLength = (ushort) (23 + selectedCheckBoxeIds.Length * 4 +
                                         textEntries.Length * 4 +
                                         textEntries.Sum(textEntryValue => textEntryValue.Item2.Length * 2));
            var payload = new byte[packetLength];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte) PacketDefinitions.GumpMenuSelection.Id);
            writer.WriteUShort(packetLength);
            writer.WriteUInt(id.Value);
            writer.WriteUInt(gumpId.Value);
            writer.WriteUInt(triggerId.Value);
            writer.WriteUInt((uint) selectedCheckBoxeIds.Length);
            foreach (var checkBoxId in selectedCheckBoxeIds)
                writer.WriteUInt(checkBoxId.Value);
            writer.WriteUInt((uint) textEntries.Length);
            foreach (var textEntry in textEntries)
            {
                writer.WriteUShort(textEntry.Item1);
                writer.WriteUShort((ushort) textEntry.Item2.Length);
                writer.WriteUnicodeString(textEntry.Item2);
            }

            rawPacket = new Packet(PacketDefinitions.GumpMenuSelection.Id, payload);
        }

        public override Packet RawPacket => rawPacket;

        public GumpControlId TriggerId { get; private set; }

        public GumpInstanceId GumpId { get; private set; }

        public GumpTypeId Id { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(3);
            Id = new GumpTypeId(reader.ReadUInt());
            GumpId = new GumpInstanceId(reader.ReadUInt());
            TriggerId = new GumpControlId(reader.ReadUInt());
        }
    }
}