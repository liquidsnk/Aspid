#region License
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Aspid.Core.Extensions;

namespace Aspid.Core.Utils
{
    /// <summary>
    /// Static utility methods that work on enums.
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// Gets the list of <see cref="Aspid.Core.EnumElement&lt;T&gt;"/> for the given enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration</typeparam>
        /// <param name="exceptValues">The enumeration values to exclude from the returned list.</param>
        /// <returns></returns>
        public static IList<EnumElement<T>> GetEnumElements<T>(params T[] exceptValues) where T : struct
        {
            var enumElementList = new List<EnumElement<T>>();

            foreach (var element in EnumUtils.GetElements<T>().Except(exceptValues))
            {
                enumElementList.Add(GetEnumElement((T)element));
            }

            return enumElementList;
        }

        /// <summary>
        /// Gets the <see cref="Aspid.Core.EnumElement&lt;T&gt;"/> corresponding to the given element of the enum.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration</typeparam>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static EnumElement<T> GetEnumElement<T>(T element) where T : struct
        {
            return new EnumElement<T>(element, GetElementDescription(element));
        }

        /// <summary>
        /// Gets the element description for the given element of the enumeration.
        /// If the element is a flag the ", " separator is used to return the contained element descriptions list.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration</typeparam>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static string GetElementDescription<T>(T element) where T : struct
        {
            return GetElementDescription(element, ", ");
        }

        /// <summary>
        /// Gets the element description for the given element of the enumeration.
        /// If the element is a flag the given separator is used to return the contained element descriptions list.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration</typeparam>
        /// <param name="element">The element.</param>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        public static string GetElementDescription<T>(T element, string separator) where T : struct
        {
            if (IsFlagsEnum(typeof(T)))
            {
                //If flag represents the 0 value ("None" element), return the description for that element
                var descriptions = new List<string>();
                long longFlags = GetElementValue(element);
                if ((longFlags) == 0) return GetSingleElementDescription(element);

                //if flag represents more than a value, form the concatenated descriptions
                foreach (var enumElement in GetElements<T>())
                {
                    long longElement = GetElementValue(enumElement);
                    
                    //ignore the "None" element of enums, the element with value 0 is always contained by a flag
                    if (longElement == 0) continue;

                    if (ContainsElement(longFlags, longElement))
                    {
                        descriptions.Add(GetSingleElementDescription(enumElement));
                    }
                }

                return separator.Join(descriptions);
            }
            else
            {
                return GetSingleElementDescription(element);
            }
        }

        /// <summary>
        /// Determines whether the specified enum type is flag enum.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns>
        /// 	<c>true</c> if the specified enum type is a flag enum; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFlag(Type enumType)
        {
            return IsFlagsEnum(enumType);
        }

        /// <summary>
        /// Determines whether the specified flag contains the given element.
        /// The element with value 0 is always contained by the flag.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="flag">The flag.</param>
        /// <param name="element">The element.</param>
        /// <returns>
        /// 	<c>true</c> if the specified flag contains element; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsElement<T>(T flag, T element) where T : struct
        {
            return ContainsElement(GetElementValue(flag), GetElementValue(element));
        }

        /// <summary>
        /// Determines whether the specified flag contains the given element.
        /// The 0 value is always contained by the flag.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <param name="element">The element.</param>
        /// <returns>
        /// 	<c>true</c> if the specified flag contains element; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsElement(long flag, long element)
        {
            return ((flag & element) == element);
        }

        /// <summary>
        /// Gets a list formed by the elements of the given enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetElements<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Gets the element value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration</typeparam>
        /// <param name="enumElement">The enum element.</param>
        /// <returns></returns>
        public static long GetElementValue<T>(T enumElement) where T : struct
        {
            return Convert.ToInt64(enumElement);
        }

        private static string GetSingleElementDescription<T>(T element) where T : struct
        {
            string description;
            if (TryGetDescriptionAttributeValue(element, out description))
            {
                return description;
            }

            return element.ToStringOrEmpty().CaseSeparate();
        }

        private static bool IsFlagsEnum(Type enumType)
        {
            var flagAttribute = (FlagsAttribute)Attribute.GetCustomAttribute(enumType, typeof(FlagsAttribute));
            return (flagAttribute != null);
        }

        private static bool TryGetDescriptionAttributeValue<T>(T enumElement, out string description) where T : struct
        {
            var enumType = typeof(T);
            var name = Enum.GetName(enumType, enumElement);
            description = string.Empty;

            if (name == null) return false;
            var fieldInfo = enumType.GetField(name);
            var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));

            if (descriptionAttribute != null)
            {
                description = descriptionAttribute.Description;
                return true;
            }

            return false;
        }
    }
}
