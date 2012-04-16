#region License
#endregion

using System;
using System.Windows.Forms;
using System.Drawing;

namespace Aspid.Core.Extensions
{
    public static class FormExtensions
    {
        /// <summary>
        /// Sets the size of the form to a 90% of the Working area of the active screen.
        /// and centers the form to the screen.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        public static void SetScreeRelativeSize(this Form form)
        {
            int workingHeigth = Screen.FromControl(form).WorkingArea.Height;
            int workinWidth = Screen.FromControl(form).WorkingArea.Width;

            double left = Math.Round(workinWidth * 0.1 / 2);
            double top = Math.Round(workingHeigth * 0.1 / 2);

            form.Width = (int)Math.Floor(workinWidth * 0.9);
            form.Height = (int)Math.Floor(workingHeigth * 0.9);

            form.Location = new Point((int)left, (int)top);
        }

        /// <summary>
        /// Sets the size of the form to a 'percent' of the Working area of the active screen.
        /// and centers the form to the screen.
        /// Percent must be given as a number between 0 and 1
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        public static void SetScreeRelativeSize(this Form form, double percent)
        {
            form.ThrowIfNull("form cannot be null");
            if (percent > 1 || percent < 0) throw new ArgumentOutOfRangeException("percent must be between 0 and 1");

            int workingHeigth = Screen.FromControl(form).WorkingArea.Height;
            int workinWidth = Screen.FromControl(form).WorkingArea.Width;

            double left = Math.Round(workinWidth * (1 - percent) / 2);
            double top = Math.Round(workingHeigth * (1 - percent) / 2);

            form.Width = (int)Math.Floor(workinWidth * percent);
            form.Height = (int)Math.Floor(workingHeigth * percent);

            form.Location = new Point((int)left, (int)top);
        }
    }
}
