namespace UltimaRX.Gumps
{
    public sealed class CancelGumpResponse : GumpResponse
    {
        public CancelGumpResponse(Gump gump) : base(gump)
        {
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}