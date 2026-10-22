namespace Infusion.Scripts.UOErebor.Extensions.StatusBars
{
    public sealed class StatusesConfiguration
    {
        public int Width { get; set; }
        public int Height { get; set;  }

        public WindowDock GameClientWindowDock { get; set; }

        public StatusesConfiguration(int width, int height, WindowDock gameClientWindowDock)
        {
            Width = width;
            Height = height;
            GameClientWindowDock = gameClientWindowDock;
        }
    }
}