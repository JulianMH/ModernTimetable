using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    public class SubjectViewModel : Abstract.DataViewModel<Subject>
    {
        public string Name { get { return this.data.Name; } set { if (!IsValidSubject) throw new InvalidOperationException(); else this.data.Name = value; } }

        public string Teacher { get { return this.data.Teacher; } set { if (!IsValidSubject) throw new InvalidOperationException(); else this.data.Teacher = value; } }


        public string Color { get { return this.data.Color; } }

        public bool IsValidSubject { get { return this.data.IsValidSubject; } }

        public bool IsDeleted { get; private set; }

        public SubjectViewModel(Subject subject)
            : base(subject)
        {
            //Alle ereignisse werden weitergeben
            this.IsDeleted = false;
        }

        internal void SetOnHomework(Homework homework)
        {
            homework.Subject = this.data;
        }

        //returns default room
        internal string SetOnLesson(bool isOddWeek, Lesson lesson, Timetable timetable)
        {
            if (isOddWeek) lesson.DataOddWeek.Subject = this.data;
            else lesson.DataEvenWeek.Subject = this.data;

            return timetable.GetDefaultRoom(this.data);
        }

        public int GetLessonCount(Timetable timetable)
        {
            return timetable.GetAllLessons(this.data).Count();
        }

        public void RemoveSubject(Timetable timetable)
        {
            this.IsDeleted = true;
            timetable.RemoveSubject(this.data);
        }

        public void AddToTimetable(Timetable timetable, string newName)
        {
            this.data.Name = newName;
            timetable.Subjects.Add(this.data);
        }

        public int GetIndex(IList<Subject> subjects)
        {
            return subjects.IndexOf(this.data);
        }

        public DateTime GetNextDate(Timetable timetable)
        {
            return timetable.GetNextDate(this.data, LessonViewModel.GetIsOddWeek());
        }
    }
}
