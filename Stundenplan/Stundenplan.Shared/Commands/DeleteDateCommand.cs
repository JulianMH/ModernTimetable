using Stundenplan.Localization;
using Stundenplan.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Popups;

namespace Stundenplan.Commands
{
    public class DeleteDateCommand : GoBackCommand, ICommand
    {
        public bool GoBack { get; set; }

        public async override void Execute(object parameter)
        {
            var date = parameter as DateViewModel;

            if (date != null)
            {
                var messageDialog = new MessageDialog(Strings.PageDateOverviewRemoveWarning, Strings.MessageBoxRemoveCaption);
                messageDialog.Commands.Add(new UICommand(Strings.MessageDialogOK, c =>
                {
                    App.Timetable.Dates.RemoveAt(date.GetIndex(App.Timetable.Dates));

                    if (this.GoBack)
                        NavigationFrame.GoBack();
                }));
                messageDialog.Commands.Add(new UICommand(Strings.MessageDialogCancel));
                await messageDialog.ShowAsync();
            }
            else if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        public DeleteDateCommand() { GoBack = false; }
    }
}
