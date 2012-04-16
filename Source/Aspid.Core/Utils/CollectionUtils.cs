#region License
#endregion

using System.Collections;

namespace Aspid.Core.Utils
{
    public static class CollectionUtils
    {
        /// <summary>
        /// Determines whether the specified collection is null or empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// 	<c>true</c> if the specified collection is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(ICollection collection)
        {
            if (collection == null) return true;
            return (collection.Count <= 0);
        }
    }
}
