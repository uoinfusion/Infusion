using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Infusion.Injection.Avalonia.InjectionObjects
{
    public class ObjectsControl : UserControl
    {
        private readonly ObjectsViewModel viewModel;

        public ObjectsControl()
        {
            InitializeComponent();

            viewModel = new ObjectsViewModel();
            DataContext = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        internal void SetServices(IInjectionObjectServices objectServices)
        {
            viewModel.Services = objectServices;
        }
    }
}
