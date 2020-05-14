using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Packets.Client
{
    internal sealed class SellListReply: MaterializedPacket
    {
        private Packet rawPacket;
        public override Packet RawPacket => rawPacket;
        public SellListReply()
        {

        }

        public SellListReply(ObjectId vendor, SellListItem[] list)
        {
            var payload = new byte[1 + 2 + 4 + 2 + list.Length * 6];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteUInt(vendor);
            writer.WriteUShort((ushort)list.Length);

            for (int i = 0; i < list.Length; i++)
            {
                SellListItem sli = list[i];
                writer.WriteUInt(sli.Serial);
                writer.WriteUShort(sli.Amount);
            }
                        
            rawPacket = new Packet(PacketDefinitions.SellListReply.Id, payload);

        }

        public override void Deserialize(Packet rawPacket)
        {
            //throw new NotImplementedException();
        }
    }

    
}
