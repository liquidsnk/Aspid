#region License
#endregion

using System;
using System.Linq.Expressions;

using Aspid.Core.Utils;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extension methods for Object class
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Throws an ArgumentNullException if the object is null.
        /// </summary>
        /// <param name="paramName">Name of the parameter that's being checked.</param>
        public static void ThrowIfNull(this object value, string parameterName) 
        {
            ObjectUtils.ThrowIfNull(value, parameterName);
        }

        /// <summary>
        /// Throws an ArgumentNullException if the object is null.
        /// </summary>
        /// <param name="paramName">Name of the parameter that's being checked.</param>
        /// <param name="message">Custom message for the exception.</param>
        public static void ThrowIfNull(this object value, string parameterName, string message)
        {
            ObjectUtils.ThrowIfNull(value, parameterName, message);
        }

        /// <summary>
        /// Performs a ToString on the object.
        /// If the object is null, returns the empty string.
        /// It also ensures that the string returned by ToString is empty if null.
        /// </summary>
        public static string ToStringOrEmpty(this object value)
        {
            return ObjectUtils.ToStringOrEmpty(value);
        }

        /// <summary>
        /// Returns the result of Object.Equals(value, other).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public static bool ObjectEquals(this object value, object other)
        {
            return object.Equals(value, other);
        }

        /// <summary>
        /// Navigates a path through the current object, represented by a lambda.
        /// While navigating the path if it encounters a null value, the end result will be the default()
        /// value for the return type of the lambda.
        /// Basically: It provides a way to avoid null checks on path traversals.
        /// It may not work for a complex pathExpression, so beware.
        /// </summary>
        /// <typeparam name="T">The type of the current object</typeparam>
        /// <typeparam name="K">The type of the result of the pathEcpression lambda</typeparam>
        /// <param name="self">The current object to navigate</param>
        /// <param name="pathExpression">
        /// The lambda expression that represents the path traversal.
        /// It must start at the given lambda paramenter and may go through fields, properties and methods with constant parameters.
        /// </param>
        /// <returns>The value returned by the path traversal, or the default value for the K type if any intermediate result is null</returns>
        public static K SafelyNavigate<T, K>(this T self, Expression<Func<T, K>> pathExpression)
            where T : class
        {
            return ObjectUtils.SafelyNavigate(self, pathExpression);
        }
    }
}
