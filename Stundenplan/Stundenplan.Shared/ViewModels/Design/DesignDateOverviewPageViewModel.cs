using Stundenplan.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stundenplan.ViewModels.Design
{
    public class DesignDateOverviewPageViewModel : DateOverviewPageViewModel
    {
        public DesignDateOverviewPageViewModel() : base(DesignTimetable.Instance,
                Strings.DateTimePast,
                Strings.DateTimeToday,
                Strings.DateTimeThisWeek,
                Strings.DateTimeThisMonth,
                Strings.DateTimeFuture)
        {

        }
    }
}
