#region License
#endregion

using NUnit.Framework;

namespace Aspid.Core.Tests
{
    [TestFixture]
    public class GenericTypeConverterTests
    {
        [Test]
        [TestCase("True", true)]
        [TestCase("true", true)]
        [TestCase(1, true)]
        [TestCase(11, true)]
        [TestCase(-1, true)]
        [TestCase("False", false)]
        [TestCase("false", false)]
        [TestCase(0, false)]
        public void ChangeType_ToBoolType_SupportsConvertionOfCommonBooleanValues(object value, bool expected)
        {
            Assert.AreEqual(expected, GenericTypeConverter.ChangeType<bool>(value));
        }
    }
}
