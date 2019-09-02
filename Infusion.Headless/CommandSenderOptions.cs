using CommandLine;

namespace Infusion.Headless
{
    [Verb("sendcmd", HelpText = "Sends a command to a Infusion Proxy instance.")]
    public class CommandSenderOptions
    {
        [Option("pipe", HelpText = "Pipe name to connect to Infusion proxy.", Default = "infusionPipe")]
        public string PipeName { get; set; }

        [Option("cmd", Required = true, HelpText = "Command or speech to be send to Infusion proxy.")]
        public string Command { get; set; }
    }
}
