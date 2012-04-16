#region License
#endregion

using System;

namespace Aspid.Core
{
    [Flags]
    public enum DayOfWeekFlag
    {
        None =      0,
        Sunday =    1,
        Monday =    1 << 1,
        Tuesday =   1 << 2,
        Wednesday = 1 << 3,
        Thursday =  1 << 4,
        Friday =    1 << 5,
        Saturday =  1 << 6,
        All = Sunday | Monday | Tuesday | Wednesday | Thursday | Friday | Saturday,
    }
}
