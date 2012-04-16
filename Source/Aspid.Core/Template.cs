#region License
#endregion

using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System;

using Aspid.Core.Extensions;

namespace Aspid.Core
{
    public class ParseOptions
    {
        readonly static string[] defaultNulls = new string[] { null };
        public readonly static ParseOptions Default = new ParseOptions();

        public bool SwallowNewlineOnEmptyParameter { get; set; }

        public Func<DateTime?, DateTime?> CustomDateConverter { get; set; }

        public bool Trim { get; set; }

        public ParseOptions()
        {
        }

        string[] considerAsNull;
        public string[] ConsiderAsNull 
        {
            get { return (considerAsNull ?? defaultNulls).ToArray(); }
            set { considerAsNull = defaultNulls.Union(value).ToArray(); }
        }
    }

    /// <summary>
    /// Really simple and specialized template engine.
    /// example:
    /// You can mix text {With parameterized $TextProperty}
    /// The first "TextProperty" found on a given object is used to replace $TextProperty
    /// if none it's found or it's value is null, all the text between {} is turned to empty.
    /// </summary>
    public class Template
    {
        static Regex parameter = new Regex(@"(?<parameter>\{(?<argument>.*?)\})(\s*?\n)?", RegexOptions.Singleline | RegexOptions.Compiled);
        static Regex propertyBinding = new Regex(@"\$(?<propertyName>\w*(\.\w+)*)(:\w)?", RegexOptions.Compiled);
        static string[] allowedDateFormats = new string[] { "t", "T", "d", "D", "f", "F", "g", "G", "m", "M", "y", "Y", "r", "R", "u", "s" };

        private Template()
        {
        }

        /// <summary>
        /// Parses the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static string Parse(string text, params object[] args)
        {
            return Parse(text, ParseOptions.Default, args);
        }

        /// <summary>
        /// Parses the specified text with the specified options.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="parseOptions">The parse options.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static string Parse(string text, ParseOptions parseOptions, params object[] args)
        {
            if (parseOptions == null) throw new ArgumentNullException("parseOptions", "parseOptions is null.");

            if (HasParameters(text)) return ProcessParameters(text, parseOptions, args);
            return text;
        }

        private static bool HasParameters(string text)
        {
            return parameter.IsMatch(text);
        }

        private static string ProcessParameters(string text, ParseOptions parseOptions, params object[] args)
        {
            var matches = parameter.Matches(text);
            
            foreach (Match match in matches)
            {
                string argument = match.Groups["argument"].Value;
                string replacementText = argument;
                string textToReplace = match.Groups["parameter"].Value;

                if (HasProperties(argument))
                {
                    string bindedText = BindProperties(argument, parseOptions, args);

                    if (bindedText == null && parseOptions.SwallowNewlineOnEmptyParameter)
                    {
                        textToReplace = match.Value;
                    }

                    replacementText = bindedText;
                }

                text = text.Replace(textToReplace, replacementText);
            }

            return text;
        }

        private static string BindProperties(string argument, ParseOptions parseOptions, object[] args)
        {
            while (HasProperties(argument))
            {
                var firstMatch = propertyBinding.Matches(argument)[0];
                var replaceValue = GetArgumentValue(GetPropertyPath(firstMatch.Value), parseOptions, GetPropertyFormatOptions(firstMatch.Value), args);
                string textBefore = argument.Substring(0, firstMatch.Index);
                string textAfter = argument.Substring(firstMatch.Index + firstMatch.Length);

                if (replaceValue == null) return null;
                argument = textBefore + replaceValue + textAfter;
            }

            return argument;
        }

        private static string GetPropertyPath(string argument)
        {
            return argument.Substring(1).Split(':').First();
        }

        private static bool HasProperties(string argument)
        {
            return propertyBinding.IsMatch(argument);
        }

        private static string GetPropertyFormatOptions(string argument)
        {
            var splitted = argument.Split(':');
            if (splitted.Length == 1) return string.Empty;
            return splitted[1];
        }

        private static string GetArgumentValue(string argumentName, ParseOptions parseOptions, string formatOption, object[] args)
        {
            if (args == null || args.Length == 0) return null;

            string argumentValue = null;
            foreach (var formatAgument in args)
            {
                if (formatAgument == null) continue;
                if (TryGetPropertyValue(formatAgument, argumentName, formatOption, out argumentValue, parseOptions))
                {
                    if (parseOptions.ConsiderAsNull.Contains(argumentValue)) return null;
                    return (parseOptions.Trim && argumentValue != null) ? argumentValue.Trim() : argumentValue;
                }
            }

            return null;
        }

        private static bool TryGetPropertyValue(object formatArgument, 
                                                string argumentName,
                                                string formatOption,
                                                out string argumentValue,
                                                ParseOptions parseOptions)
        {
            argumentValue = null;
            var argumentPath = argumentName.Split('.');

            PropertyInfo property = null;
            //Get nested property value
            foreach (var pathItem in argumentPath)
            {
                var type = formatArgument.GetType();
                property = type.GetProperty(pathItem);
                if (property == null) break;

                formatArgument = property.GetValue(formatArgument, null);
                if (formatArgument == null) break;
            }

            if (property == null) return false;
            if (formatArgument == null) return false;
            
            if (formatArgument is DateTime)
            {
                //Convert date if necessary
                if (parseOptions.CustomDateConverter != null)
                {
                    formatArgument = parseOptions.CustomDateConverter((DateTime)formatArgument);
                }

                if (!formatOption.IsNullOrEmpty() && allowedDateFormats.Contains(formatOption))
                {
                    argumentValue = ((DateTime)formatArgument).ToString(formatOption);
                }
                else
                {
                    argumentValue = formatArgument.ToString();
                }
            }
            else
            {
                argumentValue = formatArgument.ToString();
            }

            return true;
        }
    }
}
