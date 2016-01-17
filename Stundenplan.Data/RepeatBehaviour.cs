using System;

namespace Stundenplan.Data
{
    /// <summary>
    /// Verhalten eines Ereignisses (e.g Date), das sich regelmäßig wiederholt.
    /// </summary>
    public enum RepeatBehaviour
    {
        /// <summary>
        /// Das Ereignis ist einmalig und wiederholt sich nicht.
        /// </summary>
        None,
        /// <summary>
        /// Das Ereignis wiederholt sich jede Woche
        /// </summary>
        Weekly,
        /// <summary>
        /// Das Ereignis wiederholt sich jeden Monat.
        /// </summary>
        Monthly
    }
}
