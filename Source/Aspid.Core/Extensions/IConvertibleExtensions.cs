#region License
#endregion

using System;
using System.Globalization;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extension methods for IConvertible interface
    /// </summary>
    public static class IConvertibleExtensions
    {
        /// <summary>
        /// Converts the item to string using the ToString method and InvariantCulture.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The string representation of the item using InvariantCulture</returns>
        public static string ToInvariantString(this IConvertible item)
        {
            return item.ToString(CultureInfo.InvariantCulture);
        }
    }
}
