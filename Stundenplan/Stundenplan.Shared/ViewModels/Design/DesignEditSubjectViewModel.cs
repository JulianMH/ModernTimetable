using Stundenplan.Localization;
using Stundenplan.Data;
using Stundenplan.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stundenplan.ViewModels.Design
{
    public class DesignEditSubjectViewModel : EditSubjectPageViewModel
    {
        public DesignEditSubjectViewModel()
            : base(new Subject("Lorem", "Ipsum", "F472D0"), DesignTimetable.Instance)
        {
        }
    }
}
