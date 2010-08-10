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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using NUnit.Framework;

using Aspid.Core.Extensions;

namespace Aspid.Core.PerformanceTests.Extensions
{
    /// <summary>
    /// Performance tests for object class extension methods.
    /// </summary>
    [TestFixture]
    public class ObjectExtensionsPerformanceTests
    {
        class SelfReturnClass
        {
            public SelfReturnClass Self { get { return this; } }
        }

        /// <summary>
        /// Test always fails, in order for runner to display a message with the performance decrease percentage.
        /// (find a better way, perhaps?)
        /// </summary>
        [Test]
        public void SafelyNavigate_WhenGoingThrough10LevelsOfNotNullProperties_ShouldNotBeMoreThanXPercentSlower()
        {
            //Just to rise a red flag of *how* slow things are right now
            var sut = new SelfReturnClass();

            Stopwatch normalOperationTime = Stopwatch.StartNew();
            for (int i = 0; i < 40000; i++)
			{
			    sut.Self.Self.Self.Self.Self.Self.Self.Self.Self.Self.ToString();
            }
            normalOperationTime.Stop();

            Stopwatch safelyNavigateOperationTime = Stopwatch.StartNew();
            for (int i = 0; i < 40000; i++)
            {
                sut.SafelyNavigate(x => x.Self.Self.Self.Self.Self.Self.Self.Self.Self.Self.ToString());
            }
            safelyNavigateOperationTime.Stop();

            long percentage = ((safelyNavigateOperationTime.ElapsedTicks * 100) / (normalOperationTime.ElapsedTicks + 1)) - 100;
            Assert.Fail(string.Format("Safely navigation is {0}% slower that normal traversal", percentage));
        }
    }
}
