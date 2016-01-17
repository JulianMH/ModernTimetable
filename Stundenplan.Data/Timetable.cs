
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Xml;
using System.Text;
using System.Globalization;

namespace Stundenplan.Data
{
    /// <summary>
    /// Stellt einen kompletten Stundenplan da.
    /// </summary>
    public sealed class Timetable : NotifyPropertyChangedObject
    {
        #region Daten-Eigenschaften
        public ObservableCollection<Day> Days { get; private set; }
        public ObservableCollection<LessonTime> LessonTimes { get; private set; }
        public ObservableCollection<Homework> Homeworks { get; private set; }
        public ObservableCollection<Date> Dates { get; private set; }
        public ObservableCollection<Subject> Subjects { get; set; }

        private string _name;
        public string Name { get { return _name; } set { _name = value; NotifyPropertyChanged("Name"); } }

        private string _fileName;
        public string FileName { get { return _fileName; } }
        #endregion

        /// <summary>
        /// Konstruktor. Erstellt einen neuen Stundenplan mit Standarteinstellungen und ohne Inhalt.
        /// </summary>
        /// <param name="name">Der Name des Stundenplans.</param>
        public Timetable(string name, string fileName)
        {
            this.Name = name;
            this._fileName = fileName;

            this.Days = new ObservableCollection<Day>();
            this.LessonTimes = new ObservableCollection<LessonTime>();
            this.Subjects = new ObservableCollection<Subject>();
            this.Homeworks = new ObservableCollection<Homework>();
            this.Dates = new ObservableCollection<Date>();
        }

        #region Datenzugriffs und veränderungsmethoden
        #region Subject
        public Day GetDay(DayOfWeek day)
        {
            return this.Days.FirstOrDefault(p => p.DayOfWeek == day);
        }

        public Subject GetSubject(string name)
        {
            foreach (Subject t in this.Subjects)
            {
                if (t.Name == name)
                    return t;
            }
            return Subject.None;
        }

        public bool RemoveSubject(Subject item)
        {
            var lessons = GetAllLessons(item);
            var homeworks = Homeworks.Where(p => p.Subject == item);

            foreach (Lesson lesson in lessons)
            {
                if (lesson.DataEvenWeek.Subject == item)
                    lesson.DataEvenWeek.Subject = Subject.None;
                if (lesson.DataOddWeek.Subject == item)
                    lesson.DataOddWeek.Subject = Subject.None;
            }

            List<Homework> homeworksToRemove = new List<Homework>(homeworks);
            foreach (Homework homework in homeworksToRemove)
                Homeworks.Remove(homework);

            this.Subjects.Remove(item);
            return true;
        }

        public Subject AddSubject(string name)
        {
            Subject returnValue = new Subject(name);
            this.Subjects.Add(returnValue);
            return returnValue;
        }

        public string GetSubjectName(string subjectNewFormat)
        {
            int id = 1;
            string name = String.Format(subjectNewFormat, id);
            while (this.GetSubject(name).IsValidSubject)
            {
                id++;
                name = String.Format(subjectNewFormat, id);
            }
            return name;
        }
        #endregion

        public IEnumerable<Lesson> GetAllLessons(Subject subject)
        {
            return this.Days.SelectMany(p => p.Lessons.Where(q => (q.DataOddWeek.Subject == subject) || (q.DataEvenWeek.Subject == subject)));
        }

        #region Suchalgorithmen
        /// <summary>
        /// Listet systematisch alle Stunden zwischen den Angegebenen Terminen auf, Geordnet nach Tagen und Uhrzeiten.
        /// </summary>
        /// <param name="from">Beginn der Auflistung. Datum und Uhrzeit werden beachtet.</param>
        /// <param name="to">Ende der Auflistung. Nur das Datum wird beachtet.</param>
        /// <param name="
        /// ">Soll in einer ungeraden oder einer graden Woche gearbeitet werden.</param>
        /// <returns>Strukturierte auflistung aller Stunden.</returns>
        public Dictionary<DateTime, Dictionary<LessonTime, SubjectRoom>> GetListedLessons(DateTime from, DateTime to, bool isOddWeek)
        {
            if (to <= from)
                throw new InvalidOperationException();

            Dictionary<DateTime, Dictionary<LessonTime, SubjectRoom>> returnDictionary = new Dictionary<DateTime, Dictionary<LessonTime, SubjectRoom>>();
            DateTime current = from;
            while (current < to)
            {
                Dictionary<LessonTime, SubjectRoom> currentDay = new Dictionary<LessonTime, SubjectRoom>();

                Day day = Days.FirstOrDefault(p => p.DayOfWeek == current.DayOfWeek);
                if (day != null)
                {
                    foreach (Lesson lesson in day.Lessons)
                    {
                        if (lesson.LessonTime.End.TimeOfDay > current.TimeOfDay)
                        {
                            if (isOddWeek)
                                currentDay.Add(lesson.LessonTime, lesson.DataOddWeek);
                            else
                                currentDay.Add(lesson.LessonTime, lesson.DataEvenWeek);
                        }
                    }
                }

                returnDictionary.Add(current.Date, currentDay);

                current = current.Date.AddDays(1);
            }

            return returnDictionary;
        }

