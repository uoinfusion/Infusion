namespace Infusion
{
    public struct ModelId
    {
        public ModelId(ushort value) => Value = value;
        public ushort Value { get; }

        public static implicit operator ModelId(ushort value) => new ModelId(value);

        public static implicit operator ushort(ModelId model) => model.Value;

        public static bool operator ==(ModelId model1, ModelId model2) => model1.Equals(model2);

        public static bool operator !=(ModelId model1, ModelId model2) => !model1.Equals(model2);

        public override bool Equals(object obj)
        {
            if (obj is ModelId)
            {
                var model = (ModelId) obj;
                return Equals(model);
            }

            return false;
        }

        public bool Equals(ModelId other) => Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"0x{Value:X4}";
    }
}