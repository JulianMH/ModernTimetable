using System;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Stundenplan.Converters
{
    public class SubjectThatUseColorListConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty IgnoreProperty =
             DependencyProperty.Register("Ignore", typeof(object),
             typeof(SubjectThatUseColorListConverter), new PropertyMetadata(null));

        public object Ignore
        {
            get { return GetValue(IgnoreProperty); }
            set { SetValue(IgnoreProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var color = (string)value;
            var subjects = App.Timetable.Subjects.Where(p => p.Color == color && p != Ignore).Select(p => p.Name).OrderBy(p => p);

            int maxAmount = 3;
            var stringBuilder = new StringBuilder();

            if(!subjects.Any())
                return "";

            var returnValue = string.Join("," + Environment.NewLine, subjects.Take(maxAmount));

            if (subjects.Count() > 3)
                returnValue += "," + Environment.NewLine + "...";

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
