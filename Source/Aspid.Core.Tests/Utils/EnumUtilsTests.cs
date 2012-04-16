#region License
#endregion

using System;
using System.Linq;
using ComponentModel = System.ComponentModel;

using NUnit.Framework;

using Aspid.Core.Extensions;

namespace Aspid.Core.Utils.Tests
{
    /// <summary>
    /// Tests for enum utillity methods.
    /// </summary>
    [TestFixture]
    public class EnumUtilsTests
    {
        const string fifthElementDescription = "Like the movie";
        enum TestEnum
        {
            FirstElement,
            SecondElement,
            ThirdElement,
            FourthElement,

            [ComponentModel.DescriptionAttribute(fifthElementDescription)]
            FifthElement,
        }

        const string flag1Description = "This is the first flag";
        const string flag3Description = "This is the third flag";
        [Flags]
        enum TestFlag
        {
            ThisShouldNotBeTakenIntoAccount = 0,

            [ComponentModel.DescriptionAttribute(flag1Description)]
            Flag1 = 1,
            Flag2 = 2,

            [ComponentModel.DescriptionAttribute(flag3Description)]
            Flag3 = 4,

            Flag4 = 8,
        }
        
        [Test]
        public void GetEnumElements_GivenNoElementToOmit_ReturnsAListOfAllElements()
        {
            var elements = EnumUtils.GetEnumElements<TestEnum>();

            var enumElements = Enum.GetValues(typeof(TestEnum));
            Assert.AreEqual(enumElements.Length, elements.Count);

            foreach (var tuple in Tuple.FromLists(enumElements.Cast<TestEnum>(), elements))
            {
                Assert.AreEqual(tuple.FirstItem, tuple.SecondItem.Value);
            }
        }
                
        [Test]
        public void GetEnumElements_GivenElementsToOmit_ReturnsAListOfAllElementsExceptTheOmitted()
        {
            var elements = EnumUtils.GetEnumElements(TestEnum.FifthElement, TestEnum.ThirdElement);

            var enumElements = Enum.GetValues(typeof(TestEnum));
            Assert.AreEqual(enumElements.Length - 2, elements.Count); //two elements omitted

            Assert.IsTrue(elements.Contains(TestEnum.FirstElement));
            Assert.IsTrue(elements.Contains(TestEnum.SecondElement));
            Assert.IsTrue(elements.Contains(TestEnum.FourthElement));

            Assert.IsFalse(elements.Contains(TestEnum.ThirdElement));
            Assert.IsFalse(elements.Contains(TestEnum.FifthElement));
        }

        [Test]
        public void GetEnumElement_GivenAnElementFromAnEnum_GetsAnEnumElementRepresentingIt()
        {
            var element = EnumUtils.GetEnumElement(TestEnum.FourthElement);

            Assert.AreEqual(TestEnum.FourthElement, element.Value);
        }

        [Test]
        public void GetElementDescription_GivenAnElementWithDescriptionAttribute_ReturnsTheDescriptionProvidedByTheAttribute()
        {
            Assert.AreEqual(fifthElementDescription, EnumUtils.GetElementDescription(TestEnum.FifthElement));
        }

        [Test]
        public void GetElementDescription_GivenAnElementWithNoDescriptionAttribute_ReturnsTheCaseSeparatedEnumElementName()
        {
            Assert.AreEqual(TestEnum.FourthElement.ToString().CaseSeparate(), EnumUtils.GetElementDescription(TestEnum.FourthElement));
        }

        [Test]
        public void GetElementDescription_GivenAFlag_ReturnsTheCommaSeparatedDescriptionsOfTheElementsOnTheFlag()
        {
            Assert.AreEqual("Flag 2, Flag 4", EnumUtils.GetElementDescription(TestFlag.Flag2 | TestFlag.Flag4));
        }

        [Test]
        public void GetElementDescription_GivenAFlagWithElementsThatUseDescriptionAttribute_ReturnsTheCommaSeparatedDescriptions()
        {
            Assert.AreEqual(String.Format("{0}, Flag 2, {1}", flag1Description, flag3Description),
                            EnumUtils.GetElementDescription(TestFlag.Flag1 | TestFlag.Flag2 | TestFlag.Flag3));
        }

        [Test]
        public void GetElementDescription_GivenAFlagWithSingleElement_ReturnsTheElementDescription()
        {
            Assert.AreEqual(flag1Description, EnumUtils.GetElementDescription(TestFlag.Flag1));
        }

        [Test]
        public void GetElementDescription_GivenAFlagAndCustomSeparator_ReturnsTheDescriptionsSeparatedByTheSeparator()
        {
            Assert.AreEqual(String.Format("{0}; Flag 2; {1}", flag1Description, flag3Description),
                            EnumUtils.GetElementDescription(TestFlag.Flag1 | TestFlag.Flag2 | TestFlag.Flag3, "; "));
        }

        [Test]
        public void GetElementDescription_GivenTheNoneElementOfTheEnum_ReturnsTheDescriptionOfTheNoneElement()
        {
            Assert.AreEqual(TestFlag.ThisShouldNotBeTakenIntoAccount.ToString().CaseSeparate(),
                            EnumUtils.GetElementDescription(TestFlag.ThisShouldNotBeTakenIntoAccount));
        }
    }
}
