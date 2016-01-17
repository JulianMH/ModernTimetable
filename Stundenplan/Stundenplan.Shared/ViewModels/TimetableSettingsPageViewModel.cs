using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    public class TimetableSettingsPageViewModel
    {
        private Timetable timetable;

        public string Name { get; private set; }
        public ReadOnlyCollection<DayOfWeekSettingsViewModel> DaysOfWeek { get; private set; }

        public TimetableSettingsPageViewModel(Timetable timetable)
        {
            this.timetable = timetable;

            this.Name = timetable.Name;
            this.DaysOfWeek = new ReadOnlyCollection<DayOfWeekSettingsViewModel>(new DayOfWeek[]
            { 
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            }.Select(p => new DayOfWeekSettingsViewModel(p, timetable.Days.Any(q => q.DayOfWeek == p))).ToList());
        }

    }
}
