using System;
using System.Windows;
using System.Windows.Input;
using Stundenplan.ViewModels;

namespace Stundenplan.Commands
{
    public class HomeworkToggleDoneCommand : ICommand
    {
        public void Execute(object parameter)
        {
            var homework = parameter as HomeworkViewModel;

            if (homework != null)
            {
                homework.IsDone = !homework.IsDone;
            }
            else if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}
