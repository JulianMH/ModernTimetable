using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Stundenplan.Converters
{
    public class ColorCodeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var valueString = value as string;
            if (valueString == null || valueString == "")
                return App.Current.Resources["SystemColorControlAccentBrush"];
            else
                return new SolidColorBrush(
                Color.FromArgb(
                    255,
                    System.Convert.ToByte(valueString.Substring(0, 2), 16),
                    System.Convert.ToByte(valueString.Substring(2, 2), 16),
                    System.Convert.ToByte(valueString.Substring(4, 2), 16)
                )
            );
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
