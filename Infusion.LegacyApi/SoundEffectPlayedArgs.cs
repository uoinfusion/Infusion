namespace Infusion.LegacyApi
{
    public struct SoundEffectPlayedArgs
    {
        public Location3D Location { get; }
        public SoundId Id { get; }

        public SoundEffectPlayedArgs(SoundId soundId, Location3D location)
        {
            Id = soundId;
            Location = location;
        }
    }
}