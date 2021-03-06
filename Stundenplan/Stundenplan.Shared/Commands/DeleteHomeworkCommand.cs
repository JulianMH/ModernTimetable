﻿using Stundenplan.Localization;
using Stundenplan.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Popups;

namespace Stundenplan.Commands
{
    public class DeleteHomeworkCommand : GoBackCommand, ICommand
    {
        public bool GoBack { get; set; }

        public async override void Execute(object parameter)
        {
            var homework = parameter as HomeworkViewModel;

            if (homework != null)
            {
                var messageDialog = new MessageDialog(Strings.PageEditHomeworkRemoveWarning, Strings.MessageBoxRemoveCaption);
                messageDialog.Commands.Add(new UICommand(Strings.MessageDialogOK, c =>
                {
                    App.Timetable.Homeworks.RemoveAt(homework.GetIndex(App.Timetable.Homeworks));

                    if (this.GoBack)
                        NavigationFrame.GoBack();
                }));
                messageDialog.Commands.Add(new UICommand(Strings.MessageDialogCancel));
                await messageDialog.ShowAsync();
            }
            else if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        public DeleteHomeworkCommand() { GoBack = false; }
    }
}
