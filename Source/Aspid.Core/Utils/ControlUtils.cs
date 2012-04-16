#region License
#endregion

using System;
using System.Windows.Forms;

using Aspid.Core.Extensions;

namespace Aspid.Core.Utils
{
    public static class ControlUtils
    {
        readonly static ILogger logger = Logging.GetLogger("ControlUtils");

        /// <summary>
        /// Invokes the specified action for this control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        public static void Invoke(Control control, Action action)
        {
            control.ThrowIfNull("control");
            action.ThrowIfNull("action");
            if (!control.Created || control.IsDisposed || !control.IsHandleCreated) return;

            try
            {
                control.Invoke(action);
            }
            catch (ObjectDisposedException ex)
            {
                logger.LogError("Control {0} disposed while trying to process an Invoke on it".InvariantFormat(control.Name));
                logger.LogException(ex);
            }
            catch (InvalidOperationException ioe)
            {
                logger.LogError("Invalid operation exception triggered by {0} control while trying to invoke".InvariantFormat(control.Name));
                logger.LogException(ioe);
            }
        }

        /// <summary>
        /// Invokes asynchronously the specified action for this control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        public static void BeginInvoke(Control control, Action action)
        {
            control.ThrowIfNull("control");
            action.ThrowIfNull("action");
            if (!control.Created || control.IsDisposed || !control.IsHandleCreated) return;

            control.BeginInvoke(action);
        }
    }
}
