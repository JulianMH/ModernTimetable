using System;
using System.ComponentModel;

namespace Stundenplan.Data
{
    /// <summary>
    /// Datenklasse. Stellt eine den Fach und den Raum einer Stunde im Stundenplan dar.
    /// </summary>
    public sealed class SubjectRoom : NotifyPropertyChangedObject
    {
        private Subject subject;
        public Subject Subject
        {
            get { return subject; }
            set
            {
                if(this.subject != null)
                    this.subject.PropertyChanged -= SubjectPropertyChanged;
                this.subject = value;
                NotifyPropertyChanged("Subject");
            }
        }

        private void SubjectPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) { NotifyPropertyChanged("Subject"); }

        private string room;
        public string Room
        {
            get { return room; }
            set
            {
                this.room = value;
                NotifyPropertyChanged("Room");
            }
        }

        public SubjectRoom(Subject subject, string room)
        {
            this.room = room;
            this.subject = subject;
            subject.PropertyChanged += SubjectPropertyChanged;
        }

        //Debug Helfer
        public override string ToString()
        {
            return this.subject.Name + this.room;
        }
    }

}