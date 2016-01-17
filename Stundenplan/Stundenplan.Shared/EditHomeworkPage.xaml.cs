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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Stundenplan
{
    public sealed partial class EditHomeworkPage : AppPage
    {
        public EditHomeworkPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
            this.NavigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (this.DataContext == null)
            {
                if (e.NavigationParameter is int)
                {
                    this.DataContext = new EditHomeworkPageViewModel(App.Timetable.Homeworks[(int)e.NavigationParameter], App.Timetable);
                }
                else
                    await new MessageDialog(Strings.PageEditHomeworkNotFoundWarning).ShowAsync();
            }
        }
    }
}
