namespace Infusion.Packets
{
    public struct ModelId
    {
        public ushort Value { get; }

        public ModelId(ushort value)
        {
            Value = value;
        }

        public static implicit operator ModelId(ushort value)
        {
            return new ModelId(value);
        }

        public static implicit operator ushort(ModelId model)
        {
            return model.Value;
        }

        public static bool operator ==(ModelId model1, ModelId model2)
        {
            return model1.Equals(model2);
        }

        public static bool operator !=(ModelId model1, ModelId model2)
        {
            return !model1.Equals(model2);
        }

        public override bool Equals(object obj)
        {
            if (obj is ModelId)
            {
                var model = (ModelId) obj;
                return Equals(model);
            }

            return false;
        }

        public bool Equals(ModelId other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString("X4");
        }
    }
}