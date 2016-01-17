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
    public sealed partial class EditLessonTimePage : AppPage
    {
        public EditLessonTimePage()
        {
            this.InitializeComponent();

            this.NavigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is int)
            {
                this.DataContext = new EditLessonTimePageViewModel(App.Timetable.LessonTimes[(int)e.NavigationParameter], Strings.LessonTimeNumberTextFormat);
            }
            else
                await new MessageDialog(Strings.PageEditLessonTimeNotFoundWarning).ShowAsync();
        }

    }
}
