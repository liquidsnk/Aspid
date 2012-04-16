using System.Windows.Forms;

namespace Aspid.Core.WinForms
{
    public class AcrossFirstTabControlComparer : TabControlComparer
    {
        public override int Compare(Control control1, Control control2)
        {
            if (control1 == null || control2 == null) return 0;

            // The primary direction to sort is the y direction (using the Top property).
            // If two controls have the same y coordination, then we sort them by their x's.
            if (control1.Top < control2.Top)
            {
                return -1;
            }
            else if (control1.Top > control2.Top)
            {
                return 1;
            }
            else
            {
                return (control1.Left.CompareTo(control2.Left));
            }
        }
    }
}
