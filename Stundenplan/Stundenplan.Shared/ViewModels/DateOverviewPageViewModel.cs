using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    public class DateOverviewPageViewModel : NotifyPropertyChangedObject
    {
        private Timetable timetable;
        private string statePast, stateToday, stateThisWeek, stateThisMonth, stateFuture;

        public ReadOnlyCollection<PublicGrouping<string, DateViewModel>> DatesGrouped { get; private set; }
        public bool IsDatesGroupedEmpty { get { return !DatesGrouped.Any(); } }

        public DateOverviewPageViewModel(Timetable timetable, string statePast, string stateToday, string stateThisWeek, string stateThisMonth, string stateFuture)
        {
            this.timetable = timetable;
            this.timetable.Dates.CollectionChanged += (sender, e) => { UpdateDatesGrouped(); };
            this.statePast = statePast;
            this.stateToday = stateToday;
            this.stateThisWeek = stateThisWeek;
            this.stateThisMonth = stateThisMonth;
            this.stateFuture = stateFuture;

            this.UpdateDatesGrouped();
        }

        private void UpdateDatesGrouped()
        {
            timetable.UpdateRepeatingDates();

            var list = timetable.Dates.OrderBy(p => p.Due).Select(p => new DateViewModel(p)).GroupBy(p => GetState(p)).Select(p => new Data.PublicGrouping<string, DateViewModel>(p)).ToList();
            this.DatesGrouped = new ReadOnlyCollection<PublicGrouping<string, DateViewModel>>(list);
            NotifyPropertyChanged("DatesGrouped");
            NotifyPropertyChanged("IsDatesGroupedEmpty");
        }

        private string GetState(DateViewModel date)
        {
            if (date.Due.Date < DateTime.Now.Date)
                return statePast;
            else if (date.Due.Date == DateTime.Now.Date)
                return stateToday;
            else if (date.Due.Date < DateTime.Now.AddDays(7).Date)
                return stateThisWeek;
            else if (date.Due.Date < DateTime.Now.AddMonths(1).Date)
                return stateThisMonth;
            else
                return stateFuture;
        }
    }
}
