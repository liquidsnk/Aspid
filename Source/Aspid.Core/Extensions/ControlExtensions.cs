#region License
#endregion

using System;
using System.Windows.Forms;

using Aspid.Core.Utils;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extension methods for Control class
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// Invokes the specified action for this control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        public static void Invoke(this Control control, Action action)
        {
            ControlUtils.Invoke(control, action);
        }

        /// <summary>
        /// Invokes asynchronously the specified action for this control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        public static void BeginInvoke(this Control control, Action action)
        {
            ControlUtils.BeginInvoke(control, action);
        }
    }
}
