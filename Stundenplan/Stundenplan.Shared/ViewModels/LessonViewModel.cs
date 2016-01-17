using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    public class LessonViewModel : Abstract.DataViewModel<Lesson>
    {
        public LessonTimeViewModel LessonTime { get { if (this.data.LessonTime == null) return null; else return new LessonTimeViewModel(this.data.LessonTime); } }
        public SubjectViewModel ThisWeekSubject { get { return new SubjectViewModel(this.GetData().Subject); } }
        public string ThisWeekRoom { get { return this.GetData().Room; } }

        /// <summary>
        /// Das Fach und der Raum, die dies Woche gültig sind.
        /// </summary>
        private SubjectRoom GetData()
        {
            if (IsChangingLesson != true)
                return this.data.DataEvenWeek;
            else if (isOddWeek)
                return this.data.DataOddWeek;
            else
                return this.data.DataEvenWeek;
        }

        public bool IsChangingLesson
        {
            get
            {
                return !((this.data.DataEvenWeek.Subject == this.data.DataOddWeek.Subject) && (this.data.DataEvenWeek.Room == this.data.DataOddWeek.Room));
            }
        }

        //Eigenschaften nur zum Rendern, wird von anderen Klassen Festgelegt
        public bool UnfinishedHomework
        {
            get;
            private set;
        }


        public LessonViewModel(Lesson lesson)
            : base(lesson)
        {
            IsOddWeekChanged += new EventHandler(LessonViewModel_isOddWeekChanged);

            this.data.DataEvenWeek.PropertyChanged += DataPropertyChanged;
            this.data.DataOddWeek.PropertyChanged += DataPropertyChanged;
        }

        private void DataPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Room" || e.PropertyName == "Subject")
            {
                this.NotifyPropertyChanged("ThisWeekSubject");
                this.NotifyPropertyChanged("ThisWeekRoom");
                this.NotifyPropertyChanged("IsChangingLesson");
            }
        }

        #region IsOddWeek Change
        private void LessonViewModel_isOddWeekChanged(object sender, EventArgs e)
        {
            this.NotifyPropertyChanged("ThisWeekSubject");
            this.NotifyPropertyChanged("ThisWeekRoom");
        }

        private static bool isOddWeek = ConvertHelpers.IsOddWeek(DateTime.Now);
        public static event EventHandler IsOddWeekChanged;

        public static void SetIsOddWeek(bool isOddWeek)
        {
            LessonViewModel.isOddWeek = isOddWeek;
            if (IsOddWeekChanged != null)
                IsOddWeekChanged(null, new EventArgs());
        }

        public static bool GetIsOddWeek()
        {
            return isOddWeek;
        }
        #endregion

        internal void UpdateAllDesignData(IList<Subject> subjectsWithHomework)
        {
            int index = this.ThisWeekSubject.GetIndex(subjectsWithHomework);
            this.UnfinishedHomework = (index != -1);
            NotifyPropertyChanged("UnfinishedHomework");
        }

        /// <summary>
        /// Wandelt diese Unterichtstunde in eine Freistunde um.
        /// </summary>
        public void SetToNoSubject()
        {
            this.data.DataOddWeek.Subject = Subject.None;
            this.data.DataOddWeek.Room = "";
            this.data.DataEvenWeek.Subject = Subject.None;
            this.data.DataEvenWeek.Room = "";
        }

        public int GetIndex(IList<Lesson> lessons)
        {
            return lessons.IndexOf(this.data);
        }
    }
}
