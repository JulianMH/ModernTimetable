using Stundenplan.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace Stundenplan.Commands
{
    public class EditHomeworkCommand : NavigateToCommand
    {
        public override void Execute(object parameter)
        {
            var homework = parameter as HomeworkViewModel;

            if (homework != null)
            {
                int index = homework.GetIndex(App.Timetable.Homeworks);
                if (index != -1)
                {
                    NavigationFrame.Navigate(typeof(EditHomeworkPage), index);
                }
                else if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            }
            else if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
    }
}
