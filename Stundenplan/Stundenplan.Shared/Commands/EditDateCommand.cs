using System;
using System.Windows;
using System.Windows.Input;
using Stundenplan.Data;
using Stundenplan.ViewModels;
using Stundenplan.Localization;

namespace Stundenplan.Commands
{
    public class EditDateCommand : NavigateToCommand
    {
        public override void Execute(object parameter)
        {
            var date = parameter as DateViewModel;

            if (date == null) //Add new Date if null
            {
                Date dateModel = new Date(Strings.DateNew, DateTime.Now.AddDays(1), RepeatBehaviour.None);

                App.Timetable.Dates.Add(dateModel);

                date = new DateViewModel(dateModel);
            }

            NavigationFrame.Navigate(typeof(EditDatePage), date.GetIndex(App.Timetable.Dates));
        }
    }
}
