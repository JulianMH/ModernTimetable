using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    public class DayViewModel : Abstract.DataViewModel<Day>
    {
        private Timetable timetable;

        public DayOfWeek DayOfWeek { get { return data.DayOfWeek; } }
        public string LocalizedName
        {
            get
            {
                return System.Globalization.DateTimeFormatInfo.CurrentInfo.GetDayName(this.DayOfWeek);
            }
        }

        public bool IsEventToday
        {
            get { return timetable.Dates.Any(p => p.Due.DayOfWeek == this.DayOfWeek && new DateViewModel(p).IsFollowingWeek); }
        }

        public bool IsToday { get { return this.DayOfWeek == DateTime.Today.DayOfWeek; } }

        public object DataContext { get { return this; } }

        public ReadOnlyCollection<LessonViewModel> Lessons { get; private set; }

        public DayViewModel(Timetable timetable, Day day)
            : base(day)
        {
            this.timetable = timetable;

            this.data.Lessons.CollectionChanged += Lessons_CollectionChanged;
            this.Lessons = new ReadOnlyCollection<LessonViewModel>(day.Lessons.Select(p => new LessonViewModel(p)).ToList());
        }

        void Lessons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Lessons = new ReadOnlyCollection<LessonViewModel>(this.data.Lessons.Select(p => new LessonViewModel(p)).ToList());
            NotifyPropertyChanged("Lessons");
        }

        internal void UpdateAllDesignData(IList<Subject> subjectsWithHomework)
        {
            NotifyPropertyChanged("IsEventToday");
            foreach (var lesson in this.Lessons)
                lesson.UpdateAllDesignData(subjectsWithHomework);
        }

        public int GetLessonIndex(LessonViewModel lesson)
        {
            return lesson.GetIndex(this.data.Lessons);
        }

        public LessonViewModel AddLesson()
        {
            return new LessonViewModel(timetable.AddLesson(this.data));
        }

        public void CheckLessons()
        {
            this.data.CheckLessons();
        }
    }
}
