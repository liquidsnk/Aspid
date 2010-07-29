#region License
/*
 * tl;dr: NetBSD type of license (two-clause BSD OSI compliant),
 * use it for whatever you like but reproducing this copyright notice.
 * 
 * Copyright (c) 2008 Fredy H. Treboux.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Aspid.Core.Extensions;

namespace Aspid.Core.Tests.Extensions
{
    /// <summary>
    /// Tests for object class extension methods.
    /// </summary>
    [TestFixture]
    public class ObjectExtensionsTests
    {
        [Test]
        public void ToStringOrEmpty_UsedOnNullObject_ReturnsEmptyString()
        {
            object sut = null;
            Assert.AreEqual(string.Empty, sut.ToStringOrEmpty());
        }

        [Test]
        public void ToStringOrEmpty_UsedOnNonNullObject_ReturnsObjectToString()
        {
            object sut = new object();
            Assert.AreEqual(sut.ToString(), sut.ToStringOrEmpty());
        }

        [Test]
        public void ThrowIfNull_UsedOnNonNullObject_DoesNothing()
        {
            object sut = new object();
            Assert.DoesNotThrow(() => sut.ThrowIfNull("sut"));
        }

        [Test]
        public void ThrowIfNull_UsedOnNullObject_ThrowsArgumentNullException()
        {
            object sut = null;
            Assert.Throws<ArgumentNullException>(() => sut.ThrowIfNull("sut"));
        }

        [Test]
        public void ThrowIfNull_UsedOnNullObject_ThrowsArgumentNullExceptionWithParameterName()
        {
            const string parameterName = "sut";
            object sut = null;
            var exception = Assert.Throws<ArgumentNullException>(() => sut.ThrowIfNull(parameterName));
            Assert.AreEqual(parameterName, exception.ParamName);
        }

        [Test]
        public void ThrowIfNull_UsedOnNullObjectWithMessage_ThrowsArgumentNullExceptionWithGivenMessage()
        {
            const string parameterName = "sut";
            const string message = "Given message";
            object sut = null;
            var exception = Assert.Throws<ArgumentNullException>(() => sut.ThrowIfNull(parameterName, message));
            Assert.AreEqual(new ArgumentNullException(parameterName, message).Message, exception.Message);
        }
        
        [Test]
        public void SafelyNavigate_OnNullObjectWithLambdaThatReturnsTheObjectItself_ReturnsNull()
        {
            object sut = null;
            Assert.AreEqual(null, sut.SafelyNavigate(x => x));
        }

        [Test]
        public void SafelyNavigate_WithLambdaThatReturnsTheObjectItself_ReturnsTheObject()
        {
            object sut = new object();
            Assert.AreEqual(sut, sut.SafelyNavigate(x => x));
        }

        [Test]
        public void SafelyNavigate_OnNullObjectWithLambdaThatReturnsAProperty_ReturnsTheDefault()
        {
            string sut = null;
            Assert.AreEqual(default(int), sut.SafelyNavigate(x => x.Length));
        }

        [Test]
        [TestCase("")]
        [TestCase("SomeString")]
        [TestCase("And another string")]
        public void SafelyNavigate_WithLambdaThatReturnsAProperty_ReturnsThePropertyValue(string sut)
        {
            Assert.AreEqual(sut.Length, sut.SafelyNavigate(x => x.Length));
        }

        [Test]
        public void SafelyNavigate_WithLambdaThatsNotJustMemberAccessPath_Throws()
        {
            const string exceptionMessage = "In order to safely navigate an expression for an object, the expression must be a member access path to the given parameter";
            string sut = "some string";
            
            var exception = Assert.Throws<InvalidOperationException>(() => sut.SafelyNavigate(x => (x + "stuff").ToString()));
            Assert.AreEqual(exceptionMessage, exception.Message);

            //Other asserts for cases that should also throw
            Assert.Throws<InvalidOperationException>(() => sut.SafelyNavigate(x => (x + "stuff").Length), "Extra case 1 failed");
        }

        [Test]
        public void SafelyNavigate_WithLambdaThatStartsFromAnObjectThatsNotTheParameter_Throws()
        {
            const string exceptionMessage = "In order to safely navigate an expression for an object, the expression must be a member access path to the given parameter";
            string sut = null;

            var exception = Assert.Throws<InvalidOperationException>(() => sut.SafelyNavigate(x => "NotTheParameter!"));
            Assert.AreEqual(exceptionMessage, exception.Message);

            //Other asserts for cases that should also throw
            Assert.Throws<InvalidOperationException>(() => sut.SafelyNavigate(x => "NotTheParameter!".Length), "Extra case 1 failed");
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
            var sut = new ClassWithPropertyThatHaveSideEffect();

            Assert.AreEqual(default(int), sut.SafelyNavigate(x => x.PropertyWithSideEffect.Length));
            Assert.AreEqual(1, sut.AffectedProperty);
        }
               
        [Test]
        public void SafelyNavigate_OnNonNullObjectWithLambdaThatPassesThroughNullProperty_ReturnsTheDefault()
        {
            var sut = new
            {
                StringProperty = (string)null,
            };

            Assert.AreEqual(default(int), sut.SafelyNavigate(x => x.StringProperty.Length));
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
            var sut = new ClassWithPropertyThatThrowsIgnoreException();

            var exception = Assert.Throws<InvalidOperationException>(() => sut.SafelyNavigate(x => x.PropertyThatThrowsNullReference.Length));
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
            var returnedValue = new  ClassWithPublicFields();
            var sut = new ClassWithPublicFields
            {
                publicField = new ClassWithPublicFields
                {
                    publicField = returnedValue,
                }
            };

            Assert.AreSame(returnedValue, sut.SafelyNavigate(x => x.publicField.publicField));
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatGoesThroughFieldsAndReturnsProperty_ReturnsThePropertyValue()
        {
            var sut = new ClassWithPublicFields
            {
                publicField = new ClassWithPublicFields(),
            };

            int expectedLength = sut.publicField.StringRepresentation.Length;
            Assert.AreEqual(expectedLength, sut.SafelyNavigate(x => x.publicField.StringRepresentation.Length));
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
            var sut = new ClassWithIndexedProperty();

            string expectedString = ClassWithIndexedProperty.VALUE[3].ToString();
            Assert.AreEqual(expectedString, sut.SafelyNavigate(x => x.This.This[3]));
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatEndsOnMethod_ReturnsMethodReturnValue()
        {
            string sut = "some string";
            Assert.AreEqual(sut, sut.SafelyNavigate(x => x.ToString()));
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatGoesThroughMethod_ReturnsValueOfExpression()
        {
            var sut = "some string";
            Assert.AreEqual(sut.Length, sut.SafelyNavigate(x => x.ToString().Length));
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatGoesThroughMethodThatReturnsNull_ReturnsDefaultValueOfExpression()
        {
            var sut = new
            {
                SomeProperty = (string)null,
            };

            Assert.AreEqual(default(int), sut.SafelyNavigate(x => x.SomeProperty.Length));
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatGoesThroughMethodWithNonConstantParameters_ThrowsNotSupportedException()
        {
            const string exceptionMessage = "Methods with non-constant parameters are not supported";
            var sut = "some string";

            var exception = Assert.Throws<NotSupportedException>(() => sut.SafelyNavigate(x => x.ToString()[x.Length - 5].ToString()));
            Assert.AreEqual(exceptionMessage, exception.Message);
        }

        [Test]
        public void SafelyNavigate_OnObjectWithLambdaThatGoesThroughMethodWithMultipleParameters_WorksCorrectly()
        {
            var sut = "some string";
            var expectedSubStringLength = sut.Substring(2, 3).ToString().Length;
            Assert.AreEqual(expectedSubStringLength, sut.SafelyNavigate(x => x.Substring(2, 3).ToString().Length));
        }
    }
}
