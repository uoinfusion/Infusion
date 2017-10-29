using System;

namespace Infusion.LegacyApi
{
    public struct TargetInfo
    {
        public Location3D Location { get; }
        public TargetType Type { get; }
        public ModelId ModelId { get; }
        public ObjectId? Id { get; }

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
                case TargetType.Item:
                    return $"{ModelId} {Id ?? 0}";
                case TargetType.Tile:
                    return $"{ModelId} {Location.X} {Location.Y} {Location.Z}";
                default:
                    throw new NotImplementedException($"Support for TargetType {Type} not implemented.");
            }            
        }
    }
}