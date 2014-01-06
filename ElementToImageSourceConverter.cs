namespace Hbo.Sheepish
{
    using Standard;
    using System;
    using System.Windows;
    using System.Windows.Data;

    class ElementToImageSourceConverter : IValueConverter
    {
        private static void _RoundSizeToInt(ref Size iconSize)
        {
            iconSize.Width = Math.Round(iconSize.Width);
            iconSize.Height = Math.Round(iconSize.Height);
        }

        #region IValueConverter Implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var element = (FrameworkElement)value;
            Size sourceSize = new Size(SystemParameters.IconWidth, SystemParameters.IconHeight);
            if (parameter != null && parameter.ToString().Equals("Small", StringComparison.InvariantCultureIgnoreCase))
            {
                sourceSize = new Size(SystemParameters.SmallIconWidth, SystemParameters.SmallIconHeight);
            }

            Size iconSize = DpiHelper.LogicalSizeToDevice(sourceSize);
            _RoundSizeToInt(ref iconSize);
            return Utility.GenerateBitmapSource(element, (int)iconSize.Width, (int)iconSize.Height, true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
