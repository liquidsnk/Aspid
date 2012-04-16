#region License
#endregion

using System;

using NUnit.Framework;

namespace Aspid.Core.Utils.Tests
{
    [TestFixture]
    public class ObjectUtilsTests
    {
        [Test]
        public void ThrowIfNull_WithNullParameter_ThrowsArgumentNullExceptionWithGivenParameterName()
        {
            const string parameterName = "parameterName";
            object nullObject = null;

            var exception = Assert.Throws<ArgumentNullException>(() => ObjectUtils.ThrowIfNull(nullObject, parameterName));
            Assert.AreEqual(parameterName, exception.ParamName);
        }

        [Test]
        public void ThrowIfNull_WithNullParameterAndMessage_ThrowsArgumentNullExceptionWithGivenParameterNameAndMessage()
        {
            const string parameterName = "parameterName";
            const string message = "This is the exception message";
            object nullObject = null;

            var exception = Assert.Throws<ArgumentNullException>(() => ObjectUtils.ThrowIfNull(nullObject, parameterName, message));
            Assert.AreEqual(parameterName, exception.ParamName);
            //The ArgumentNullException exception concatenates the parameter name with the message on the message property, so check exactly that.
            Assert.AreEqual(new ArgumentNullException(parameterName, message).Message, exception.Message); 
        }

        [Test]
        public void ThrowIfNull_WithNotNullParameter_DoesntThrow()
        {
            object testObject = new object();
            Assert.DoesNotThrow(() => ObjectUtils.ThrowIfNull(testObject, ""));
        }

        [Test]
        public void ThrowIfNull_WithValueTypeParameter_DoesntThrow()
        {
            ObjectUtils.ThrowIfNull(3, "");
        }

