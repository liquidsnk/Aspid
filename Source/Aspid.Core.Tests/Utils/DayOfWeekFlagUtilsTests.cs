#region License
#endregion

using System;

using NUnit.Framework;

namespace Aspid.Core.Utils.Tests
{
    [TestFixture]
    public class DayOfWeekFlagUtilsTests
    {
        DayOfWeek[] OrderedAllDaysArray = new DayOfWeek[]
        {
            DayOfWeek.Sunday,
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
        };

        DayOfWeekFlag[] OrderedAllDaysFlagSingleArray = new DayOfWeekFlag[]
        {
            DayOfWeekFlag.Sunday,
            DayOfWeekFlag.Monday,
            DayOfWeekFlag.Tuesday,
            DayOfWeekFlag.Wednesday,
            DayOfWeekFlag.Thursday,
            DayOfWeekFlag.Friday,
            DayOfWeekFlag.Saturday,
        };

        [Test]
        public void FromDayOfWeek_GivenIndividualDay_ConvertsItCorrectly()
        {
            //Checking it works for all days
            for (int i = 0; i < OrderedAllDaysArray.Length; i++)
            {
                Assert.AreEqual(OrderedAllDaysFlagSingleArray[i], DayOfWeekFlagUtils.FromDayOfWeek(OrderedAllDaysArray[i]));
            }
        }

        [Test]
        public void ContainsDay_GivenConvertedIndividualDayAndSameDay_ReturnsTrue()
        {
            //Checking it works for all days
            for (int i = 0; i < OrderedAllDaysArray.Length; i++)
            {
                var dayFlag = DayOfWeekFlagUtils.FromDayOfWeek(OrderedAllDaysArray[i]);
                Assert.IsTrue(DayOfWeekFlagUtils.ContainsDay(dayFlag, OrderedAllDaysArray[i]));
            }
        }

        [Test]
        public void ContainsDay_GivenTheNoneFlag_ReturnsFalseForAllDays()
        {
            //Checking it doesn't contain any of the days
            for (int i = 0; i < OrderedAllDaysArray.Length; i++)
            {
                Assert.IsFalse(DayOfWeekFlagUtils.ContainsDay(DayOfWeekFlag.None, OrderedAllDaysArray[i]));
            }
        }

        [Test]
        public void ContainsDay_GivenAFlagContainingAllDays_ReturnsTrueForAnyDay()
        {
            //Checking it doesn't contain any of the days
            var allDaysFlag = DayOfWeekFlagUtils.FromDaysOfWeek(OrderedAllDaysArray);
            for (int i = 0; i < OrderedAllDaysArray.Length; i++)
            {
                Assert.IsTrue(DayOfWeekFlagUtils.ContainsDay(allDaysFlag, OrderedAllDaysArray[i]));
            }
        }

        [Test]
        public void ContainsDay_GivenAFlagAndADayToCheck_AssertsThatOlyDaysOnTheFlagAreContained()
        {
            var daysFlag = DayOfWeekFlagUtils.FromDaysOfWeek(DayOfWeek.Friday, DayOfWeek.Monday, DayOfWeek.Saturday);

            Assert.IsTrue(DayOfWeekFlagUtils.ContainsDay(daysFlag, DayOfWeek.Friday));
            Assert.IsTrue(DayOfWeekFlagUtils.ContainsDay(daysFlag, DayOfWeek.Monday));
            Assert.IsTrue(DayOfWeekFlagUtils.ContainsDay(daysFlag, DayOfWeek.Saturday));

            Assert.IsFalse(DayOfWeekFlagUtils.ContainsDay(daysFlag, DayOfWeek.Sunday));
            Assert.IsFalse(DayOfWeekFlagUtils.ContainsDay(daysFlag, DayOfWeek.Thursday));
            Assert.IsFalse(DayOfWeekFlagUtils.ContainsDay(daysFlag, DayOfWeek.Tuesday));
            Assert.IsFalse(DayOfWeekFlagUtils.ContainsDay(daysFlag, DayOfWeek.Wednesday));
        }

        [Test]
        public void GetDayOfWeekList_GivenNoneFlag_ReturnsEmptyList()
        {
            var listOfDays = DayOfWeekFlagUtils.GetDayOfWeekList(DayOfWeekFlag.None);

            Assert.IsNotNull(listOfDays);
            Assert.IsTrue(listOfDays.Count == 0);
        }

        [Test]
        public void GetDayOfWeekList_GivenASingleDayFlag_ReturnsAListWithThatDayAsOnlyElement()
        {
            var listOfDays = DayOfWeekFlagUtils.GetDayOfWeekList(DayOfWeekFlag.Sunday);

            Assert.IsNotNull(listOfDays);
            Assert.IsTrue(listOfDays.Count == 1);
            Assert.AreEqual(DayOfWeek.Sunday, listOfDays[0]);
        }

        [Test]
        public void GetDayOfWeekList_GivenAFlagWithMultipleDays_ReturnsAListWithAllDaysOnTheFlag()
        {
            var daysFlag = DayOfWeekFlagUtils.FromDaysOfWeek(DayOfWeek.Friday, DayOfWeek.Monday, DayOfWeek.Saturday);
            var listOfDays = DayOfWeekFlagUtils.GetDayOfWeekList(daysFlag);

            Assert.IsNotNull(listOfDays);
            Assert.IsTrue(listOfDays.Count == 3);

            //They will come ordered as they are on the DayOfWeekEnum (probably the order should be irrelevant)
            Assert.AreEqual(DayOfWeek.Monday, listOfDays[0]);
            Assert.AreEqual(DayOfWeek.Friday, listOfDays[1]);
            Assert.AreEqual(DayOfWeek.Saturday, listOfDays[2]);
        }
    }
}
