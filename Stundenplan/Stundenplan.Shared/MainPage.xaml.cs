using Stundenplan.Localization;
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
    public sealed partial class MainPage : AppPage
    {
        bool statusBarVisible = true;

        public MainPage()
        {
            this.InitializeComponent();


            this.DataContext = new MainPageViewModel(App.Timetable);

            LessonViewModel.IsOddWeekChanged += LessonViewModel_IsOddWeekChanged;
            LessonViewModel_IsOddWeekChanged(null, null);
            
            Windows.Graphics.Display.DisplayInformation.GetForCurrentView().OrientationChanged += MainPage_OrientationChanged;
        }

        async void MainPage_OrientationChanged(Windows.Graphics.Display.DisplayInformation sender, object args)
        {
            MainHub.Visibility = Visibility.Collapsed;
            LandscapeWeekView.Visibility = Visibility.Collapsed;
            LandscapeProgressView.Visibility = Visibility.Collapsed;
            AppBar.Visibility = Visibility.Collapsed;
            var showStatusBar = false;

            if (Windows.Graphics.Display.DisplayInformation.GetForCurrentView().CurrentOrientation == Windows.Graphics.Display.DisplayOrientations.Landscape)
                LandscapeWeekView.Visibility = Visibility.Visible;
            else if (Windows.Graphics.Display.DisplayInformation.GetForCurrentView().CurrentOrientation == Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped)
                LandscapeProgressView.Visibility = Visibility.Visible;
            else
            {
                showStatusBar = true;
                MainHub.Visibility = Visibility.Visible;
                AppBar.Visibility = Visibility.Visible;
            }

            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();

            if (showStatusBar && !statusBarVisible)
            {
                await statusBar.ShowAsync();
                statusBarVisible = true;
            }
            else if (!showStatusBar && statusBarVisible)
            {
                await statusBar.HideAsync();
                statusBarVisible = false;
            }
        }

        void LessonViewModel_IsOddWeekChanged(object sender, EventArgs e)
        {
            EvenOddWeeksAppBarButton.Label = LessonViewModel.GetIsOddWeek() ?
                Strings.PageMainPageShowEvenWeeks :
                Strings.PageMainPageShowOddWeeks;
        }

        private void NotesIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(DateOverviewPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(!((MainPageViewModel)this.DataContext).IsSameTimetable(App.Timetable))
            {
                this.DataContext = new MainPageViewModel(App.Timetable);
            }

            base.OnNavigatedTo(e);
        }
    }
}
