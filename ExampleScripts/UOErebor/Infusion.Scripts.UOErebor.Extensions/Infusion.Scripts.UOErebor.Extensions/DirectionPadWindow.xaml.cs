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
using Infusion.LegacyApi;

namespace Infusion.Scripts.UOErebor.Extensions
{
    public partial class DirectionPadWindow : Window
    {
        public DirectionPadWindow()
        {
            InitializeComponent();
        }

        private void NorthWest_Click(object sender, RoutedEventArgs e)
        {
            UO.ClientWindow.PressKey(KeyCode.Up);
        }

        private void North_Click(object sender, RoutedEventArgs e)
        {
            UO.ClientWindow.PressKey(KeyCode.PageUp);
        }

        private void NorthEast_Click(object sender, RoutedEventArgs e)
        {
            UO.ClientWindow.PressKey(KeyCode.Right);
        }

        private void West_Click(object sender, RoutedEventArgs e)
        {
            UO.ClientWindow.PressKey(KeyCode.Home);
        }

        private void East_Click(object sender, RoutedEventArgs e)
        {
            UO.ClientWindow.PressKey(KeyCode.PageDown);
        }

        private void SouthWest_Click(object sender, RoutedEventArgs e)
        {
            UO.ClientWindow.PressKey(KeyCode.Left);
        }

        private void South_Click(object sender, RoutedEventArgs e)
        {
            UO.ClientWindow.PressKey(KeyCode.End);
        }

        private void SouthEast_Click(object sender, RoutedEventArgs e)
        {
            UO.ClientWindow.PressKey(KeyCode.Down);
        }

        private static Direction[] clockwiseRotation = { Direction.North, Direction.Northeast, Direction.East, Direction.Southeast, Direction.South, Direction.Southwest, Direction.West, Direction.Northwest,  };

        private void Rotate_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < clockwiseRotation.Length; i++)
            {
                ChangeDirectionClockwise();
                UO.Wait(20);
            }
        }

        private void ChangeDirectionClockwise()
        {
            var currentDirection = clockwiseRotation.Select((x, idx) => new { Direction = x, Index = idx })
                .First(x => x.Direction == UO.Me.Direction);

            var nextIndex = currentDirection.Index == clockwiseRotation.Length - 1 ? 0 : currentDirection.Index + 1;
            UO.Walk(clockwiseRotation[nextIndex]);
        }
    }
}
