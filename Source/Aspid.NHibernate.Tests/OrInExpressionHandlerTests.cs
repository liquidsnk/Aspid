#region License
#endregion

using System;

using NUnit.Framework;
using NHibernate.Criterion;

using Aspid.NHibernate;

namespace Aspid.Core.Tests
{
    [TestFixture]
    public class OrInExpressionHandlerTests
    {
        [Test]
        public void GetCriterion_GivenPropertyNameAndListOfLessThanThousandElements_ReturnsSingleInExpression()
        {
            ICriterion criterion = OrInExpressionHandler.GetCriterion("propertyName", new[] { 1, 2, 3, 4, 5 });
            Assert.IsInstanceOf<InExpression>(criterion);
            Assert.AreEqual("propertyName in (1, 2, 3, 4, 5)", criterion.ToString());
        }

        [Test]
        public void GetCriterion_GivenPropertyNameAndMaxElementsAndListOfMoreThanMaxElements_ReturnsOrExpression()
        {
            ICriterion criterion = OrInExpressionHandler.GetCriterion("property", Array.CreateInstance(typeof(int), 6), 2);
            Assert.IsInstanceOf<OrExpression>(criterion);
            Assert.AreEqual("(property in (0, 0) or (property in (0, 0) or property in (0, 0)))", criterion.ToString());
        }

        [Test]
        public void GetCriterion_GivenPropertyNameAndMaxElementsAndListOfMoreThanMaxElements_ReturnsOrExpression_2()
        {
            ICriterion criterion = OrInExpressionHandler.GetCriterion("property", Array.CreateInstance(typeof(int), 8), 3);
            Assert.IsInstanceOf<OrExpression>(criterion);
            Assert.AreEqual("(property in (0, 0, 0) or (property in (0, 0, 0) or property in (0, 0)))", criterion.ToString());
        }
    }
}
