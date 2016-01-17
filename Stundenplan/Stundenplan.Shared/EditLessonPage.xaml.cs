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
    public sealed partial class EditLessonPage : AppPage
    {
        public EditLessonPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
            this.NavigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if(this.DataContext == null)
            { 
            if (e.NavigationParameter is Tuple<DayOfWeek, int>)
            {
                var tuple = (Tuple<DayOfWeek, int>)e.NavigationParameter;
                var day = App.Timetable.GetDay(tuple.Item1);

                //Lesson hinzufügen wenn notwendig
                while (tuple.Item2 >= day.Lessons.Count)
                    App.Timetable.AddLesson(day);
                var lesson = day.Lessons[tuple.Item2];

                if (lesson.LessonTime == null)
                {
                    await new MessageDialog(Strings.PageEditLessonNewLessonTimeWarning, Strings.MessageBoxWarningCaption).ShowAsync();
                    lesson.LessonTime = App.Timetable.AddLessonTime();
                    this.Frame.Navigate(typeof(EditLessonTimePage), App.Timetable.LessonTimes.IndexOf(lesson.LessonTime));
                }
                if (lesson.LessonTime == null)
                {
                    day.Lessons.Remove(lesson);
                    this.Frame.GoBack();
                }

                PageTitle.Text = string.Format(Strings.PageEditLessonHeader,
                    System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(day.DayOfWeek),
                     String.Format(Strings.LessonTimeNumberTextFormat, lesson.LessonTime.Number + 1));

                this.DataContext = new EditLessonPageViewModel(App.Timetable, day, lesson, Strings.PageEditLessonNewSubject);

            }
            else
                await new MessageDialog(Strings.PageEditLessonNotFoundWarning).ShowAsync();
            }
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


        private void SubjectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var subject = ((ComboBox)sender).SelectedItem as SubjectViewModel;
            var viewModel = this.DataContext as EditLessonPageViewModel;

            if (subject != null && viewModel != null && subject == ((EditLessonPageViewModel)this.DataContext).NewSubject)
            {
                subject.AddToTimetable(App.Timetable, App.Timetable.GetSubjectName(Strings.SubjectNewFormat));

                viewModel.SetupSubjects();
                this.Frame.Navigate(typeof(EditSubjectPage), subject.GetIndex(App.Timetable.Subjects));
            }
        }
    }
}
