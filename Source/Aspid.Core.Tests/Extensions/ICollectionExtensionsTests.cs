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

using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using Aspid.Core.Extensions;

namespace Aspid.Core.Tests.Extensions
{
    /// <summary>
    /// Tests for ICollection and ICollection<T> interfaces extension methods.
    /// </summary>
    [TestFixture]
    public class ICollectionExtensionsTests
    {
        [Test]
        public void IsNullOrEmpty_GivenANullCollection_ReturnsTrue()
        {
            ICollection sut = null;
            Assert.IsTrue(sut.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_GivenAnEmptyCollection_ReturnsTrue()
        {
            ICollection sut = new ArrayList();
            Assert.IsTrue(sut.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_GivenANonEmptyCollection_ReturnsFalse()
        {
            ICollection sut = new ArrayList { null };
            Assert.IsFalse(sut.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_GivenANullGenericCollection_ReturnsTrue()
        {
            ICollection<object> sut = null;
            Assert.IsTrue(sut.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_GivenAnEmptyGenericCollection_ReturnsTrue()
        {
            ICollection<object> sut = new List<object>();
            Assert.IsTrue(sut.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_GivenANonEmptyGenericCollection_ReturnsFalse()
        {
            ICollection<object> sut = new List<object> { null };
            Assert.IsFalse(sut.IsNullOrEmpty());
        }
    }
}
