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
using System.Globalization;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// IConvertible interface extension methods.
    /// </summary>
    public static class IConvertibleExtensions
    {
        /// <summary>
        /// Returns the invariant culture string representation for the current object, or empty if it's null.
        /// </summary>
        /// <param name="self">The current object</param>
        /// <returns>The invariant culture string representation, or empty if self is null</returns>
        public static string ToInvariantString(this IConvertible self)
        {
            if (self == null) return string.Empty;
            return self.ToString(CultureInfo.InvariantCulture);
        }
    }
}
