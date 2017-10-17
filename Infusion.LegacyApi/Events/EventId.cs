using System;
using System.Threading;

namespace Infusion.LegacyApi.Events
{
    public struct EventId : IComparable<EventId>, IEquatable<EventId>
    {
        internal EventId(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public int CompareTo(EventId other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(EventId other)
        {
            return Value == other.Value;
        }

        public override string ToString() => Value.ToString();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is EventId ev && Equals(ev);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator >(EventId ev1, EventId ev2) => ev1.Value > ev2.Value;
        public static bool operator <(EventId ev1, EventId ev2) => ev1.Value < ev2.Value;
        public static bool operator >=(EventId ev1, EventId ev2) => ev1.Value >= ev2.Value;
        public static bool operator <=(EventId ev1, EventId ev2) => ev1.Value <= ev2.Value;
        public static bool operator ==(EventId ev1, EventId ev2) => ev1.Equals(ev2);
        public static bool operator !=(EventId ev1, EventId ev2) => !ev1.Equals(ev2);
    }
}