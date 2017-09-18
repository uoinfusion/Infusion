using System;

namespace Infusion.LegacyApi
{
    public struct SpecSpecificity : IComparable<SpecSpecificity>, IEquatable<SpecSpecificity>
    {
        internal static readonly SpecSpecificity CompositeSpecificity = new SpecSpecificity(0, "Composite");
        internal static readonly SpecSpecificity Type = new SpecSpecificity(1, "Type");
        internal static readonly SpecSpecificity TypeAndColor = new SpecSpecificity(2, "Type and Color");
        internal static readonly SpecSpecificity Name = new SpecSpecificity(3, "Name");
        private readonly string description;

        private readonly int value;

        private SpecSpecificity(int value, string description)
        {
            this.value = value;
            this.description = description;
        }

        public int CompareTo(SpecSpecificity other) => value.CompareTo(other.value);

        public bool Equals(SpecSpecificity other) => value == other.value;

        public override string ToString() => description;

        public override bool Equals(object obj) =>
            obj != null && obj is SpecSpecificity other && Equals(other);

        public override int GetHashCode() => value.GetHashCode();

        public static bool operator ==(SpecSpecificity specificity1, SpecSpecificity specificity2) =>
            specificity1.Equals(specificity2);

        public static bool operator !=(SpecSpecificity specificity1, SpecSpecificity specificity2) =>
            !specificity1.Equals(specificity2);

        public static bool operator >(SpecSpecificity specificity1, SpecSpecificity specificity2) =>
            specificity1.CompareTo(specificity2) == 1;

        public static bool operator >=(SpecSpecificity specificity1, SpecSpecificity specificity2) =>
            specificity1.CompareTo(specificity2) >= 0;

        public static bool operator <(SpecSpecificity specificity1, SpecSpecificity specificity2) =>
            specificity1.CompareTo(specificity2) == -1;

        public static bool operator <=(SpecSpecificity specificity1, SpecSpecificity specificity2) =>
            specificity1.CompareTo(specificity2) <= 0;
    }
}