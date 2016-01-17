using System;

namespace Stundenplan.Localization
{
    /// <summary>
    /// Hilfsklasse um Strings aus Resourcendateien im XAML zu laden.
    /// </summary>
    public class XamlStrings
    {
        private static Strings strings = new Strings();

        public Strings Strings { get { return strings; } }
    }
}
