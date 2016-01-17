using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    public class EditDatePageViewModel : DateViewModel
    {
        public ReadOnlyCollection<RepeatBehaviour> RepeatBehaviours { get; private set; }

        public EditDatePageViewModel(Date date)
            : base(date)
        {
            this.RepeatBehaviours = new ReadOnlyCollection<RepeatBehaviour>(new List<RepeatBehaviour>() { RepeatBehaviour.None, RepeatBehaviour.Weekly, RepeatBehaviour.Monthly });

        }
    }
}
