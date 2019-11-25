using CommandLine;
using CommandLine.Text;
using System;
using System.Globalization;

namespace Infusion.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var parser = new Parser(c =>
            {
                c.AutoHelp = true;
                c.AutoVersion = true;
                c.IgnoreUnknownArguments = false;
                c.ParsingCulture = CultureInfo.InvariantCulture;
            });

            var result = parser.ParseArguments<SendCommandOptions, LaunchHeadlessOptions, LaunchOptions>(args)
                .WithParsed<SendCommandOptions>(options =>
                {
                    SendCommand.Send(options.PipeName, options.Command);
                })
                .WithParsed<LaunchHeadlessOptions>(options =>
                {
                    Console.WriteLine("Press ctrl + c to exit headless Infusion client.");
                    Console.WriteLine();

                    var infusion = new LaunchHeadlessCommand(options);
                    infusion.StartClient();
                    infusion.ListenPipeCommands();
                })
                .WithParsed<LaunchOptions>(options =>
                {
                    var command = new LaunchCommand(options);
                    command.Execute();
                });

            result.WithNotParsed(errors =>
            {
                var helpText = HelpText.AutoBuild(result,
                    h =>
                    {
                        h.AdditionalNewLineAfterOption = false;
                        return HelpText.DefaultParsingErrorsHandler(result, h);
                    },
                    e => e);

                Console.WriteLine(helpText);
            });

            return;
        }
    }
}
