using System;

namespace Stundenplan.Data
{
    /// <summary>
    /// Datenklasse. Stellt ein Unterichtsfach dar.
    /// </summary>
    public sealed class Subject : NotifyPropertyChangedObject
    {
        /// <summary>
        /// Die Freistunde.
        /// </summary>
        public static readonly Subject None = new Subject("");

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

        private string teacher;
        public string Teacher
        {
            get { return teacher; }
            set
            {
                this.teacher = value;
                NotifyPropertyChanged("Teacher");
            }
        }

        private string color;
        public string Color
        {
            get { return color; }
            set
            {
                this.color = value;
                NotifyPropertyChanged("Color");
            }
        }

        public bool IsValidSubject { get { return (this != Subject.None); } }

        public Subject(string name)
        {
            this.name = name;
        }

        public Subject(string name, string teacher)
        {
            this.name = name;
            this.teacher = teacher;
        }

        public Subject(string name, string teacher, string color)
        {
            this.name = name;
            this.teacher = teacher;
            this.color = color;
        }

        public override string ToString()
        {
            return "Subject: " + this.Name;
        }
    }
}
