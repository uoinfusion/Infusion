using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ultima;
using Brushes = System.Windows.Media.Brushes;

namespace Infusion.Scripts.UOErebor.Extensions.StatusBars
{
    internal partial class StatusBarControl : UserControl
    {
        private static readonly Lazy<Bitmap> friendBackgroundBitmap =
            new Lazy<Bitmap>(() => LoadGump(0x804, Hues.GetHue(90)));

        private static readonly Lazy<Bitmap> enemyBackgroundBitmap =
            new Lazy<Bitmap>(() => LoadGump(0x804, Hues.GetHue(38)));

        private static readonly Lazy<Bitmap> petBackgroundBitmap = new Lazy<Bitmap>(() => LoadGump(0x804));
        private static readonly Lazy<Bitmap> emptyBarBitmap = new Lazy<Bitmap>(() => LoadGump(0x805));
        private static readonly Lazy<Bitmap> fullBarBitmap = new Lazy<Bitmap>(() => LoadGump(0x806));
        private static readonly Lazy<Bitmap> fullBarPoisonedBitmap = new Lazy<Bitmap>(() => LoadGump(0x808));
        private static readonly Lazy<Bitmap> resurrectionBitmap = new Lazy<Bitmap>(() => LoadGump(0x1B62));
        private Bitmap textBitmap;

        public StatusBarControl(StatusBar statusBar)
        {
            InitializeComponent();

            StatusBar = statusBar;
            StatusBar.PropertyChanged += StatusBarOnPropertyChanged;
        }

        internal StatusBar StatusBar { get; }

        private Bitmap RenderName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return ASCIIText.DrawText(10, string.Empty);

            name = name.Length <= 12 ? name : name.Substring(0, 12) + "...";

            return ASCIIText.DrawText(10, name);
        }

        private void StatusBarOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "Name":
                    textBitmap = RenderName(StatusBar.Name);
                    break;
            }

            Render();
        }

        private Window GetParentWindow(DependencyObject child)
        {
            var parent = VisualTreeHelper.GetParent(child);

            if (parent == null) return null;

            if (parent is Window parentWindow)
                return parentWindow;

            return GetParentWindow(parent);
        }

        internal void Render()
        {
            var statusBarBitmap = new Bitmap(friendBackgroundBitmap.Value.Width, friendBackgroundBitmap.Value.Height);

            using (var graphics = Graphics.FromImage(statusBarBitmap))
            {
                var backgroundBrush =
                    (SolidColorBrush) (Background ?? GetParentWindow(this)?.Background ?? Brushes.Black);
                var backgroundDrawingBrush = new SolidBrush(System.Drawing.Color.FromArgb(backgroundBrush.Color.R,
                    backgroundBrush.Color.G,
                    backgroundBrush.Color.B));

                graphics.FillRectangle(backgroundDrawingBrush, 0, 0, friendBackgroundBitmap.Value.Width,
                    friendBackgroundBitmap.Value.Height);
                var backgroundBitmap = StatusBar.Type == StatusBarType.Friend
                    ? friendBackgroundBitmap
                    : StatusBar.Type == StatusBarType.Pet
                        ? petBackgroundBitmap
                        : enemyBackgroundBitmap;

                graphics.DrawImage(backgroundBitmap.Value, 0, 0);
                graphics.DrawImage(emptyBarBitmap.Value, 34, 38);

                if (StatusBar.MaxHealth > 0)
                {
                    var currentHealth = StatusBar.CurrentHealth <= StatusBar.MaxHealth
                        ? StatusBar.CurrentHealth
                        : StatusBar.MaxHealth;

                    var fbp = StatusBar.IsPoisoned ? fullBarPoisonedBitmap.Value : fullBarBitmap.Value;
                    graphics.DrawImageUnscaledAndClipped(fbp,
                        new Rectangle(34, 38,
                            fbp.Width * currentHealth / StatusBar.MaxHealth,
                            fbp.Height));
                }

                if (textBitmap == null && !string.IsNullOrEmpty(StatusBar.Name))
                    textBitmap = RenderName((StatusBar.NamePrefix ?? string.Empty) + StatusBar.Name);
                if (textBitmap != null)
                    graphics.DrawImage(textBitmap, 17, 9);
                if (StatusBar.IsDead)
                    graphics.DrawImage(resurrectionBitmap.Value,
                        friendBackgroundBitmap.Value.Width - 5 - (int)(resurrectionBitmap.Value.Width * 0.75), 5,
                        (int)(resurrectionBitmap.Value.Width * 0.75), (int)(resurrectionBitmap.Value.Height * 0.75));
            }

            _image.Source = BitmapToImageSource(statusBarBitmap);
            _image.Stretch = Stretch.None;
            _image.Opacity = StatusBar.IsOutOfSight ? 0.5 : 1;
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private static Bitmap LoadGump(int gumpId, Hue hue)
        {
            var bitmap = Ultima.Gumps.GetGump(gumpId, hue, false, out bool _);
            bitmap.MakeTransparent(System.Drawing.Color.Black);

            return bitmap;
        }

        private static Bitmap LoadGump(int gumpId)
        {
            var bitmap = Ultima.Gumps.GetGump(gumpId);
            bitmap.MakeTransparent(System.Drawing.Color.Black);

            return bitmap;
        }

        public event EventHandler<StatusBar> Clicked;

        private void OnClick(object sender, RoutedEventArgs e)
        {
            Clicked?.Invoke(this, StatusBar);
        }
    }
}