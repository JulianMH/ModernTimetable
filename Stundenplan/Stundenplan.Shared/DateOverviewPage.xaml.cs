using Stundenplan.Common;
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
    public sealed partial class DateOverviewPage : AppPage
    {
        public DateOverviewPage()
        {
            this.InitializeComponent();

            this.NavigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.DataContext = new DateOverviewPageViewModel(App.Timetable,
                Strings.DateTimePast,
                Strings.DateTimeToday,
                Strings.DateTimeThisWeek,
                Strings.DateTimeThisMonth,
                Strings.DateTimeFuture);
        }
    }
}
