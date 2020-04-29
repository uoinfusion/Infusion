using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ultima;

namespace Infusion.LegacyApi
{
    public class TileSpec
    {
        private readonly TileSpec[] childSpecs;

        public ModelId? Type { get; }
        public Color? Color { get; }
        public int? Z { get; }

        public SpecSpecificity Specificity { get; }

        public TileSpec(ModelId type, Color? color = null, int? z = null)
        {
            Specificity = color.HasValue ? SpecSpecificity.TypeAndColor : SpecSpecificity.Type;

            Type = type;
            Color = color;
            Z = z;
        }

        public TileSpec(params TileSpec[] childSpecs)
        {
            for (int i = 0; i < childSpecs.Length; i++)
            {
                if (childSpecs[i] == null)
                    throw new ArgumentException($"Subspec at index {0} is null.");
            }

            this.childSpecs = childSpecs;
            Specificity = SpecSpecificity.CompositeSpecificity;
        }

        internal bool Matches(HuedTile tile)
        {
            if (Type.HasValue)
                return tile.ID == Type && (!Color.HasValue || Color == tile.Hue);
            if (childSpecs != null && childSpecs.Length > 0)
                return childSpecs.Any(s => s.Matches(tile));

            throw new NotImplementedException();
        }

        public bool Matches(Tile tile)
        {
            if (Type.HasValue)
                return tile.Type == Type && (!Color.HasValue || Color == tile.Color);
            if (childSpecs != null && childSpecs.Length > 0)
                return childSpecs.Any(s => s.Matches(tile));

            throw new NotImplementedException();
        }

        public bool Matches(ModelId type)
        {
            if (Type.HasValue)
                return type == Type && !Color.HasValue;

            return childSpecs.Any(s => s.Matches(type));
        }

        public bool IsKindOf(TileSpec spec)
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

        public TileSpec Including(params TileSpec[] childSpecs) 
            => new TileSpec(childSpecs.Concat(new[] { this }).ToArray());

        public static implicit operator TileSpec(ushort[] types) 
            => new TileSpec(types.Select(t => new TileSpec(t)).ToArray());

        public static implicit operator TileSpec(ModelId[] types) 
            => new TileSpec(types.Select(t => new TileSpec(t)).ToArray());

        public static implicit operator TileSpec(int[] types) 
            => new TileSpec(types.Select(t => new TileSpec((ushort)t)).ToArray());

        public static implicit operator TileSpec(TileSpec[] specs) 
            => new TileSpec(specs);

        public static implicit operator TileSpec(ushort type) 
            => new TileSpec(type);
    }
}
