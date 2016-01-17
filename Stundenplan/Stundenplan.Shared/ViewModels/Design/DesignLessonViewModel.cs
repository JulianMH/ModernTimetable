using Stundenplan.Data;
using Stundenplan.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stundenplan.ViewModels.Design
{
    public class DesignLessonViewModel : LessonViewModel
    {
        public DesignLessonViewModel()
            : base(DesignTimetable.Instance.Days[2].Lessons[2])
        {
        }
    }
}
