using System;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Stundenplan.Data;
using Stundenplan.Localization;

namespace Stundenplan.Converters
{
    /// <summary>
    /// Konvertiert Boolean zu Text.
    /// </summary>
    public class BooleanToDoneConverter : IValueConverter
    {
        /// <summary>
        /// Text der Zurückgeliefert wird wenn
        /// </summary>
        public string TrueString { get; set; }
        public string FalseString { get; set; }

        public BooleanToDoneConverter()
        {
            this.TrueString = Strings.BooleanNotDone;
            this.FalseString = Strings.BooleanDone;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
                return this.TrueString;
            else
                return this.FalseString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((value as string) == this.TrueString)
                return true;
            else
                return false;
        }
    }
}
