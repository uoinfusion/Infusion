using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infusion.Avalonia.Common
{
    public class FileControl : UserControl
    {
        public FileControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public string FilterName { get; set; }
        public string FilterExtensions { get; set; }

        public FileDialogFilter Filter { get; set; }

        private async void OnSelectPath(object sender, RoutedEventArgs e)
        {
            DataContext = await SelectPath((string)DataContext);
        }

        private async Task<string> SelectPath(string initialPath)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.AllowMultiple = false;


            //if (!string.IsNullOrEmpty(FilterName))
            //{
            //    var filter = new FileDialogFilter();
            //    filter.Name = FilterName;
            //    if (string.IsNullOrEmpty(FilterExtensions))
            //        filter.Extensions = FilterExtensions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //    openFileDialog.Filters = new List<FileDialogFilter>() { filter };
            //}

            openFileDialog.Filters = new List<FileDialogFilter>() { Filter };

            openFileDialog.InitialDirectory = Path.GetDirectoryName(initialPath);
            var result = await openFileDialog.ShowAsync(VisualRoot as Window);

            if (result.Any())
                return result.Single();

            return initialPath;
        }
    }
}
