using Stundenplan.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace Stundenplan.Commands
{
    public class EditLessonTimeCommand : NavigateToCommand
    {
        public override void Execute(object parameter)
        {
            var lessonTime = parameter as LessonTimeViewModel;

            if (lessonTime == null) //Add new lLessonTime if null
                lessonTime = new LessonTimeViewModel(App.Timetable.AddLessonTime());

            NavigationFrame.Navigate(typeof(EditLessonTimePage), lessonTime.Number);
        }
    }
}
