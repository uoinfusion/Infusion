namespace Infusion.LegacyApi.Events
{
    public struct SoundEffectPlayedEvent
    {
        public Location3D Location { get; }
        public SoundId Id { get; }

        public SoundEffectPlayedEvent(SoundId soundId, Location3D location)
        {
            Id = soundId;
            Location = location;
        }
    }
}