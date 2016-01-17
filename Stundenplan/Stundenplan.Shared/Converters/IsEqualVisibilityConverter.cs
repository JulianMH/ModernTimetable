using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Stundenplan.Converters
{
    public class IsEqualVisibilityConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty CompareToProperty =
             DependencyProperty.Register("CompareTo", typeof(object),
             typeof(IsEqualVisibilityConverter), new PropertyMetadata(null));

        public object CompareTo
        {
            get { return GetValue(CompareToProperty); }
            set { SetValue(CompareToProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString() == CompareTo.ToString() ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
