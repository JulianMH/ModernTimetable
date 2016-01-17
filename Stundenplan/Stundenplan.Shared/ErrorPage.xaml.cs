using Stundenplan.Common;
using Stundenplan.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
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
    public sealed partial class ErrorPage : AppPage
    {
        internal static Exception Exception { get; set; }

        public ErrorPage()
        {
            this.InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;
        }

        void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState.Add("Exception", Exception);
        }

        void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Exception = (Exception)e.PageState["Exception"];
        }

        private async void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            if (Exception == null)
                await new MessageDialog(Strings.PageErrorPageDataLostWarning, Strings.MessageBoxCriticalError).ShowAsync();
            else
            {
                var appVersion = Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor;
                                
                var mail = new EmailMessage();

                mail.Subject = Strings.PageErrorPageReportSubject;
                mail.Body = string.Format(Strings.PageErrorPageReport, Strings.AppName, appVersion,
                    Exception.GetType().Name, Exception.Message, Exception.StackTrace, App.NavigationHistory);

                mail.To.Add(new EmailRecipient()
                {
                    Address = "support@philbi.de"
                });

                await EmailManager.ShowComposeNewEmailAsync(mail);
            }
            ButtonClose_Click(null, null);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
