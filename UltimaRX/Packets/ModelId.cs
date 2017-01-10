namespace UltimaRX.Packets
{
    public struct ModelId
    {
        private readonly ushort value;

        public ModelId(ushort value)
        {
            this.value = value;
        }

        public static explicit operator ModelId(ushort value)
        {
            return new ModelId(value);
        }

        public static explicit operator ushort(ModelId model)
        {
            return model.value;
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
            return value == other.value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString("X4");
        }
    }
}