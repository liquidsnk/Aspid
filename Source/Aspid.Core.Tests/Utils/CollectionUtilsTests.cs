#region License
#endregion

using System;

using NUnit.Framework;

namespace Aspid.Core.Utils.Tests
{
    [TestFixture]
    public class CollectionUtilsTests
    {
        [Test]
        public void IsNullOrEmpty_GivenEmptyCollection_ReturnsTrue()
        {
            Assert.IsTrue(CollectionUtils.IsNullOrEmpty(null));
        }

        [Test]
        public void IsNullOrEmpty_GivenNullCollection_ReturnsTrue()
        {
            var emptyCollection = new object[] {};
            Assert.IsTrue(CollectionUtils.IsNullOrEmpty(emptyCollection));
        }

        [Test]
        public void IsNullOrEmpty_GivenNonNullCollection_ReturnsFalse()
        {
            var collection = new object[] { "something", 123, DateTime.Now, 1.5m };
            Assert.IsFalse(CollectionUtils.IsNullOrEmpty(collection));
        }
    }
}
