using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Cli
{
    [Verb("launch", HelpText = "Starts Infusion proxy and connects to a specified Ultima Online server.")]
    internal sealed class LaunchOptions
    {
        [Option("server", Required = true, HelpText = "Ultima Online server address, without port number. For example: 'server.uoerbor.cz'.")]
        public string ServerAddress { get; set; }

        [Option("port", HelpText = "Ultima Online server port number. Default value is 2593.")]
        public int Port { get; set; } = 2593;

        [Option("proxyPort", HelpText = "Local proxy port number. Default value is 60000.")]
        public int LocalPort { get; set; } = 60000;

        [Option("protocolVersion", Required = true, HelpText = "Communication protocol version.")]
        public Version ProtocolVersion { get; set; }

        [Option("script", HelpText = "Initial script file name.")]
        public string ScriptFileName { get; set; }

        [Option("encrypt", HelpText = "Client encryption. Valid values are 'add' (adds encryption to the client), 'remove' (removes encryption from client), 'auto' (uses client encryption as-is). Default value is 'auto'.")]
        public ClientEncryption Encryption { get; set; } = ClientEncryption.auto;
    }
}
