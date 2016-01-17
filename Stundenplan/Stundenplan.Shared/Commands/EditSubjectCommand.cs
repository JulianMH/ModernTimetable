using Stundenplan.Localization;
using Stundenplan.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace Stundenplan.Commands
{
    public class EditSubjectCommand : NavigateToCommand
    {
        public override void Execute(object parameter)
        {
            var subject = parameter as SubjectViewModel;

            if (subject == null)
                subject = new SubjectViewModel(App.Timetable.AddSubject(App.Timetable.GetSubjectName(Strings.SubjectNewFormat)));

            int index = subject.GetIndex(App.Timetable.Subjects);
            if (index != -1)
            {
                this.NavigationFrame.Navigate(typeof(EditSubjectPage), index);
            }
            else if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

        }
    }
}
