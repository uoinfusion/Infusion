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

            var result = parser.ParseArguments<CommandSenderOptions, HeadlessOptions>(args)
                .WithParsed<CommandSenderOptions>(options =>
                {
                    CommandPipeSender.Send(options.PipeName, options.Command);
                })
                .WithParsed<HeadlessOptions>(options =>
                {
                    Console.WriteLine("Press ctrl + c to exit headless Infusion client.");
                    Console.WriteLine();

                    var infusion = new HeadlessInfusion(options);
                    infusion.StartClient();
                    infusion.ListenPipeCommands();
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
