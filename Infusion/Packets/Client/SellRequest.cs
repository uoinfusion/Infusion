using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Packets.Client
{
    internal sealed class SellRequest: MaterializedPacket
    {
        private Packet rawPacket;
        public override Packet RawPacket => rawPacket;
        public SellRequest()
        {

        }

        public SellRequest(ObjectId vendor, SellListItem[] list)
        {
            if (list == null || list.Length == 0)
            {
                return;
            }

            ushort length = (ushort)(9 + list.Length * 6);
            byte[] payload = new byte[length];
                  
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.SellRequest.Id);
            writer.WriteUShort(length);
            writer.WriteUInt(vendor); 
            writer.WriteUShort((ushort)list.Length);

            foreach (var item in list)
            {
                writer.WriteUInt(item.Serial);
                writer.WriteUShort(item.Amount);
            }

                        
            rawPacket = new Packet(PacketDefinitions.SellRequest.Id, payload);

        }

        public override void Deserialize(Packet rawPacket)
        {
            
        }
    }

    
}
