#region License
#endregion

using System;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extension methods for Nullable<T> class
    /// </summary>
    public static class NullableExtensions
    {
        /// <summary>
        /// If the Nullable<T> is null return default(TReturn), otherwise it returns what's obtained by calling the function ifNotNull.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nullable">The nullable.</param>
        /// <param name="ifNotNull">The function that generates the return value if the nullable is not null.</param>
        /// <returns></returns>
        public static TReturn NullOr<TNullable, TReturn>(this Nullable<TNullable> nullable, Func<TReturn> ifNotNull) where TNullable : struct
        {
            if (!nullable.HasValue) return default(TReturn);
            return ifNotNull();
        }

        /// <summary>
        /// If the Nullable<T> is null return default(TReturn), otherwise it returns what's obtained by calling the function ifNotNull.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nullable">The nullable.</param>
        /// <param name="ifNotNull">The function that generates the return value if the nullable is not null.</param>
        /// <returns></returns>
        public static TReturn NullOr<TNullable, TReturn>(this Nullable<TNullable> nullable, Func<TNullable, TReturn> ifNotNull) where TNullable : struct
        {
            if (!nullable.HasValue) return default(TReturn);
            return ifNotNull(nullable.Value);
        }
    }
}
