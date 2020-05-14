namespace Infusion.Commands
{
    public class CommandSyntax
    {
        internal CommandSyntax(string name, string parameters, string prefix)
        {
            Name = name;
            Parameters = parameters;
            Prefix = prefix;
            PrefixAndName = Prefix + Name;
        }

        public bool HasParameters => !string.IsNullOrEmpty(Parameters);

        public string Name { get; }
        public string PrefixAndName { get; }
        public string Parameters { get; }
        public string Prefix { get; }
    }
}