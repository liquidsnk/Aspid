#region License
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;

using Aspid.Core.Extensions;

namespace Aspid.Core
{
    /// <summary>
    /// Convenience class used to create tuples
    /// </summary>
    public static class Tuple
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Tuple&lt;T, K&gt;"/> class.
        /// </summary>
        /// <param name="firstItem">The first item.</param>
        /// <param name="secondItem">The second item.</param>
        public static Tuple<TFirst, TSecond> FromItems<TFirst, TSecond>(TFirst firstItem, TSecond secondItem)
        {
            return new Tuple<TFirst, TSecond>(firstItem, secondItem);
        }

        /// <summary>
        /// Returns a list of tuples formed from the items on the supplied lists.
        /// If the lists are of diffrent lenghts, the returned tuple enumeration will have as many items as the longest list,
        /// returning default values for the item corresponding to the shortest list when the shortest list it's finished.
        /// </summary>
        /// <param name="firstListOfItems">The first list of items.</param>
        /// <param name="secondListOfItems">The second list of items.</param>
        /// <returns>A list of tuples formed from items of both lists</returns>
        public static IEnumerable<Tuple<TFirst, TSecond>> FromLists<TFirst, TSecond>(IEnumerable<TFirst> firstListOfItems, IEnumerable<TSecond> secondListOfItems)
        {
            firstListOfItems.ThrowIfNull("firstListOfItems");
            secondListOfItems.ThrowIfNull("secondListOfItems");

            var firstEnumerator = firstListOfItems.GetEnumerator();
            var secondEnumerator = secondListOfItems.GetEnumerator();

            bool firstListHaveItems = firstEnumerator.MoveNext();
            bool secondListHaveItems = secondEnumerator.MoveNext();

            while (firstListHaveItems || secondListHaveItems)
            {
                yield return new Tuple<TFirst, TSecond>(firstListHaveItems ? firstEnumerator.Current : default(TFirst),
                                             secondListHaveItems ? secondEnumerator.Current : default(TSecond));

                firstListHaveItems = firstEnumerator.MoveNext();
                secondListHaveItems = secondEnumerator.MoveNext();
            }
        }
    }

    /// <summary>
    /// Represents a pair of items
    /// </summary>
    /// <typeparam name="T">Type for the first item of the pair</typeparam>
    /// <typeparam name="K">Type for the second item of the pair</typeparam>
    [Serializable]
    public class Tuple<TFirst, TSecond> : IEquatable<Tuple<TFirst, TSecond>>
    {
        /// <summary>
        /// Gets the first item of the pair.
        /// </summary>
        /// <value>The first item.</value>
        public TFirst FirstItem { get; private set; }

        /// <summary>
        /// Gets the second item of the pair.
        /// </summary>
        /// <value>The second item.</value>
        public TSecond SecondItem { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;T, K&gt;"/> class.
        /// </summary>
        /// <param name="firstItem">The first item.</param>
        /// <param name="secondItem">The second item.</param>
        public Tuple(TFirst firstItem, TSecond secondItem)
        {
            FirstItem = firstItem;
            SecondItem = secondItem;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (obj == null) return base.Equals(obj);
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Tuple<TFirst, TSecond>)) return false;
            var other = (Tuple<TFirst, TSecond>)obj;

            return Equals(FirstItem, other.FirstItem) &&
                   Equals(SecondItem, other.SecondItem);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Tuple<TFirst, TSecond> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(FirstItem, other.FirstItem) &&
                   Equals(SecondItem, other.SecondItem);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            const int _seedPrimeNumber = 691;
            const int _fieldPrimeNumber = 397;

            var result = _seedPrimeNumber;
            unchecked
            {
                result = (result * _fieldPrimeNumber) ^ (FirstItem != null ? FirstItem.GetHashCode() : 0);
                result = (result * _fieldPrimeNumber) ^ (SecondItem != null ? SecondItem.GetHashCode() : 0);
            }
            return result;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1})", FirstItem, SecondItem);
        }
    }
}
