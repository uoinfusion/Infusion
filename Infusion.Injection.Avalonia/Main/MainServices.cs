using Infusion.LegacyApi;
using Infusion.LegacyApi.Injection;

namespace Infusion.Injection.Avalonia.Main
{
    public class MainServices : IMainServices
    {
        private readonly Legacy infusionApi;
        private readonly InjectionHost injectionHost;

        public MainServices(Legacy infusionApi, InjectionHost injectionHost)
        {
            this.infusionApi = infusionApi;
            this.injectionHost = injectionHost;
        }

        public bool AutoOpen { get => injectionHost.AutoOpenGui; set => injectionHost.AutoOpenGui = value; }

        public void SetLightFiltering(bool value)
        {
            if (value)
                infusionApi.ClientFilters.Light.Enable();
            else
                infusionApi.ClientFilters.Light.Disable();
        }
    }
}
