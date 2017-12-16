using System.Collections.Generic;
using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi.Filters
{
    internal sealed class ShapeshiftingFilter : IShapeshiftingFilter
    {
        private bool enabled;

        private readonly List<ItemShapeShiftDefinition> itemShapeShiftDefinitions = new List<ItemShapeShiftDefinition>()
            ;

        public ShapeshiftingFilter(IServerPacketSubject serverPacketHandler, UltimaClient client)
        {
            serverPacketHandler.RegisterOutputFilter(FilterItemShapes);
        }

        public void AddShapeShift(ItemSpec spec, ModelId targetType, Color targetColor)
        {
            itemShapeShiftDefinitions.Add(new ItemShapeShiftDefinition(spec, targetType, targetColor));
        }

        public void Reset()
        {
            itemShapeShiftDefinitions.Clear();
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
        }

        private Packet? FilterItemShapes(Packet rawPacket)
        {
            if (enabled && rawPacket.Id == PacketDefinitions.ObjectInfo.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<ObjectInfoPacket>(rawPacket);

                foreach (var def in itemShapeShiftDefinitions)
                {
                    if (def.SourceSpec.Matches(packet.Type))
                    {
                        var newPacket =
                            new ObjectInfoPacket(packet.Id, def.TargetType, packet.Location, def.TargetColor);

                        return newPacket.RawPacket;
                    }
                }
            }

            return rawPacket;
        }

        private struct ItemShapeShiftDefinition
        {
            public ItemShapeShiftDefinition(ItemSpec sourceSpec, ModelId targetType, Color targetColor)
            {
                SourceSpec = sourceSpec;
                TargetType = targetType;
                TargetColor = targetColor;
            }

            public ItemSpec SourceSpec { get; }
            public ModelId TargetType { get; }
            public Color TargetColor { get; }
        }
    }
}