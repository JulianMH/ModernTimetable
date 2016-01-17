using Stundenplan.Localization;
using Stundenplan.Data;
using Stundenplan.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stundenplan.ViewModels.Design
{
    public class DesignEditLessonViewModel : EditLessonPageViewModel
    {
        public DesignEditLessonViewModel()
            : base(DesignTimetable.Instance, DesignTimetable.Instance.Days[2], DesignTimetable.Instance.Days[2].Lessons[2], Strings.PageEditLessonNewSubject)
        {
        }
    }
}
