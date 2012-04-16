using System.Collections.Generic;
using System.Windows.Forms;

namespace Aspid.Core.WinForms
{
    public abstract class TabControlComparer : IComparer<Control>
    {
        internal class NonComparingTabControlComparer : TabControlComparer
        {
            public override int Compare(Control control1, Control control2)
            {
                if (control1 == null || control2 == null) return 0;

                // Nothing to do.
                return 0;
            }
        }

        public abstract int Compare(Control x, Control y);

        public static TabControlComparer None { get { return new NonComparingTabControlComparer(); } }

        public static TabControlComparer AcrossFirst { get { return new AcrossFirstTabControlComparer(); } }

        public static TabControlComparer DownFirst { get { return new DownFirstTabControlComparer(); } }
    }
}
