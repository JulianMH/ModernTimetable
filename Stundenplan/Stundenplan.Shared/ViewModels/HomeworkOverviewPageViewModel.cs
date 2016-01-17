using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    /// <summary>
    /// ViewModel für die Stundenplan Homework Overview Page.
    /// </summary>
    public class HomeworkOverviewPageViewModel : NotifyPropertyChangedObject
    {
        private Timetable timetable;

        public enum OrderBy
        {
            From,
            To,
            Subject
        }

        private OrderBy homeworkOrder = OrderBy.Subject;
        public OrderBy HomeworkOrder { get { return homeworkOrder; } set { this.homeworkOrder = value; NotifyPropertyChanged("HomeworkOrder"); this.ResortHomework(); } }

        public SubjectViewModel NoFilterSubject { get; private set; }
        private SubjectViewModel homeworkFilter = null;
        public SubjectViewModel HomeworkFilter { get { return homeworkFilter; } set { this.homeworkFilter = value; NotifyPropertyChanged("HomeworkFilter"); this.ResortHomework(); } }

        public ReadOnlyCollection<HomeworkViewModel> TodayHomework { get; private set; }
        public ReadOnlyCollection<HomeworkViewModel> TommorowHomework { get; private set; }
        public ReadOnlyCollection<HomeworkViewModel> FilteredHomework { get; private set; }

        public ReadOnlyCollection<SubjectViewModel> Subjects { get; private set; }

        public ReadOnlyCollection<OrderBy> HomeworkOrders { get; private set; }

        public HomeworkOverviewPageViewModel(Timetable timetable, string noFilterSubjectName)
        {
            this.timetable = timetable;
            this.timetable.Homeworks.CollectionChanged += (sender, e) => { this.ResortHomework(); };
            this.timetable.Subjects.CollectionChanged += (sender, e) => { this.ReloadSubjects(); };

            this.NoFilterSubject = new SubjectViewModel(new Subject(noFilterSubjectName));
            ReloadSubjects();

            this.HomeworkOrders = new ReadOnlyCollection<OrderBy>(new List<OrderBy>(Enum.GetValues(typeof(OrderBy)).Cast<OrderBy>()));

            this.ResortHomework();
        }

        private void ReloadSubjects()
        {
            this.homeworkFilter = this.NoFilterSubject;
            List<SubjectViewModel> subjects = timetable.Subjects.Select(p => new SubjectViewModel(p)).ToList();
            subjects.Insert(0, NoFilterSubject);
            this.Subjects = new ReadOnlyCollection<SubjectViewModel>(subjects);
        }

        public void ResortHomework()
        {
            var today = DateTime.Today.Date;
            var tomorrow = today.AddDays(1).Date;

            this.TodayHomework = this.GetSortedHomework(p => p.ToDate.Date == today);
            this.TommorowHomework = this.GetSortedHomework(p => p.ToDate.Date == tomorrow);
            this.FilteredHomework = this.GetSortedHomework(p => { if (this.homeworkFilter == this.NoFilterSubject) return true; else return new SubjectViewModel(p.Subject).Equals(this.homeworkFilter); });

            NotifyPropertyChanged("TodayHomework");
            NotifyPropertyChanged("TommorowHomework");
            NotifyPropertyChanged("FilteredHomework");
        }

        private ReadOnlyCollection<HomeworkViewModel> GetSortedHomework(Func<Homework, bool> filter)
        {
            IEnumerable<Homework> output;
            switch (this.homeworkOrder)
            {
                case OrderBy.Subject:
                    output = timetable.Homeworks.OrderBy(p => p.Subject.Name).ThenBy(p => p.IsDone);
                    break;
                case OrderBy.To:
                    output = timetable.Homeworks.OrderBy(p => p.IsDone).ThenBy(p => p.ToDate);
                    break;
                case OrderBy.From:
                    output = timetable.Homeworks.OrderBy(p => p.IsDone).ThenBy(p => p.FromDate);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return new ReadOnlyCollection<HomeworkViewModel>(output.Where(filter).Select(p => new HomeworkViewModel(p)).ToList());
        }
    }
}
