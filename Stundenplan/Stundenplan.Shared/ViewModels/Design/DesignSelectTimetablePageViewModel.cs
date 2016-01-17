using Stundenplan.Localization;
using Stundenplan.Data;
using Stundenplan.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Stundenplan.ViewModels.Design
{
    public class DesignSelectTimetablePageViewModel : SelectTimetablePageViewModel
    {
        public DesignSelectTimetablePageViewModel() : base(null, null)
        {
        }

        public override void LoadData()
        {
            this.timetables = new ObservableCollection<TimetableDescription>()
            {
                new TimetableDescription("Test", "Lorem", 13, 5),
                new TimetableDescription("Test", "Ipsum", 12, 5)
            };
            this.Timetables = new ReadOnlyObservableCollection<TimetableDescription>(timetables);
            NotifyPropertyChanged("Timetables");
        }
    }
}
