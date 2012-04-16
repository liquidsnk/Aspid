#region License
#endregion

using System;

using NUnit.Framework;

namespace Aspid.Core.Extensions.Tests
{
    [TestFixture]
    public class ObjectExtensionsTests
    {
        [Test]
        public void ObjectEquals_OnTwoNullObjects_ReturnsNull()
        {
            object sut = null;
            Assert.IsTrue(sut.ObjectEquals(null));
        }

        [Test]
        public void ObjectEquals_OnTwoEqualObjects_ReturnsTrue()
        {
            Assert.IsTrue("something".ObjectEquals("something"));
        }

        [Test]
        public void ObjectEquals_OnTwoNonEqualObjects_ReturnsFalse()
        {
            Assert.IsFalse("something".ObjectEquals("diffrent"));
        }
    }
}
