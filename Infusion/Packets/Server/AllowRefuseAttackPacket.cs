using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class AllowRefuseAttackPacket : MaterializedPacket
    {
        public ObjectId AttackTargetId { get; private set; }

        public AllowRefuseAttackPacket()
        {
        }

        public AllowRefuseAttackPacket(ObjectId attackTargetId)
        {
            AttackTargetId = attackTargetId;

            var payload = new byte[5];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.AllowRefuseAttack.Id);
            writer.WriteId(attackTargetId);

            rawPacket = new Packet(PacketDefinitions.AllowRefuseAttack.Id, payload);
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            AttackTargetId = reader.ReadObjectId();
        }

        public override Packet RawPacket => rawPacket;

        private Packet rawPacket;
    }
}
