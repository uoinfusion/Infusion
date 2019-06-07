using Infusion.Config;
using System;

namespace Infusion.Proxy.Launcher
{
    public static class ConfigBagRepositoryLauncherOptionsExtensions
    {
        private const string name = "launcher";

        public static LauncherOptions GetLauncherOptions(this IConfigBagRepository configBag, LauncherOptions defaultValue)
            => configBag.Get(name, defaultValue);

        public static LauncherOptions GetLauncherOptions(this IConfigBagRepository configBag, Func<LauncherOptions> defaultValue)
            => configBag.Get(name, defaultValue);

        public static LauncherOptions GetLauncherOptions(this IConfigBagRepository configBag) 
            => configBag.Get<LauncherOptions>(name);

        public static void UpdateLauncherOptions(this IConfigBagRepository configBag, LauncherOptions value)
            => configBag.Update(name, value);
    }
}
