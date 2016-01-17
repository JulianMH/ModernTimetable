using Stundenplan.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace Stundenplan.Commands
{
    public class EditLessonCommand : NavigateToCommand
    {
        public static readonly DependencyProperty DayProperty =
             DependencyProperty.Register("Day", typeof(DayViewModel),
             typeof(EditLessonCommand), new PropertyMetadata(null));

        public DayViewModel Day
        {
            get { return (DayViewModel)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }

        public override void Execute(object parameter)
        {
            if (this.Day != null)
            {
                var lesson = parameter as LessonViewModel;

                if (lesson == null) //Add new lesson if null
                    lesson = this.Day.AddLesson();

                int index = this.Day.GetLessonIndex(lesson);
                if (index != -1)
                {
                    this.NavigationFrame.Navigate(typeof(EditLessonPage), new Tuple<DayOfWeek, int>(this.Day.DayOfWeek, index));
                }
                else if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            }
            else if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
    }
}
