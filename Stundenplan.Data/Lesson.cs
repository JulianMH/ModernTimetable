using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Linq;
using System.Xml;

namespace Stundenplan.Data
{
    /// <summary>
    /// Datenklasse. Stellt eine Stunde im Stundenplan dar.
    /// </summary>
    public sealed class Lesson : NotifyPropertyChangedObject
    {
        /// <summary>
        /// Das Fach und der Raum in Geraden Wochen, dieser Wert wird verwendet wenn alle Wochen gleich sind
        /// </summary>
        public SubjectRoom DataEvenWeek { get; private set; }
        /// <summary>
        /// Das Fach und der Raum in Ungeraden Wochen
        /// </summary>
        public SubjectRoom DataOddWeek { get; private set; }

        private LessonTime officalLessonTime;
        private LessonTime lessonTime;
        public LessonTime LessonTime
        {
            get { return lessonTime; }
            set
            {
                this.lessonTime = value;
                NotifyPropertyChanged("LessonTime");
            }
        }

        private bool isCustomLessonTime;
        public bool IsCustomLessonTime
        {
            get { return isCustomLessonTime; }
            set
            {
                if (value == true)
                    this.LessonTime = new LessonTime(lessonTime.Number, lessonTime.Start, lessonTime.End);
                else
                    this.LessonTime = officalLessonTime;

                this.isCustomLessonTime = value;
                NotifyPropertyChanged("IsCustomLessonTime");
            }
        }

        public Lesson(LessonTime lessonTime)
        {
            this.officalLessonTime = lessonTime;
            this.IsCustomLessonTime = false;

            this.DataEvenWeek = new SubjectRoom(Subject.None, "");
            this.DataOddWeek = new SubjectRoom(this.DataEvenWeek.Subject, this.DataEvenWeek.Room);
        }

        public Lesson(LessonTime lessonTime, SubjectRoom data)
        {
            this.officalLessonTime = lessonTime;
            this.IsCustomLessonTime = false;

            this.DataEvenWeek = data;
            this.DataOddWeek = new SubjectRoom(this.DataEvenWeek.Subject, this.DataEvenWeek.Room);
        }

        #region XML
        internal Lesson(LessonTime lessonTime, Timetable timetable, XElement element)
        {
            this.officalLessonTime = lessonTime;

            if (element.Attribute("Start") != null && element.Attribute("End") != null)
            {
                this.isCustomLessonTime = true;
                this.LessonTime = new LessonTime(officalLessonTime.Number,
                    DateTime.Parse(element.Attribute("Start").Value),
                    DateTime.Parse(element.Attribute("End").Value));
            }
            else
                this.IsCustomLessonTime = false;

            if (element.Attribute("Subject") != null)
            {
                //Ins neue Format konvertieren
                this.DataEvenWeek = new SubjectRoom(timetable.GetSubject(element.Attribute("Subject").Value), element.Attribute("Room").Value);
                this.DataOddWeek = new SubjectRoom(this.DataEvenWeek.Subject, this.DataEvenWeek.Room);
            }
            else
            {
                this.DataEvenWeek = new SubjectRoom(
                    timetable.GetSubject(element.Attribute("SubjectEvenWeek").Value),
                    element.Attribute("RoomEvenWeek").Value);

                bool value;
                if (element.Attribute("IsChangingLesson") != null &&
                    bool.TryParse(element.Attribute("IsChangingLesson").Value, out value)
                    && !value)
                {
                    this.DataOddWeek = new SubjectRoom(DataEvenWeek.Subject, DataEvenWeek.Room);
                }
                else
                {
                    this.DataOddWeek = new SubjectRoom(
                        timetable.GetSubject(element.Attribute("SubjectOddWeek").Value),
                        element.Attribute("RoomOddWeek").Value);
                }
            }
        }

        internal void Write(XmlWriter writer)
        {
            writer.WriteStartElement("Lesson");

            writer.WriteAttributeString("SubjectEvenWeek", this.DataEvenWeek.Subject.Name);
            writer.WriteAttributeString("RoomEvenWeek", this.DataEvenWeek.Room);

            writer.WriteAttributeString("SubjectOddWeek", this.DataOddWeek.Subject.Name);
            writer.WriteAttributeString("RoomOddWeek", this.DataOddWeek.Room);

            if (this.isCustomLessonTime)
            {
                writer.WriteAttributeString("Start", this.lessonTime.Start.ToString("HH:mm"));
                writer.WriteAttributeString("End", this.lessonTime.End.ToString("HH:mm"));
            }

            writer.WriteEndElement();
        }
        #endregion

    }
}
