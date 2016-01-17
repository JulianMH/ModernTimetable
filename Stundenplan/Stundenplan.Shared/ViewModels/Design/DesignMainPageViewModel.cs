using Stundenplan.Data;
using Stundenplan.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stundenplan.ViewModels.Design
{
    public class DesignMainPageViewModel : MainPageViewModel
    {
        public DesignMainPageViewModel()
            : base(DesignTimetable.Instance)
        { }
    }
}
