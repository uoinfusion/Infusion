namespace Infusion.Commands
{
    public struct CommandSyntax
    {
        internal CommandSyntax(string name, string parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        public bool HasParameters => !string.IsNullOrEmpty(Parameters);

        public string Name { get; }
        public string Parameters { get; }
    }
}