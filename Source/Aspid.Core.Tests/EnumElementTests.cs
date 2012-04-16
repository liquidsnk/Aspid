#region License
#endregion

using NUnit.Framework;

namespace Aspid.Core.Tests
{
    [TestFixture]
    public class EnumElementTests
    {
        public enum SimpleEnum
        {
            One = 1,
            Two = 2,
            TwentyOne = 21,
        }

        //TODO: Find out why tests below doesn't work for the DevExpress Runner. (Something to do with TestCase and an enum?)
        //[Test]
        //[TestCase(SimpleEnum.One)]
        //[TestCase(SimpleEnum.Two)]
        //[TestCase(SimpleEnum.TwentyOne)]
        //public void EnumElement_CreatedProvidingADescription_GetsTheElementValueAndTheGivenDescription(SimpleEnum value)
        //{
        //    const string providedDescription = "Provided Description";
        //    var enumElement = new EnumElement<SimpleEnum>(value, providedDescription);

        //    Assert.AreEqual(value, enumElement.Value);
        //    Assert.AreEqual(providedDescription, enumElement.Description);
        //}

        //[Test]
        //[TestCase(SimpleEnum.One)]
        //[TestCase(SimpleEnum.Two)]
        //[TestCase(SimpleEnum.TwentyOne)]
        //public void EnumElement_BeingImplicitlyConvertedFromAnEnumElement_GetsTheElementValue(SimpleEnum value)
        //{
        //    EnumElement<SimpleEnum> simpleEnumElement = value;

        //    Assert.AreEqual(value, simpleEnumElement.Value);
        //}

        //[Test]
        //[TestCase(SimpleEnum.One)]
        //[TestCase(SimpleEnum.Two)]
        //[TestCase(SimpleEnum.TwentyOne)]
        //public void EnumElement_BeingImplicitlyConvertedFromAnEnumElement_GetsTheElementDescriptionLikeUsingEnumUtils(SimpleEnum value)
        //{
        //    EnumElement<SimpleEnum> simpleEnumElement = value;

        //    Assert.AreEqual(EnumUtils.GetElementDescription(value), simpleEnumElement.Description);
        //}

        [Test]
        public void Equals_ForTwoEnumElementsOfSameValue_ReturnsTrue()
        {
            var firstEnumElement = new EnumElement<SimpleEnum>(SimpleEnum.TwentyOne, "description");
            var secondEnumElement = new EnumElement<SimpleEnum>(SimpleEnum.TwentyOne, "description");

            Assert.IsTrue(firstEnumElement.Equals(secondEnumElement));
        }

        [Test]
        public void Equals_ForTwoEnumElementsOfSameValue_ReturnsTrueEvenForDiffrentDescription()
        {
            var firstEnumElement = new EnumElement<SimpleEnum>(SimpleEnum.TwentyOne, "description");
            var secondEnumElement = new EnumElement<SimpleEnum>(SimpleEnum.TwentyOne, "diffrent description");

            Assert.IsTrue(firstEnumElement.Equals(secondEnumElement));
        }

        [Test]
        public void Equals_ForTwoEnumElementsOfDiffrentValue_ReturnsFalse()
        {
            var firstEnumElement = new EnumElement<SimpleEnum>(SimpleEnum.One, "description");
            var secondEnumElement = new EnumElement<SimpleEnum>(SimpleEnum.TwentyOne, "description");

            Assert.IsFalse(firstEnumElement.Equals(secondEnumElement));
        }
    }
}
