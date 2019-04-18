using Infusion.Injection.Avalonia.Main;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia.TestApp
{
    public class TestMainServices : IMainServices
    {
        public bool AutoOpen { get; set; }

        public void SetLightFiltering(bool value) { }
    }
}
