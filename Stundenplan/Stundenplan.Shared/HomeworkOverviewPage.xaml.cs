using Stundenplan.Localization;
using Stundenplan.Common;
using Stundenplan.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Stundenplan.Data;

namespace Stundenplan
{
    public sealed partial class HomeworkOverviewPage : AppPage
    {
        private bool isLoaded = false;

        public HomeworkOverviewPage()
        {
            this.InitializeComponent();
            this.NavigationHelper.LoadState += this.NavigationHelper_LoadState;

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;

            NavigationHelper.GoBackCommand = new RelayCommand(
                        () =>
                        {
                            if (SortPopup.Visibility == Windows.UI.Xaml.Visibility.Visible)
                            {
                                SortPopup.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                                AppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                            }
                            else
                                NavigationHelper.GoBack();
                        },
                        () => SortPopup.Visibility == Windows.UI.Xaml.Visibility.Visible || NavigationHelper.CanGoBack());            
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (!isLoaded)
            {
                this.DataContext = new HomeworkOverviewPageViewModel(App.Timetable, Strings.PageHomeworkOverviewAll);
                isLoaded = true;
                if (e.NavigationParameter is int)
                {
                    var data = App.Timetable.Subjects[(int)e.NavigationParameter];
                    var viewModel = ((HomeworkOverviewPageViewModel)this.DataContext);
                    viewModel.HomeworkFilter = new SubjectViewModel(data);

                    this.DataPivot.SelectedItem = FilteredPivotItem;
                }

                SortPopup.Visibility = Visibility.Collapsed;
            }
        }

        private void SortFullModeSelector_ItemClick(object sender, ItemClickEventArgs e)
        {
            ((HomeworkOverviewPageViewModel)this.DataContext).HomeworkOrder = (HomeworkOverviewPageViewModel.OrderBy)e.ClickedItem;
            SortPopup.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private async void AppBarButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (App.Timetable.Subjects.Any())
            {
                var viewModel = ((HomeworkOverviewPageViewModel)this.DataContext);

                var subject = App.Timetable.Subjects.First();

                if (this.DataPivot.SelectedItem == FilteredPivotItem && viewModel.HomeworkFilter != viewModel.NoFilterSubject)
                    subject = App.Timetable.Subjects[viewModel.HomeworkFilter.GetIndex(App.Timetable.Subjects)];

                var homework = new Homework(subject);
                homework.FromDate = DateTime.Today;
                homework.ToDate = App.Timetable.GetNextDate(subject, LessonViewModel.GetIsOddWeek());
                App.Timetable.Homeworks.Add(homework);

                Frame.Navigate(typeof(EditHomeworkPage), App.Timetable.Homeworks.IndexOf(homework));
            }
            else
            {
                await new MessageDialog(Strings.PageHomeworkOverviewNoSubjectWarning, Strings.MessageBoxWarningCaption).ShowAsync();
                Frame.Navigate(typeof(GeneralDataEditor));
            }
        }

        private void AppBarButtonSort_Click(object sender, RoutedEventArgs e)
        {
            SortPopup.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
