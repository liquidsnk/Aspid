#region License
#endregion

using System;
using System.Collections;

using Aspid.Core.Utils;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extension methods for ICollection interface
    /// </summary>
    public static class ICollectionExtensions
    {
        /// <summary>
        /// Determines whether the specified collection is null or empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// 	<c>true</c> if the specified collection is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this ICollection collection)
        {
            return CollectionUtils.IsNullOrEmpty(collection);
        }
    }
}

