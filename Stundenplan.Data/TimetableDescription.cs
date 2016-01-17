using System;
using System.Xml.Linq;
using System.IO;

namespace Stundenplan.Data
{
    /// <summary>
    /// Beschreibt einige Eigenschaften eines Stundenplans. Ermöglicht es Informationen über diesen zu erhalten ohne ihn komplett zu lesen.
    /// </summary>
    public sealed class TimetableDescription
    {
        /// <summary>
        /// Dateiname des Stundenplans
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// Name des Stundenplans
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Anzahl zu erledigender Hausaufgaben
        /// </summary>
        public int HomeworksCount { get; private set; }

        /// <summary>
        /// Anzahl verbleibender Tagesnotizen.
        /// </summary>
        public int DatesCount { get; private set; }

        public TimetableDescription(string filename, string name, int homeworksCount, int datesCount)
        {
            this.FileName = filename;
            this.Name = name;
            this.HomeworksCount = homeworksCount;
            this.DatesCount = datesCount;
        }

        public TimetableDescription(string fileContent, string filename, string noName, string invalidName)
        {
            this.FileName = filename;
            try
            {
                XDocument document = XDocument.Parse(fileContent);

                XAttribute nameAttribute = document.Root.Attribute("Name");
                if (nameAttribute != null)
                    this.Name = nameAttribute.Value;
                else
                    this.Name = noName;

                int homeworksCount = 0;
                XElement homeworksElement = document.Root.Element("Homeworks");
                foreach (XElement e in homeworksElement.Elements())
                {
                    if (bool.Parse(e.Attribute("IsDone").Value) == false)
                        homeworksCount++;
                }
                this.HomeworksCount = homeworksCount;

                int datesCount = 0;
                XElement datesElement = document.Root.Element("Dates");
                if (datesElement != null)
                {
                    foreach (XElement e in datesElement.Elements())
                    {
                        RepeatBehaviour repeatBehaviour = (RepeatBehaviour)Enum.Parse(typeof(RepeatBehaviour), e.Attribute("Repeat").Value, true);

                        DateTime date = ConvertHelpers.ParseDate(e.Attribute("Due").Value);

                        if (!(repeatBehaviour == RepeatBehaviour.None && date < DateTime.Today))
                            datesCount++;

                    }
                }
                this.DatesCount = datesCount;
            }
            catch
            {
                this.Name = invalidName;
            }
        }
    }
}
