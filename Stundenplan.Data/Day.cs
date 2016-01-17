using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Stundenplan.Data
{
    /// <summary>
    /// Stellt einen Wochentag da, verwaltet den Stundenplan für einen einzelnen Tag.
    /// </summary>
    public sealed class Day : NotifyPropertyChangedObject
    {
        public DayOfWeek DayOfWeek { get; private set; }
        public ObservableCollection<Lesson> Lessons { get; private set; }

        public Day(DayOfWeek day)
        {
            this.DayOfWeek = day;
            this.Lessons = new ObservableCollection<Lesson>();
        }

        public void CheckLessons()
        {
            var newLessons = this.Lessons;// new ObservableCollection<Lesson>(this.Lessons);

            int i = newLessons.Count - 1;
            while (i >= 0 && (!this.Lessons[i].DataEvenWeek.Subject.IsValidSubject && !this.Lessons[i].DataOddWeek.Subject.IsValidSubject))
            {
                //if (i == 0)
                //    day.Lessons.Clear();
                //else
                //if(i < day.Lessons.Count)
                newLessons.RemoveAt(i);

                i--;
            }
            this.Lessons = newLessons;
        }
    }
}
