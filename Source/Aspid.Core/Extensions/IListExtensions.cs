#region License
#endregion

using System;
using System.Collections.Generic;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extension methods for IList / IList<T> interface
    /// </summary>
    public static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> range)
        {
            list.ThrowIfNull("list");
            range.ThrowIfNull("range");

            foreach (var item in range)
            {
                list.Add(item);
            }
        }
    }
}
