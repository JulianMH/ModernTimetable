using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    /// <summary>
    /// ViewModel für die EditLesson Page.
    /// </summary>
    public class EditLessonPageViewModel : Abstract.DataViewModel<Lesson>
    {
        private Timetable timetable;
        private string newSubjectString;

        public ReadOnlyCollection<SubjectViewModel> Subjects { get; private set; }
        public ReadOnlyCollection<string> Rooms { get; private set; }
        public DayViewModel Day { get; private set; }

        public SubjectViewModel NewSubject { get; private set; }

        //Even Week
        public SubjectViewModel EvenWeekSubject
        {
            get { return new SubjectViewModel(this.data.DataEvenWeek.Subject); }
            set
            {
                if (value != null)
                {
                    //Sollte automatisch PropertyChanged auslösen
                    if (!this.IsChangingLesson)
                        value.SetOnLesson(true, this.data, this.timetable);
                    this.EvenWeekRoom = value.SetOnLesson(false, this.data, this.timetable);
                }
                else
                    NotifyPropertyChanged("EvenWeekSubject");
            }
        }

        public string EvenWeekRoom
        {
            get { return this.data.DataEvenWeek.Room; }
            set
            {
                //Sollte automatisch PropertyChanged auslösen
                //if (!this.IsChangingLesson)
                this.data.DataOddWeek.Room = value;

                this.data.DataEvenWeek.Room = value;
            }
        }

        //Odd Week
        public SubjectViewModel OddWeekSubject
        {
            get { return new SubjectViewModel(this.data.DataOddWeek.Subject); }
            set
            {
                if (value != null)
                {
                    //Sollte automatisch PropertyChanged auslösen
                    this.OddWeekRoom = value.SetOnLesson(true, this.data, this.timetable);
                }
                else
                    NotifyPropertyChanged("OddWeekSubject");
            }
        }

        public string OddWeekRoom
        {
            get { return this.data.DataOddWeek.Room; }
            set
            {
                //Sollte automatisch PropertyChanged auslösen
                this.data.DataOddWeek.Room = value;
            }
        }


        private bool isChangingLesson;
        public bool IsChangingLesson
        {
            get
            {
                return this.isChangingLesson;
            }
            set
            {
                this.isChangingLesson = value;
                NotifyPropertyChanged("IsChangingLesson");
            }
        }

        public bool IsCustomLessonTime
        {
            get { return this.data.IsCustomLessonTime; }
            set
            {
                //Sollte automatisch PropertyChanged auslösen
                this.data.IsCustomLessonTime = value;

                //Sicher gehen das die Standartuhrzeiten nicht verwendet werden.
                if (this.data.IsCustomLessonTime)
                {
                    this.data.LessonTime = new LessonTime(this.data.LessonTime.Number, this.data.LessonTime.Start, this.data.LessonTime.End);
                }
                else
                {
                    this.data.LessonTime = timetable.LessonTimes[this.data.LessonTime.Number];
                }
                this.LessonTime = new LessonTimeViewModel(this.data.LessonTime);
                NotifyPropertyChanged("LessonTime");
            }
        }

        public LessonTimeViewModel LessonTime { get; private set; }

        public EditLessonPageViewModel(Timetable timetable, Day day, Lesson lesson, string newSubjectString)
            : base(lesson)
        {
            this.timetable = timetable;

            this.timetable.Subjects.CollectionChanged += (sender, e) => { SetupSubjects(); };

            this.isChangingLesson = !((this.data.DataEvenWeek.Subject == this.data.DataOddWeek.Subject) && (this.data.DataEvenWeek.Room == this.data.DataOddWeek.Room));
            this.LessonTime = new LessonTimeViewModel(this.data.LessonTime);

            List<string> rooms = new List<string>();
            foreach (var d in timetable.Days)
                foreach (var currentlesson in d.Lessons)
                {
                    rooms.Add(currentlesson.DataEvenWeek.Room);
                    rooms.Add(currentlesson.DataOddWeek.Room);
                }
            this.Rooms = new ReadOnlyCollection<string>(rooms.Distinct().ToList());
            this.data.DataEvenWeek.PropertyChanged += DataPropertyChanged;
            this.data.DataOddWeek.PropertyChanged += DataPropertyChanged;

            this.newSubjectString = newSubjectString;

            this.Day = new DayViewModel(timetable, day);

            //Setup Subjects
            SetupSubjects();
        }

        private void DataPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Subject")
            {
                if (sender == data.DataOddWeek)
                    this.NotifyPropertyChanged("OddWeekSubject");
                else if (sender == data.DataEvenWeek)
                    this.NotifyPropertyChanged("EvenWeekSubject");
            }
            else if (e.PropertyName == "Room")
            {
                if (sender == data.DataOddWeek)
                    this.NotifyPropertyChanged("OddWeekRoom");
                else if (sender == data.DataEvenWeek)
                    this.NotifyPropertyChanged("EvenWeekRoom");
            }
        }
        /// <summary>
        /// Wandelt diese Unterichtstunde in eine Freistunde um.
        /// </summary>
        public void SetToNoSubject()
        {
            this.data.DataOddWeek.Subject = Subject.None;
            this.data.DataEvenWeek.Subject = Subject.None;
        }

        public void SetupSubjects()
        {
            this.NewSubject = new SubjectViewModel(new Subject(newSubjectString));

            var subjects = new List<SubjectViewModel>(timetable.Subjects.OrderBy(p => p.Name).Select(p => new SubjectViewModel(p)));
            subjects.Insert(0, new SubjectViewModel(Subject.None));
            subjects.Add(this.NewSubject);

            this.Subjects = new ReadOnlyCollection<SubjectViewModel>(subjects);
            NotifyPropertyChanged("Subjects");
        }


        public LessonViewModel AsLessonViewModel()
        {
            return new LessonViewModel(this.data);
        }
    }
}
