namespace Infusion.LegacyApi.Events
{
    public sealed class SoundEffectPlayedEvent : IEvent
    {
        public Location3D Location { get; }
        public SoundId Id { get; }

        internal SoundEffectPlayedEvent(SoundId soundId, Location3D location)
        {
            Id = soundId;
            Location = location;
        }
    }
}