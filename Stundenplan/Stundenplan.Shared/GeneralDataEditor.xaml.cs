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

namespace Stundenplan
{
    public sealed partial class GeneralDataEditor : AppPage
    {
        bool alreadyMovedToPage = false;

        public GeneralDataEditor()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
            this.NavigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.DataContext = new GeneralDataEditorPageViewModel(App.Timetable);

            if (alreadyMovedToPage == false)
            {
                alreadyMovedToPage = true;

                if (e.NavigationParameter as string == "Subjects")
                    this.DataPivot.SelectedItem = this.SubjectsPivotItem;
                else if (e.NavigationParameter as string == "LessonTimes")
                    this.DataPivot.SelectedItem = this.LessonTimesPivotItem;
            }
        }

        private void DataPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataPivot.SelectedItem == SubjectsPivotItem)
            {
                AddLessonTimeAppBarButton.Visibility = Visibility.Collapsed;
                CleanUpLessonTimesAppBarButton.Visibility = Visibility.Collapsed;
                AddSubjectAppBarButton.Visibility = Visibility.Visible;
            }
            else
            {
                AddLessonTimeAppBarButton.Visibility = Visibility.Visible;
                CleanUpLessonTimesAppBarButton.Visibility = Visibility.Visible;
                AddSubjectAppBarButton.Visibility = Visibility.Collapsed;
            }
        }

    }
}
