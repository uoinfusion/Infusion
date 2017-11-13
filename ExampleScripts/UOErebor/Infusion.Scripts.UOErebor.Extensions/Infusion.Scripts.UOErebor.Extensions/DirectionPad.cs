using System;
using System.Windows;
using System.Windows.Threading;

namespace Infusion.Scripts.UOErebor.Extensions
{
    public static class DirectionPad
    {
        private static DirectionPadWindow window;

        public static void Show()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (window != null)
                {
                    Hide();
                }

                window = new DirectionPadWindow();
                window.Show();
            }));
        }

        public static void Hide()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (window != null)
                {
                    window.Close();
                    window = null;
                }
            }));
        }
    }
}