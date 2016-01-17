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
    public sealed partial class EditDatePage : AppPage
    {
        public EditDatePage()
        {
            this.InitializeComponent();
            
            this.NavigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is int)
            {
                this.DataContext = new EditDatePageViewModel(App.Timetable.Dates[(int)e.NavigationParameter]);
            }
            else
                await new MessageDialog(Strings.PageEditDateNotFound, Strings.MessageBoxCriticalError).ShowAsync();
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
