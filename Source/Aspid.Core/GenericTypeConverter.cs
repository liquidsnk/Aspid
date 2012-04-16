#region License
#endregion

using System;
using System.ComponentModel;

namespace Aspid.Core
{
    /// <summary>
    /// Generic helper methods to use on type conversions
    /// </summary>
    public static class GenericTypeConverter
    {
        /// <summary>
        /// Changes the type of the value for the given one by using pre-configured TypeConverters.
        /// </summary>
        /// <typeparam name="T">The new type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ChangeType<T>(object value)
        {
            return (T)ChangeType(typeof(T), value);
        }

        /// <summary>
        /// Changes the type of the value for the given one by using pre-configured TypeConverters.
        /// </summary>
        /// <param name="type">The new type.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object ChangeType(Type type, object value)
        {
            try
            {
                if (value == null || value == DBNull.Value) return null;

                if (type.IsAssignableFrom(value.GetType()))
                {
                    return value;
                }

                if (value is IConvertible)
                {
                    try
                    {
                        return Convert.ChangeType(value, type);
                    }
                    catch (InvalidCastException)
                    {
                    }
                }

                TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
                return typeConverter.ConvertFrom(value);
            }
            catch (Exception ex)
            {
                throw new FormatException(string.Format("Unable to convert the given value {0} to the requested type {1}", value, type), ex);
            }
        }

        /// <summary>
        /// Registers the given type converter type, for the given type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <typeparam name="TC">The type of the type converter.</typeparam>
        public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {
            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
        }
    }
}
