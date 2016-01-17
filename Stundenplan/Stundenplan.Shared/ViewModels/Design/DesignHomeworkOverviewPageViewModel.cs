using Stundenplan.Localization;
using Stundenplan.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stundenplan.ViewModels.Design
{
    class DesignHomeworkOverviewPageViewModel : HomeworkOverviewPageViewModel
    {
        public DesignHomeworkOverviewPageViewModel() :
            base(DesignTimetable.Instance, Strings.PageHomeworkOverviewAll)
        {

        }
    }
}
