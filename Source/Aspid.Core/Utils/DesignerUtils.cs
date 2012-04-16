#region License
#endregion

using System.Diagnostics;
using System.ComponentModel;
using System;

namespace Aspid.Core.Utils
{
    /// <summary>
    /// Utillity methods to work with the IDE Designer
    /// </summary>
    public static class DesignerUtils
    {
        private const string VisualStudio_Process_Name = "devenv";

        static bool? designMode;
        /// <summary>
        /// Determines whether the process is running in the IDE, in design mode.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the process is in design mode; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInDesignMode()
        {
            try
            {
                if (designMode.HasValue) return designMode.Value;

                var process = Process.GetCurrentProcess();
                if (process == null) return true;
                designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime) ||
                              process.ProcessName == VisualStudio_Process_Name;

                return designMode ?? true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Determines whether the process is in runtime mode.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the process is in runtime mode; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInRuntimeMode()
        {
            return !IsInDesignMode();
        }
    }
}
