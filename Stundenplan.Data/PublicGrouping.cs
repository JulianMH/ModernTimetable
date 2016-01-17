using System;
using System.Linq;
using System.Collections.Generic;

namespace Stundenplan.Data
{
    /// <summary>
    /// A class used to expose the Key property on a dynamically-created Linq grouping.
    /// The grouping will be generated as an internal class, so the Key property will not
    /// otherwise be available to databind.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TElement">The type of the items.</typeparam>
    public class PublicGrouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
    {
        private TKey key;

        public PublicGrouping(IGrouping<TKey, TElement> internalGrouping)
            : base(internalGrouping)
        {
            this.key = internalGrouping.Key;
        }

        public override bool Equals(object obj)
        {
            PublicGrouping<TKey, TElement> that = obj as PublicGrouping<TKey, TElement>;

            return (that != null) && (this.Key.Equals(that.Key));
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #region IGrouping<TKey,TElement> Members

        public TKey Key
        {
            get { return key; }
        }

        #endregion
    }
}
