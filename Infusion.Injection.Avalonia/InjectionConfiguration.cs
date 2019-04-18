using Infusion.Config;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia
{
    public class InjectionWindowConfiguration
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool AlwaysOnTop { get; set; }
    }

    public class InjectionConfiguration
    {
        private readonly ConfigBag configBag;
        public InjectionWindowConfiguration Window { get; set; } = new InjectionWindowConfiguration();
        public InjectionOptions Options { get; set; }

        public bool AutoOpen { get; set; }

        public InjectionConfiguration(ConfigBag configBag, InjectionOptions options)
        {
            this.configBag = configBag;
            this.Options = options;

            configBag.Register("injection.window", () => Window);
        }

        public void Save() => configBag.Save();
    }
}