        /// <summary>
        /// Sucht nach dem nächsten Termin andem ein bestimmtes Fach stattfindet, wenn kein Wert gefunden wurde wird Morgen zurückgeliefert.
        /// </summary>
        /// <param name="subject">Das Fach nachdem gesucht werden soll.</param>
        /// <param name="isOddWeek">Soll in einer ungeraden oder einer graden Woche gearbeitet werden.</param>
        /// <returns>Das Datum, andem das Fach wieder stattfindet, wennn keines gefunden der morgige Tag.</returns>
        public DateTime GetNextDate(Subject subject, bool isOddWeek)
        {
            DateTime tomorrow = DateTime.Today.AddDays(1);

            //Für die nächsten 2 Wochen gucken, wegen A/B Wochen.
            var listedLessons = GetListedLessons(tomorrow, tomorrow.AddDays(13), isOddWeek);

            foreach (var day in listedLessons)
            {
                if (day.Value.Any(p => p.Value.Subject == subject))
                    return day.Key;
            }

            //Ansonsten den morgigen Tag verwenden
            return tomorrow;
        }

        /// <summary>
        /// Versucht den Raum zu ermitteln, in dem das Fach stattfinden könnte.
        /// </summary>
        /// <param name="subject">Das Fach für das der Raum ermittelt wird.</param>
        /// <returns>Den Namen des Raumes oder einen leeren String.</returns>
        public string GetDefaultRoom(Subject subject)
        {
            foreach (Day day in this.Days)
            {
                foreach (Lesson lesson in day.Lessons)
                {
                    if (lesson.DataEvenWeek.Subject == subject)
                        return lesson.DataEvenWeek.Room;
                    else if (lesson.DataOddWeek.Subject == subject)
                        return lesson.DataOddWeek.Room;
                }
            }
            return "";
        }
        #endregion

        #region Lesson
        public Lesson AddLesson(Day day)
        {
            Lesson lesson = new Lesson(CheckLessonTime(day.Lessons.Count));
            day.Lessons.Add(lesson);

            return lesson;
        }

        public LessonTime AddLessonTime()
        {
            int number = this.LessonTimes.Count;
            if (number == 0)
                LessonTimes.Add(new LessonTime(0, new DateTime(1, 1, 1, 8, 00, 00), new DateTime(1, 1, 1, 8, 45, 0)));
            else
                LessonTimes.Add(new LessonTime(number, LessonTimes[number - 1].End, LessonTimes[number - 1].End + new TimeSpan(0, 45, 0)));

            return this.LessonTimes[number];
        }

        /// <summary>
        /// Liefert zu einer Stundennummer die ensprechenden Stundenzeiten zurück.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private LessonTime CheckLessonTime(int number)
        {
            if (LessonTimes.Count <= number)
                return null;
            else
                return LessonTimes[number];
        }
        #endregion

        public void UpdateRepeatingDates()
        {
            foreach (Date date in this.Dates)
            {
                switch (date.RepeatBehaviour)
                {
                    case RepeatBehaviour.Monthly:
                        while (DateTime.Now.Date > date.Due.Date)
                            date.Due = date.Due.AddMonths(1);
                        break;
                    case RepeatBehaviour.Weekly:
                        while (DateTime.Now.Date > date.Due.Date)
                            date.Due = date.Due.AddDays(7);
                        break;
                }
            }
        }

        public void SwitchEvenOddWeeks()
        {
            foreach (Day day in this.Days)
            {
                foreach (Lesson lesson in day.Lessons)
                {
                    var oldOddWeekSubject = lesson.DataOddWeek.Subject;
                    var oldOddWeekRoom = lesson.DataOddWeek.Room;

                    lesson.DataOddWeek.Subject = lesson.DataEvenWeek.Subject;
                    lesson.DataOddWeek.Room = lesson.DataEvenWeek.Room;

                    lesson.DataEvenWeek.Subject = oldOddWeekSubject;
                    lesson.DataEvenWeek.Room = oldOddWeekRoom;
                }
            }
        }

        #endregion

        #region Laden Speichern Neu
        #region Neuer Stundenplan
        /// <summary>
        /// Erstellt einen neuen, Leeren Stundenplan.
        /// Einziger Inhalt sind 5 Wochentage und 6 Zeiten für Unterrichtsstunden
        /// </summary>
        /// <param name="name">Name des Stundenplans</param>
        /// <returns>Ein neuer Stundenplan.</returns>
        public static Timetable CreateNew(string name, string fileName)
        {
            Timetable timetable = new Timetable(name, fileName);
            timetable.Days.Add(new Day(DayOfWeek.Monday));
            timetable.Days.Add(new Day(DayOfWeek.Tuesday));
            timetable.Days.Add(new Day(DayOfWeek.Wednesday));
            timetable.Days.Add(new Day(DayOfWeek.Thursday));
            timetable.Days.Add(new Day(DayOfWeek.Friday));

           // for (int i = 0; i < 6; i++)
             //   timetable.AddLessonTime();

            return timetable;
        }
        #endregion

