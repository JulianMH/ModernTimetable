using System;
using System.Windows;
using System.Windows.Input;
using Stundenplan.ViewModels;

namespace Stundenplan.Commands
{
    public class NavigateToHomeworkOverviewPageCommand : NavigateToCommand
    {
        public override void Execute(object parameter)
        {
            var lesson = parameter as LessonViewModel;

            if (lesson != null)
            {
                if (lesson.ThisWeekSubject.IsValidSubject)
                {
                    int index = lesson.ThisWeekSubject.GetIndex(App.Timetable.Subjects);
                    if (index != -1)
                    {
                        NavigationFrame.Navigate(typeof(HomeworkOverviewPage), index);
                    }
                    else if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();
                }
            }
            else
                NavigationFrame.Navigate(typeof(HomeworkOverviewPage));

        }
    }
}