        [Test]
        public void ThrowIfNull_WithNullParameterName_ThrowsExceptionWithNullParamName()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => ObjectUtils.ThrowIfNull(null, null));
            Assert.IsNull(exception.ParamName);
        }

        [Test]
        public void ToStringOrEmpty_WithNullArgument_ReturnsTheEmptyString()
        {
            Assert.AreEqual(ObjectUtils.ToStringOrEmpty(null), string.Empty);
        }

        #region ClassWithNullToString - Mock class that returns a null from ToString()
        ///// <summary>
        ///// Mock class that returns a null from ToString().
        ///// </summary>
        class ClassWithNullToString
        {
            public override string ToString()
            {
                return null;
            }
        }
        #endregion

        [Test]
        public void ToStringOrEmpty_WithObjectThatReturnsNullFromToString_ReturnsTheEmptyString()
        {
            object someObject = new ClassWithNullToString();
            Assert.AreEqual(ObjectUtils.ToStringOrEmpty(someObject), string.Empty);
        }

        [Test]
        public void ToStringOrEmpty_WithNotNullObject_ReturnsToStringRepresentation()
        {
            object someNotNullObject = "ImNotNull";
            Assert.AreEqual(ObjectUtils.ToStringOrEmpty(someNotNullObject), someNotNullObject.ToString());
        }

        [Test]
        public void SafelyNavigate_OnNullObjectWithLambdaThatReturnsTheObjectItself_ReturnsNull()
        {
            object testObject = null;
            Assert.AreEqual(null, ObjectUtils.SafelyNavigate(testObject, x => x));
        }

        [Test]
        public void SafelyNavigate_WithLambdaThatReturnsTheObjectItself_ReturnsTheObject()
        {
            object testObject = new object();
            Assert.AreEqual(testObject, ObjectUtils.SafelyNavigate(testObject, x => x));
        }

        [Test]
        public void SafelyNavigate_OnNullObjectWithLambdaThatReturnsAProperty_ReturnsTheDefault()
        {
            string testObject = null;
            Assert.AreEqual(default(int), ObjectUtils.SafelyNavigate(testObject, x => x.Length));
        }

        [Test]
        [TestCase("")]
        [TestCase("SomeString")]
        [TestCase("And another string")]
        public void SafelyNavigate_WithLambdaThatReturnsAProperty_ReturnsThePropertyValue(string testObject)
        {
            Assert.AreEqual(testObject.Length, ObjectUtils.SafelyNavigate(testObject, x => x.Length));
        }

        [Test]
        public void SafelyNavigate_WithLambdaThatsNotJustMemberAccessPath_Throws()
        {
            const string exceptionMessage = "In order to safely navigate an expression for an object, the expression must be a member access path to the given parameter";
            string testObject = "some string";

            var exception = Assert.Throws<InvalidOperationException>(() => ObjectUtils.SafelyNavigate(testObject, x => (x + "stuff").ToString()));
            Assert.AreEqual(exceptionMessage, exception.Message);

            //Other asserts for cases that should also throw
            Assert.Throws<InvalidOperationException>(() => ObjectUtils.SafelyNavigate(testObject, x => (x + "stuff").Length), "Extra case 1 failed");
        }

        [Test]
        public void SafelyNavigate_WithLambdaThatStartsFromAnObjectThatsNotTheParameter_Throws()
        {
            const string exceptionMessage = "In order to safely navigate an expression for an object, the expression must be a member access path to the given parameter";
            string testObject = null;

            var exception = Assert.Throws<InvalidOperationException>(() => ObjectUtils.SafelyNavigate(testObject, x => "NotTheParameter!"));
            Assert.AreEqual(exceptionMessage, exception.Message);

            //Other asserts for cases that should also throw
            Assert.Throws<InvalidOperationException>(() => ObjectUtils.SafelyNavigate(testObject, x => "NotTheParameter!".Length), "Extra case 1 failed");
        }

        class ClassWithPropertyThatHaveSideEffect
        {
            public string PropertyWithSideEffect
            {
                get
                {
                    AffectedProperty++;
                    return null;
                }
            }

            public int AffectedProperty { get; set; }
        }

        [Test]
        public void SafelyNavigate_WithLambdaThatGoesThroughPropertiesWithSideEffects_TraversesPropertiesJustOnce()
        {
            var testObject = new ClassWithPropertyThatHaveSideEffect();

            Assert.AreEqual(default(int), ObjectUtils.SafelyNavigate(testObject, x => x.PropertyWithSideEffect.Length));
            Assert.AreEqual(1, testObject.AffectedProperty);
        }

        [Test]
        public void SafelyNavigate_OnNonNullObjectWithLambdaThatPassesThroughNullProperty_ReturnsTheDefault()
        {
            var testObject = new
            {
                StringProperty = (string)null,
            };

            Assert.AreEqual(default(int), ObjectUtils.SafelyNavigate(testObject, x => x.StringProperty.Length));
        }

        class ClassWithPropertyThatThrowsIgnoreException
        {
            public string PropertyThatThrowsNullReference
            {
                get { throw new IgnoreException("Just for testing"); }
            }
        }

        [Test]
        public void SafelyNavigate_OnObjectWithPropertyThatThrows_ThrowsInvalidExceptionWithThrownAsInnerException()
        {
            const string exceptionMessage = "Accessing the navigation chain has caused and exception to be thrown, see the inner exception for details";
            var testObject = new ClassWithPropertyThatThrowsIgnoreException();

            var exception = Assert.Throws<InvalidOperationException>(() => ObjectUtils.SafelyNavigate(testObject, x => x.PropertyThatThrowsNullReference.Length));
            Assert.AreEqual(exceptionMessage, exception.Message);
            Assert.IsInstanceOf(typeof(IgnoreException), exception.InnerException);
        }

        class ClassWithPublicFields
        {
            public ClassWithPublicFields publicField;

            public string StringRepresentation
            {
                get { return ToString(); }
            }
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatGoesThroughFieldsAndReturnsField_ReturnsTheFieldValue()
        {
            var returnedValue = new ClassWithPublicFields();
            var testObject = new ClassWithPublicFields
            {
                publicField = new ClassWithPublicFields
                {
                    publicField = returnedValue,
                }
            };

            Assert.AreSame(returnedValue, ObjectUtils.SafelyNavigate(testObject, x => x.publicField.publicField));
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatGoesThroughFieldsAndReturnsProperty_ReturnsThePropertyValue()
        {
            var testObject = new ClassWithPublicFields
            {
                publicField = new ClassWithPublicFields(),
            };

            int expectedLength = testObject.publicField.StringRepresentation.Length;
            Assert.AreEqual(expectedLength, ObjectUtils.SafelyNavigate(testObject, x => x.publicField.StringRepresentation.Length));
        }

        class ClassWithIndexedProperty
        {
            public const string VALUE = "ThisIsSomeIndexedString";

            public string this[int index]
            {
                get { return VALUE[index].ToString(); }
            }

            public string StringRepresentation
            {
                get { return ToString(); }
            }

            public ClassWithIndexedProperty This
            {
                get { return this; }
            }
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatEndsOnIndexedProperty_WorksCorrectly()
        {
            var testObject = new ClassWithIndexedProperty();

            string expectedString = ClassWithIndexedProperty.VALUE[3].ToString();
            Assert.AreEqual(expectedString, ObjectUtils.SafelyNavigate(testObject, x => x.This.This[3]));
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatEndsOnMethod_ReturnsMethodReturnValue()
        {
            string testObject = "some string";
            Assert.AreEqual(testObject, ObjectUtils.SafelyNavigate(testObject, x => x.ToString()));
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatGoesThroughMethod_ReturnsValueOfExpression()
        {
            var testObject = "some string";
            Assert.AreEqual(testObject.Length, ObjectUtils.SafelyNavigate(testObject, x => x.ToString().Length));
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatGoesThroughMethodThatReturnsNull_ReturnsDefaultValueOfExpression()
        {
            var testObject = new
            {
                SomeProperty = (string)null,
            };

            Assert.AreEqual(default(int), ObjectUtils.SafelyNavigate(testObject, x => x.SomeProperty.Length));
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatGoesThroughMethodWithNonConstantParameters_ThrowsNotSupportedException()
        {
            const string exceptionMessage = "Methods with non-constant parameters are not supported";
            var testObject = "some string";

            var exception = Assert.Throws<NotSupportedException>(() => ObjectUtils.SafelyNavigate(testObject, x => x.ToString()[x.Length - 5].ToString()));
            Assert.AreEqual(exceptionMessage, exception.Message);
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatGoesThroughMethodWithMultipleParameters_WorksCorrectly()
        {
            var testObject = "some string";
            var expectedSubStringLength = testObject.Substring(2, 3).ToString().Length;
            Assert.AreEqual(expectedSubStringLength, ObjectUtils.SafelyNavigate(testObject, x => x.Substring(2, 3).ToString().Length));
        }
    }
}
