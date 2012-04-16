#region License
#endregion

using System;
using System.Collections.Generic;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extensions for the random class.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Randomly selects and return an element from the given collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        /// <param name="random">The random object.</param>
        /// <param name="collection">The collection.</param>
        /// <returns>A random element of the collection</returns>
        public static T PickOne<T>(this Random random, IList<T> collection)
        {
            collection.ThrowIfNull("collection");

            return collection[random.Next(0, collection.Count)];
        }
    }
}
