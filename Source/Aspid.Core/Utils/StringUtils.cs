#region License
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using Aspid.Core.Extensions;

namespace Aspid.Core.Utils
{
    public static class StringUtils
    {
        /// <summary>
        /// Throws an ArgumentNullException if the string is null or ArgumentException if it's empty.
        /// </summary>
        /// <param name="paramName">Name of the parameter that's being checked.</param>
        public static void ThrowIfNullOrEmpty(string value, string parameterName)
        {
            value.ThrowIfNull(parameterName);
            if (value.IsNullOrEmpty()) { throw new ArgumentException(parameterName); }
        }

        /// <summary>
        /// Throws an ArgumentNullException if the object is null.
        /// </summary>
        /// <param name="paramName">Name of the parameter that's being checked.</param>
        /// <param name="message">Custom message for the exception.</param>
        public static void ThrowIfNullOrEmpty(string value, string parameterName, string message)
        {
            value.ThrowIfNull(parameterName, message);
            if (value.IsNullOrEmpty()) { throw new ArgumentException(parameterName, message); }
        }

        /// <summary>
        /// Applies string.Format using this string as format and InvariantCulture.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <returns>The formatted string using InvariantCulture</returns>
        public static string InvariantFormat(string format, params object[] args)
        {
            format.ThrowIfNull("format");

            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// Deletes all the characters from the given string beginning at a specified position and continuing through the last position.
        /// If the string Length is equal or smaller than the provided startIndex, the original string is returned.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        public static string SafeRemove(string text, int startIndex)
        {
            text.ThrowIfNull("text");

            return (text.Length > startIndex) ? text.Remove(startIndex) : text;
        }

        public static string SafeTrim(string text)
        {
            if (text == null) return null;
            return text.Trim();
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
        public static bool ContainsAny(string text, IEnumerable<string> substrings)
        {
            substrings.ThrowIfNull("substrings");

            //Only check for null here, since the empty string contains the empty string
            if (text == null) return false;
                        
            bool atLeastOneElement = false;
            foreach (var str in substrings)
            {
                atLeastOneElement = true;
                if (text.Contains(str)) return true;
            }

            //If there are no elements on the substrings enumeration, return true.
            return !atLeastOneElement;
        }

        static readonly Regex camelCaseSeparation = new Regex(@"([A-Z0-9]+(?=$|[A-Z0-9][a-z])|[A-Z0-9]?[a-z]+)", RegexOptions.Compiled);
        /// <summary>
        /// Splits the given string by case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static IList<string> CaseSplit(string text)
        {
            return camelCaseSeparation.Matches(text)
                                      .Cast<Match>()
                                      .Select(x => x.Value)
                                      .Where(x => !string.IsNullOrEmpty(x))
                                      .ToList();
        }

        /// <summary>
        /// Separates the given string by case, using a space as the separator.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CaseSeparate(string text)
        {
            return CaseSeparate(text, " ");
        }

        /// <summary>
        /// Separates the given string by case, using the given separator.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        public static string CaseSeparate(string text, string separator)
        {
            return separator.Join(CaseSplit(text));
        }

        /// <summary>
        /// Searches the given string for the given value and returns it if found.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string SubstringSearch(string text, string value)
        {
            return text.SubstringSearch(value, null);
        }

        public static string SubstringSearch(string text, string value, char[] charsToIgnore)
        {
            if (text.IsNullOrEmpty() || value.IsNullOrEmpty()) return string.Empty;

            //No ignored chars
            if (charsToIgnore == null || charsToIgnore.Length == 0)
            {
                var substringIndex = text.IndexOf(value, StringComparison.InvariantCultureIgnoreCase);
                if (substringIndex < 0) return string.Empty;
                return text.Substring(substringIndex, value.Length);
            }
            else
            {
                //Use regex when there are ignored chars
                var regex = BuildSubstringSearchRegex(value, charsToIgnore);
                var match = regex.Match(text);
                return match.Success ? match.Value : string.Empty;
            }
        }

        private static Regex BuildSubstringSearchRegex(string value, char[] charsToIgnore)
        {
            const string ignorePattern = "[{0}]*?";

            var ignoreString = string.Format(ignorePattern, "".Join(charsToIgnore.Select(x => SanitizeCharForRegex(x))));

            var regexString = new StringBuilder();
            foreach (var character in value)
            {
                regexString.Append(SanitizeCharForRegex(character) + ignoreString);
            }

            return new Regex(regexString.ToString(), RegexOptions.IgnoreCase);
        }

        private static string SanitizeCharForRegex(char character)
        {
            //escape the dash
            if (character == '[') return @"\[";
            if (character == ']') return @"\]";
            if (character == '-') return @"\-";
            return Regex.Escape(character.ToString());
        }
        
        /// <summary>
        /// Capitalizes the specified string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string Capitalize(string text)
        {
            const string separator = " ";
            if (text.IsNullOrEmpty()) return string.Empty;

            var words = text.Split(separator[0]);

            for (int i = 0; i < words.Length; i++)
            {
                var word = words[i];
                string capitalizedWord = word.Length <= 1 ?
                                         word.ToUpper() :
                                         word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();

                words[i] = capitalizedWord;
            }

            return separator.Join(words);
        }
    }
}
