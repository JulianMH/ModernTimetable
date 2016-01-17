using System;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Stundenplan.Data;
using Stundenplan.Localization;
using Stundenplan.ViewModels;

namespace Stundenplan.Converters
{
    public class OrderByToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch((HomeworkOverviewPageViewModel.OrderBy)value)
            {
                case HomeworkOverviewPageViewModel.OrderBy.From:
                    return Strings.PageHomeworkOverviewByFromDate;
                case HomeworkOverviewPageViewModel.OrderBy.To:
                    return Strings.PageHomeworkOverviewByToDate;
                case HomeworkOverviewPageViewModel.OrderBy.Subject:
                    return Strings.PageHomeworkOverviewBySubject;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((string)value == Strings.PageHomeworkOverviewByFromDate)
                return HomeworkOverviewPageViewModel.OrderBy.From;
            if ((string)value == Strings.PageHomeworkOverviewByToDate)
                return HomeworkOverviewPageViewModel.OrderBy.To;
            if ((string)value == Strings.PageHomeworkOverviewBySubject)
                return HomeworkOverviewPageViewModel.OrderBy.Subject;

            return null;
        }
    }
}
