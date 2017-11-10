using System;
using System.CodeDom;

namespace Infusion.LegacyApi
{
    public struct TargetInfo
    {
        public Location3D Location { get; }
        public TargetType Type { get; }
        public ModelId ModelId { get; }
        public ObjectId? Id { get; }

        public bool TargetsObject => Type == TargetType.Object && Id.HasValue;

        public TargetInfo(Location3D location, TargetType type, ModelId modelId, ObjectId? id)
        {
            Location = location;
            Type = type;
            ModelId = modelId;
            Id = id;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case TargetType.Object:
                    return $"{ModelId} {Id}";
                case TargetType.Tile:
                    return $"{ModelId} {Location.X} {Location.Y} {Location.Z}";
                default:
                    throw new NotImplementedException($"Support for TargetType {Type} not implemented.");
            } 
        }

        public override bool Equals(object obj)
        {
            if (obj is TargetInfo other)
            {
                return other.Id == this.Id && other.Type == this.Type && other.ModelId == this.ModelId &&
                       other.Id == this.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Location.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) Type;
                hashCode = (hashCode * 397) ^ ModelId.GetHashCode();
                hashCode = (hashCode * 397) ^ Id.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(TargetInfo other)
        {
            return Location.Equals(other.Location) && Type == other.Type && ModelId.Equals(other.ModelId) && Id.Equals(other.Id);
        }

        public static bool operator ==(TargetInfo info1, TargetInfo info2)
            => info1.Equals(info2);

        public static bool operator !=(TargetInfo info1, TargetInfo info2)
            => !info1.Equals(info2);

 
    }
}