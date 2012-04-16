#region License
#endregion

using System;
using System.Collections.Generic;

namespace Aspid.Core.Utils
{
    /// <summary>
    /// Static utility methods that work on DayOfWeekFlag enumeration.
    /// </summary>
    public static class DayOfWeekFlagUtils
    {
        /// <summary>
        /// Determines whether the specified days flag contains the given day.
        /// </summary>
        /// <param name="daysFlag">The days flag.</param>
        /// <param name="day">The day.</param>
        /// <returns>
        /// 	<c>true</c> if the specified days flag contains the given day; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsDay(DayOfWeekFlag daysFlag, DayOfWeek day)
        {
            return ContainsDays(daysFlag, day);
        }

        /// <summary>
        /// Determines whether the specified days flag contains the given days.
        /// </summary>
        /// <param name="daysFlag">The days flag.</param>
        /// <param name="days">The days.</param>
        /// <returns>
        /// 	<c>true</c> if the specified days flag contains the given days; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsDays(DayOfWeekFlag daysFlag, params DayOfWeek[] days)
        {
            if (daysFlag == DayOfWeekFlag.None) return false;
            var givenDaysFlag = FromDaysOfWeek(days);
            return EnumUtils.ContainsElement(daysFlag, givenDaysFlag);
        }

        /// <summary>
        /// Creates a DayOfWeekFlag from the given days of week.
        /// </summary>
        /// <param name="days">The days.</param>
        /// <returns></returns>
        public static DayOfWeekFlag FromDaysOfWeek(params DayOfWeek[] days)
        {
            var daysFlag = DayOfWeekFlag.None;

            foreach (var day in days)
            {
                daysFlag = daysFlag | FromDayOfWeek(day);
            }

            return daysFlag;
        }

        /// <summary>
        /// Creates a DayOfWeekFlag from the given day of week.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        public static DayOfWeekFlag FromDayOfWeek(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Friday:
                    return DayOfWeekFlag.Friday;
                case DayOfWeek.Monday:
                    return DayOfWeekFlag.Monday;
                case DayOfWeek.Saturday:
                    return DayOfWeekFlag.Saturday;
                case DayOfWeek.Sunday:
                    return DayOfWeekFlag.Sunday;
                case DayOfWeek.Thursday:
                    return DayOfWeekFlag.Thursday;
                case DayOfWeek.Tuesday:
                    return DayOfWeekFlag.Tuesday;
                case DayOfWeek.Wednesday:
                    return DayOfWeekFlag.Wednesday;
                default:
                    return DayOfWeekFlag.None;
            }
        }

        /// <summary>
        /// Gets the list of day of week represented by the flag.
        /// </summary>
        /// <param name="days">The days.</param>
        /// <returns></returns>
        public static IList<DayOfWeek> GetDayOfWeekList(DayOfWeekFlag daysFlag)
        {
            var list = new List<DayOfWeek>();

            if (daysFlag == DayOfWeekFlag.None) return list;

            foreach (var day in EnumUtils.GetElements<DayOfWeek>())
            {
                if (ContainsDay(daysFlag, day))
                {
                    list.Add(day);
                }
            }

            return list;
        }
    }
}
