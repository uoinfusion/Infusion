namespace Infusion
{
    internal enum UltimaClientConnectionStatus
    {
        Initial,
        AfterInitialSeed,
        ServerLogin,
        PreGameLogin,
        GameLogin,
        Game,
    }
}