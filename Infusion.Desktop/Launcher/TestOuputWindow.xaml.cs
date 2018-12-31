using Infusion.Desktop.Scripts;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Infusion.Desktop.Launcher
{
    /// <summary>
    /// Interaction logic for TestOuputWindow.xaml
    /// </summary>
    public partial class TestOuputWindow : Window
    {
        public ScriptEngine ScriptEngine => _console.ScriptEngine.Value;

        public TestOuputWindow()
        {
            InitializeComponent();
        }
    }
}
