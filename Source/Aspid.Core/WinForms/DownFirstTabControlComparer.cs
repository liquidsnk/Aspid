using System.Windows.Forms;

namespace Aspid.Core.WinForms
{
    public class DownFirstTabControlComparer : TabControlComparer
    {
        public override int Compare(Control control1, Control control2)
        {
            if (control1 == null || control2 == null) return 0;

            // The primary direction to sort is the x direction (using the Left property).
            // If two controls have the same x coordination, then we sort them by their y's.
            if (control1.Left < control2.Left)
            {
                return -1;
            }
            else if (control1.Left > control2.Left)
            {
                return 1;
            }
            else
            {
                return (control1.Top.CompareTo(control2.Top));
            }
        }
    }
}
