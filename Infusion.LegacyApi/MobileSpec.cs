using System;
using System.Linq;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class MobileSpec
    {
        private readonly MobileSpec[] childSpecs;

        public MobileSpec(ModelId type, Color? color = null)
        {
            Specificity = color.HasValue ? ItemSpecSpecificity.TypeAndColor : ItemSpecSpecificity.Type;

            Type = type;
            Color = color;
        }

        public MobileSpec(params MobileSpec[] childSpecs)
        {
            Specificity = ItemSpecSpecificity.CompositeSpecificity;
            this.childSpecs = childSpecs;
        }

        private ModelId? Type { get; }
        private Color? Color { get; }
        public ItemSpecSpecificity Specificity { get; }

        public bool Matches(Mobile item)
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

        public MobileSpec Including(params MobileSpec[] childSpecs)
        {
            return new MobileSpec(childSpecs.Concat(new[] { this }).ToArray());
        }

        public static implicit operator MobileSpec(ushort[] types)
        {
            return new MobileSpec(types.Select(t => new MobileSpec(t)).ToArray());
        }

        public static implicit operator MobileSpec(ModelId[] types)
        {
            return new MobileSpec(types.Select(t => new MobileSpec(t)).ToArray());
        }

        public static implicit operator MobileSpec(int[] types)
        {
            return new MobileSpec(types.Select(t => new MobileSpec((ushort)t)).ToArray());
        }

        public static implicit operator MobileSpec(MobileSpec[] specs)
        {
            return new MobileSpec(specs);
        }

        public static implicit operator MobileSpec(ushort type)
        {
            return new MobileSpec(type);
        }
    }
}