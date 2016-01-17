using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    /// <summary>
    /// ViewModel für die Verlaufsanzeige
    /// </summary>
    public class ProgressViewModel : NotifyPropertyChangedObject
    {
        private Timetable timetable;

        public LessonViewModel CurrentLesson { get; private set; }
        public LessonViewModel NextLesson { get; private set; }

        public double CurrentLessonProgress
        {
            get
            {
                if (CurrentLesson != null && CurrentLesson.LessonTime != null)
                    return (DateTime.Now.TimeOfDay - this.CurrentLesson.LessonTime.Start.TimeOfDay).TotalSeconds /
                        (this.CurrentLesson.LessonTime.End - this.CurrentLesson.LessonTime.Start).TotalSeconds;
                else
                    return 0;
            }
        }

        public string ProgressString
        {
            get
            {
                if (CurrentLesson != null && CurrentLesson.LessonTime != null)
                {
                    TimeSpan difference = CurrentLesson.LessonTime.End - DateTime.Now;

                    return String.Format(progressStringFormat,
                        (int)difference.TotalHours, difference.Minutes, difference.Seconds);
                }
                else return "";
            }
        }

        private LessonViewModel notEnoughData;
        private LessonViewModel empty;
        private string freeTimeString;
        private string progressStringFormat;

        public ProgressViewModel(Timetable timetable, string notEnoughDataString, string freeTimeString, string progressStringFormat)
        {
            this.timetable = timetable;

            this.notEnoughData = new LessonViewModel(new Lesson(null, new SubjectRoom(new Subject(notEnoughDataString), "")));
            this.empty = new LessonViewModel(new Lesson(null, new SubjectRoom(new Subject(""), "")));
            this.freeTimeString = freeTimeString;
            this.progressStringFormat = progressStringFormat;
            this.UpdateData();
        }

        private void UpdateData()
        {
            DateTime now = DateTime.Now;

            var listedLessons = timetable.GetListedLessons(now, DateTime.Now.AddDays(7), LessonViewModel.GetIsOddWeek());

            var currentLesson = GetNextLesson(listedLessons, now);

            if (currentLesson == null) //In diesem Falle ists einfach: Nicht genügend Daten vorhanden
            {
                this.CurrentLesson = notEnoughData;
                this.NextLesson = empty;
            }
            else if (currentLesson.LessonTime.Start > now) //Aktuell: Freizeit. Nächstes: Untericht.
            {
                this.NextLesson = new LessonViewModel(currentLesson);
                this.CurrentLesson = new LessonViewModel(new Lesson(new LessonTime(0, now, this.NextLesson.LessonTime.Start), new SubjectRoom(new Subject(freeTimeString), "")));
            }
            else //Aktuell: Unterricht. Nächstes: Untericht.
            {
                this.CurrentLesson = new LessonViewModel(currentLesson);
                var nextLesson = GetNextLesson(listedLessons, currentLesson.LessonTime.End);

                if (nextLesson != null && (this.CurrentLesson.LessonTime.Start.Date == nextLesson.LessonTime.Start.Date))
                {
                    this.NextLesson = new LessonViewModel(nextLesson);
                }
                else //Aktuell Unterricht nächstes Freizeit
                    this.NextLesson = new LessonViewModel(new Lesson(null, new SubjectRoom(new Subject(freeTimeString), "")));
            }

            //Eigentlich haben sich alle Eigenschaften verändert.
            NotifyPropertyChanged(null);
        }

        private Lesson GetNextLesson(Dictionary<DateTime, Dictionary<LessonTime, SubjectRoom>> lessons, DateTime startTime)
        {
            foreach (var day in lessons)
            {
                if (day.Key.Date >= startTime.Date)
                {
                    foreach (var lesson in day.Value)
                    {
                        DateTime lessonStart = day.Key.Date + lesson.Key.Start.TimeOfDay;
                        DateTime lessonEnd = day.Key.Date + lesson.Key.End.TimeOfDay;

                        if ((lessonEnd > startTime) && (lesson.Value.Subject != Subject.None))
                        {
                            return new Lesson(new LessonTime(lesson.Key.Number, lessonStart, lessonEnd), lesson.Value);
                        }
                    }
                }
            }
            return null;
        }
    }
}
