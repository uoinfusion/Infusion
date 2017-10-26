namespace Infusion.LegacyApi.Events
{
    public sealed class SkillRequestedEvent : IEvent
    {
        internal SkillRequestedEvent(Skill skill)
        {
            Skill = skill;
        }

        public Skill Skill { get; }
    }
}