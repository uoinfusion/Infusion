using System;
using System.Linq;
using Infusion.Packets;

namespace Infusion.Proxy.LegacyApi
{
    public class ItemSpec
    {
        private readonly ItemSpec[] childSpecs;

        public ItemSpec(ModelId type, Color? color = null)
        {
            Type = type;
            Color = color;
        }

        public ItemSpec(params ItemSpec[] childSpecs)
        {
            this.childSpecs = childSpecs;
        }

        private ModelId? Type { get; }
        private Color? Color { get; }

        public bool Matches(Item item)
        {
            if (Type.HasValue)
                return item.Type == Type && (!Color.HasValue || Color == item.Color);
            if (childSpecs != null && childSpecs.Length > 0)
            {
                return childSpecs.Any(s => s.Matches(item));
            }
            throw new NotImplementedException();
        }

        public ItemSpec Including(params ItemSpec[] childSpecs) =>
            new ItemSpec(childSpecs.Concat(new[] {this}).ToArray());

        public static implicit operator ItemSpec(ushort[] types) =>
            new ItemSpec(types.Select(t => new ItemSpec(t)).ToArray());

        public static implicit operator ItemSpec(ModelId[] types) =>
            new ItemSpec(types.Select(t => new ItemSpec(t)).ToArray());

        public static implicit operator ItemSpec(int[] types) =>
            new ItemSpec(types.Select(t => new ItemSpec((ushort)t)).ToArray());

        public static implicit operator ItemSpec(ItemSpec[] specs) =>
            new ItemSpec(specs);

        public static implicit operator ItemSpec(ushort type) =>
            new ItemSpec(type);
    }
}