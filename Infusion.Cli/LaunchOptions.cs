using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Cli
{
    [Verb("launch", HelpText = "Starts Infusion proxy and connects to a specified Ultima Online server.")]
    internal sealed class LaunchOptions
    {
        [Option("server", Required = true, HelpText = "Ultima Online server address, including port number. For example: 'server.uoerbor.cz,2593'.")]
        public string ServerAddress { get; set; }

        [Option("protocolVersion", Required = true, HelpText = "Communication protocol version.")]
        public Version ProtocolVersion { get; set; }

        [Option("script", HelpText = "Initial script file name.")]
        public string ScriptFileName { get; set; }
    }
}
