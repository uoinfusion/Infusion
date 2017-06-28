using System;

namespace Infusion.Gumps
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
            throw new GumpException(FailureMessage);
        }
    }
}