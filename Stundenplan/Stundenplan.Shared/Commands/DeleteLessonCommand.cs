using Stundenplan.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace Stundenplan.Commands
{
    public class DeleteLessonCommand : GoBackCommand, ICommand
    {
        public static readonly DependencyProperty DayProperty =
             DependencyProperty.Register("Day", typeof(DayViewModel),
             typeof(DeleteLessonCommand), new PropertyMetadata(null));

        public DayViewModel Day
        {
            get { return (DayViewModel)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }

        public bool GoBack { get; set; }

        public override void Execute(object parameter)
        {
            if (this.Day != null)
            {
                var lesson = parameter as LessonViewModel;

                if (parameter is EditLessonPageViewModel)
                    lesson = ((EditLessonPageViewModel)parameter).AsLessonViewModel();

                if (lesson != null)
                {
                    lesson.SetToNoSubject();
                    this.Day.CheckLessons();

                    if (this.GoBack)
                        NavigationFrame.GoBack();
                }
                else if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            }
            else if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
        public DeleteLessonCommand() { GoBack = false; }
    }
}
