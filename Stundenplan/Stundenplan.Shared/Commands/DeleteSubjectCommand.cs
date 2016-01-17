using Stundenplan.Localization;
using Stundenplan.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Stundenplan.Commands
{
    public class DeleteSubjectCommand : GoBackCommand, ICommand
    {
        public bool GoBack { get; set; }

        public async override void Execute(object parameter)
        {
            var subject = parameter as SubjectViewModel;

            if (subject != null)
            {
                int lessonCount = subject.GetLessonCount(App.Timetable);

                if (lessonCount > 0)
                {
                    string numberText;
                    if (lessonCount > 1)
                        numberText = string.Format(Strings.TimetableRemoveSubjectWarningPluralFormat, subject.Name, lessonCount);
                    else
                        numberText = string.Format(Strings.TimetableRemoveSubjectWarningSingularFormat, subject.Name);

                    var messageDialog = new MessageDialog(numberText, Strings.MessageBoxRemoveCaption);
                    messageDialog.Commands.Add(new UICommand(Strings.MessageDialogOK, c =>
                    {
                        subject.RemoveSubject(App.Timetable);

                        if (this.GoBack)
                            NavigationFrame.GoBack();
                    }));
                    messageDialog.Commands.Add(new UICommand(Strings.MessageDialogCancel));
                    await messageDialog.ShowAsync();
                }
                else
                    subject.RemoveSubject(App.Timetable);
            }
            else if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        public DeleteSubjectCommand() { GoBack = false; }
    }
}
