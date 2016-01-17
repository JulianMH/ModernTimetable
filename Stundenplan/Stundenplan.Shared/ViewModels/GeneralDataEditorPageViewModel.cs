using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    public class GeneralDataEditorPageViewModel : NotifyPropertyChangedObject
    {
        private Timetable timetable;

        public ReadOnlyCollection<SubjectViewModel> Subjects { get; private set; }
        public ReadOnlyCollection<LessonTimeViewModel> LessonTimes { get; private set; }

        public GeneralDataEditorPageViewModel(Timetable timetable)
        {
            this.timetable = timetable;

            this.Subjects = new ReadOnlyCollection<SubjectViewModel>(timetable.Subjects.Select(p => new SubjectViewModel(p)).ToList());
            this.LessonTimes = new ReadOnlyCollection<LessonTimeViewModel>(timetable.LessonTimes.Select(p => new LessonTimeViewModel(p)).ToList());

            timetable.Subjects.CollectionChanged += Subjects_CollectionChanged;
            timetable.LessonTimes.CollectionChanged += LessonTimes_CollectionChanged;
        }

        void LessonTimes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.LessonTimes = new ReadOnlyCollection<LessonTimeViewModel>(timetable.LessonTimes.Select(p => new LessonTimeViewModel(p)).ToList());
            NotifyPropertyChanged("LessonTimes");
        }

        void Subjects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Subjects = new ReadOnlyCollection<SubjectViewModel>(timetable.Subjects.Select(p => new SubjectViewModel(p)).ToList());
            NotifyPropertyChanged("Subjects");
        }
    }
}
