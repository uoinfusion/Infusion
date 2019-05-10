using Infusion.Config;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia
{
    public class InjectionWindowConfiguration
    {
        private int x;
        private int y;

        public int X
        {
            get => x;
            // x position of window should never be negative, but user can have already negative position in her/his profile
            set => x = value >= 0 ? value : 0;
        }

        public int Y
        {
            get => y;
            // y position of window should never be negative, but user can have already negative position in her/his profile
            set => y = value >= 0 ? value : 0;
        }

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
