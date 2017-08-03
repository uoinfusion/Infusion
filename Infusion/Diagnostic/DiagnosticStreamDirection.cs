namespace Infusion.Diagnostic
{
    internal enum DiagnosticStreamDirection : uint
    {
        ServerToClient = 0xEEBEADDE,
        ClientToServer = 0xEFBEADDE
    }
}
