namespace Infusion.Commands
{
    public static class CommandParser
    {
        public static CommandSyntax Parse(string commandInvocationSyntax)
        {
            string name;
            var parameters = string.Empty;

            var firstSpaceIndex = commandInvocationSyntax.IndexOf(' ');
            if (firstSpaceIndex < 0)
                name = commandInvocationSyntax.Substring(1, commandInvocationSyntax.Length - 1);
            else
            {
                name = commandInvocationSyntax.Substring(1, firstSpaceIndex - 1);
                parameters = commandInvocationSyntax.Substring(firstSpaceIndex + 1,
                    commandInvocationSyntax.Length - firstSpaceIndex - 1);
            }

            return new CommandSyntax(name, parameters);
        }
    }
}