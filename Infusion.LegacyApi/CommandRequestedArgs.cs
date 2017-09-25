namespace Infusion.LegacyApi
{
    public struct CommandRequestedArgs
    {
        public string InvocationSyntax { get; }

        public CommandRequestedArgs(string invocationSyntax)
        {
            InvocationSyntax = invocationSyntax;
        }
    }
}