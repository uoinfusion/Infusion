namespace Infusion.Proxy.LegacyApi
{
    public enum CommandExecutionMode
    {
        /// <summary>
        /// Command is executed as a script and can be terminated. If you don't know
        /// which mode to use, select Script.
        /// </summary>
        Script,

        /// <summary>
        /// Command is executed on its own thread and cannot be terminated. Use it
        /// only in cases where you really know what you are trying to achieve.
        /// </summary>
        OwnThread
    }
}