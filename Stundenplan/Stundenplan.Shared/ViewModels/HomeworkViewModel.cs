using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    public class HomeworkViewModel : Abstract.DataViewModel<Homework>
    {
        public SubjectViewModel Subject
        {
            get { return new SubjectViewModel(this.data.Subject); }
            set
            {
                value.SetOnHomework(this.data);
                this.ToDate = this.Subject.GetNextDate(App.Timetable);
            }
        }
        public DateTime ToDate { get { return this.data.ToDate; } set { this.data.ToDate = value; } }
        public DateTimeOffset ToDateTimeOffset { get { return new DateTimeOffset(this.data.ToDate); } set { this.data.ToDate = value.DateTime; } }
        public DateTime FromDate { get { return this.data.FromDate; } set { this.data.FromDate = value; } }
        public DateTimeOffset FromDateTimeOffset { get { return new DateTimeOffset(this.data.FromDate); } set { this.data.FromDate = value.DateTime; } }
        public string Text { get { return this.data.Text; } set { this.data.Text = value; } }
        public bool IsDone { get { return this.data.IsDone; } set { this.data.IsDone = value; } }

        public HomeworkViewModel(Homework homework)
            : base(homework)
        { 
        }

        public int GetIndex(IList<Homework> homeworks)
        {
            return homeworks.IndexOf(this.data);
        }
    }
}
