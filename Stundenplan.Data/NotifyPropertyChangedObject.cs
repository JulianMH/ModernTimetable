using System;
using System.ComponentModel;

namespace Stundenplan.Data
{
    /// <summary>
    /// Abstrakte Basisklasse für jedes Object, welches das interface INotifyPropertyChanged implementieren will.
    /// Spart Codezeilen :)
    /// </summary>
    public abstract class NotifyPropertyChangedObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Event, tritt auf wenn eine Eigenschaft dieses Objektes sich ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Feuere das Event PropertyChanged, sofern dieses gültig ist.
        /// </summary>
        /// <param name="property">Die Eigenschaft, die sich geändert hat</param>
        protected void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
