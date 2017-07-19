using System;
using System.Linq;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class ItemSpec
    {
        private readonly ItemSpec[] childSpecs;

        public ItemSpec(ModelId type, Color? color = null)
        {
            Specificity = color.HasValue ? ItemSpecSpecificity.TypeAndColor : ItemSpecSpecificity.Type;

            Type = type;
            Color = color;
        }

        public ItemSpec(params ItemSpec[] childSpecs)
        {
            Specificity = ItemSpecSpecificity.CompositeSpecificity;
            this.childSpecs = childSpecs;
        }

        private ModelId? Type { get; }
        private Color? Color { get; }
        public ItemSpecSpecificity Specificity { get; }

        public bool Matches(Item item)
        {
            if (Type.HasValue)
                return item.Type == Type && (!Color.HasValue || Color == item.Color);
            if (childSpecs != null && childSpecs.Length > 0)
                return childSpecs.Any(s => s.Matches(item));

            throw new NotImplementedException();
        }

        public bool Matches(ModelId type)
        {
            if (Type.HasValue)
                return type == Type && !Color.HasValue;

            return childSpecs.Any(s => s.Matches(type));
        }

        public ItemSpec Including(params ItemSpec[] childSpecs)
        {
            return new ItemSpec(childSpecs.Concat(new[] {this}).ToArray());
        }

        public static implicit operator ItemSpec(ushort[] types)
        {
            return new ItemSpec(types.Select(t => new ItemSpec(t)).ToArray());
        }

        public static implicit operator ItemSpec(ModelId[] types)
        {
            return new ItemSpec(types.Select(t => new ItemSpec(t)).ToArray());
        }

        public static implicit operator ItemSpec(int[] types)
        {
            return new ItemSpec(types.Select(t => new ItemSpec((ushort) t)).ToArray());
        }

        public static implicit operator ItemSpec(ItemSpec[] specs)
        {
            return new ItemSpec(specs);
        }

        public static implicit operator ItemSpec(ushort type)
        {
            return new ItemSpec(type);
        }
    }
}