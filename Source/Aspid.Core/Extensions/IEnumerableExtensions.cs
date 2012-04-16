#region License
#endregion

using System;
using System.Collections.Generic;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extension methods for IEnumerable interface
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Performs an action or actions over every item of the list.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="actions">The action or list of actions.</param>
        public static void ForEach<TInputType>(this IEnumerable<TInputType> items, params Action<TInputType>[] actions)
        {
            foreach (var item in items)
            {
                item.RunActions(actions);
            }
        }

        private static void RunActions<TInputType>(this TInputType item, params Action<TInputType>[] actions)
        {
            foreach (var action in actions)
            {
                action(item);
            }
        }
    }
}

