#region License
#endregion

using System;
using System.Globalization;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extension methods for IFormattable interface
    /// </summary>
    public static class IFormattableExtensions
    {
        /// <summary>
        /// Converts the item to string using the ToString method, the format and InvariantCulture.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="format">The format.</param>
        /// <returns>
        /// The string representation of the item using format InvariantCulture.
        /// </returns>
        public static string ToInvariantString(this IFormattable item, string format)
        {
            return item.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
