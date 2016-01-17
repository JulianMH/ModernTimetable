using Stundenplan.Data;
using Stundenplan.Localization;
using Stundenplan.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using System.Threading.Tasks;
using Stundenplan.LiveTile;

namespace Stundenplan
{
    public sealed partial class TimetableSettingsPage : AppPage
    {
        public TimetableSettingsPage()
        {
            this.InitializeComponent();
            

            this.NavigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        private void NavigationHelper_LoadState(object sender, Stundenplan.Common.LoadStateEventArgs e)
        {
            this.DataContext = new TimetableSettingsPageViewModel(App.Timetable);
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
        
        private async void MailAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            var homeworks = App.Timetable.Homeworks.Where(p => p.IsDone == false).OrderBy(p => p.ToDate);
            if (!homeworks.Any())
                builder.AppendLine(Strings.TimetableReportNone);
            else
            {
                builder.AppendLine();
                foreach (Homework homework in homeworks)
                {
                    builder.AppendLine(String.Format(Strings.TimetableReportHomework, homework.Subject.Name, homework.FromDate.ToString("D"), homework.ToDate.ToString("D"), homework.Text));
                }

            }

            string homeworkString = builder.ToString();
            builder.Clear();

            var dates = App.Timetable.Dates.Where(p => p.Due > DateTime.Now).OrderBy(p => p.Due);
            if (!dates.Any())
                builder.AppendLine(Strings.TimetableReportNone);
            else
            {
                builder.AppendLine();
                foreach (Date date in dates)
                {
                    builder.AppendLine(String.Format(Strings.TimetableReportDate, date.Due.ToString("D"), date.Name));
                }

            }

            string dateString = builder.ToString();
            
            var mail = new EmailMessage();
            mail.Subject = string.Format(Strings.TimetableReportBody, App.Timetable.Name, homeworkString, dateString);
            mail.Body = string.Format(Strings.TimetableReportSubject, DateTime.Now.ToString("D"));

            await EmailManager.ShowComposeNewEmailAsync(mail);
        } 

        private async void DoneAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (TimetableSettingsPageViewModel)this.DataContext;

            App.Timetable.Name = this.NameTextBox.Text;

            if (viewModel.DaysOfWeek.Any(p => p.IsEnabled))
            {
                List<Day> oldDays = new List<Day>();
                foreach (Day day in App.Timetable.Days)
                    oldDays.Add(day);

                App.Timetable.Days.Clear();

                foreach (DayOfWeekSettingsViewModel helperDay in viewModel.DaysOfWeek.Where(p => p.IsEnabled))
                {
                    Day oldDay = oldDays.FirstOrDefault(p => p.DayOfWeek == helperDay.DayOfWeek);
                    if (oldDay == null)
                        oldDay = new Day(helperDay.DayOfWeek);

                    App.Timetable.Days.Add(oldDay);
                }
            }
            else
                await new MessageDialog(Strings.PageTimetableSettingsWeekDaysWarning, Strings.MessageBoxWarningCaption).ShowAsync();

            this.Frame.GoBack();
        }

        private async void SwitchEvenOddWeeksButton_Click(object sender, RoutedEventArgs e)
        {
            App.Timetable.SwitchEvenOddWeeks();
            await new MessageDialog(Strings.PageTimetableSettingsSwitchEvenOddWeeksWarning).ShowAsync();
        }

        private async void PinAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await TimetableIO.SaveTimetable(App.Timetable);

            string taskName = "Stundenplan.LiveTile";
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if( backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity )
            {
                foreach( var task in BackgroundTaskRegistration.AllTasks )
                {
                    task.Value.Unregister(true);
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = "Stundenplan.LiveTile.AppTileUpdater";
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                var registration = taskBuilder.Register();
                
                SecondaryTile tileData = new SecondaryTile()
                {
                    TileId = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(App.Timetable.FileName)),
                    DisplayName = App.Timetable.Name,
                    Arguments = App.Timetable.FileName
                };
                tileData.VisualElements.ShowNameOnSquare150x150Logo = true;
                tileData.VisualElements.ShowNameOnWide310x150Logo = true;

                var tileControl = await AppTileUpdater.ReadTileControl();
                LayoutRoot.Children.Add(tileControl);
                await AppTileUpdater.UpdateTileImage(tileControl, tileData);
                LayoutRoot.Children.Remove(tileControl);

                await tileData.RequestCreateAsync();
            }
        }
    }
}
