using System;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Stundenplan.Data;
using Stundenplan.Localization;

namespace Stundenplan.Converters
{
    /// <summary>
    /// Konvertiert RepeatBehaviour zu Text
    /// </summary>
    public class RepeatBehaviourTextConverter : IValueConverter
    {
        private static Dictionary<RepeatBehaviour, string> values = new Dictionary<RepeatBehaviour, string>()
        {
            {RepeatBehaviour.None, Strings.RepeatBehaviourNone},
            {RepeatBehaviour.Weekly, Strings.RepeatBehaviourWeekly},
            {RepeatBehaviour.Monthly, Strings.RepeatBehaviourMonthly}
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return values[(RepeatBehaviour)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return values.First(p => p.Value == (string)value).Key;
        }
    }
}
