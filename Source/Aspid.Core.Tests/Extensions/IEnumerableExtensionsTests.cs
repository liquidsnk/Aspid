#region License
#endregion

using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Aspid.Core.Extensions.Tests
{
    [TestFixture]
    class IEnumerableExtensionsTests
    {
        [Test]
        public void ForEach_GivenSomeActions_PerformsEachActionToEachItemOnTheEnumeration()
        {
            List<object> list1 = new List<object>();
            List<object> list2 = new List<object>();
            IEnumerable<object> objects = new object[] { "Some", 123, 24.3, 7F, "asd" };

            //Add each item of the enumeration to both lists
            objects.ForEach(x => list1.Add(x),
                            x => list2.Add(x));

            //Assert the items were added
            var tuple1 = Tuple.FromLists(objects, list1);
            var tuple2 = Tuple.FromLists(objects, list2);
            foreach (var item in tuple1)
            {
                Assert.AreEqual(item.FirstItem, item.SecondItem);
            }
            foreach (var item in tuple2)
            {
                Assert.AreEqual(item.FirstItem, item.SecondItem);
            }
        }
    }
}
