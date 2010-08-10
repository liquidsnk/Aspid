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

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// String class extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the current string is null or empty.
        /// </summary>
        /// <param name="self">The current string</param>
        /// <returns>True if the current string is null or empty, false otherwise</returns>
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        /// <summary>
        /// Deletes all characters from the current string beginning at the specified position.
        /// Safe remove will handle out of bounds start indexes gracefully, returning the given string.
        /// </summary>
        /// <param name="self">The current string</param>
        /// <param name="startIndex">The zero-based position to begin deleting characters</param>
        /// <returns>A new System.String that represents the current string except characters beginning from the specified position </returns>
        /// <exception cref="ArgumentException">
        /// StartIndex is less than zero.
        /// </exception>
        public static string SafeRemove(this string self, int startIndex)
        {
            return self.SafeRemove(startIndex, (self ?? "").Length);
        }

        /// <summary>
        /// Deletes a specified number of characters from the current string beginning at the specified position.
        /// Safe remove will handle out of bounds ranges gracefully, returning the given string minus the removed chars.
        /// </summary>
        /// <param name="self">The current string</param>
        /// <param name="startIndex">The zero-based position to begin deleting characters</param>
        /// <param name="count">The number of characters to delete</param>
        /// <returns>A new System.String that represents the current string except the specified character range </returns>
        /// <exception cref="ArgumentException">
        /// Either startIndex or count are less than zero.
        /// </exception>
        public static string SafeRemove(this string self, int startIndex, int count)
        {
            if (startIndex < 0) throw new ArgumentException("Start index should be equal or greater than zero");
            if (count < 0) throw new ArgumentException("Count should be equal or greater than zero");

            if (self == null) return null;
            if (startIndex >= self.Length) return self;
            if ((startIndex  + count) > self.Length) return self.Remove(startIndex);
            return self.Remove(startIndex, count);
        }
    }
}