        #region Save and Load
        public string WriteXml(string version)
        {
            StringBuilder builder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(builder);

            writer.WriteStartElement("Timetable");
            writer.WriteAttributeString("Name", this.Name);
            writer.WriteAttributeString("Version", version);
            writer.WriteAttributeString("FreeLessonTimesEnabled", true.ToString()); //Backwards Compatibilty

            writer.WriteStartElement("LessonTimes");
            foreach (LessonTime time in this.LessonTimes)
            {
                writer.WriteStartElement("LessonTime");
                writer.WriteAttributeString("Start", time.Start.ToString("HH:mm"));
                writer.WriteAttributeString("End", time.End.ToString("HH:mm"));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Subjects");
            foreach (Subject subject in this.Subjects)
            {
                writer.WriteStartElement("Subject");
                writer.WriteAttributeString("Name", subject.Name);
                writer.WriteAttributeString("Teacher", subject.Teacher);
                writer.WriteAttributeString("Color", subject.Color);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Homeworks");
            foreach (Homework homework in this.Homeworks)
            {
                writer.WriteStartElement("Homework");
                writer.WriteAttributeString("IsDone", homework.IsDone.ToString());
                writer.WriteAttributeString("Subject", homework.Subject.Name);
                writer.WriteAttributeString("From", homework.FromDate.ToString("dd.MM.yyyy"));
                writer.WriteAttributeString("To", homework.ToDate.ToString("dd.MM.yyyy"));
                writer.WriteAttributeString("Description", homework.Text.Replace("\n", "\\n"));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Lessons");

            foreach (Day day in this.Days)
            {
                writer.WriteStartElement(day.DayOfWeek.ToString());

                foreach (Lesson lesson in day.Lessons)
                    lesson.Write(writer);

                writer.WriteEndElement();
            }
            writer.WriteEndElement();


            writer.WriteStartElement("Dates");
            foreach (Date date in this.Dates)
            {
                writer.WriteStartElement("Date");

                writer.WriteAttributeString("Name", date.Name);
                writer.WriteAttributeString("Due", date.Due.ToString("dd.MM.yyyy"));
                writer.WriteAttributeString("Repeat", date.RepeatBehaviour.ToString());


                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.Flush();

            writer.Dispose();

            return builder.ToString();
        }

        public static Timetable ParseXml(string xml, string timetableFilename, string legacyTimetableNewName)
        {
            XDocument document = XDocument.Parse(xml);
            Timetable timetable;
            if (document.Root.Attribute("Name") == null)
                timetable = new Timetable(legacyTimetableNewName, timetableFilename);
            else
                timetable = new Timetable(document.Root.Attribute("Name").Value, timetableFilename);

            XElement lessonTimesElement = document.Root.Element("LessonTimes");
            int n = 0;
            foreach (XElement e in lessonTimesElement.Elements())
                timetable.LessonTimes.Add(new LessonTime(n++, DateTime.Parse(e.Attribute("Start").Value), DateTime.Parse(e.Attribute("End").Value)));

            XElement subjectsElement = document.Root.Element("Subjects");
            foreach (XElement e in subjectsElement.Elements())
            {
                var color = "";
                if (e.Attribute("Color") != null)
                    color = e.Attribute("Color").Value;

                timetable.Subjects.Add(new Subject(e.Attribute("Name").Value,
                    e.Attribute("Teacher").Value, color));

            }

            XElement homeworksElement = document.Root.Element("Homeworks");
            foreach (XElement e in homeworksElement.Elements())
                timetable.Homeworks.Add(new Homework(timetable.GetSubject(e.Attribute("Subject").Value))
                {
                    FromDate = ConvertHelpers.ParseDate(e.Attribute("From").Value),
                    ToDate = ConvertHelpers.ParseDate(e.Attribute("To").Value),
                    Text = e.Attribute("Description").Value.Replace("\\n", "\n"),
                    IsDone = bool.Parse(e.Attribute("IsDone").Value)
                });

            XElement lessonsElement = document.Root.Element("Lessons");
            foreach (XElement dayLessonElement in lessonsElement.Elements())
            {
                Day day = new Day((DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayLessonElement.Name.LocalName, true));

                int i = 0;
                foreach (XElement e in dayLessonElement.Elements())
                {
                    LessonTime lessonTime = timetable.LessonTimes[i];

                    day.Lessons.Add(new Lesson(lessonTime, timetable, e));

                    i++;
                }

                timetable.Days.Add(day);
            }

            XElement datesElement = document.Root.Element("Dates");
            if (datesElement != null)
            {
                foreach (XElement e in datesElement.Elements())
                    timetable.Dates.Add(new Date(e.Attribute("Name").Value,
                        ConvertHelpers.ParseDate(e.Attribute("Due").Value),
                        (RepeatBehaviour)Enum.Parse(typeof(RepeatBehaviour), e.Attribute("Repeat").Value, true)));
            }

            return timetable;
        }
        #endregion

        #endregion
    }
}
