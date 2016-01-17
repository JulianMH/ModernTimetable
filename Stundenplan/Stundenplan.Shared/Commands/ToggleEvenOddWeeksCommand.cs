using System;
using System.Windows.Input;
using Stundenplan.ViewModels;

namespace Stundenplan.Commands
{
    public class ToggleEvenOddWeeksCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public void Execute(object parameter)
        {
            LessonViewModel.SetIsOddWeek(!LessonViewModel.GetIsOddWeek());
        }
    }
}
