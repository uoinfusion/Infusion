using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class SkillChangedEvent : IEvent
    {
        public Skill Skill { get; }
        public int OldValue { get; }
        public int NewValue { get; }

        internal SkillChangedEvent(Skill skill, int oldValue, int newValue)
        {
            Skill = skill;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
