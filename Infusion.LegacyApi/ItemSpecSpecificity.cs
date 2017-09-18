using System;

namespace Infusion.LegacyApi
{
    public struct ItemSpecSpecificity : IComparable<ItemSpecSpecificity>, IEquatable<ItemSpecSpecificity>
    {
        internal static readonly ItemSpecSpecificity CompositeSpecificity = new ItemSpecSpecificity(0, "Composite");
        internal static readonly ItemSpecSpecificity Type = new ItemSpecSpecificity(1, "Type");
        internal static readonly ItemSpecSpecificity TypeAndColor = new ItemSpecSpecificity(2, "Type and Color");
        internal static readonly ItemSpecSpecificity Name = new ItemSpecSpecificity(3, "Name");
        private readonly string description;

        private readonly int value;

        private ItemSpecSpecificity(int value, string description)
        {
            this.value = value;
            this.description = description;
        }

        public int CompareTo(ItemSpecSpecificity other) => value.CompareTo(other.value);

        public bool Equals(ItemSpecSpecificity other) => value == other.value;

        public override string ToString() => description;

        public override bool Equals(object obj) =>
            obj != null && obj is ItemSpecSpecificity other && Equals(other);

        public override int GetHashCode() => value.GetHashCode();

        public static bool operator ==(ItemSpecSpecificity specificity1, ItemSpecSpecificity specificity2) =>
            specificity1.Equals(specificity2);

        public static bool operator !=(ItemSpecSpecificity specificity1, ItemSpecSpecificity specificity2) =>
            !specificity1.Equals(specificity2);

        public static bool operator >(ItemSpecSpecificity specificity1, ItemSpecSpecificity specificity2) =>
            specificity1.CompareTo(specificity2) == 1;

        public static bool operator >=(ItemSpecSpecificity specificity1, ItemSpecSpecificity specificity2) =>
            specificity1.CompareTo(specificity2) >= 0;

        public static bool operator <(ItemSpecSpecificity specificity1, ItemSpecSpecificity specificity2) =>
            specificity1.CompareTo(specificity2) == -1;

        public static bool operator <=(ItemSpecSpecificity specificity1, ItemSpecSpecificity specificity2) =>
            specificity1.CompareTo(specificity2) <= 0;
    }
}