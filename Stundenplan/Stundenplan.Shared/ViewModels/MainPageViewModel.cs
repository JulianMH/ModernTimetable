using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    /// <summary>
    /// ViewModel für die Stundenplan Main Page.
    /// </summary>
    public class MainPageViewModel : NotifyPropertyChangedObject
    {
        private Timetable timetable;

        public String TimetableName { get { return timetable.Name; } }

        public DayViewModel DefaultDay
        {
            get
            {
                DayViewModel day = (DayViewModel)this.Days.FirstOrDefault(p => ((DayViewModel)p).DayOfWeek == DateTime.Now.DayOfWeek);

                if (day != null)
                    return day;
                else
                    return (DayViewModel)this.Days.FirstOrDefault();
            }
        }

        public ReadOnlyCollection<object> Days { get; private set; }
        public ReadOnlyCollection<LessonTimeViewModel> LessonTimes { get; private set; }

        public int WeekNumber
        {
            get
            {
                int weekNumber = ConvertHelpers.GetWeekNumber(DateTime.Now);
                if (LessonViewModel.GetIsOddWeek() != ((weekNumber % 2) == 1))
                    weekNumber++;
                return weekNumber;
            }
        }

        public MainPageViewModel(Timetable timetable)
        {
            this.timetable = timetable;

            timetable.PropertyChanged += (sender, e) => { if (e.PropertyName == "Name") NotifyPropertyChanged("TimetableName"); };

            this.Days = new ReadOnlyCollection<object>(this.timetable.Days.Select(p => (object)new DayViewModel(this.timetable, p)).ToList());
            this.LessonTimes = new ReadOnlyCollection<LessonTimeViewModel>(this.timetable.LessonTimes.Select(p => new LessonTimeViewModel(p)).ToList());

            this.timetable.Days.CollectionChanged += Days_CollectionChanged;

            LessonViewModel.IsOddWeekChanged += (s, e) => NotifyPropertyChanged("WeekNumber");
        }

        void Days_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Days = new ReadOnlyCollection<object>(this.timetable.Days.Select(p => (object)new DayViewModel(this.timetable, p)).ToList());
            NotifyPropertyChanged("Days");
            NotifyPropertyChanged("DefaultDay");
        }

        public bool IsSameTimetable(Timetable timetable)
        {
            return timetable == this.timetable;
        }

        public void UpdateData()
        {
            var subjectsWithHomework = timetable.Homeworks.Where(p => p.IsDone == false).Select(p => p.Subject).Distinct().ToList();
            foreach (var day in this.Days)
                ((DayViewModel)day).UpdateAllDesignData(subjectsWithHomework);

            NotifyPropertyChanged("WeekNumber");
        }
    }
}
