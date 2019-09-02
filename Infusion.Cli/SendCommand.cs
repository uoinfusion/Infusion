using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Cli
{
    internal static class SendCommand
    {
        public static void Send(string pipeName, string command)
        {
            using (var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.None))
            {
                Console.WriteLine($"Connecting to './{pipeName}' pipe...");
                pipeClient.Connect();

                Console.WriteLine($"Sending '{command}'");
                var writer = new StreamString(pipeClient);
                writer.WriteString(command);
            }
        }
    }
}
