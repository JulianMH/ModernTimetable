using Stundenplan.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stundenplan.ViewModels.Design
{
    public class DesignTimetableSettingsPageViewModel : TimetableSettingsPageViewModel
    {
        public DesignTimetableSettingsPageViewModel() : base(DesignTimetable.Instance)
        {

        }
    }
}
