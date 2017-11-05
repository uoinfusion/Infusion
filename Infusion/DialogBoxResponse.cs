namespace Infusion
{
    public class DialogBoxResponse
    {
        public byte Index { get; }
        public ModelId Type { get; }
        public Color Color { get; }
        public string Text { get; }

        public DialogBoxResponse(byte index, ModelId type, Color color, string text)
        {
            Index = index;
            Type = type;
            Color = color;
            Text = text;
        }

        public override string ToString()
            => $"{Index}. {Text} ({Type}/{Color})";
    }
}