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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

namespace Stundenplan
{
    public sealed partial class AboutPage : AppPage
    {
        public AboutPage()
        {
            this.InitializeComponent();

            var appVersion = Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor;
            VersionTextBlock.Text = string.Format(Strings.PageInfoPageVersion, appVersion);

            string language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (language == "pt")
            {
                this.TranslationTitleText.Visibility = Visibility.Visible;
                this.TranslationPersonText.Visibility = Visibility.Visible;
            }
        }

        private void MailButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMailForm();
        }

        public static async void ShowMailForm()
        {
            var appVersion = Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor;
            
            var mail = new EmailMessage();

            mail.Subject = string.Format(Strings.PageInfoPageMailSubject, Strings.AppName);
            mail.Body = string.Format(Strings.PageInfoPageMailBody, Strings.AppName, appVersion);

            mail.To.Add(new EmailRecipient()
            {
                Address = "support@philbi.de"
            });

            await EmailManager.ShowComposeNewEmailAsync(mail);
        }

        private async void WebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.philbi.de/"));
        }
    }
}
