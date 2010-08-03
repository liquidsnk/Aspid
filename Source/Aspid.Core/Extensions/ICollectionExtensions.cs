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

using System.Collections;
using System.Collections.Generic;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// ICollection and ICollection<T> interfaces extension methods.
    /// </summary>
    public static class ICollectionExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the current collection is null or empty.
        /// </summary>
        /// <param name="self">The current collection</param>
        /// <returns>True if the current collection is null or empty, false otherwise</returns>
        public static bool IsNullOrEmpty(this ICollection self)
        {
            if (self == null || self.Count == 0) return true;
            return false;
        }

        /// <summary>
        /// Returns a value indicating whether the current collection is null or empty.
        /// </summary>
        /// <typeparam name="T">Type of elements of the collection</typeparam>
        /// <param name="self">The current collection</param>
        /// <returns>True if the current collection is null or empty, false otherwise</returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> self)
        {
            if (self == null || self.Count == 0) return true;
            return false;
        }
    }
}
