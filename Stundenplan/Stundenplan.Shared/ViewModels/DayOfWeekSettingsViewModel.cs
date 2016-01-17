using System;
using System.Globalization;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    public class DayOfWeekSettingsViewModel : NotifyPropertyChangedObject
    {
        public DayOfWeek DayOfWeek { get; private set; }

        public bool IsEnabled { get; set; }
        public string Name { get { return DateTimeFormatInfo.CurrentInfo.GetDayName(this.DayOfWeek); } }

        public DayOfWeekSettingsViewModel(DayOfWeek dayOfWeek, bool isEnabled)
        {
            this.DayOfWeek = dayOfWeek;
            this.IsEnabled = isEnabled;
        }
    }
}
