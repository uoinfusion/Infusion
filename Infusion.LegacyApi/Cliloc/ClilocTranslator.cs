using System.Collections.Generic;

namespace Infusion.LegacyApi.Cliloc
{
    internal sealed class ClilocTranslator
    {
        private readonly IClilocSource clilocSource;

        public ClilocTranslator() : this(new MulClilocSource())
        {
        }

        public ClilocTranslator(IClilocSource clilocSource)
        {
            this.clilocSource = clilocSource;
        }

        public string Translate(int id, string arg)
            => Translate(clilocSource.GetString(id), arg);

        private string Translate(string baseCliloc, string arg)
        {
            if (baseCliloc == null)
                return null;


            while (arg.Length != 0 && arg[0] == '\t')
                arg = arg.Remove(0, 1);

            var arguments = new List<string>();

            while (true)
            {
                int pos = arg.IndexOf('\t');

                if (pos != -1)
                {
                    arguments.Add(arg.Substring(0, pos));
                    arg = arg.Substring(pos + 1);
                }
                else
                {
                    arguments.Add(arg);

                    break;
                }
            }

            for (int i = 0; i < arguments.Count; i++)
            {
                int pos = baseCliloc.IndexOf('~');

                if (pos == -1)
                    break;

                int pos2 = baseCliloc.IndexOf('~', pos + 1);

                if (pos2 == -1)
                    break;

                string a = arguments[i];

                if (a.Length > 1 && a[0] == '#')
                {
                    int id1 = int.Parse(a.Substring(1));
                    arguments[i] = clilocSource.GetString(id1);
                }

                baseCliloc = baseCliloc.Remove(pos, pos2 - pos + 1).Insert(pos, arguments[i]);
            }

            return baseCliloc;
        }
    }
}
