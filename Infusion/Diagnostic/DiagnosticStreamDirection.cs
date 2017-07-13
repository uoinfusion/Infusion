namespace Infusion.Diagnostic
{
    public enum DiagnosticStreamDirection : uint
    {
        ServerToClient = 0xEEBEADDE,
        ClientToServer = 0xEFBEADDE
    }
}
