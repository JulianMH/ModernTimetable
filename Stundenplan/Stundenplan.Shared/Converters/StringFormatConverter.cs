using System;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Stundenplan.Data;
using Stundenplan.Localization;
using Windows.UI.Xaml;

namespace Stundenplan.Converters
{
    public class StringFormatConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty FormatProperty =
             DependencyProperty.Register("Format", typeof(string),
             typeof(StringFormatConverter), new PropertyMetadata(null));

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (this.Format != null)
                return String.Format(Format, value);
            else
                return String.Format((string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException(); // Wird nicht gebraucht
        }
    }
}
