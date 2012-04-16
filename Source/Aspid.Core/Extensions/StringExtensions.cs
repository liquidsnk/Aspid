#region License
#endregion

using System.Collections.Generic;
using System.Linq;

using Aspid.Core.Utils;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extension methods for String class
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Throws an ArgumentNullException if the string is null, or an ArgumentException if its empty.
        /// </summary>
        /// <param name="paramName">Name of the parameter that's being checked.</param>
        public static void ThrowIfNullOrEmpty(this string value, string parameterName)
        {
            StringUtils.ThrowIfNullOrEmpty(value, parameterName);
        }

        /// <summary>
        /// Throws an ArgumentNullException if the string is null, or an ArgumentException if its empty.
        /// </summary>
        /// <param name="paramName">Name of the parameter that's being checked.</param>
        /// <param name="message">Custom message for the exception.</param>
        public static void ThrowIfNullOrEmpty(this string value, string parameterName, string message)
        {
            StringUtils.ThrowIfNullOrEmpty(value, parameterName, message);
        }

        /// <summary>
        /// Applies string.Format using this string as format and InvariantCulture.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <returns>The formatted string using InvariantCulture</returns>
        public static string InvariantFormat(this string format, params object[] args)
        {
            return StringUtils.InvariantFormat(format, args);
        }

        /// <summary>
        /// Deletes all the characters from this string beginning at a specified position and continuing through the last position.
        /// If the string Length is equal or smaller than the provided startIndex, the original string is returned.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        public static string SafeRemove(this string text, int startIndex)
        {
            return StringUtils.SafeRemove(text, startIndex);
        }

        /// <summary>
        /// Trims the given string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string SafeTrim(this string text)
        {
            return StringUtils.SafeTrim(text);
        }

        /// <summary>
        /// Determines whether the specified string contains any of the given substrings.
        /// </summary>
        /// <typeparam name="TInputType">The type of the input type.</typeparam>
        /// <param name="text">The text.</param>
        /// <param name="items">The substrings.</param>
        /// <returns>
        /// 	<c>true</c> if the specified list contains any of the given substrings; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAny(this string text, IEnumerable<string> substrings)
        {
            return StringUtils.ContainsAny(text, substrings);
        }
        
        /// <summary>
        /// Determines whether the specified text is null or empty.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the specified text is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// Returns null if the specified string is empty, otherwise, returns the specified string.
        /// </summary>
        /// <param name="text">The specified string.</param>
        /// <returns></returns>
        public static string EmptyToNull(this string text)
        {
            if (!string.IsNullOrEmpty(text)) return text;
            return null;
        }

        /// <summary>
        /// Joins the given list of strings with the specified separator.
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static string Join(this string separator, IEnumerable<string> values)
        {
            if (values == null) return string.Empty;

            separator = separator ?? string.Empty;
            return string.Join(separator, values.ToArray());
        }

        public static IList<string> CaseSplit(this string text)
        {
            return StringUtils.CaseSplit(text);
        }

        public static string CaseSeparate(this string text)
        {
            return StringUtils.CaseSeparate(text);
        }

        public static string CaseSeparate(this string text, string separator)
        {
            return StringUtils.CaseSeparate(text, separator);
        }

        public static string SubstringSearch(this string text, string value)
        {
            return StringUtils.SubstringSearch(text, value);
        }

        public static string SubstringSearch(this string text, string value, char[] charsToIgnore)
        {
            return StringUtils.SubstringSearch(text, value, charsToIgnore);
        }

        public static string Capitalize(this string text)
        {
            return StringUtils.Capitalize(text);
        }
    }
}
