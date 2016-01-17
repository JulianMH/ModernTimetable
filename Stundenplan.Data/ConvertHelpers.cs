using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace Stundenplan.Data
{
    /// <summary>
    /// Stellt einige allgemeine statische Helfermehoden zum Umgang mit Wochentagen zur Verfügung.
    /// </summary>
    public static class ConvertHelpers
    {
        /// <summary>
        /// Liefert zurück, welcher Wochentag heute ist, bzw. wenn kein Werktag ist welches der nächste ist.
        /// </summary>
        /// <param name="isSixDayWeek">Wenn dieser Wert true ist, zählt Samstag als Wochentag</param>
        /// <returns>Wochentag als DayOfWeek-Wert</returns>
        public static DayOfWeek GetCurrentWorkDayOfWeek(IEnumerable<DayOfWeek> days)
        {
            DateTime now = DateTime.Now;
            while (!days.Contains(now.DayOfWeek))
            {
                now = now.Add(new TimeSpan(1, 0, 0, 0));
            }
            return now.DayOfWeek;
        }

        /// <summary>
        /// Wandelt einen deutschen Datums-String in einen DateTime-Wert um.
        /// </summary>
        /// <param name="input">Datums-String im Format: TT.MM.YYYY</param>
        /// <returns>DateTime-Wert, der erstellt wurde</returns>
        public static DateTime ParseDate(string input)
        {
            //Muss dies Lokalisiert werden? Nein ich entscheide wie Dateien Formatiert werden
            string[] split = input.Split('.');
            return new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0])).Date;
        }

        /// <summary>
        /// Liefert eine Liste aller Wochentage zurück
        /// </summary>
        /// <returns>Liste aller Wochentage</returns>
        public static List<DayOfWeek> GetFullWeekDays()
        {
            return new List<DayOfWeek>()
            {
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            };
        }

        /// <summary>
        /// Bestimmt anhand eines Datums, ob es sich um eine ungerade Woche handelt.
        /// </summary>
        /// <param name="date">Das Datum, anhand dessen bestimmt wird, ob es sich um eine gerade oder ungerade Woche handelt.</param>
        public static bool IsOddWeek(DateTime date)
        {
            return ((GetWeekNumber(date) % 2) == 1);
        }

        /// <summary>
        /// Ermittelt die Kalenderwoche in der sich das angegebene Datum befindet.
        /// </summary>
        /// <param name="date">Das Datum, dessen Kalenderwoch bestimmt werden soll.</param>
        /// <returns>Nummer der Kalenderwoche.</returns>
        public static int GetWeekNumber(DateTime date)
        {
            //Code für gerade/ungerade Wochen: 
            //http://stackoverflow.com/questions/4236725/c-to-which-week-does-the-date-fall-even-or-odd-given-the-start-and-end-of-ses
            //Neu
            //http://stackoverflow.com/questions/1497586/how-can-i-calculate-find-the-week-number-of-a-given-date
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
