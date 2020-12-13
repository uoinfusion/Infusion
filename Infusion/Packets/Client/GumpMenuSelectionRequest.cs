using System;
using System.Linq;
using Infusion.IO;

namespace Infusion.Packets.Client
{
    internal sealed class GumpMenuSelectionRequest : MaterializedPacket
    {
        private Packet rawPacket;

        public GumpMenuSelectionRequest()
        {
        }

        public GumpMenuSelectionRequest(GumpTypeId gumpTypeId, GumpInstanceId id, GumpControlId triggerId,
            GumpControlId[] selectedCheckBoxIds, Tuple<ushort, string>[] textEntries)
        {
            GumpTypeId = gumpTypeId;
            Id = id;
            TriggerId = triggerId;

            var packetLength = (ushort) (23 + selectedCheckBoxIds.Length * 4 +
                                         textEntries.Length * 4 +
                                         textEntries.Sum(textEntryValue => textEntryValue.Item2.Length * 2));
            var payload = new byte[packetLength];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte) PacketDefinitions.GumpMenuSelection.Id);
            writer.WriteUShort(packetLength);
            writer.WriteUInt(id.Value);
            writer.WriteUInt(gumpTypeId.Value);
            writer.WriteUInt(triggerId.Value);
            writer.WriteUInt((uint) selectedCheckBoxIds.Length);
            foreach (var checkBoxId in selectedCheckBoxIds)
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

        public GumpInstanceId Id { get; private set; }

        public GumpTypeId GumpTypeId { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(3);
            Id = new GumpInstanceId(reader.ReadUInt());
            GumpTypeId = new GumpTypeId(reader.ReadUInt());
            TriggerId = new GumpControlId(reader.ReadUInt());
        }
    }
}