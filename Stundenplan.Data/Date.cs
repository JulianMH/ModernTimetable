using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;

namespace Stundenplan.Data
{
    /// <summary>
    /// Datenklasse. Stellt einen Termin dar.
    /// </summary>
    public sealed class Date : NotifyPropertyChangedObject
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                this.name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private DateTime due;
        public DateTime Due
        {
            get { return due; }
            set
            {
                this.due = value;
                NotifyPropertyChanged("Due");
            }
        }

        private RepeatBehaviour repeatBehaviour;
        public RepeatBehaviour RepeatBehaviour
        {
            get { return repeatBehaviour; }
            set
            {
                this.repeatBehaviour = value;
                NotifyPropertyChanged("RepeatBehaviour");
                NotifyPropertyChanged("RepeatingVisibility");
            }
        }

        public Date(string name, DateTime due, RepeatBehaviour repeatBehaviour)
        {
            this.name = name;
            this.due = due;
            this.repeatBehaviour = repeatBehaviour;
        }
    }
}
