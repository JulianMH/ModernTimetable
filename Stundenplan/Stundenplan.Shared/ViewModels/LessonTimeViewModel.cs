using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;
using Windows.UI.Xaml.Controls;

namespace Stundenplan.ViewModels
{
    public class LessonTimeViewModel : Abstract.DataViewModel<LessonTime>
    {
        public DateTime Start { get { return this.data.Start; } set { this.data.Start = value; } }
        public DateTime End { get { return this.data.End; } set { this.data.End = value; } }

        public TimeSpan StartTimeSpan { get { return this.data.Start.TimeOfDay; } set { this.data.Start = new DateTime(2015, 1, 1, value.Hours, value.Minutes, value.Seconds); } }
        public TimeSpan EndTimeSpan { get { return this.data.End.TimeOfDay; } set { this.data.End = new DateTime(2015, 1, 1, value.Hours, value.Minutes, value.Seconds); } }
        public int Number { get { return this.data.Number; } }
        public int NumberPlusOne { get { return this.data.Number + 1; } }

        public LessonTimeViewModel(LessonTime lessonTime)
            : base(lessonTime)
        {
            //Notify PropertyChanged wird hier schon von der Basisklasse übernommen.
        }
    }
}
