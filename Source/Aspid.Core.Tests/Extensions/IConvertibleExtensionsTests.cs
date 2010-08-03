﻿#region License
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
using System.Globalization;

using NUnit.Framework;

using Aspid.Core.Extensions;

namespace Aspid.Core.Tests.Extensions
{
    /// <summary>
    /// Tests for IConvertible interface extension methods.
    /// </summary>
    [TestFixture]
    public class IConvertibleExtensionsTests
    {
        [Test]
        public void ToInvariantString_OnNullObject_ReturnsEmtpyString()
        {
            IConvertible sut = null;
            Assert.AreEqual(string.Empty, sut.ToInvariantString());
        }

        [Test]
        public void ToInvariantString_OnNonNullObject_ReturnsItsInvariantCultureStringRepresentation()
        {
            IConvertible sut = "string";
            Assert.AreEqual(sut.ToString(CultureInfo.InvariantCulture), sut.ToInvariantString());

            //Some extra assertions
            sut = DateTime.Now;
            Assert.AreEqual(sut.ToString(CultureInfo.InvariantCulture), sut.ToInvariantString(), "Extra Assertion 1 Failed");
        }
    }
}
