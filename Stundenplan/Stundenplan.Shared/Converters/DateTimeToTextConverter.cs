using System;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Stundenplan.Data;
using Stundenplan.Localization;

namespace Stundenplan.Converters
{
    /// <summary>
    /// Konvertiert DateTime zu Text
    /// </summary>
    public class DateTimeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string formatString = "d";
            if (parameter is string)
                formatString = (string)parameter;
            return string.Format("{0:" + formatString + "}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DateTime.Parse((string)value);
        }
    }
}
