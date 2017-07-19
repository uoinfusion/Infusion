using System;

namespace Infusion.Commands
{
    public struct CommandAutocompletion
    {
        public static CommandAutocompletion Empty = new CommandAutocompletion(Array.Empty<string>(), null);

        internal CommandAutocompletion(string[] potentialPotentialCommandNames, string autocompletedCommandLineLine)
        {
            PotentialCommandNames = potentialPotentialCommandNames;
            AutocompletedCommandLine = autocompletedCommandLineLine;
        }

        public bool IsAutocompleted => !string.IsNullOrEmpty(AutocompletedCommandLine);

        public string[] PotentialCommandNames { get; }
        public string AutocompletedCommandLine { get; }
    }
}