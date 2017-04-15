using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infusion.Desktop.Launcher;
using Infusion.Desktop.Profiles;
using RoslynPad.UI;

namespace Infusion.Desktop
{
    /// <summary>
    /// Interaction logic for ScriptsControl.xaml
    /// </summary>
    public partial class ScriptsControl : UserControl
    {
        private CSharpScriptEngine scriptEngine;
        private LauncherOptions options;

        public ScriptsControl()
        {
            InitializeComponent();
        }

        public void Initialize(CSharpScriptEngine scriptEngine, LauncherOptions options)
        {
            this.scriptEngine = scriptEngine;
            this.options = options;
            _scriptTextBox.Text = options.InitialScriptFileName;
        }

        public void Edit()
        {
            string scriptFileName = _scriptTextBox.Text;
            if (!string.IsNullOrEmpty(scriptFileName) && File.Exists(scriptFileName))
            {
                string scriptPath = System.IO.Path.GetDirectoryName(scriptFileName);
                scriptEngine.ScriptRootPath = scriptPath;
                var roslynPadWindow = new RoslynPad.MainWindow(scriptEngine, scriptPath);
                roslynPadWindow.Show();
            }
        }

        public void Reload()
        {
            string scriptFileName = null;

            Dispatcher.Invoke(() => scriptFileName = _scriptTextBox.Text);
#pragma warning disable 4014
            Reload(scriptFileName);
#pragma warning restore 4014
        }

        private async Task Reload(string scriptFileName)
        {
            if (!string.IsNullOrEmpty(scriptFileName) && File.Exists(scriptFileName))
            {
                await scriptEngine.ExecuteScript(_scriptTextBox.Text);
            }
            else
            {
                // TODO: handle error
            }
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            Edit();
        }

        private async void ReloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Reload(_scriptTextBox.Text);
        }

        private void PickButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
