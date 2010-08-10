#region License
/*
 * tl;dr: NetBSD type of license (two-clause BSD OSI compliant),
 * use it for whatever you like but reproducing this copyright notice.
 * 
 * Copyright (c) 2010 Fredy H. Treboux.
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

using NUnit.Framework;

using Aspid.Core.Extensions;

namespace Aspid.Core.Tests.Extensions
{
    /// <summary>
    /// Tests for object class extension methods.
    /// </summary>
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void IsNullOrEmpty_GivenANullString_ReturnsTrue()
        {
            string sut = null;
            Assert.IsTrue(sut.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_GivenAnEmptyString_ReturnsTrue()
        {
            string sut = string.Empty;
            Assert.IsTrue(sut.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_GivenANonEmptyString_ReturnsFalse()
        {
            string sut = "some string";
            Assert.IsFalse(sut.IsNullOrEmpty());
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(5, 10)]
        public void SafeRemove_ToANullString_ReturnsNull(int startIndex, int count)
        {
            string sut = null;
            Assert.AreEqual(null, sut.SafeRemove(startIndex, count));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Some string")]
        public void SafeRemove_GivenANegativeStartIndex_ThrowsArgumentException(string sut)
        {
            const string exceptionMessage = "Start index should be equal or greater than zero";
            
            var exception = Assert.Throws<ArgumentException>(() => sut.SafeRemove(-1, 0));
            Assert.AreEqual(exceptionMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Some string")]
        public void SafeRemove_GivenANegativeCount_ThrowsArgumentException(string sut)
        {
            const string exceptionMessage = "Count should be equal or greater than zero";

            var exception = Assert.Throws<ArgumentException>(() => sut.SafeRemove(0, -1));
            Assert.AreEqual(exceptionMessage, exception.Message);
        }
        
        [Test]
        [TestCase("", 3)]
        [TestCase("Some string", 5)]
        [TestCase("Some string", 2)]
        public void SafeRemove_ToAStringGivenACountOfZero_ReturnsTheString(string sut, int startIndex)
        {
            Assert.AreEqual(sut, sut.SafeRemove(startIndex, 0));
        }

        [Test]
        [TestCase("", 3, 2)]
        [TestCase("Some string", 20, 100)]
        [TestCase("Another string", 200, 1)]
        public void SafeRemove_ToAStringGivenAStartIndexOutOfRange_ReturnsTheString(string sut, int startIndex, int count)
        {
            Assert.AreEqual(sut, sut.SafeRemove(startIndex, count));
        }

        [Test]
        [TestCase("Some string", 2, 1)]
        [TestCase("Another string", 3, 5)]
        public void SafeRemove_ToAStringWithInsideBoundsParameters_ReturnsTheStringAsIfRemoveWereUsed(string sut, int startIndex, int count)
        {
            Assert.AreEqual(sut.Remove(startIndex, count), sut.SafeRemove(startIndex, count));
        }

        [Test]
        public void SafeRemove_ToAStringWithInsideBoundsStartIndexAndOutsideBoundsCount_ReturnsTheStringMinusTheRemovedChars()
        {
            string sut = "Another string";
            Assert.AreEqual("Anoth", sut.SafeRemove(5, 500));
        }

        [Test]
        [TestCase("Some string", 2)]
        [TestCase("Another string", 5)]
        public void SafeRemove_ToAStringWithInsideBoundsStartIndexAndNoCount_ReturnsTheStringAsIfRemoveWereUsed(string sut, int startIndex)
        {
            Assert.AreEqual(sut.Remove(startIndex), sut.SafeRemove(startIndex));
        }

        [Test]
        [TestCase("")]
        [TestCase("string")]
        public void SafeRemove_ToAStringWithStartIndexEqualToLength_ReturnsTheString(string sut)
        {
            Assert.AreEqual(sut, sut.SafeRemove(sut.Length));
        }
    }
}
