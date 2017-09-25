using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace Infusion.Scripts.UOErebor.Extensions.StatusBars
{
    /// <summary>
    /// Interaction logic for StatusesWindow.xaml
    /// </summary>
    public partial class StatusesWindow : Window
    {
        private readonly Statuses statusBarCollection;

        public StatusesWindow(Statuses statusBarCollection)
        {
            this.statusBarCollection = statusBarCollection;
            InitializeComponent();
            this.statusBarCollection.StatusBars.CollectionChanged += StatusBarsOnCollectionChanged;

            CreateStatusControls(statusBarCollection.StatusBars);
        }

        private void CreateStatusControls(IEnumerable<StatusBar> statusBars)
        {
            foreach (var statusBar in statusBars)
            {
                var statusBarControl =
                    new StatusBarControl(statusBar);
                _panel.Children.Add(statusBarControl);
                statusBarControl.Render();
            }
        }

        private void StatusBarsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CreateStatusControls(notifyCollectionChangedEventArgs.NewItems.Cast<StatusBar>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (StatusBar statusBar in notifyCollectionChangedEventArgs.OldItems)
                    {
                        var control = _panel.Children.OfType<StatusBarControl>().FirstOrDefault(x => x.StatusBar == statusBar);
                        if (control != null)
                            _panel.Children.Remove(control);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _panel.Children.Clear();
                    break;
                case NotifyCollectionChangedAction.Move:
                    var movedControl = _panel.Children[notifyCollectionChangedEventArgs.OldStartingIndex];
                    _panel.Children.RemoveAt(notifyCollectionChangedEventArgs.OldStartingIndex);
                    _panel.Children.Insert(notifyCollectionChangedEventArgs.NewStartingIndex, movedControl);
                    break;
            }
        }
    }
}
