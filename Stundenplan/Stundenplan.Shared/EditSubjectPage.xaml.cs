using Stundenplan.Localization;
using Stundenplan.Common;
using Stundenplan.Data;
using Stundenplan.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Stundenplan
{
    public sealed partial class EditSubjectPage : AppPage
    {
        public EditSubjectPage()
        {
            this.InitializeComponent();

            this.NavigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if(e.NavigationParameter is int)
            {
                var data = App.Timetable.Subjects[(int)e.NavigationParameter];
                this.DataContext = new EditSubjectPageViewModel(data, App.Timetable);
            }
            else
                await new MessageDialog(Strings.PageEditSubjectNotFound).ShowAsync();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var viewModel = ((EditSubjectPageViewModel)this.DataContext);
            if (!viewModel.IsDeleted)
            {
                var namingProblem = viewModel.TryApplyName(this.NameTextBox.Text,
                    App.Timetable.Subjects, new string[] { Subject.None.Name, Strings.PageEditLessonNewSubject });

                switch (namingProblem)
                {
                    case EditSubjectPageViewModel.NamingProblem.NameIsKeyword:
                        await new MessageDialog(Strings.PageEditSubjectNameNotAllowedWarning, Strings.MessageBoxWarningCaption).ShowAsync();
                        break;
                    case EditSubjectPageViewModel.NamingProblem.NameAlreadyUsed:
                        await new MessageDialog(Strings.PageEditSubjectNameAlreadyUsedWarning, Strings.MessageBoxWarningCaption).ShowAsync();
                        break;
                    case EditSubjectPageViewModel.NamingProblem.NameEmpty:
                        await new MessageDialog(Strings.PageEditSubjectNeedsNameWarning, Strings.MessageBoxWarningCaption).ShowAsync();
                        break;
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            //Um das Virtuelle Keyboard zu verstecken und die Eingabe abzuschließen,
            //nur wenn Enter gedrückt wurde
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ((TextBox)sender).IsEnabled = false;
                ((TextBox)sender).IsEnabled = true;
            }
        }
    }
}
