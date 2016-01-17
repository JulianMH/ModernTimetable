using Stundenplan.Data;
using Stundenplan.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Stundenplan
{
    public sealed partial class SelectTimetablePage : AppPage
    {
        public SelectTimetablePage()
        {
            this.InitializeComponent();

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;

            this.DataContext = new SelectTimetablePageViewModel(() => Frame.GoBack(), () => Frame.Navigate(typeof(TimetableSettingsPage)));
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ((SelectTimetablePageViewModel)this.DataContext).SelectCommand.Execute(e.ClickedItem);
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ((SelectTimetablePageViewModel)this.DataContext).AddCommand.Execute(null);
        }

        private void ImportAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".xml");
            picker.PickSingleFileAndContinue();
        }

        private async void ExportMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var timetable = (TimetableDescription)((FrameworkElement)sender).DataContext;
            if (timetable.FileName == App.Timetable.FileName)
                await TimetableIO.SaveTimetable(App.Timetable);

            var picker = new FileSavePicker();

            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            picker.SuggestedFileName = System.Text.RegularExpressions.Regex.Replace(timetable.Name, invalidRegStr, "_");
            picker.FileTypeChoices.Add("XML", new List<string>() { ".xml" });

            picker.ContinuationData.Add("FileName", timetable.FileName);
            picker.PickSaveFileAndContinue();
        }

    }
}
