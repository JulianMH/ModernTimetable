using Stundenplan.Localization;
using Stundenplan.Data;
using Stundenplan.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stundenplan.ViewModels.Design
{
    public class DesignProgressViewModel : ProgressViewModel
    {
        public DesignProgressViewModel()
            : base(DesignTimetable.Instance, Strings.PageLandscapeViewNoLesson, Strings.PageLandscapeViewFreeTime,
                    Strings.PageLandscapeViewRemainingTimeFormat)
        {
        }
    }
}
