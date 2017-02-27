namespace Infusion.Gumps
{
    public abstract class GumpResponse
    {
        public GumpResponse(Gump gump)
        {
            Gump = gump;
        }

        public Gump Gump { get; }

        public abstract void Execute();
    }
}