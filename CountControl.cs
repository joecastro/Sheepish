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
        private readonly ElementToImageSourceConverter _elementToImageSourceConverter = new ElementToImageSourceConverter();
        private readonly FrameworkElement _rootElement;
        private readonly TextBlock _countBlock;

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count",
            typeof(int),
            typeof(CountControl),
            new UIPropertyMetadata(0,
                (d, e) => ((CountControl)d)._UpdateImageSource()));

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

        public CountControl()
        {
            _countBlock = new TextBlock
            {
                Foreground = Brushes.White,
                FontSize = 12,
                Text = Count.ToString(),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontFamily = new FontFamily("Segoe UI")
            };

            _rootElement = new Grid
            {
                Children =
                {
                    new Border
                    {
                        Background = (Brush)FindResource("SheepSilhouetteBrush")
                    },
                    new Viewbox
                    {
                        Margin = new Thickness(0,2,0,2),
                        Child = _countBlock,
                    },
                }
            };
            _UpdateImageSource();
        }

        private void _UpdateImageSource()
        {
            Dispatcher.BeginInvoke((Action)(() => {
                _countBlock.Text = Count.ToString();
                ImageSource = (ImageSource)_elementToImageSourceConverter.Convert(_rootElement, typeof(ImageSource), null, null);
            }));
        }
    }
}
