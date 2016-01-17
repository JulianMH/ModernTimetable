using Stundenplan.Localization;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Popups;

namespace Stundenplan.Commands
{
    public class CleanUpLessonTimesCommand : ICommand
    {
        public async void Execute(object parameter)
        {
            int difference = App.Timetable.LessonTimes.Count -
                App.Timetable.Days.Select(p => p.Lessons.Count).Max();

            if (difference > 0)
            {
                string message = String.Format(Strings.PageGeneralDataEditorCleanUpLessonTimesWarningPlural, difference);
                if (difference == 1)
                    message = Strings.PageGeneralDataEditorCleanUpLessonTimesWarningSingular;

                var messageDialog = new MessageDialog(message, Strings.MessageBoxRemoveCaption);
                messageDialog.Commands.Add(new UICommand(Strings.MessageDialogOK, c =>
                {
                    for (int i = 0; i < difference; i++)
                        App.Timetable.LessonTimes.RemoveAt(App.Timetable.LessonTimes.Count - 1);
                }));
                messageDialog.Commands.Add(new UICommand(Strings.MessageDialogCancel));
                await messageDialog.ShowAsync();
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}
