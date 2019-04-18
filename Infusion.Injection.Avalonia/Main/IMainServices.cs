using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia.Main
{
    public interface IMainServices
    {
        bool AutoOpen { get; set; }
        void SetLightFiltering(bool value);
    }
}
