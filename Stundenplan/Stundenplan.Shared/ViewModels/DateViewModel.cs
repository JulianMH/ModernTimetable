using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    public class DateViewModel : Abstract.DataViewModel<Date>
    {
        public string Name { get { return this.data.Name; } set { this.data.Name = value; } }
        public DateTime Due { get { return this.data.Due; } set { this.data.Due = value; NotifyPropertyChanged("IsFollowingWeek"); } }
        public DateTimeOffset DueDateTimeOffset { get { return new DateTimeOffset(this.data.Due); } set { this.data.Due = value.DateTime; NotifyPropertyChanged("IsFollowingWeek"); } }
        public RepeatBehaviour RepeatBehaviour { get { return this.data.RepeatBehaviour; } set { this.data.RepeatBehaviour = value; NotifyPropertyChanged("IsRepeating"); } }

        public bool IsRepeating
        {
            get
            {
                return (this.RepeatBehaviour != RepeatBehaviour.None);
            }
        }

        public bool IsFollowingWeek
        {
            get
            {
                if (this.Due.Date < DateTime.Now.Date)
                    return false;
                else if (this.Due.Date < DateTime.Now.AddDays(7).Date)
                    return true;
                else
                    return false;

            }
        }

        public DateViewModel(Date date)
            : base(date)
        {

        }

        public int GetIndex(IList<Date> dates)
        {
            return dates.IndexOf(this.data);
        }
    }
}
