using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class PersonalLightLevelPacket : MaterializedPacket
    {
        public ObjectId MobileId { get; private set; }
        public byte Level { get; private set; }


        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            MobileId = reader.ReadObjectId();
            Level = reader.ReadByte();
        }

        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;
    }
}
