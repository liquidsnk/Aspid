#region License
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Aspid.Core.Tests
{
    [TestFixture]
    public class TupleTests
    {
        [Test]
        public void FromItems_WithTwoItemsOfDiffrentType_CreatesTuple()
        {
            
            Assert.IsNotNull(Tuple.FromItems("SomeItem", 12));
        }

        [Test]
        public void FirstItemAndSecondItem_OnATuple_ReturnTheCorrectItems()
        {
            //We're also checking that values and references work correctly
            var someValue = 123;
            var someReference = new object();
            var tuple = Tuple.FromItems(someValue, someReference);

            Assert.AreEqual(someValue, tuple.FirstItem);
            Assert.AreSame(someReference, tuple.SecondItem);
        }

        [Test]
        public void FromLists_OnListsOfSameSize_CreatesListOfTupleElements()
        {
            var list1 = new List<object>() { "something", new StringBuilder(), new object(), "test", new object() }; //references
            var list2 = new List<int>() { 1, 2, 8, 17, 5 }; //values
            Assert.AreEqual(list1.Count, list2.Count);

            var listOfTuples = Tuple.FromLists(list1, list2);

            Assert.AreEqual(list1.Count, listOfTuples.Count());
            //Check every item
            int i = 0;
            foreach (var tuple in listOfTuples)
            {
                Assert.AreSame(list1[i], tuple.FirstItem);
                Assert.AreEqual(list2[i], tuple.SecondItem);
                i++;
            }
        }

        [Test]
        public void FromLists_OnListsOfDiffrentSizesAndTheFirstListIsSmaller_CreatesListOfTupleElementsFillingSmallerListItemsWithDefault()
        {
            var list1 = new List<object>() { "something", new StringBuilder(), new object() }; //references
            var list2 = new List<int>() { 1, 2, 7, 19, 5 }; //values
            Assert.IsTrue(list1.Count < list2.Count);

            var listOfTuples = Tuple.FromLists(list1, list2);

            //The count is equal to the count of the larger list
            Assert.AreEqual(list2.Count, listOfTuples.Count()); 
            
            int i = 0;
            foreach (var tuple in listOfTuples)
            {
                if (i < list1.Count)
                {
                    Assert.AreSame(list1[i], tuple.FirstItem);
                }
                else
                {
                    //Last items that correspond to the smaller list, are the default for the Type
                    Assert.AreEqual(default(object), tuple.FirstItem);
                }

                Assert.AreEqual(list2[i], tuple.SecondItem);
                i++;
            }
        }

        [Test]
        public void FromLists_OnListsOfDiffrentSizesAndTheSecondListIsSmaller_CreatesListOfTupleElementsFillingSmallerListItemsWithDefault()
        {
            var list1 = new List<object>() { "something", new StringBuilder(), new object(), new object() }; //references
            var list2 = new List<int>() { 1, 2, 25, }; //values
            Assert.IsTrue(list2.Count < list1.Count);

            var listOfTuples = Tuple.FromLists(list1, list2);

            //The count is equal to the count of the larger list
            Assert.AreEqual(list1.Count, listOfTuples.Count());

            int i = 0;
            foreach (var tuple in listOfTuples)
            {
                if (i < list2.Count)
                {
                    Assert.AreEqual(list2[i], tuple.SecondItem);
                }
                else
                {
                    //Last items that correspond to the smaller list, are the default for the Type
                    Assert.AreEqual(default(int), tuple.SecondItem);
                }

                Assert.AreSame(list1[i], tuple.FirstItem);
                i++;
            }
        }

        [Test]
        public void Equals_GivenATupleWithEqualElements_ShouldReturnTrue()
        {
            //values
            var tuple1 = Tuple.FromItems(1, 2);
            var tuple2 = Tuple.FromItems(1, 2);
            Assert.IsTrue(tuple1.Equals(tuple2));
            
            //references
            var tuple3 = Tuple.FromItems("hello", "byebye");
            var tuple4 = Tuple.FromItems("hello", "byebye");
            Assert.IsTrue(tuple3.Equals(tuple4));
        }

        [Test]
        public void GetHashCode_GivenATupleWithEqualElements_ShouldReturnTheSame()
        {
            //values
            var tuple1 = Tuple.FromItems(1, 2);
            var tuple2 = Tuple.FromItems(1, 2);
            Assert.AreEqual(tuple1.GetHashCode(), tuple2.GetHashCode());

            //references
            var tuple3 = Tuple.FromItems("hello", "byebye");
            var tuple4 = Tuple.FromItems("hello", "byebye");
            Assert.AreEqual(tuple3.GetHashCode(), tuple4.GetHashCode());
        }
        
        [Test]
        public void Equals_GivenATupleWithDiffrentElements_ShouldReturnFalse()
        {
            //values
            var tuple1 = Tuple.FromItems(1, 2);
            var tuple2 = Tuple.FromItems(1, 4);
            Assert.IsFalse(tuple1.Equals(tuple2));

            //references
            var tuple3 = Tuple.FromItems("hello", "byebye");
            var tuple4 = Tuple.FromItems("hello", "bye");
            Assert.IsFalse(tuple3.Equals(tuple4));
        }
    }
}
