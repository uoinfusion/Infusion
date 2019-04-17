using Infusion.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia
{
    public class InjectionWindowConfiguration
    {
        private readonly ConfigBag configBag;

        public InjectionWindowConfiguration(ConfigBag configBag)
        {
            this.configBag = configBag;

            configBag.Register(() => WindowX);
            configBag.Register(() => WindowY);

            configBag.Register(() => ShowInjectionWindow);
            configBag.Register(() => AlwaysLight);
        }

        public void Save() => configBag.Save();

        public int WindowX { get; set; }
        public int WindowY { get; set; }

        public bool ShowInjectionWindow { get; set; }
        public bool AlwaysLight { get; set; }
    }
}
