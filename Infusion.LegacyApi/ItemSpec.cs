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
            Specificity = color.HasValue ? SpecSpecificity.TypeAndColor : SpecSpecificity.Type;

            Type = type;
            Color = color;
        }

        internal ItemSpec(params ItemSpec[] childSpecs)
        {
            Specificity = SpecSpecificity.CompositeSpecificity;
            this.childSpecs = childSpecs;
        }

        private ModelId? Type { get; }
        private Color? Color { get; }
        public SpecSpecificity Specificity { get; }

        public bool Matches(Item item)
        {
            if (Type.HasValue)
                return item.Type == Type && (!Color.HasValue || Color == item.Color);
            if (childSpecs != null && childSpecs.Length > 0)
                return childSpecs.Any(s => s.Matches(item));

            throw new NotImplementedException();
        }

        public bool IsKindOf(ItemSpec spec)
        {
            if (Type.HasValue && spec.Type.HasValue)
            {
                if (spec.Color.HasValue)
                {
                    if (!Color.HasValue)
                        return false;

                    return spec.Type.Value == Type.Value && Color.Value == spec.Color.Value;
                }

                return spec.Type.Value == Type.Value;
            }

            if (spec.childSpecs != null && spec.childSpecs.Length > 0)
            {
                if ((childSpecs == null || childSpecs.Length == 0))
                    return spec.childSpecs.Any(s => s.IsKindOf(this));
                else
                    return spec.childSpecs.All(s => childSpecs.Any(x => s.IsKindOf(x)));
            }

            return false;
        }

        public bool Matches(ModelId type)
        {
            if (Type.HasValue)
                return type == Type && !Color.HasValue;

            return childSpecs.Any(s => s.Matches(type));
        }

        public ItemSpec Including(params ItemSpec[] childSpecs)
        {
            return new ItemSpec(childSpecs.Concat(new[] { this }).ToArray());
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
            return new ItemSpec(types.Select(t => new ItemSpec((ushort)t)).ToArray());
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