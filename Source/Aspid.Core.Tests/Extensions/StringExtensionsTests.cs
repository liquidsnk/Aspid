#region License
#endregion

using NUnit.Framework;

namespace Aspid.Core.Extensions.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void EmptyToNull_GivenNullString_ReturnsNullString()
        {
            string sut = null;
            Assert.IsNull(sut.EmptyToNull());
        }

        [Test]
        public void EmptyToNull_GivenEmptyString_ReturnsNullString()
        {
            string sut = string.Empty;
            Assert.IsNull(sut.EmptyToNull());
        }

        [Test]
        public void EmptyToNull_GivenNonEmptyString_ReturnsGivenString()
        {
            string sut = "something";
            Assert.AreEqual(sut, sut.EmptyToNull());
        }

        [Test]
        public void IsNullOrEmpty_GivenNullString_ReturnsTrue()
        {
            string sut = null;
            Assert.IsTrue(sut.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_GivenEmptyString_ReturnsTrue()
        {
            string sut = string.Empty;
            Assert.IsTrue(sut.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_GivenNonEmptyString_ReturnsFalse()
        {
            string sut = "something";
            Assert.IsFalse(sut.IsNullOrEmpty());
        }

        [Test]
        public void Join_WithNullSeparator_ConcatenatesGivenValues()
        {
            var values = new[] { "value1", "value2", "value3" };
            string separator = null;
            Assert.AreEqual("value1value2value3", separator.Join(values));
        }

        [Test]
        public void Join_WithEmptySeparator_ConcatenatesGivenValues()
        {
            var values = new[] { "value1", "value2", "value3" };
            string separator = string.Empty;
            Assert.AreEqual("value1value2value3", separator.Join(values));
        }

        [Test]
        public void Join_WithSeparator_ConcatenatesGivenValuesSeparatedByGivenSeparator()
        {
            var values = new[] { "v1", "v2", "v3" };
            string separator = " !!! ";
            Assert.AreEqual("v1 !!! v2 !!! v3", separator.Join(values));

            separator = "&&/(%&(/)($# ";
            Assert.AreEqual("v1&&/(%&(/)($# v2&&/(%&(/)($# v3", separator.Join(values));
        }

        [Test]
        public void Join_WithEmptyListOfValues_ReturnsEmptyString()
        {
            var values = new string[] {};
            string separator = ", ";
            Assert.AreEqual(string.Empty, separator.Join(values));
        }

        [Test]
        public void Join_WithNullListOfValues_ReturnsEmptyString()
        {
            string separator = ", ";
            Assert.AreEqual(string.Empty, separator.Join(null));
        }
    }
}
