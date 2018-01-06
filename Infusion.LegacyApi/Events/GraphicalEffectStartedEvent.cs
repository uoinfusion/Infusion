using Infusion.Packets.Server;

namespace Infusion.LegacyApi.Events
{
    public class GraphicalEffectStartedEvent : IEvent
    {
        internal GraphicalEffectStartedEvent(EffectDirectionType directionType, ObjectId characterId,
            ObjectId targetId, ModelId type, Location3D location, Location3D targetLocation)
        {
            DirectionType = directionType;
            CharacterId = characterId;
            TargetId = targetId;
            Type = type;
            Location = location;
            TargetLocation = targetLocation;
        }

        public EffectDirectionType DirectionType { get; }
        public ObjectId CharacterId { get; }
        public ObjectId TargetId { get; }
        public ModelId Type { get; }
        public Location3D Location { get; }
        public Location3D TargetLocation { get; }
    }
}