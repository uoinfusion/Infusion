using Infusion.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia
{
    public class InjectionWindowConfiguration
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool AutoOpen { get; set; }
        public bool AlwaysOnTop { get; set; }
    }

    public class InjectionOptionsConfiguration
    {
        public bool Light { get; set; }
    }

    public class InjectionConfiguration
    {
        private readonly ConfigBag configBag;
        public InjectionWindowConfiguration Window { get; set; } = new InjectionWindowConfiguration();
        public InjectionOptionsConfiguration Options { get; set; } = new InjectionOptionsConfiguration();

        public InjectionConfiguration(ConfigBag configBag)
        {
            this.configBag = configBag;

            configBag.Register("injection.window", () => Window);
            configBag.Register("injection.options", () => Options);
        }

        public void Save() => configBag.Save();
    }
}
