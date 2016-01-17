using System;
using System.ComponentModel;

namespace Stundenplan.Data
{
    /// <summary>
    /// Datenklasse. Stellt eine Uhrzeiten einer Stunde im Stundenplan dar.
    /// </summary>
    public sealed class LessonTime : NotifyPropertyChangedObject
    {
        //Von den DateTimes wird nur die Zeit verwendet
        private DateTime start;
        public DateTime Start
        {
            get { return start; }
            set
            {
                this.start = value;
                this.NotifyPropertyChanged("Start");
                this.End = this.start + new TimeSpan(0, 45, 0);
            }
        }

        private DateTime end;
        public DateTime End
        {
            get { return end; }
            set
            {
                this.end = value;
                this.NotifyPropertyChanged("End");
            }
        }

        public int Number { get; private set; }

        public LessonTime(int number, DateTime start, DateTime end)
        {
            this.Number = number;
            this.start = start;
            this.end = end;
        }
    }
}
