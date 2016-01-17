using Stundenplan.Localization;
using Stundenplan.ViewModels;
using Stundenplan.ViewModels.Design;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
    public sealed partial class LandscapeProgressView : UserControl
    {
        private Timer updateTimer;

        public LandscapeProgressView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            updateTimer = new Timer(new TimerCallback(TimerCallbackMethod), null, 0, 500);
        }

        async void TimerCallbackMethod(object o)
        {
            try
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Refresh);
            }
            catch { }
        }

        delegate void RefreshDelegate();
        private void Refresh()
        {
            if (this.IsEnabled && this.Visibility
                == Visibility.Visible)
            {
                this.DataContext = new ProgressViewModel(App.Timetable, Strings.PageLandscapeViewNoLesson, Strings.PageLandscapeViewFreeTime,
                    Strings.PageLandscapeViewRemainingTimeFormat);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            updateTimer.Dispose();
            updateTimer = null;
        }
    }
}
