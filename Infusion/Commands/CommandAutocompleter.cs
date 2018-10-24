using System;
using System.Collections.Generic;
using System.Linq;

namespace Infusion.Commands
{
    public class CommandAutocompleter
    {
        private readonly Func<IEnumerable<string>> commandNameSource;

        public CommandAutocompleter(Func<IEnumerable<string>> commandNameSource)
        {
            this.commandNameSource = commandNameSource;
        }

        private string GetSharedStart(string[] words)
        {
            return new string(
                words.First().Substring(0, words.Min(s => s.Length))
                    .TakeWhile((c, i) => words.All(s => s[i] == c)).ToArray());
        }

        public CommandAutocompletion Autocomplete(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
                return CommandAutocompletion.Empty;

            var syntax = CommandParser.Parse(commandLine);

            if (syntax.HasParameters)
                return CommandAutocompletion.Empty;

            var exactMatch = commandNameSource().FirstOrDefault(x => x == syntax.Name);
            if (!string.IsNullOrEmpty(exactMatch))
                return new CommandAutocompletion(new[] {exactMatch}, "," + exactMatch + " ");

            var startingWith = commandNameSource().Where(x => x.StartsWith(syntax.Name)).OrderBy(x => x).ToArray();
            if (startingWith.Length == 1)
                return new CommandAutocompletion(startingWith, "," + startingWith[0] + " ");
            if (startingWith.Length > 1)
                return new CommandAutocompletion(startingWith,
                    startingWith.Length > 0 ? "," + GetSharedStart(startingWith) : null);

            return new CommandAutocompletion(commandNameSource().OrderBy(x => x).ToArray(), null);
        }
    }
}