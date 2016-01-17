using System;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Stundenplan.Data;
using Stundenplan.Localization;

namespace Stundenplan.Converters
{
    public sealed class DictionaryStringConverter : IValueConverter
    {
        private Dictionary<object, string> dictionary;

        internal DictionaryStringConverter(Dictionary<object, string> dictionary)
        {
            this.dictionary = dictionary;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string text;
            if (dictionary.TryGetValue(value, out text))
                return text;
            else
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string text = (string)value;

            if (!dictionary.Any(p => p.Value == text))
                throw new InvalidOperationException("Object (" + value + ") not in Dictionary: " + dictionary.ToString());
            else
                return dictionary.FirstOrDefault(p => p.Value == text).Key;
        }
    }
}
