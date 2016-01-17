using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels
{
    /// <summary>
    /// ViewModel für die EditSubject Page.
    /// </summary>
    public class EditSubjectPageViewModel : SubjectViewModel
    {
        public ReadOnlyCollection<string> Teachers { get; private set; }

        public class ColorViewModel : NotifyPropertyChangedObject
        {
            private Subject subject;

            public string Color { get; private set; }
            public string OtherSubjects { get; private set; }
            public bool IsSelected { get { return subject.Color == Color; } }

            public ColorViewModel(string color, Subject subject, Timetable timetable)
            {
                this.Color = color;

                var subjects = timetable.Subjects.Where(p => p.Color == color && p != subject).Select(p => p.Name).OrderBy(p => p);

                int maxAmount = 3;

                if (!subjects.Any())
                    OtherSubjects = "";
                else
                {
                    OtherSubjects = string.Join("," + Environment.NewLine, subjects.Take(maxAmount));

                    if (subjects.Count() > 3)
                        OtherSubjects += "," + Environment.NewLine + "...";
                }

                this.subject = subject;
                subject.PropertyChanged += subject_PropertyChanged;
            }

            void subject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "Color")
                {
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        public ReadOnlyCollection<ColorViewModel> AvailableColors { get; private set; }

        public ColorViewModel SelectedCustomColor
        {
            get
            {
                return AvailableColors.FirstOrDefault(p => p.Color == this.data.Color) ?? AvailableColors.First();
            }
            set
            {
                if (value == null)
                    return;

                if (!IsValidSubject) throw new InvalidOperationException();
                else this.data.Color = value.Color;
                NotifyPropertyChanged("IsCustomColor");
                NotifyPropertyChanged("SelectedCustomColor");
            }
        }

        public bool IsCustomColor
        {
            get
            {
                return this.data.Color != null && this.data.Color != "";
            }
            set
            {
                if (!IsValidSubject) throw new InvalidOperationException(); 
                else this.data.Color = value ? AvailableColors.First().Color : "";
                NotifyPropertyChanged("IsCustomColor");
                NotifyPropertyChanged("SelectedCustomColor");
            }
        }


        private string oldName;

        public EditSubjectPageViewModel(Subject subject, Timetable timetable)
            : base(subject)
        {
            this.oldName = subject.Name;
            this.Teachers = new ReadOnlyCollection<string>(timetable.Subjects.Select(p => p.Teacher).Distinct().ToList());

            this.AvailableColors = new ReadOnlyCollection<ColorViewModel>(new String[]
            {
                "A4C400",
                "60A917",
                "008A00",
                "00ABA9",

                "1BA1E2",
                "3E65FF",
                "6A00FF",
                "AA00FF",

                "F472D0",
                "D80073",
                "A20025",
                "E51400",

                "FA6800",
                "F0A30A",
                "E3C800",
                "825A2C",

                "6D8764",
                "647687",
                "76608A",
                "87794E"
            }.Select(p => new ColorViewModel(p, subject, timetable)).ToList());
        }

        public enum NamingProblem
        {
            None, NameEmpty, NameAlreadyUsed, NameIsKeyword
        }

        public NamingProblem TryApplyName(string newName, IEnumerable<Subject> subjects, IEnumerable<string> keywords)
        {
            if (newName == oldName)
                return NamingProblem.None;
            else if (newName.Trim() == "")
            {
                this.Name = oldName;
                return NamingProblem.NameEmpty;
            }
            else if (keywords.Contains(newName))
            {
                this.Name = oldName;
                return NamingProblem.NameIsKeyword;
            }
            else if (subjects.Any(p => p.Name == newName))
            {
                this.Name = oldName;
                return NamingProblem.NameAlreadyUsed;
            }
            else
            {
                this.Name = newName;
                return NamingProblem.None;
            }
        }
    }
}
