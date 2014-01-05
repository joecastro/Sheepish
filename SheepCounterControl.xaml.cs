namespace Hbo.Sheepish
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public partial class SheepCounterControl
    {
        static SheepCounterControl()
        {
            UserControl.BackgroundProperty.OverrideMetadata(typeof(SheepCounterControl), new FrameworkPropertyMetadata(Brushes.Black));
        }

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count",
            typeof(int),
            typeof(SheepCounterControl),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public SheepCounterControl()
        {
            InitializeComponent();
        }
    }
}
