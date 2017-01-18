using System;

namespace UltimaRX.Gumps
{
    internal sealed class GumpFailureResponse : GumpResponse
    {
        public GumpFailureResponse(Gump gump, string failureMessage) : base(gump)
        {
            FailureMessage = failureMessage;
        }

        public string FailureMessage { get; }

        public override void Execute()
        {
            throw new InvalidOperationException(FailureMessage);
        }
    }
}