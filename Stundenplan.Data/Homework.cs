using System;
using System.ComponentModel;

namespace Stundenplan.Data
{
    /// <summary>
    /// Datenklasse. Stellt eine Hausaufgabe dar.
    /// </summary>
    public sealed class Homework : NotifyPropertyChangedObject
    {
        /// <summary>
        /// Der Konstruktor.
        /// </summary>
        /// <param name="subject">Das Fach, in dem diese Hausaufgabe auf ist.</param>
        public Homework(Subject subject)
        {
            this.subject = subject;
            this.fromDate = DateTime.Now.Date;
            this.toDate = DateTime.Today.Date.AddDays(1);
            this.text = "";
        }

        private Subject subject;
        public Subject Subject
        {
            get { return subject; }
            set
            {
                this.subject = value;
                NotifyPropertyChanged("Subject");
            }
        }

        private DateTime toDate;
        public DateTime ToDate
        {
            get { return toDate.Date; }
            set
            {
                this.toDate = value;
                NotifyPropertyChanged("ToDate");
            }
        }

        private DateTime fromDate;
        public DateTime FromDate
        {
            get { return fromDate.Date; }
            set
            {
                this.fromDate = value;
                NotifyPropertyChanged("FromDate");
            }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                this.text = value;
                NotifyPropertyChanged("Text");
            }
        }

        private bool isDone;
        public bool IsDone
        {
            get { return isDone; }
            set
            {
                this.isDone = value;
                NotifyPropertyChanged("IsDone");
            }
        }
    }
}
