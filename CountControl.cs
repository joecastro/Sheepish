namespace Hbo.Sheepish
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Standard;
    using System;

    public class CountControl : Control
    {
        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count",
            typeof(int),
            typeof(CountControl),
            new UIPropertyMetadata(0,
                (d, e) => ((CountControl)d)._OnDisplayCountChanged()));

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        private static readonly DependencyPropertyKey ImageSourcePropertyKey = DependencyProperty.RegisterReadOnly(
            "ImageSource",
            typeof(ImageSource),
            typeof(CountControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ImageSourceProperty = ImageSourcePropertyKey.DependencyProperty;

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            private set { SetValue(ImageSourcePropertyKey, value); }
        }

        private void _OnDisplayCountChanged()
        {
            _UpdateImageSource();
        }

        private static void _RoundSizeToInt(ref Size iconSize)
        {
            iconSize.Width = Math.Round(iconSize.Width);
            iconSize.Height = Math.Round(iconSize.Height);
        }

        private void _UpdateImageSource()
        {
            Size iconSize = DpiHelper.LogicalSizeToDevice(new Size(SystemParameters.IconWidth, SystemParameters.IconHeight));
            _RoundSizeToInt(ref iconSize);
            var element = new Grid
            {
                Width = iconSize.Width,
                Height = iconSize.Height,
                Children =
                {
                    new Border
                    {
                        Background = (Brush)FindResource("SheepSilhouetteBrush")
                    },
                    new Viewbox
                    {
                        Margin = new Thickness(0,2,0,2),
                        Child = new TextBlock
                        {
                            Foreground = Brushes.White,
                            FontSize = 12,
                            Text = Count.ToString(),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            FontFamily = new FontFamily("Segoe UI")
                        },
                    },
                },
            };

            ImageSource = Utility.GenerateBitmapSource(element, (int)element.Width, (int)element.Height, true);
        }
    }
}
