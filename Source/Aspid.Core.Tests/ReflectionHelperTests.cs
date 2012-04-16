#region License
#endregion

using System;

using NUnit.Framework;
using System.Diagnostics;

namespace Aspid.Core.Tests
{
    [TestFixture]
    public class ReflectionHelperTests
    {
        [Test]
        public void GetPropertyPathGetAccessor_GivenFirstLevelProperty_ReturnsGetAccessorToProperty()
        {
            var testString = "This is just for testing";
            Assert.AreEqual(testString.Length, ReflectionHelper.GetPropertyPathGetAccessor(typeof(string), "Length").DynamicInvoke(testString));
        }

        [Test]
        public void GetPropertyPathGetAccessor_GivenSecondLevelProperty_ReturnsGetAccessorToProperty()
        {
            var testDate = DateTime.Now;
            Assert.AreEqual(testDate.Date.DayOfWeek, ReflectionHelper.GetPropertyPathGetAccessor(typeof(DateTime), "Date.DayOfWeek").DynamicInvoke(testDate));
        }

        [Test]
        public void GetPropertyPathGetAccessor_GivenrProperty_CachesAccessorCreation()
        {
            Assert.AreSame(ReflectionHelper.GetPropertyPathGetAccessor(typeof(string), "Length"),
                           ReflectionHelper.GetPropertyPathGetAccessor(typeof(string), "Length"));
        }

        [Test]
        public void GetPropertyPathValue_GivenSecondLevelProperty_ReturnsCorrectValue()
        {
            var testDate = DateTime.Now;
            Assert.AreEqual(testDate.Date.DayOfWeek, ReflectionHelper.GetPropertyPathValue(testDate, "Date.DayOfWeek"));
        }

        class TestClass
        {
            public class TestClass2
            {
                public int SomeIntProperty { get; set; }
            }

            public TestClass()
            {
                InnerTestClassObject = new TestClass2();
            }

            public int SomeIntProperty { get; set; }

            public TestClass2 InnerTestClassObject { get; set; }
        }

        [Test]
        public void SetPropertyPathValue_GivenFirstLevelProperty_ReturnsCorrectValue()
        {
            var test = new TestClass();
            ReflectionHelper.SetPropertyPathValue(test, "SomeIntProperty", 42);

            Assert.AreEqual(42, test.SomeIntProperty);
        }

        [Test]
        public void SetPropertyPathValue_GivenSecondLevelProperty_ReturnsCorrectValue()
        {
            var test = new TestClass();
            ReflectionHelper.SetPropertyPathValue(test, "InnerTestClassObject.SomeIntProperty", 42);

            Assert.AreEqual(42, test.InnerTestClassObject.SomeIntProperty);
        }
    }
}
