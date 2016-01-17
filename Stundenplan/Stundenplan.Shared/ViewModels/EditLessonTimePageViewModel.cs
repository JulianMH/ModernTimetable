using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    public class EditLessonTimePageViewModel : LessonTimeViewModel
    {
        public string Title { get; private set; }

        public EditLessonTimePageViewModel(LessonTime lessonTime, string titleFormat)
            : base(lessonTime)
        {
            this.Title = String.Format(titleFormat, this.NumberPlusOne);
        }
    }
}
