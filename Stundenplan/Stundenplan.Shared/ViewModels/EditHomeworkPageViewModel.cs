using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    /// <summary>
    /// ViewModel für die EditHomework Page.
    /// </summary>
    public class EditHomeworkPageViewModel : HomeworkViewModel
    {
        public ReadOnlyCollection<SubjectViewModel> Subjects { get; private set; }

        public EditHomeworkPageViewModel(Homework homework, Timetable timetable)
            : base(homework)
        {
            this.Subjects = new ReadOnlyCollection<SubjectViewModel>(timetable.Subjects.Select(p => new SubjectViewModel(p)).ToList());
        }
    }
}
